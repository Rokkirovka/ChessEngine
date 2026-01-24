using MyChess.Core.Board;
using MyChess.Models.Moves;

namespace MyChess.Models.MoveHistoryItems;

public class NullMoveHistoryItem(ChessMove move, BoardState stateBeforeMove) 
    : MoveHistoryItem(move, stateBeforeMove);