using MyChess.Core;
using MyChess.Models;
using MyChess.Models.MoveHistoryItems;
using MyChess.Models.Moves;

namespace MyChess.Services.MoveExecution.Interfaces;

public interface IMoveStrategy
{
    bool CanExecute(ChessMove move, ChessBoard board, BoardState boardState);
    void Execute(ChessMove castlingMove, ChessBoard board, BoardState boardState);
    void Undo(MoveHistoryItem historyItem, ChessBoard board, BoardState boardState);
    MoveHistoryItem CreateHistoryItem(ChessMove move, BoardState stateBeforeMove, ChessBoard board);
    IEnumerable<ChessCell> GetCellsWillChange(ChessMove move, ChessBoard board, BoardState boardState);
}