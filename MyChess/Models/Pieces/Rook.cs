using MyChess.Rules.MoveGenerator;

namespace MyChess.Models.Pieces;

public class Rook : IChessPiece
{
    public static readonly Rook White = new(ChessColor.White);
    public static readonly Rook Black = new(ChessColor.Black);
    
    private Rook(ChessColor color) => Color = color;
    
    public ChessColor Color { get; }
    public int Index => Color == ChessColor.White ? 3 : 9;
    public IMoveGenerator GetMoveGenerator() => RookMoveGenerator.Instance;
}