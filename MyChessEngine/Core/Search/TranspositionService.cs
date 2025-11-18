using MyChess.Models.Moves;
using MyChessEngine.Transposition;

namespace MyChessEngine.Core.Search;

public static class TranspositionService
{
    private static readonly TranspositionTable Table = new();
    
    public static void IncrementAge() => Table.IncrementAge();
    
    public static bool TryGetBestMove(SearchContext context, ulong hash, int currentDepth, int alpha, int beta, out int score, out NodeType nodeType)
    {
        score = 0;
        nodeType = NodeType.Exact;
        if (context.Parameters.UseTranspositionTable is false) return false;
    
        if (!Table.TryGet(hash, out var entry)) return false;

        if (entry.Depth < currentDepth) return false;
        score = entry.Score;
        nodeType = entry.NodeType;

        if (nodeType == NodeType.Exact) return true;
        if (nodeType == NodeType.LowerBound && score >= beta) return true;  
        if (nodeType == NodeType.UpperBound && score <= alpha) return true;
        return false;
    }

    public static void Store(SearchContext context, ulong hash, int score, int depth, ChessMove? bestMove, NodeType nodeType)
    {
        if (context.Parameters.UseTranspositionTable is false) return;
        Table.Store(hash, score, depth, bestMove, nodeType);
    }
}