using MyChess.Models;
using MyChess.Models.Moves;
using MyChessEngine.Core.Debug;
using MyChessEngine.Core.Evaluation.Moves;
using MyChessEngine.Models;
using MyChessEngine.Models.Debug;
using MyChessEngine.Transposition;

namespace MyChessEngine.Core.Search;

public class SearchOrchestrator(MoveOrderingService moveOrderingService)
{
    public EngineResult? FindBestMove(SearchContext context)
    {
        TranspositionService.IncrementAge();
        var game = context.Game;
        var searchParameters = context.Parameters;

        // Start debug session if enabled
        context.Debugger?.StartSearch(searchParameters.Depth);
        if (context.Debugger?.IsEnabled == true)
        {
            context.Debugger.EnterNode(null, searchParameters.Depth, -1_000_000, 1_000_000, 0, 
                game.GetAllPossibleMoves().Count());
        }

        var moves = moveOrderingService.OrderMoves(game.Board, game.Ply, game.GetAllPossibleMoves());

        var alpha = -1_000_000;
        const int beta = 1_000_000;
        var color = game.CurrentPlayer == ChessColor.White ? 1 : -1;
        ChessMove? bestMove = null;

        var moveIndex = 0;
        foreach (var move in moves)
        {
            if (context.SearchCanceler?.ShouldStop is true)
            {
                context.Debugger?.MarkBranchRejected(BranchRejectionReason.SearchCancelled, "Search was cancelled");
                return null;
            }
            
            game.MakeMove(move);
            var score = -AlphaBetaSearch.SearchInternal(context, searchParameters.Depth - 1, -beta, -alpha, -color);
            game.UndoLastMove();

            if (score is null)
            {
                context.Debugger?.MarkBranchRejected(BranchRejectionReason.SearchCancelled, "Search returned null");
                return null;
            }
            if (context.SearchCanceler?.ShouldStop is true)
            {
                context.Debugger?.MarkBranchRejected(BranchRejectionReason.SearchCancelled, "Search was cancelled");
                return null;
            }

            if (score > alpha)
            {
                alpha = score.Value;
                bestMove = move;
                context.PvTableService.UpdatePvLine(move, 0);
            }

            moveOrderingService.UpdateHeuristics(context, move, searchParameters.Depth);
            moveIndex++;
        }

        // Complete debug session
        if (context.Debugger?.IsEnabled == true)
        {
            context.Debugger.ExitNode(alpha * color, "Root", bestMove, true);
        }

        return new EngineResult(
            alpha * color, 
            bestMove, 
            context.NodesVisited, 
            context.PvTableService.GetPrincipalVariation()
        );
    }
}