using MyChess.Models;
using MyChess.Models.Moves;
using MyChessEngine.Core.Evaluation;
using MyChessEngine.Core.Services;
using MyChessEngine.Models;
using MyChessEngine.Transposition;

namespace MyChessEngine.Core.Search;

public class SearchOrchestrator(Evaluator evaluator)
{
    private readonly MoveOrderingService _moveOrderingService = new(evaluator);

    public EngineResult? FindBestMove(SearchContext context)
    {
        TranspositionService.IncrementAge();
        var game = context.Game;
        var searchParameters = context.Parameters;

        var moves = _moveOrderingService.OrderMoves(game, game.GetAllPossibleMoves());

        var alpha = -1_000_000;
        const int beta = 1_000_000;
        var color = game.CurrentPlayer == ChessColor.White ? 1 : -1;
        ChessMove? bestMove = null;

        foreach (var move in moves)
        {
            if (context.SearchCanceler?.ShouldStop is true) return null;
            game.MakeMove(move);
            var score = -AlphaBetaSearch.SearchInternal(context, searchParameters.Depth - 1, -beta, -alpha, -color);
            game.UndoLastMove();

            if (score is null) return null; // Поиск был отменен
            if (context.SearchCanceler?.ShouldStop is true) return null;

            if (score > alpha)
            {
                alpha = score.Value;
                bestMove = move;
                context.PvTableService.UpdatePvLine(move, 0);
            }

            _moveOrderingService.UpdateHeuristics(context, move, searchParameters.Depth);
        }

        return new EngineResult(
            alpha * color, 
            bestMove, 
            context.NodesVisited, 
            context.PvTableService.GetPrincipalVariation()
        );
    }
}