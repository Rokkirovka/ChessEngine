using MyChess.Core.Board;
using MyChess.Models.Moves;
using MyChess.Models.Pieces;

namespace MyChess.Models.MoveHistoryItems;

public class StandardMoveHistoryItem(ChessMove move, BoardState stateBeforeMove, IChessPiece? capturedPiece)
    : MoveHistoryItem(move, stateBeforeMove)
{
    public readonly IChessPiece? CapturedPiece = capturedPiece;
}