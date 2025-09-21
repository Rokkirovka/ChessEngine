using MyChess.Models.Moves;

namespace MyChess.Services.MoveExecution.Interfaces;

public interface IMoveStrategyFactory
{ 
    IMoveStrategy GetMoveStrategy(ChessMove move);
}