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
        var pvArray = new ChessMove[searchParameters.Depth * (searchParameters.Depth + 1) / 2];
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
            var score = -Search(game, searchParameters,  evaluator, pvArray, searchParameters.Depth - 1, ref nodesVisited, -beta, -alpha, -color);
            game.UndoLastMove();

            if (score > alpha)
            {
                alpha = score;
                bestMove = move;
                
                pvArray[0] = move;
                for (var i = 1; i < searchParameters.Depth; i++)
                    pvArray[i] = pvArray[i + searchParameters.Depth - 1];
            }

            if (game.GetPiece(move.To) is null)
            {
                if (searchParameters.UseKillerMoves)
                    evaluator.UpdateKillerMoves(move, game.Ply);
                if (searchParameters.UseHistoryTable)
                    evaluator.UpdateHistoryTable(game.GetPiece(move.From)!.Index, move.To, searchParameters.Depth);
            }
        }

        return new EngineResult(alpha * color, bestMove, nodesVisited, pvArray[..searchParameters.Depth]);
    }

    private static int Search(
        ChessGame game, 
        SearchParameters searchParameters, 
        Evaluator evaluator, 
        ChessMove[] pvArray,
        int currentDepth,
        ref int nodesVisited, int alpha, int beta, int color)
    {
        if (game.IsCheckmate)
        {
            nodesVisited++;
            return -100000 - currentDepth;
        }
        if (game.IsStalemate)
        {
            nodesVisited++;
            return 0;
        }
        if (currentDepth == 0)
        {
            if (!searchParameters.UseQuiescenceSearch) return Evaluator.EvaluatePosition(game.GetClonedBoard()) * color;
            return QuiescenceSearch.Search(game, currentDepth, evaluator, ref nodesVisited, alpha, beta, color);
        }

        if (currentDepth > 2 && !game.IsKingInCheck() && searchParameters.UseNullMovePruning)
        {
            game.SwapPlayers();
            var enPassantPiece = game.GetEnPassantTarget();
            game.SetEnPassantTarget(null);
            var score = -Search(game, searchParameters, evaluator, pvArray, currentDepth - 3, ref nodesVisited, -beta, -beta + 1, -color);
            game.SetEnPassantTarget(enPassantPiece);
            game.SwapPlayers();
            if (score >= beta) return beta;
        }

        var moves = game.GetAllPossibleMoves()
            .OrderByDescending(move => evaluator.EvaluateMove(game, move));

        foreach (var move in moves)
        {
            game.MakeMove(move);
            var score = -Search(game, searchParameters, evaluator, pvArray, currentDepth - 1, ref nodesVisited, -beta, -alpha, -color);
            game.UndoLastMove();

            if (score > alpha)
            {
                alpha = score;

                var row = searchParameters.Depth - currentDepth;
                var moveIndex = row * (2 * searchParameters.Depth - row + 1) / 2;
                pvArray[moveIndex] = move;
                for (var i = moveIndex + 1; i < moveIndex + searchParameters.Depth - row; i++)
                    pvArray[i] = pvArray[i + searchParameters.Depth - row - 1];
            }
            if (alpha >= beta)
            {
                if (game.GetPiece(move.To) is null)
                {
                    if (searchParameters.UseKillerMoves)
                        evaluator.UpdateKillerMoves(move, game.Ply);
                    if (searchParameters.UseHistoryTable)
                        evaluator.UpdateHistoryTable(game.GetPiece(move.From)!.Index, move.To, currentDepth);
                }

                break;
            }
        }

        return alpha;
    }
}