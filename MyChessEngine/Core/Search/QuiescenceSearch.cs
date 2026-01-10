using MyChess.Hashing;
using MyChessEngine.Core.Evaluation.Moves;
using MyChessEngine.Core.Evaluation.Position;
using MyChessEngine.Models;
using MyChessEngine.Transposition;

namespace MyChessEngine.Core.Search;

public static class QuiescenceSearch
{
    private static readonly TranspositionTable QuiescenceTable = new(1 << 16, 2);
    private static readonly PositionEvaluator PositionEvaluator = new();

    public static int? Search(SearchContext context, int depth, MoveOrderingService moveOrderingService, int alpha,
        int beta, int color)
    {
        var originalAlpha = alpha;
        if (context.SearchCanceler?.ShouldStop is true) return null;
        if (!context.Parameters.UseQuiescenceSearch) return PositionEvaluator.Evaluate(context.Game.Board) * color;
        var game = context.Game;
        var hash = ZobristHasher.CalculateInitialHash(game.Board, game.State);

        if (QuiescenceTable.TryGet(hash, out var entry, context))
        {
            switch (entry.NodeType)
            {
                case NodeType.Exact:
                    return entry.Score;
                case NodeType.LowerBound:
                    if (entry.Score >= beta) return beta;
                    alpha = Math.Max(alpha, entry.Score);
                    break;
                case NodeType.UpperBound:
                    if (entry.Score <= alpha) return alpha;
                    beta = Math.Min(beta, entry.Score);
                    break;
            }

            if (alpha >= beta) return entry.NodeType == NodeType.LowerBound ? beta : alpha;
        }

        if (game.IsCheckmate) return -100000 - depth;
        if (game.IsStalemate || game.IsDrawByRepetition) return 0;

        var evaluation = PositionEvaluator.Evaluate(game.Board) * color;

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

        QuiescenceTable.Store(hash, scoreToStore, 0, null, nodeType);
        return returnValue;
    }
}