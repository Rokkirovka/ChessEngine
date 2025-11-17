using MyChess.Models.Moves;

namespace MyChessEngine.Core.Search;

public class PvTableManager(int maxDepth)
{
    private readonly ChessMove?[] _pvArray = new ChessMove?[maxDepth * maxDepth];

    private int GetIndex(int ply, int depth)
    {
        return ply * maxDepth + depth;
    }

    public void UpdatePvLine(ChessMove bestMove, int currentDepth, int searchDepth)
    {
        var ply = searchDepth - currentDepth;
        
        if (ply >= maxDepth) return;

        _pvArray[GetIndex(ply, ply)] = bestMove;

        for (var nextPly = ply + 1; nextPly < searchDepth && nextPly < maxDepth; nextPly++)
        {
            _pvArray[GetIndex(ply, nextPly)] = _pvArray[GetIndex(ply + 1, nextPly)];
        }
    }
    
    public ChessMove[] GetPrincipalVariation(int depth)
    {
        var result = new List<ChessMove>();
        var actualDepth = Math.Min(depth, maxDepth);
        
        for (var i = 0; i < actualDepth; i++)
        {
            var move = _pvArray[GetIndex(0, i)];
            if (move is not null) result.Add(move);
            else break;
        }
        return result.ToArray();
    }
}