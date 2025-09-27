using MyChess.Rules.MoveGenerator;

namespace MyChess.Models.Pieces;

public class Knight : IChessPiece
{
    public static readonly Knight White = new(ChessColor.White);
    public static readonly Knight Black = new(ChessColor.Black);
    
    private Knight(ChessColor color) => Color = color;
    
    public ChessColor Color { get; }
    public int Index => Color == ChessColor.White ? 1 : 7;
    public IMoveGenerator GetMoveGenerator() => KnightMoveGenerator.Instance;
}