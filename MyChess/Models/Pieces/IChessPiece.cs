using MyChess.Rules.MoveGenerator;

namespace MyChess.Models.Pieces;

public interface IChessPiece
{
    ChessColor Color { get; }
    
    IMoveGenerator GetMoveGenerator();
}