using MyChess.Models.Moves;
using MyChessEngine.Models;

namespace MyChessEngine.Core.Search;

public static class NullMovePruning
{
    public static bool TryNullMovePruning(SearchContext context, int currentDepth, int beta, int color)
    {
        if (context.SearchCanceler?.ShouldStop is true) return false;
        
        if (currentDepth <= 2 || 
            context.Game.IsKingInCheck() || 
            !context.Parameters.UseNullMovePruning ||
            context.NullMovePlayedInCurrentBranch)
            return false;
        
        context.NullMovePlayedInCurrentBranch = true;
        context.Game.MakeMove(new NullMove());
        var searchResult = -AlphaBetaSearch.SearchInternal(context, currentDepth - 3, -beta, -beta + 1, -color);
        context.Game.UndoLastMove();
        
        context.NullMovePlayedInCurrentBranch = false;
        
        if (context.SearchCanceler?.ShouldStop is true) return false;

        return searchResult >= beta;
    }
}