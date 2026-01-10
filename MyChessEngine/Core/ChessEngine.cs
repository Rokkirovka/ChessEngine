using MyChess.Core;
using MyChessEngine.Core.Debug;
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
    private SearchDebuggerService? _debuggerService;

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

        SearchDebugger? debugger = null;
        if (searchParameters.EnableDebugger)
        {
            _debuggerService ??= new SearchDebuggerService();
            _debuggerService.SetEnabled(true);
            debugger = _debuggerService.Debugger;
        }

        var context = new SearchContext(game, searchParameters, new PvTableService(searchParameters.Depth),
            _moveOrderingService, searchCanceler, debugger);
        return _iterativeDeepeningSearch.FindBestMove(context);
    }

    /// <summary>
    /// Gets the debugger service if debugging was enabled.
    /// </summary>
    public SearchDebuggerService? GetDebuggerService() => _debuggerService;
}