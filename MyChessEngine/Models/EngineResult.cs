using MyChess.Models.Moves;

namespace MyChessEngine.Models;

public class EngineResult(int score, ChessMove? bestMove)
{
    public int Score { get; } = score;
    public ChessMove? BestMove { get; } = bestMove;
}