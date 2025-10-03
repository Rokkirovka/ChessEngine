using MyChess.Core;
using MyChess.Models;
using MyChess.Models.Moves;
using MyChessEngine.Core.Evaluation;
using MyChessEngine.Models;

namespace MyChessEngine.Core.Search;

public static class AlphaBetaSearch
{
    public static EngineResult Search(ChessGame game, SearchParameters searchParameters, Evaluator evaluator)
    {
        var nodesVisited = 0;
        ChessMove? bestMove = null;

        var moves =
            game.GetAllPossibleMoves()
                .OrderByDescending(move => evaluator.EvaluateMove(game, move));

        var alpha = -1_000_000;
        const int beta = 1_000_000;
        var color = game.CurrentPlayer == ChessColor.White ? 1 : -1;

        foreach (var move in moves)
        {
            game.MakeMove(move);
            var score = -Search(game, searchParameters with{Depth = searchParameters.Depth - 1}, evaluator, ref nodesVisited, -beta, -alpha, -color);
            game.UndoLastMove();

            if (score > alpha)
            {
                alpha = score;
                bestMove = move;
            }

            if (game.GetPiece(move.To) is null)
            {
                if (searchParameters.UseKillerMoves)
                    evaluator.UpdateKillerMoves(move, game.PlyCount);
                if (searchParameters.UseHistoryTable)
                    evaluator.UpdateHistoryTable(game.GetPiece(move.From)!.Index, move.To, searchParameters.Depth);
            }
        }

        return new EngineResult(alpha, bestMove, nodesVisited);
    }

    private static int Search(ChessGame game, SearchParameters searchParameters, Evaluator evaluator, ref int nodesVisited, int alpha, int beta, int color)
    {
        if (game.IsCheckmate)
        {
            nodesVisited++;
            return -100000 - searchParameters.Depth;
        }
        if (game.IsStalemate)
        {
            nodesVisited++;
            return 0;
        }
        if (searchParameters.Depth == 0)
        {
            if (!searchParameters.UseQuiescenceSearch) return Evaluator.EvaluatePosition(game.GetClonedBoard()) * color;
            return QuiescenceSearch.Search(game, searchParameters.Depth, evaluator, ref nodesVisited, alpha, beta, color);
        }

        if (searchParameters.Depth > 2 && !game.IsKingInCheck() && searchParameters.UseNullMovePruning)
        {
            game.SwapPlayers();
            var enPassantPiece = game.GetEnPassantTarget();
            game.SetEnPassantTarget(null);
            var score = -Search(game, searchParameters with{Depth = searchParameters.Depth - 3}, evaluator, ref nodesVisited, -beta, -beta + 1, -color);
            game.SetEnPassantTarget(enPassantPiece);
            game.SwapPlayers();
            if (score >= beta) return beta;
        }

        var moves = game.GetAllPossibleMoves()
            .OrderByDescending(move => evaluator.EvaluateMove(game, move));

        foreach (var move in moves)
        {
            game.MakeMove(move);
            var score = -Search(game, searchParameters with{Depth = searchParameters.Depth - 1}, evaluator, ref nodesVisited, -beta, -alpha, -color);
            game.UndoLastMove();

            if (score > alpha) alpha = score;
            if (alpha >= beta)
            {
                if (game.GetPiece(move.To) is null)
                {
                    if (searchParameters.UseKillerMoves)
                        evaluator.UpdateKillerMoves(move, game.PlyCount);
                    if (searchParameters.UseHistoryTable)
                        evaluator.UpdateHistoryTable(game.GetPiece(move.From)!.Index, move.To, searchParameters.Depth);
                }

                break;
            }
        }

        return alpha;
    }
}