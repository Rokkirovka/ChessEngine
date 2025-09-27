using MyChess.Models;

namespace MyChess.Core;

public class BoardState
{
    public ChessColor CurrentMoveColor { get; private set; }
    public int? EnPassantTarget { get; set; }

    public CastlingRights CastlingRights { get; private set; } =
        CastlingRights.WhiteKingSide |
        CastlingRights.WhiteQueenSide |
        CastlingRights.BlackKingSide |
        CastlingRights.BlackQueenSide;
    
    public void DisableCastling(CastlingRights right) => CastlingRights &= ~right;

    public void ChangeColor()
    {
        CurrentMoveColor = 1 - CurrentMoveColor;
    }

    public BoardState Clone() => new()
    {
        CurrentMoveColor = CurrentMoveColor,
        EnPassantTarget = EnPassantTarget,
        CastlingRights = CastlingRights
    };

    public void RestoreFrom(BoardState stateBeforeMove)
    {
        CurrentMoveColor = stateBeforeMove.CurrentMoveColor;
        EnPassantTarget = stateBeforeMove.EnPassantTarget;
        CastlingRights = stateBeforeMove.CastlingRights;
    }
}