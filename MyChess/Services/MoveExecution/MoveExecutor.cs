using MyChess.Core;
using MyChess.Models.MoveHistoryItems;
using MyChess.Models.Moves;
using MyChess.Services.MoveExecution.Interfaces;

namespace MyChess.Services.MoveExecution;

public class MoveExecutor(IMoveStrategyFactory strategyFactory) : IMoveExecutor
{
    private readonly Stack<MoveHistoryItem> _moveHistory = new();


    public bool TryExecuteMove(ChessMove move, ChessBoard board, BoardState boardState)
    {
        var strategy = strategyFactory.GetMoveStrategy(move);
        if (!strategy.CanExecute(move, board, boardState)) return false;
        ForceMove(move, board, boardState);
        return true;
    }

    public void ForceMove(ChessMove move, ChessBoard board, BoardState boardState)
    {
        var strategy = strategyFactory.GetMoveStrategy(move);
        var stateBeforeMove = boardState.Clone();
        var historyItem = strategy.CreateHistoryItem(move, stateBeforeMove, board);
        strategy.Execute(move, board, boardState);
        _moveHistory.Push(historyItem);
        boardState.ChangeColor();
    }

    public void UndoMove(ChessBoard board, BoardState boardState)
    {
        if (_moveHistory.Count == 0) throw new InvalidOperationException("There are no moves to undo.");
        var moveHistoryItem = _moveHistory.Pop();
        var strategy = strategyFactory.GetMoveStrategy(moveHistoryItem.Move);
        strategy.Undo(moveHistoryItem, board, boardState);
    }
}