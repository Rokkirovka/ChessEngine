using MyChess.Core;
using MyChess.Models.Moves;
using MyChessEngine.Core.Evaluation;
using MyChessEngine.Models;

namespace MyChessEngine.Core.Services;

public class MoveOrderingService(MoveEvaluator moveEvaluator)
{
    public IEnumerable<ChessMove> OrderMoves(ChessGame game, IEnumerable<ChessMove> moves)
        => moves.OrderByDescending(move => moveEvaluator.EvaluateMove(game, move));
    
    public void UpdateHeuristics(SearchContext context, ChessMove move, int depth)
    {
        if (context.Game.GetPiece(move.To) is not null) return;
        if (context.Parameters.UseKillerMoves)
            moveEvaluator.UpdateKillerMoves(move, context.Game.Ply);
        if (context.Parameters.UseHistoryTable)
            moveEvaluator.UpdateHistoryTable(context.Game.GetPiece(move.From)!.Index, move.To, depth);
    }
}