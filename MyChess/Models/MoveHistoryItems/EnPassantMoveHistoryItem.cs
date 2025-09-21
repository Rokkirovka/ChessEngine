using MyChess.Core;
using MyChess.Models.Moves;

namespace MyChess.Models.MoveHistoryItems;

public class EnPassantMoveHistoryItem(ChessMove move, BoardState stateBeforeMove)
    : MoveHistoryItem(move, stateBeforeMove);