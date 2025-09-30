using MyChess.Core;
using MyChessEngine.Core.Search;
using MyChessEngine.Core.Evaluation;
using MyChessEngine.Models;

namespace MyChessEngine.Core;

public class Engine
{
    private readonly AlphaBetaSearch _searchAlgorithm;
    private readonly SearchParameters _searchParameters;

    public Engine(ChessGame game, SearchParameters searchParameters)
    {
        var evaluator = new StandardEvaluator(game);
        _searchAlgorithm = new AlphaBetaSearch(game, evaluator);
        _searchParameters = searchParameters;
        _searchAlgorithm.Initialize(game);
    }
    
    public Engine(ChessGame game) : this(game, new SearchParameters())
    {
        
    }

    public void UpdateGame(ChessGame game)
    {
        _searchAlgorithm.Initialize(game);
    }

    public EngineResult FindBestMove(int? depth = null)
    {
        var parameters = _searchParameters with
        {
            Depth = depth ?? _searchParameters.Depth
        };

        return _searchAlgorithm.Search(parameters);
    }

    public EngineResult FindBestMove(SearchParameters parameters)
    {
        return _searchAlgorithm.Search(parameters);
    }

    public void StopSearch()
    {
        _searchAlgorithm.StopSearch();
    }

    public SearchDiagnostics GetDiagnostics()
    {
        return _searchAlgorithm.GetDiagnostics();
    }
}