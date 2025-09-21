using MyChess.Rules.MoveGenerator;

namespace MyChess.Models.Pieces;

public class Knight : IChessPiece
{
    public static readonly Knight White = new Knight(ChessColor.White);
    public static readonly Knight Black = new Knight(ChessColor.Black);
    
    private Knight(ChessColor color) => Color = color;
    
    public ChessColor Color { get; }
    public IMoveGenerator GetMoveGenerator() => KnightMoveGenerator.Instance;
}