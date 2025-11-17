using MyChess.Models.Moves;
using MyChessEngine.Transposition;

namespace MyChessEngine.Core.Search;

public class TranspositionService
{
    private static readonly TranspositionTable Table = new();
    
    public static void IncrementAge() => Table.IncrementAge();
    
    public static bool TryGetBestMove(ulong hash, int currentDepth, out ChessMove? bestMove, out int score, out NodeType nodeType)
    {
        bestMove = null;
        score = 0;
        nodeType = NodeType.Exact;
        
        if (!Table.TryGet(hash, out var entry)) return false;

        if (entry.Depth < currentDepth) return false;
        bestMove = entry.BestMove;
        score = entry.Score;
        nodeType = entry.NodeType;
        return true;

    }
    
    public static void Store(ulong hash, int score, int depth, ChessMove? bestMove, NodeType nodeType)
        => Table.Store(hash, score, depth, bestMove, nodeType);
}