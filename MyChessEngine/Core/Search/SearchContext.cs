using MyChess.Core;
using MyChessEngine.Core.Evaluation;
using MyChessEngine.Models;

namespace MyChessEngine.Core.Search;

public class SearchContext(
    ChessGame game,
    SearchParameters parameters,
    Evaluator evaluator,
    PvTableManager pvTableManager,
    MoveOrderingService moveOrderingService)
{
    public ChessGame Game { get; } = game;
    public SearchParameters Parameters { get; } = parameters;
    public Evaluator Evaluator { get; } = evaluator;
    public int NodesVisited { get; set; }
    public PvTableManager PvTableManager { get; } = pvTableManager;
    public MoveOrderingService MoveOrderingService { get; } = moveOrderingService;
}

    