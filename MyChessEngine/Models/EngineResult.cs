using MyChess.Models.Moves;

namespace MyChessEngine.Models;

public class EngineResult(int score, ChessMove? bestMove, int nodesVisited, ChessMove?[] principalVariation)
{
    public int Score { get; } = score;
    public ChessMove? BestMove { get; } = bestMove;
    public int NodesVisited { get; } = nodesVisited;
    public readonly ChessMove?[] PrincipalVariation = principalVariation;
}