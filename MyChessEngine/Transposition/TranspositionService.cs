using MyChess.Models.Moves;
using MyChessEngine.Models;

namespace MyChessEngine.Transposition;

public static class TranspositionService
{
    private static readonly TranspositionTable Table = new();
    
    public static void IncrementAge() => Table.IncrementAge();
    
    public static bool TryGetBestMove(SearchContext context, ulong hash, int currentDepth, int alpha, int beta, out int score, out NodeType nodeType)
    {
        score = 0;
        nodeType = NodeType.Exact;
        if (context.Parameters.UseTranspositionTable is false) return false;
    
        if (!Table.TryGet(hash, out var entry, context)) return false;

        if (entry.Depth < currentDepth) return false;
        score = entry.Score;
        nodeType = entry.NodeType;

        switch (nodeType)
        {
            case NodeType.Exact:
            case NodeType.LowerBound when score >= beta:
            case NodeType.UpperBound when score <= alpha:
                return true;
            default:
                return false;
        }
    }

    public static void Store(SearchContext context, ulong hash, int score, int depth, ChessMove? bestMove, NodeType nodeType)
    {
        if (context.Parameters.UseTranspositionTable is false) return;
        Table.Store(hash, score, depth, bestMove, nodeType);
    }
}