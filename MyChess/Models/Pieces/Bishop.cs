using MyChess.Rules.MoveGenerator;

namespace MyChess.Models.Pieces;

public class Bishop : IChessPiece
{
    public static readonly Bishop White = new(ChessColor.White);
    public static readonly Bishop Black = new(ChessColor.Black);
    
    private Bishop(ChessColor color) => Color = color;
    
    public ChessColor Color { get; }
    public int Index => Color == ChessColor.White ? 2 : 8;
    public IMoveGenerator GetMoveGenerator() => BishopMoveGenerator.Instance;
}