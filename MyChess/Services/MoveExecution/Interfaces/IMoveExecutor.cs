using MyChess.Core;
using MyChess.Models.Moves;

namespace MyChess.Services.MoveExecution.Interfaces;

public interface IMoveExecutor
{
    bool TryExecuteMove(ChessMove move, ChessBoard board, BoardState boardState);
    void ForceMove(ChessMove move, ChessBoard board, BoardState boardState);
    void UndoMove(ChessBoard board, BoardState boardState);
}