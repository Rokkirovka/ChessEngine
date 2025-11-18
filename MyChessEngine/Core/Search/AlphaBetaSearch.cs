using MyChess.Hashing;
using MyChess.Models.Moves;
using MyChessEngine.Transposition;

namespace MyChessEngine.Core.Search;

public static class AlphaBetaSearch
{
    public static int SearchInternal(SearchContext context, int currentDepth, int alpha, int beta, int color)
    {
        context.NodesVisited++;

        if (TerminalNodeChecker.IsTerminalNode(context.Game, out var terminalScore))
            return TerminalNodeChecker.AdjustScoreForDepth(terminalScore, currentDepth);

        if (currentDepth == 0)
            return QuiescenceSearch.Search(context, currentDepth, context.Evaluator, alpha, beta, color);

        var hash = ZobristHasher.CalculateInitialHash(context.Game.Board, context.Game.State);
        if (TranspositionService.TryGetBestMove(context, hash, currentDepth, alpha, beta, out var ttScore, out var nodeType))
            return HandleTranspositionResult(ttScore, nodeType, alpha, beta);

        return NullMovePruning.TryNullMovePruning(context, currentDepth, beta, color, out var nullMoveScore)
            ? nullMoveScore
            : SearchMoves(context, currentDepth, alpha, beta, color, hash);
    }

    private static int HandleTranspositionResult(int score, NodeType nodeType, int alpha, int beta)
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

    private static int SearchMoves(SearchContext context, int currentDepth, int alpha, int beta, int color, ulong hash)
    {
        var moves = context.MoveOrderingService.OrderMoves(context.Game, context.Game.GetAllPossibleMoves());
        ChessMove? bestMoveInNode = null;
        var nodeType = NodeType.UpperBound;

        var moveIndex = 0;
        foreach (var move in moves)
        {
            var score = LateMoveReduction.SearchWithLmr(context, move, currentDepth, alpha, beta, color, moveIndex);

            if (score > alpha)
            {
                alpha = score;
                bestMoveInNode = move;
                nodeType = NodeType.Exact;
                context.PvTableManager.UpdatePvLine(move, currentDepth, context.Parameters.Depth);
            }

            if (alpha >= beta)
            {
                context.MoveOrderingService.UpdateHeuristics(context, move, currentDepth);
                nodeType = NodeType.LowerBound;
                break;
            }

            moveIndex++;
        }
        TranspositionService.Store(context, hash, alpha, currentDepth, bestMoveInNode, nodeType);
        return alpha;
    }
}