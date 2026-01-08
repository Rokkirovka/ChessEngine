using MyChess.Hashing;
using MyChessEngine.Core.Evaluation;
using MyChessEngine.Core.Evaluation.Position;
using MyChessEngine.Models;
using MyChessEngine.Transposition;

namespace MyChessEngine.Core.Search;

public static class QuiescenceSearch
{
    private static readonly TranspositionTable QuiescenceTable = new(1 << 16, 2); 
    private static readonly PositionEvaluator PositionEvaluator = new();
    
    public static int? Search(SearchContext context, int depth, MoveEvaluator moveEvaluator, int alpha, int beta, int color)
    {
        if (context.SearchCanceler?.ShouldStop is true) return null;
        
        if (!context.Parameters.UseQuiescenceSearch) 
            return PositionEvaluator.Evaluate(context.Game.Board) * color;

        var game = context.Game;
        var hash = ZobristHasher.CalculateInitialHash(game.Board, game.State);
        
        if (QuiescenceTable.TryGet(hash, out var entry))
        {
            switch (entry.NodeType)
            {
                case NodeType.Exact:
                    return entry.Score;
                case NodeType.LowerBound:
                    alpha = Math.Max(alpha, entry.Score);
                    break;
                case NodeType.UpperBound:
                    beta = Math.Min(beta, entry.Score);
                    break;
            }

            if (alpha >= beta)
                return entry.Score;
        }
        
        if (game.IsCheckmate) return -100000 - depth;
        if (game.IsStalemate || game.IsDrawByRepetition) return 0;
        
        var evaluation = PositionEvaluator.Evaluate(game.Board) * color;

        if (evaluation >= beta) return beta;
        if (evaluation > alpha) alpha = evaluation;

        var moves = game.GetAllPossibleMoves()
            .Where(move => game.GetPiece(move.To) is not null)
            .OrderByDescending(move => moveEvaluator.EvaluateMove(game, move));

        foreach (var move in moves)
        {
            if (context.SearchCanceler?.ShouldStop is true) return null;
            
            context.NodesVisited++;
            game.MakeMove(move);
            var score = -Search(context, depth, moveEvaluator, -beta, -alpha, -color);
            game.UndoLastMove();
            
            if (score is null) return null;

            if (score > alpha) alpha = score.Value;
            if (alpha >= beta) break;
        }

        QuiescenceTable.Store(hash, alpha, 0, null, NodeType.Exact);
        return alpha;
    }
}