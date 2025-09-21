using MyChess.Models;

namespace MyChess.Core;

public class BoardState
{
    public ChessColor CurrentMoveColor { get; set; }
    public ChessCell? EnPassantTarget { get; set; }
    public bool WhiteKingSideCastling { get; set; } = true;
    public bool WhiteQueenSideCastling { get; set; } = true;
    public bool BlackKingSideCastling { get; set; } = true;
    public bool BlackQueenSideCastling { get; set; } = true;

    public void ChangeColor()
    {
        CurrentMoveColor = 1 - CurrentMoveColor;
    }
    
    public BoardState Clone() => new BoardState
    {
        CurrentMoveColor = CurrentMoveColor,
        EnPassantTarget = EnPassantTarget,
        WhiteKingSideCastling = WhiteKingSideCastling,
        WhiteQueenSideCastling = WhiteQueenSideCastling,
        BlackKingSideCastling = BlackKingSideCastling,
        BlackQueenSideCastling = BlackQueenSideCastling
    };

    public void RestoreFrom(BoardState stateBeforeMove)
    {
        CurrentMoveColor = stateBeforeMove.CurrentMoveColor;
        EnPassantTarget = stateBeforeMove.EnPassantTarget;
        WhiteKingSideCastling = stateBeforeMove.WhiteKingSideCastling;
        WhiteQueenSideCastling = stateBeforeMove.WhiteQueenSideCastling;
        BlackKingSideCastling = stateBeforeMove.BlackKingSideCastling;
        BlackQueenSideCastling = stateBeforeMove.BlackQueenSideCastling;
    }
}