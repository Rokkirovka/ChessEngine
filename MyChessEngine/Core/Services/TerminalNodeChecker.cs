using MyChess.Core;

namespace MyChessEngine.Core.Services;

public static class TerminalNodeChecker
{
    public static bool IsTerminalNode(ChessGame game, out int score)
    {
        score = 0;
    
        if (game.IsCheckmate)
        {
            score = -100000;
            return true;
        }

        if (game is { IsStalemate: false, IsDrawByRepetition: false }) return false;
        score = 0;
        return true;

    }
    
    public static int AdjustScoreForDepth(int score, int depth) => score - depth;
}