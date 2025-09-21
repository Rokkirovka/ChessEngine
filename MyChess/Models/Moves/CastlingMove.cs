namespace MyChess.Models.Moves;

public class CastlingMove(ChessCell from, ChessCell to) : ChessMove(from, to)
{
    public readonly ChessCell RookFrom = to > from ? from + 3 : from - 4;
    public readonly ChessCell RookTo = to > from ? from + 1 : from - 1;
}