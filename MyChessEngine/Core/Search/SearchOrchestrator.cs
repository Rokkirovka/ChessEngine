using MyChess.Core;
using MyChess.Models;
using MyChess.Models.Moves;
using MyChessEngine.Core.Evaluation;
using MyChessEngine.Models;

namespace MyChessEngine.Core.Search;

public class SearchOrchestrator(Evaluator evaluator)
{
    private readonly TranspositionService _transpositionService = new();
    private readonly MoveOrderingService _moveOrderingService = new(evaluator);

    public EngineResult FindBestMove(ChessGame game, SearchParameters searchParameters)
    {
        var context = new SearchContext(game, searchParameters, evaluator,
            _transpositionService, _moveOrderingService);
        TranspositionService.IncrementAge();

        var moves = _moveOrderingService.OrderMoves(game, game.GetAllPossibleMoves());

        var alpha = -1_000_000;
        const int beta = 1_000_000;
        var color = game.CurrentPlayer == ChessColor.White ? 1 : -1;
        ChessMove? bestMove = null;

        foreach (var move in moves)
        {
            game.MakeMove(move);
            var score = -AlphaBetaSearch.SearchInternal(context, searchParameters.Depth - 1, -beta, -alpha, -color);
            game.UndoLastMove();

            if (score > alpha)
            {
                alpha = score;
                bestMove = move;
                context.PvTableManager.UpdatePvLine(move, 0, searchParameters.Depth);
            }

            _moveOrderingService.UpdateHeuristics(game, move, searchParameters.Depth, game.Ply);
        }

        return new EngineResult(
            alpha * color, 
            bestMove, 
            context.NodesVisited, 
            context.PvTableManager.GetPrincipalVariation(searchParameters.Depth)
        );
    }
}