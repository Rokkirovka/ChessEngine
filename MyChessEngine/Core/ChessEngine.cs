using MyChess.Core;
using MyChessEngine.Core.Evaluation;
using MyChessEngine.Core.Search;
using MyChessEngine.Models;

namespace MyChessEngine.Core;

public class ChessEngine
{
    private readonly Evaluator _evaluator = new();
    private readonly MoveOrderingService _moveOrderingService;
    private readonly IterativeDeepeningSearch _iterativeDeepeningSearch;

    public ChessEngine()
    {
        _moveOrderingService = new MoveOrderingService(_evaluator);
        var searchOrchestrator = new SearchOrchestrator(_evaluator);
        _iterativeDeepeningSearch = new IterativeDeepeningSearch(searchOrchestrator);
    }

    public EngineResult FindBestMoveWithIterativeDeepening(
        ChessGame game, 
        SearchParameters searchParameters,
        ISearchProgressReporter? progressReporter = null,
        SearchCanceler? searchCanceler = null)
    {
        if (progressReporter != null) _iterativeDeepeningSearch.SetProgressReporter(progressReporter);
        if (searchCanceler != null) _iterativeDeepeningSearch.SetSearchCanceler(searchCanceler);
        
        var context = new SearchContext(game, searchParameters, _evaluator, new PvTableManager(searchParameters.Depth), _moveOrderingService);
        return _iterativeDeepeningSearch.FindBestMove(context);
    }
}