using MyChess.Core;
using MyChess.Hashing;
using MyChessEngine.Core.Evaluation;
using MyChessEngine.Transposition;

namespace MyChessEngine.Core.Search;

public static class QuiescenceSearch
{
    private static readonly TranspositionTable QuiescenceTable = new(1 << 16, 2); 
    
    public static int Search(ChessGame game, int depth, Evaluator evaluator, SearchContext context, int alpha, int beta, int color)
    {
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
        
        context.NodesVisited++;
        var evaluation = Evaluator.EvaluatePosition(game.Board) * color;

        if (evaluation >= beta) return beta;
        if (evaluation > alpha) alpha = evaluation;

        var moves = game.GetAllPossibleMoves()
            .Where(move => game.GetPiece(move.To) is not null)
            .OrderByDescending(move => evaluator.EvaluateMove(game, move));

        foreach (var move in moves)
        {
            game.MakeMove(move);
            var score = -Search(game, depth, evaluator, context, -beta, -alpha, -color);
            game.UndoLastMove();

            if (score > alpha) alpha = score;
            if (alpha >= beta) break;
        }

        return alpha;
    }
}