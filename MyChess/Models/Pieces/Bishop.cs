using MyChess.Rules.MoveGenerator;

namespace MyChess.Models.Pieces;

public class Bishop : IChessPiece
{
    public static readonly Bishop White = new Bishop(ChessColor.White);
    public static readonly Bishop Black = new Bishop(ChessColor.Black);
    
    private Bishop(ChessColor color) => Color = color;
    
    public ChessColor Color { get; }
    public IMoveGenerator GetMoveGenerator() => BishopMoveGenerator.Instance;
}