using MyChess.Core;

namespace MyChessEngine.Core.Evaluation.Position.Interfaces;

public interface IEvaluationComponent
{
    int Evaluate(ChessBoard board);
}