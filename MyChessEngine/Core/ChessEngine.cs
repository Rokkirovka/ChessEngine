using MyChess.Core;
using MyChessEngine.Core.Evaluation;
using MyChessEngine.Core.Search;
using MyChessEngine.Models;

namespace MyChessEngine.Core;

public class ChessEngine
{
    private readonly Evaluator _evaluator = new();
    public EngineResult FindBestMove(ChessGame game, SearchParameters searchParameters)
    {
        return AlphaBetaSearch.Search(game, searchParameters, _evaluator);
    }
}