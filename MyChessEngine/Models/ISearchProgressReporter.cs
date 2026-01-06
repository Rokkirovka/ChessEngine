using MyChess.Models.Moves;

namespace MyChessEngine.Models;

public interface ISearchProgressReporter
{
    void OnIterationComplete(
        int depth, 
        int score, 
        ChessMove bestMove, 
        ChessMove[] principalVariation,
        long nodesVisited);

    void OnSearchFinished(EngineResult finalResult);
}