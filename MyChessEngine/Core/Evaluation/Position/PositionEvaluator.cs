using MyChess.Core;
using MyChessEngine.Core.Evaluation.Position.EvaluationComponents;
using MyChessEngine.Core.Evaluation.Position.Interfaces;

namespace MyChessEngine.Core.Evaluation.Position;

public class PositionEvaluator : IPositionEvaluator
{
    private readonly List<IEvaluationComponent> _components =
    [
        new MaterialEvaluator(),
        new PawnStructureEvaluator(),
        new PieceActivityEvaluator(),
        new KingSafetyEvaluator()
    ];

    public int Evaluate(ChessBoard board)
    {
        return _components.Sum(component => component.Evaluate(board));
    }
}