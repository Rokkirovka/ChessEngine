using MyChess.Core;
using MyChessEngine.Core;
using MyChessEngine.Core.Evaluation;
using MyChessEngine.Core.Search;
using MyChessEngine.Core.Services;

namespace MyChessEngine.Models;

public class SearchContext(
    ChessGame game,
    SearchParameters parameters,
    Evaluator evaluator,
    PvTableService pvTableService,
    MoveOrderingService moveOrderingService,
    SearchCanceler? searchCanceler = null)
{
    public ChessGame Game { get; } = game;
    public SearchParameters Parameters { get; } = parameters;
    public Evaluator Evaluator { get; } = evaluator;
    public int NodesVisited { get; set; }
    public PvTableService PvTableService { get; } = pvTableService;
    public MoveOrderingService MoveOrderingService { get; } = moveOrderingService;
    public SearchCanceler? SearchCanceler { get; set; } = searchCanceler;
    public bool NullMovePlayedInCurrentBranch { get; set; }
}

    