using MyChess.Core;
using MyChessEngine.Core.Evaluation;
using MyChessEngine.Core.Search;
using MyChessEngine.Models;

namespace MyChessEngine.Core;

public class ChessEngine
{
    private readonly Evaluator _evaluator = new();
    private readonly SearchOrchestrator _searchOrchestrator;

    public ChessEngine()
    {
        _searchOrchestrator = new SearchOrchestrator(_evaluator);
    }

    public EngineResult FindBestMove(ChessGame game, SearchParameters searchParameters)
    {
        return _searchOrchestrator.FindBestMove(game, searchParameters);
    }
}