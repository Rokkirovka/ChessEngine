using MyChess.Models.Pieces;

namespace MyChess.Models.Moves;

public class PromotionMove(ChessCell from, ChessCell to, IChessPiece promotionPiece) : ChessMove(from, to)
{
    public IChessPiece PromotionPiece = promotionPiece;
}