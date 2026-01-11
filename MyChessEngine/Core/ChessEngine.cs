using MyChess.Core;
using MyChessEngine.Core.Evaluation.Moves;
using MyChessEngine.Core.Evaluation.Moves.Components;
using MyChessEngine.Core.Search;
using MyChessEngine.Core.Services;
using MyChessEngine.Models;

namespace MyChessEngine.Core;

public class ChessEngine
{
    private readonly MoveOrderingService _moveOrderingService;
    private readonly IterativeDeepeningSearch _iterativeDeepeningSearch;

    public ChessEngine()
    {
        _moveOrderingService = new MoveOrderingService(new KillerMovesService(), new HistoryTableService());
        var searchOrchestrator = new SearchOrchestrator(_moveOrderingService);
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

        var context = new SearchContext(game, searchParameters, new PvTableService(searchParameters.Depth),
            _moveOrderingService, searchCanceler);
        return _iterativeDeepeningSearch.FindBestMove(context);
    }
}