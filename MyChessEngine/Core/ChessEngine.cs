using MyChess.Core;
using MyChessEngine.Core.Evaluation;
using MyChessEngine.Core.Search;
using MyChessEngine.Models;

namespace MyChessEngine.Core;

public class ChessEngine
{
    private readonly Evaluator _evaluator = new();
    private readonly MoveOrderingService _moveOrderingService;
    private readonly SearchOrchestrator _searchOrchestrator;
    private readonly IterativeDeepeningSearch _iterativeDeepeningSearch;

    public ChessEngine()
    {
        _moveOrderingService = new MoveOrderingService(_evaluator);
        _searchOrchestrator = new SearchOrchestrator(_evaluator);
        _iterativeDeepeningSearch = new IterativeDeepeningSearch(_searchOrchestrator);
    }

    public EngineResult FindBestMove(ChessGame game, SearchParameters searchParameters)
    {
        var context = new SearchContext(game, searchParameters, _evaluator, new PvTableManager(searchParameters.Depth), _moveOrderingService);
        return _searchOrchestrator.FindBestMove(context);
    }

    public EngineResult FindBestMoveWithIterativeDeepening(
        ChessGame game, 
        SearchParameters searchParameters,
        ISearchProgressReporter? progressReporter = null)
    {
        if (progressReporter != null)
        {
            _iterativeDeepeningSearch.SetProgressReporter(progressReporter);
        }
        
        var context = new SearchContext(game, searchParameters, _evaluator, new PvTableManager(searchParameters.Depth), _moveOrderingService);
        return _iterativeDeepeningSearch.FindBestMove(context);
    }
}