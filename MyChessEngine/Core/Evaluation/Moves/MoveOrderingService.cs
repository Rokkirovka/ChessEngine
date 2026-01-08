using MyChess.Core;
using MyChess.Models.Moves;
using MyChessEngine.Core.Evaluation.Moves.Components;
using MyChessEngine.Models;

namespace MyChessEngine.Core.Evaluation.Moves;

public class MoveOrderingService(
    KillerMovesService killerMovesService,
    HistoryTableService historyTableService)
{
    private readonly MoveEvaluator _moveEvaluator = new MoveEvaluator(killerMovesService, historyTableService);
    
    public IEnumerable<ChessMove> OrderMoves(ChessBoard board, int ply, IEnumerable<ChessMove> moves)
    {
        return moves.OrderByDescending(move => _moveEvaluator.EvaluateMove(board, ply, move));
    }

    public void UpdateHeuristics(SearchContext context, ChessMove move, int depth)
    {
        if (context.Game.GetPiece(move.To) is not null) return;
        if (context.Parameters.UseKillerMoves)
            killerMovesService.UpdateKillerMoves(move, context.Game.Ply);
        if (context.Parameters.UseHistoryTable)
            historyTableService.UpdateHistoryTable(context.Game.GetPiece(move.From)!.Index, move.To, depth);
    }
}