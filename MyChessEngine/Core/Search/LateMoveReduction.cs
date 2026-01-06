using MyChess.Core;
using MyChess.Models.Moves;
using MyChessEngine.Models;

namespace MyChessEngine.Core.Search;

public static class LateMoveReduction
{
    private const int FullDepthMoves = 4;
    private const int ReductionDepth = 1;
    private const int MinDepthForReduction = 3;

    public static int? SearchWithLmr(SearchContext context, ChessMove move, int currentDepth, 
        int alpha, int beta, int color, int moveIndex)
    {
        if (context.SearchCanceler?.ShouldStop is true) return null;
        
        var isCheck = context.Game.IsKingInCheck();
        var shouldReduce = ShouldReduceMove(context, move, context.Game, moveIndex, isCheck, currentDepth);

        if (!shouldReduce)
        {
            return SearchWithFullDepth(context, move, currentDepth, alpha, beta, color);
        }

        var reducedScore = SearchWithReducedDepth(context, move, currentDepth, alpha, color);
        
        if (reducedScore is null) return null;
        if (context.SearchCanceler?.ShouldStop is true) return null;
        
        return reducedScore > alpha ? SearchWithFullDepth(context, move, currentDepth, alpha, beta, color) : reducedScore;
    }

    private static bool ShouldReduceMove(SearchContext context, ChessMove move, ChessGame game, int moveIndex,
        bool isCheck, int currentDepth)
    {
        if (context.SearchCanceler?.ShouldStop is true) return false;
        if (context.Parameters.UseLateMoveReduction is false) return false;
        if (currentDepth < MinDepthForReduction) return false;
        if (moveIndex < FullDepthMoves) return false;
        if (game.GetPiece(move.To) != null) return false;
        if (isCheck) return false;
        return move is not PromotionMove;
    }

    private static int? SearchWithReducedDepth(SearchContext context, ChessMove move,
        int currentDepth, int alpha, int color)
    {
        context.Game.MakeMove(move);
        var reducedScore = -AlphaBetaSearch.SearchInternal(context, currentDepth - 1 - ReductionDepth,
            -alpha - 1, -alpha, -color);
        context.Game.UndoLastMove();
        return reducedScore;
    }

    private static int? SearchWithFullDepth(SearchContext context, ChessMove move,
        int currentDepth, int alpha, int beta, int color)
    {
        context.Game.MakeMove(move);
        var fullScore = -AlphaBetaSearch.SearchInternal(context, currentDepth - 1,
            -beta, -alpha, -color);
        context.Game.UndoLastMove();
        return fullScore;
    }
}