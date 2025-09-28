using MyChess.Models.Pieces;

namespace MyChess.Models.Moves;

public class PromotionMove(ChessCell from, ChessCell to, IChessPiece promotionPiece) : ChessMove(from, to)
{
    public readonly IChessPiece PromotionPiece = promotionPiece;

    public override string ToString()
    {
        return $"{(ChessCell)From}{(ChessCell)To}".ToLower() + GetPromotionPieceLetter();
    }

    private char GetPromotionPieceLetter()
    {
        return PromotionPiece switch
        {
            Queen => 'q',
            Rook => 'r',
            Bishop => 'b',
            Knight => 'n',
            _ => 'q'
        };
    }
}