using MyChessEngine.Models;

namespace MyChessEngine.Core.Search;

public static class NullMovePruning
{
    public static bool TryNullMovePruning(SearchContext context, int currentDepth, int beta, int color, out int? score)
    {
        score = null;
        
        if (context.SearchCanceler?.ShouldStop is true) return false;
        
        if (currentDepth <= 2 || 
            context.Game.IsKingInCheck() || 
            !context.Parameters.UseNullMovePruning ||
            context.NullMovePlayedInCurrentBranch)
            return false;
        
        context.NullMovePlayedInCurrentBranch = true;
        context.Game.SwapPlayers();
        var enPassantPiece = context.Game.GetEnPassantTarget();
        context.Game.SetEnPassantTarget(null);

        var searchResult = -AlphaBetaSearch.SearchInternal(context, currentDepth - 3, -beta, -beta + 1, -color);
        score = searchResult;

        context.Game.SetEnPassantTarget(enPassantPiece);
        context.Game.SwapPlayers();
        
        context.NullMovePlayedInCurrentBranch = false;
        
        if (context.SearchCanceler?.ShouldStop is true) return false;
        if (score is null) return false;
        
        return score >= beta;
    }
}