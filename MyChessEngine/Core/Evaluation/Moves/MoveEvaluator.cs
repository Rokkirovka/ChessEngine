using MyChess.Core.Board;
using MyChess.Models.Moves;
using MyChessEngine.Core.Evaluation.Moves.Components;

namespace MyChessEngine.Core.Evaluation.Moves;

public class MoveEvaluator(
    KillerMovesService killerMovesService, 
    HistoryTableService historyTableService)
{
    public int EvaluateMove(ChessBoard board, int ply, ChessMove move)
    {
        var movingPiece = board.GetPiece(move.From);
        if (movingPiece == null) return 0;
        var targetPiece = board.GetPiece(move.To);
        if (targetPiece is not null) return MvvLvaService.GetScore(movingPiece.Index, targetPiece.Index);
        var killerScore = killerMovesService.TryGetScore(move, ply);
        if (killerScore is not null) return killerScore.Value;
        return historyTableService.GetScore(movingPiece.Index, move.To);
    }
}