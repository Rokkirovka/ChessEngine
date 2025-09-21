using MyChess.Rules.MoveGenerator;

namespace MyChess.Models.Pieces;

public class Queen : IChessPiece
{
    public static readonly Queen White = new Queen(ChessColor.White);
    public static readonly Queen Black = new Queen(ChessColor.Black);
    
    private Queen(ChessColor color) => Color = color;
    
    public ChessColor Color { get; }
    public IMoveGenerator GetMoveGenerator() => QueenMoveGenerator.Instance;
}