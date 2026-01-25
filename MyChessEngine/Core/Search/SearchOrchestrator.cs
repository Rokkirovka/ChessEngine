using MyChess.Models;
using MyChess.Models.Moves;
using MyChessEngine.Core.Evaluation.Moves;
using MyChessEngine.Models;
using MyChessEngine.Transposition;

namespace MyChessEngine.Core.Search;

public class SearchOrchestrator(MoveOrderingService moveOrderingService)
{
    public EngineResult? FindBestMove(SearchContext context)
    {
        TranspositionService.IncrementAge();
        var game = context.Game;
        var searchParameters = context.Parameters;

        var moves = moveOrderingService.OrderMoves(game.Board, game.Ply, game.GetAllPossibleMoves(), context.PrincipalVariation?[0]);

        var alpha = -1_000_000;
        const int beta = 1_000_000;
        var color = game.CurrentPlayer == ChessColor.White ? 1 : -1;
        ChessMove? bestMove = null;

        var moveIndex = 0;
        foreach (var move in moves)
        {
            if (context.SearchCanceler?.MustStop is true) return null;
            game.MakeMove(move);
            var score = -AlphaBetaSearch.SearchInternal(context, searchParameters.Depth - 1, -beta, -alpha, -color, move, moveIndex);
            game.UndoLastMove();

            if (score is null) return null;
            if (context.SearchCanceler?.MustStop is true) return null;

            if (score > alpha)
            {
                alpha = score.Value;
                bestMove = move;
                context.PvTableService.UpdatePvLine(move, 0);
            }

            moveOrderingService.UpdateHeuristics(context, move, searchParameters.Depth);
            moveIndex++;
        }

        return new EngineResult(
            alpha, 
            bestMove, 
            context.NodesVisited, 
            context.PvTableService.GetPrincipalVariation()
        );
    }
}