using MyChess.Core.Board;
using MyChess.Models;
using MyChessEngine.Core.Evaluation.Position.Interfaces;
using MyChessEngine.Utils;

namespace MyChessEngine.Core.Evaluation.Position.EvaluationComponents;

public class MaterialEvaluator : IEvaluationComponent
{
    public int Evaluate(ChessBoard board)
    {
        var score = 0;

        for (var cell = 0; cell < 64; cell++)
        {
            var piece = board.GetPiece(cell);
            if (piece == null) continue;

            var factor = piece.Color == ChessColor.White ? 1 : -1;
            var tableIndex = piece.Color == ChessColor.White ? cell : PieceSquareTables.MirrorSquare(cell);

            score += PieceSquareTables.GetPieceTable(piece)[tableIndex] * factor;
            score += PieceSquareTables.GetPiecePrice(piece) * factor;
        }

        return score;
    }
}