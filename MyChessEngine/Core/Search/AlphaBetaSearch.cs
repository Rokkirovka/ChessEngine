using MyChess.Hashing;
using MyChess.Models.Moves;
using MyChessEngine.Core.Services;
using MyChessEngine.Models;
using MyChessEngine.Transposition;

namespace MyChessEngine.Core.Search;

public static class AlphaBetaSearch
{
    public static int? SearchInternal(SearchContext context, int depthLeft, int alpha, int beta, int color)
    {
        if (context.SearchCanceler?.ShouldStop is true) return null;
        
        context.NodesVisited++;

        if (TerminalNodeChecker.IsTerminalNode(context.Game, out var terminalScore))
            return TerminalNodeChecker.AdjustScoreForDepth(terminalScore, depthLeft);

        if (depthLeft == 0)
            return QuiescenceSearch.Search(context, depthLeft, context.Evaluator, alpha, beta, color);

        var hash = ZobristHasher.CalculateInitialHash(context.Game.Board, context.Game.State);
        if (TranspositionService.TryGetBestMove(context, hash, depthLeft, alpha, beta, out var ttScore, out var nodeType))
            return HandleTranspositionResult(ttScore, nodeType, alpha, beta);

        return NullMovePruning.TryNullMovePruning(context, depthLeft, beta, color, out var nullMoveScore)
            ? nullMoveScore
            : SearchMoves(context, depthLeft, alpha, beta, color, hash);
    }

    private static int? HandleTranspositionResult(int score, NodeType nodeType, int alpha, int beta)
    {
        switch (nodeType)
        {
            case NodeType.Exact: return score;
            case NodeType.LowerBound:
                if (score > alpha) alpha = score;
                break;
            case NodeType.UpperBound:
                beta = Math.Min(beta, score);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(nodeType), nodeType, null);
        }

        return alpha >= beta ? score : alpha;
    }

    private static int? SearchMoves(SearchContext context, int depthLeft, int alpha, int beta, int color, ulong hash)
    {
        var moves = context.MoveOrderingService.OrderMoves(context.Game, context.Game.GetAllPossibleMoves());
        ChessMove? bestMoveInNode = null;
        var nodeType = NodeType.UpperBound;

        var moveIndex = 0;
        foreach (var move in moves)
        {
            if (context.SearchCanceler?.ShouldStop is true) return null;
            
            var score = LateMoveReduction.SearchWithLmr(context, move, depthLeft, alpha, beta, color, moveIndex);
            
            if (score is null) return null;

            if (score > alpha)
            {
                alpha = score.Value;
                bestMoveInNode = move;
                nodeType = NodeType.Exact;
                context.PvTableService.UpdatePvLine(move, context.Parameters.Depth - depthLeft);
            }

            if (alpha >= beta)
            {
                context.MoveOrderingService.UpdateHeuristics(context, move, depthLeft);
                nodeType = NodeType.LowerBound;
                break;
            }

            moveIndex++;
        }
        
        TranspositionService.Store(context, hash, alpha, depthLeft, bestMoveInNode, nodeType);
        return alpha;
    }
}