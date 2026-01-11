using MyChess.Core;
using MyChess.Models;
using MyChess.Models.MoveHistoryItems;
using MyChess.Models.Moves;
using MyChess.Services.MoveExecution.Interfaces;

namespace MyChess.Services.MoveExecution.Strategies;

public class NullMoveStrategy : IMoveStrategy
{
    public bool CanExecute(ChessMove move, ChessBoard board, BoardState boardState)
    {
        return move is NullMove;
    }

    public void Execute(ChessMove move, ChessBoard board, BoardState boardState)
    {
        boardState.EnPassantTarget = null;
        boardState.CurrentMoveColor = boardState.CurrentMoveColor == ChessColor.White 
            ? ChessColor.Black 
            : ChessColor.White;
    }

    public void Undo(MoveHistoryItem historyItem, ChessBoard board, BoardState boardState)
    {
        boardState.RestoreFrom(historyItem.StateBeforeMove);
    }

    public MoveHistoryItem CreateHistoryItem(ChessMove move, BoardState stateBeforeMove, ChessBoard board)
    {
        return new NullMoveHistoryItem(move, stateBeforeMove);
    }
}