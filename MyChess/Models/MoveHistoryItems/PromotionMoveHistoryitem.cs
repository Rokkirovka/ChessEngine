using MyChess.Core;
using MyChess.Models.Moves;
using MyChess.Models.Pieces;

namespace MyChess.Models.MoveHistoryItems;

public class PromotionMoveHistoryItem(ChessMove move, BoardState stateBeforeMove, IChessPiece? capturedPiece)
    : MoveHistoryItem(move, stateBeforeMove)
{
    public readonly IChessPiece? CapturedPiece = capturedPiece;
}