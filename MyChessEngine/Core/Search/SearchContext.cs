using MyChess.Core;
using MyChessEngine.Core.Evaluation;
using MyChessEngine.Models;

namespace MyChessEngine.Core.Search;

public class SearchContext(
    ChessGame game,
    SearchParameters parameters,
    Evaluator evaluator,
    TranspositionService transpositionService,
    MoveOrderingService moveOrderingService)
{
    public ChessGame Game { get; } = game;
    public SearchParameters Parameters { get; } = parameters;
    public Evaluator Evaluator { get; } = evaluator;
    public int NodesVisited { get; set; }
    public PvTableManager PvTableManager { get; } = new(parameters.Depth);
    public TranspositionService TranspositionService { get; } = transpositionService;
    public MoveOrderingService MoveOrderingService { get; } = moveOrderingService;
}

    