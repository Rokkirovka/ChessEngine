using MyChess.Rules.MoveGenerator;

namespace MyChess.Models.Pieces;

public class King : IChessPiece
{
    public static readonly King White = new King(ChessColor.White);
    public static readonly King Black = new King(ChessColor.Black);
    
    private King(ChessColor color) => Color = color;
    
    public ChessColor Color { get; }
    public IMoveGenerator GetMoveGenerator() => KingMoveGenerator.Instance;
}