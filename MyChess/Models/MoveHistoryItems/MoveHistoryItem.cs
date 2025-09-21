using MyChess.Core;
using MyChess.Models.Moves;

namespace MyChess.Models.MoveHistoryItems;

public abstract class MoveHistoryItem(
    ChessMove move, 
    BoardState stateBeforeMove)
{
    public readonly ChessMove Move = move;
    public readonly BoardState StateBeforeMove = stateBeforeMove;
}