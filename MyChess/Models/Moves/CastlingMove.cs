namespace MyChess.Models.Moves;

public class CastlingMove(ChessCell from, ChessCell to) : ChessMove(from, to)
{
    public readonly int RookFrom = to > from ? (int)from + 3 : (int)from - 4;
    public readonly int RookTo = to > from ? (int)from + 1 : (int)from - 1;
}