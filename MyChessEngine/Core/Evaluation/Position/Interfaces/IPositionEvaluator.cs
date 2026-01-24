using MyChess.Core.Board;

namespace MyChessEngine.Core.Evaluation.Position.Interfaces;

public interface IPositionEvaluator
{
    int Evaluate(ChessBoard board);
}