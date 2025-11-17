namespace MyChessEngine.Core.Search;

public static class NullMovePruning
{
    public static bool TryNullMovePruning(SearchContext context, int currentDepth, int beta, int color, out int score)
    {
        score = 0;
        
        if (currentDepth <= 2 || 
            context.Game.IsKingInCheck() || 
            !context.Parameters.UseNullMovePruning)
            return false;

        context.Game.SwapPlayers();
        var enPassantPiece = context.Game.GetEnPassantTarget();
        context.Game.SetEnPassantTarget(null);

        score = -AlphaBetaSearch.SearchInternal(context, currentDepth - 3, -beta, -beta + 1, -color);

        context.Game.SetEnPassantTarget(enPassantPiece);
        context.Game.SwapPlayers();
        
        return score >= beta;
    }
}