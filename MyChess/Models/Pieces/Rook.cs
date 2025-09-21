using MyChess.Rules.MoveGenerator;

namespace MyChess.Models.Pieces;

public class Rook : IChessPiece
{
    public static readonly Rook White = new Rook(ChessColor.White);
    public static readonly Rook Black = new Rook(ChessColor.Black);
    
    private Rook(ChessColor color) => Color = color;
    
    public ChessColor Color { get; }
    public IMoveGenerator GetMoveGenerator() => RookMoveGenerator.Instance;
}