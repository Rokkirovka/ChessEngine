using MyChess.Core;
using MyChessEngine.Core.Evaluation;
using MyChessEngine.Core.Search;
using MyChessEngine.Core.Services;
using MyChessEngine.Models;

namespace MyChessEngine.Core;

public class ChessEngine
{
    private readonly MoveEvaluator _moveEvaluator = new();
    private readonly MoveOrderingService _moveOrderingService;
    private readonly IterativeDeepeningSearch _iterativeDeepeningSearch;

    public ChessEngine()
    {
        _moveOrderingService = new MoveOrderingService(_moveEvaluator);
        var searchOrchestrator = new SearchOrchestrator(_moveEvaluator);
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
        
        var context = new SearchContext(game, searchParameters, _moveEvaluator, new PvTableService(searchParameters.Depth), _moveOrderingService);
        return _iterativeDeepeningSearch.FindBestMove(context);
    }
}