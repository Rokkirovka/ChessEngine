using MyChess.Models;
using MyChess.Models.Pieces;

namespace MyChess.Core;

public class ChessBoard
{
    private IChessPiece?[] _pieces = new IChessPiece?[64];

    public IChessPiece? GetPiece(ChessCell cell) => _pieces[(int)cell];

    public void SetPiece(ChessCell cell, IChessPiece? piece) => _pieces[(int)cell] = piece;

    public void RemovePiece(ChessCell cell) => _pieces[(int)cell] = null;

    public void MovePiece(ChessCell from, ChessCell to)
    {
        if (GetPiece(from) is null) return;
        SetPiece(to, GetPiece(from));
        RemovePiece(from);
    }

    public ChessCell FindKing(ChessColor color)
    {
        for (var i = 0; i < 64; i++)
        {
            if (_pieces[i] is King king && king.Color == color) return (ChessCell)i;
        }
        throw new InvalidOperationException("King not found");
    }
    
    public ChessBoard Clone()
    {
        var clonedBoard = new ChessBoard
        {
            _pieces = _pieces.Select(p => p).ToArray()
        };
        return clonedBoard;
    }
}