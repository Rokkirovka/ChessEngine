using MyChess.Core;

namespace EngineTests;

public static class Tester
{
    public static ulong PerftTest(ChessGame game, int depth)
    {
        ulong nodes = 0;

        if (depth == 0) return 1UL;

        var moves = game.GetAllPossibleMoves();
        foreach (var move in moves)
        {
            game.MakeMove(move);
            nodes += PerftTest(game,depth - 1);
            game.UndoLastMove();
        }

        return nodes;
    }
}