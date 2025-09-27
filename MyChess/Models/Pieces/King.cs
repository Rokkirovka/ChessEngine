using MyChess.Rules.MoveGenerator;

namespace MyChess.Models.Pieces;

public class King : IChessPiece
{
    public static readonly King White = new(ChessColor.White);
    public static readonly King Black = new(ChessColor.Black);
    
    private King(ChessColor color) => Color = color;
    
    public ChessColor Color { get; }
    public int Index => Color == ChessColor.White ? 5 : 11;
    public IMoveGenerator GetMoveGenerator() => KingMoveGenerator.Instance;
}