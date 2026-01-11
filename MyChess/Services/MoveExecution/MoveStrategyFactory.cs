using MyChess.Models.Moves;
using MyChess.Services.MoveExecution.Interfaces;
using MyChess.Services.MoveExecution.Strategies;

namespace MyChess.Services.MoveExecution;

public class MoveStrategyFactory : IMoveStrategyFactory
{
    public IMoveStrategy GetMoveStrategy(ChessMove move)
    {
        return move switch
        {
            StandardMove => new StandardMoveStrategy(),
            EnPassantMove => new EnPassantMoveStrategy(),
            CastlingMove => new CastlingMoveStrategy(),
            PromotionMove => new PromotionMoveStrategy(),
            NullMove => new NullMoveStrategy(),
            _ => throw new ArgumentException($"Unknown move type: {move.GetType()}")
        };
    }
}