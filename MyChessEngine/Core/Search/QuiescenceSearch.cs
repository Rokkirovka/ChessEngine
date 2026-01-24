using MyChess.Hashing;
using MyChessEngine.Core.Evaluation.Moves;
using MyChessEngine.Core.Evaluation.Position;
using MyChessEngine.Models;
using MyChessEngine.Transposition;

namespace MyChessEngine.Core.Search;

public static class QuiescenceSearch
{
    private static readonly PositionEvaluator PositionEvaluator = new();
    private const int Delta = 1200;

    public static int? Search(SearchContext context, int depth, MoveOrderingService moveOrderingService, int alpha,
        int beta, int color)
    {
        var originalAlpha = alpha;
        if (context.SearchCanceler?.ShouldStop is true) return null;
        if (!context.Parameters.UseQuiescenceSearch) return PositionEvaluator.Evaluate(context.Game.Board) * color;
        var game = context.Game;
        var hash = ZobristHasher.CalculateInitialHash(game.Board, game.State);

        if (TranspositionService.TryGetBestMove(context, hash, 0, alpha, beta, out var ttScore, out var transpositionNodeType))
        {
            var result = HandleTranspositionResult(ttScore, transpositionNodeType, alpha, beta);
            if (result.HasValue) return result.Value;
        }

        if (game.IsCheckmate) return -100000 - depth;
        if (game.IsStalemate || game.IsDrawByRepetition) return 0;

        var evaluation = PositionEvaluator.Evaluate(game.Board) * color;
        if (evaluation + Delta <= alpha) return alpha;


        if (evaluation >= beta) return beta;
        if (evaluation > alpha) alpha = evaluation;

        var moves = moveOrderingService.OrderMoves(game.Board, game.Ply, game.GetAllPossibleMoves()
            .Where(move => game.GetPiece(move.To) is not null));

        foreach (var move in moves)
        {
            if (context.SearchCanceler?.ShouldStop is true) return null;

            context.NodesVisited++;
            game.MakeMove(move);
            var score = -Search(context, depth, moveOrderingService, -beta, -alpha, -color);
            game.UndoLastMove();

            if (score is null) return null;

            if (score > alpha) alpha = score.Value;
            if (alpha >= beta) break;
        }

        NodeType nodeType;
        int scoreToStore;
        int returnValue;

        if (alpha >= beta)
        {
            nodeType = NodeType.LowerBound;
            scoreToStore = beta;
            returnValue = beta;
        }
        else if (alpha <= originalAlpha)
        {
            nodeType = NodeType.UpperBound;
            scoreToStore = alpha;
            returnValue = alpha;
        }
        else
        {
            nodeType = NodeType.Exact;
            scoreToStore = alpha;
            returnValue = alpha;
        }

        TranspositionService.Store(context, hash, scoreToStore, 0, null, nodeType);
        return returnValue;
    }

    private static int? HandleTranspositionResult(int score, NodeType nodeType, int alpha, int beta)
    {
        switch (nodeType)
        {
            case NodeType.Exact: return score;
            case NodeType.LowerBound:
                if (score >= beta) return beta;
                break;
            case NodeType.UpperBound:
                if (score <= alpha) return alpha;
                break;
        }

        return null;
    }
}