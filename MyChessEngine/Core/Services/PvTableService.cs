using MyChess.Models.Moves;

namespace MyChessEngine.Core.Services;

public class PvTableService
{
    private readonly ChessMove?[][] _triangularTable;
    private readonly int _depth;

    public PvTableService(int depth)
    {
        _depth = depth;
        _triangularTable = new ChessMove?[depth][];
        for (var i = 0; i < depth; i++)
        {
            _triangularTable[i] = new ChessMove?[depth - i];
        }
    }

    public void UpdatePvLine(ChessMove bestMove, int ply)
    {
        _triangularTable[ply][0] = bestMove;

        for (var nextPly = 1; nextPly < _depth - ply; nextPly++)
        {
            _triangularTable[ply][nextPly] = _triangularTable[ply + 1][nextPly - 1];
        }
    }
    
    public ChessMove?[] GetPrincipalVariation()
    {
        return _triangularTable[0];
    }
}