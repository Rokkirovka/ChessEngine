using MyChess.Models.Moves;
using MyChessEngine.Models;

namespace MyChessEngine.Core.Search;

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