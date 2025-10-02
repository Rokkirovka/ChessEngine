using MyChess.Core;
using MyChessEngine.Core.Evaluation;

namespace MyChessEngine.Core.Search;

public static class QuiescenceSearch
{
    public static int Search(ChessGame game, int depth, Evaluator evaluator, ref int nodesVisited, int alpha, int beta, int color)
    {
        if (game.IsCheckmate) return -100000 - depth;
        if (game.IsStalemate) return 0;
        nodesVisited++;
        var evaluation = Evaluator.EvaluatePosition(game.Board) * color;

        if (evaluation >= beta) return beta;
        if (evaluation > alpha) alpha = evaluation;

        var moves = game.GetAllPossibleMoves()
            .Where(move => game.GetPiece(move.To) is not null)
            .OrderByDescending(move => evaluator.EvaluateMove(game, move));

        foreach (var move in moves)
        {
            game.MakeMove(move);
            var score = -Search(game, depth, evaluator, ref nodesVisited, -beta, -alpha, -color);
            game.UndoLastMove();

            if (score > alpha) alpha = score;
            if (alpha >= beta) break;
        }

        return alpha;
    }
}