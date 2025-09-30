using MyChess.Core;
using MyChess.Models.Moves;

namespace MyChessEngine.Core.Evaluation;

public interface IEvaluator
{
    int Score { get; set; }
    void Initialize(ChessGame game);
    void UpdatePosition(ChessMove move);
    void RemoveCellsScore(IEnumerable<int> cells);
    void UpdateScore(IEnumerable<int> cells);
    int Evaluate();
}