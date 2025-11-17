using MyChess.Core;
using MyChess.Models.Moves;
using MyChessEngine.Core.Evaluation;

namespace MyChessEngine.Core.Search;

public class MoveOrderingService(Evaluator evaluator)
{
    public IEnumerable<ChessMove> OrderMoves(ChessGame game, IEnumerable<ChessMove> moves)
        => moves.OrderByDescending(move => evaluator.EvaluateMove(game, move));
    
    public void UpdateHeuristics(ChessGame game, ChessMove move, int depth, int ply)
    {
        if (game.GetPiece(move.To) is not null) return;
        evaluator.UpdateKillerMoves(move, ply);
        evaluator.UpdateHistoryTable(game.GetPiece(move.From)!.Index, move.To, depth);
    }
}