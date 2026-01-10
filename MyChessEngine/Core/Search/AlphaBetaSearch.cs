using MyChess.Hashing;
using MyChess.Models.Moves;
using MyChessEngine.Core.Debug;
using MyChessEngine.Core.Services;
using MyChessEngine.Models;
using MyChessEngine.Models.Debug;
using MyChessEngine.Transposition;

namespace MyChessEngine.Core.Search;

public static class AlphaBetaSearch
{
    public static int? SearchInternal(SearchContext context, int depthLeft, int alpha, int beta, int color)
    {
        if (context.SearchCanceler?.ShouldStop is true) return null;

        context.NodesVisited++;

        if (TerminalNodeChecker.IsTerminalNode(context.Game, out var terminalScore))
        {
            var adjustedScore = TerminalNodeChecker.AdjustScoreForDepth(terminalScore, depthLeft);
            if (context.Debugger?.IsEnabled == true)
            {
                context.Debugger.SetNodeInfo(isTerminalNode: true);
                context.Debugger.MarkTechnique(SearchTechnique.None);
            }
            return adjustedScore;
        }

        if (depthLeft == 0)
        {
            if (context.Debugger?.IsEnabled == true)
            {
                context.Debugger.MarkTechnique(SearchTechnique.QuiescenceSearch);
            }
            return QuiescenceSearch.Search(context, depthLeft, context.MoveOrderingService, alpha, beta, color);
        }

        var hash = ZobristHasher.CalculateInitialHash(context.Game.Board, context.Game.State);
        if (TranspositionService.TryGetBestMove(context, hash, depthLeft, alpha, beta, 
                out var ttScore, out var nodeType))
        {
            if (context.Debugger?.IsEnabled == true)
            {
                context.Debugger.MarkTechnique(SearchTechnique.TranspositionTable);
                context.Debugger.SetNodeInfo(positionHash: hash);
            }
            return HandleTranspositionResult(ttScore, nodeType, alpha, beta);
        }

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
        var moves = context.MoveOrderingService.OrderMoves(context.Game.Board, context.Game.Ply,
            context.Game.GetAllPossibleMoves());
        ChessMove? bestMoveInNode = null;
        var nodeType = NodeType.UpperBound;

        var moveIndex = 0;
        foreach (var move in moves)
        {
            if (context.SearchCanceler?.ShouldStop is true)
            {
                if (context.Debugger?.IsEnabled == true)
                {
                    context.Debugger.MarkBranchRejected(BranchRejectionReason.SearchCancelled, "Search was cancelled");
                }
                return null;
            }

            // Enter node for this move
            if (context.Debugger?.IsEnabled == true)
            {
                context.Debugger.EnterNode(move, depthLeft, alpha, beta, moveIndex, moves.Count());
                context.Debugger.SetNodeInfo(nullMovePlayed: context.NullMovePlayedInCurrentBranch, positionHash: hash);
            }

            var score = LateMoveReduction.SearchWithLmr(context, move, depthLeft, alpha, beta, color, moveIndex);

            if (score is null)
            {
                if (context.Debugger?.IsEnabled == true)
                {
                    context.Debugger.MarkBranchRejected(BranchRejectionReason.SearchCancelled, "Search returned null");
                    context.Debugger.ExitNode(null, null, null, false);
                }
                return null;
            }

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
                
                if (context.Debugger?.IsEnabled == true)
                {
                    context.Debugger.MarkTechnique(SearchTechnique.AlphaBetaPruning);
                    context.Debugger.MarkBranchRejected(BranchRejectionReason.AlphaBetaCutoff, 
                        $"Beta cutoff: score {score} >= beta {beta}");
                    context.Debugger.ExitNode(score, nodeType.ToString(), move, false);
                }
                break;
            }

            // Exit node normally (not pruned)
            if (context.Debugger?.IsEnabled == true)
            {
                context.Debugger.ExitNode(score, nodeType.ToString(), bestMoveInNode, true);
            }

            moveIndex++;
        }

        TranspositionService.Store(context, hash, alpha, depthLeft, bestMoveInNode, nodeType);
        return alpha;
    }
}