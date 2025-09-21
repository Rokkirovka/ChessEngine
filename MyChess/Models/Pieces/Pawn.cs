using MyChess.Rules.MoveGenerator;

namespace MyChess.Models.Pieces;

public class Pawn : IChessPiece
{
    public static readonly Pawn White = new Pawn(ChessColor.White);
    public static readonly Pawn Black = new Pawn(ChessColor.Black);
    
    private Pawn(ChessColor color) => Color = color;
    
    public ChessColor Color { get; }
    public IMoveGenerator GetMoveGenerator() => PawnMoveGenerator.Instance;
}