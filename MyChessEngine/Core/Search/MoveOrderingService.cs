using MyChess.Core;
using MyChess.Models.Moves;
using MyChessEngine.Core.Evaluation;

namespace MyChessEngine.Core.Search;

public class MoveOrderingService(Evaluator evaluator)
{
    public IEnumerable<ChessMove> OrderMoves(ChessGame game, IEnumerable<ChessMove> moves)
        => moves.OrderByDescending(move => evaluator.EvaluateMove(game, move));
    
    public void UpdateHeuristics(SearchContext context, ChessMove move, int depth)
    {
        if (context.Game.GetPiece(move.To) is not null) return;
        if (context.Parameters.UseKillerMoves)
            evaluator.UpdateKillerMoves(move, context.Game.Ply);
        if (context.Parameters.UseHistoryTable)
            evaluator.UpdateHistoryTable(context.Game.GetPiece(move.From)!.Index, move.To, depth);
    }
}