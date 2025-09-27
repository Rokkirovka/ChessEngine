using MyChess.Rules.MoveGenerator;

namespace MyChess.Models.Pieces;

public class Pawn : IChessPiece
{
    public static readonly Pawn White = new(ChessColor.White);
    public static readonly Pawn Black = new(ChessColor.Black);

    private Pawn(ChessColor color) => Color = color;

    public ChessColor Color { get; }
    public int Index => Color == ChessColor.White ? 0 : 6;

    public IMoveGenerator GetMoveGenerator() =>
        Color == ChessColor.White ? WhitePawnMoveGenerator.Instance : BlackPawnMoveGenerator.Instance;
}