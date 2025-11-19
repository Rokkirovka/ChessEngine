using MyChessEngine.Models;

namespace MyChessEngine.Core.Search;

public class IterativeDeepeningSearch(SearchOrchestrator searchOrchestrator)
{
    private ISearchProgressReporter? _progressReporter;

    public void SetProgressReporter(ISearchProgressReporter reporter)
    {
        _progressReporter = reporter;
    }
    
    public EngineResult FindBestMove(SearchContext baseContext)
    {
        EngineResult? bestResult = null;
        
        for (var depth = 1; depth <= baseContext.Parameters.Depth; depth++)
        {
            var iterationContext = new SearchContext(
                baseContext.Game,
                baseContext.Parameters with { Depth = depth },
                baseContext.Evaluator,
                baseContext.PvTableManager,
                baseContext.MoveOrderingService
            )
            {
                NodesVisited = 0
            };
            var currentResult = searchOrchestrator.FindBestMove(iterationContext);
            
            bestResult = currentResult;
            
            _progressReporter?.OnIterationComplete(
                depth, 
                currentResult.Score, 
                currentResult.BestMove!, 
                currentResult.PrincipalVariation, 
                currentResult.NodesVisited
            );
        }
        
        var finalResult = bestResult ?? throw new InvalidOperationException("No search results");
        _progressReporter?.OnSearchFinished(finalResult);
        
        return finalResult;
    }
}