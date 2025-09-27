using MyChess.Rules.MoveGenerator;

namespace MyChess.Models.Pieces;

public class Queen : IChessPiece
{
    public static readonly Queen White = new(ChessColor.White);
    public static readonly Queen Black = new(ChessColor.Black);
    
    private Queen(ChessColor color) => Color = color;
    
    public ChessColor Color { get; }
    public int Index => Color == ChessColor.White ? 4 : 10;
    public IMoveGenerator GetMoveGenerator() => QueenMoveGenerator.Instance;
}