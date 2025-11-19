using MyChess.Models.Moves;
using MyChessEngine.Core.Search;
using MyChessEngine.Models;

namespace MyChess.UCI;

public class UciSearchReporter : ISearchProgressReporter
{
    public void OnIterationComplete(int depth, int score, ChessMove bestMove, ChessMove?[] principalVariation,
        long nodesVisited)
    {
        var pvMoves = string.Join(" ", principalVariation.TakeWhile(move => move is not null).Select(move => move!.ToString()));
        Console.WriteLine($"info depth {depth} score cp {score} nodes {nodesVisited} pv {pvMoves}");
    }

    public void OnSearchFinished(EngineResult finalResult)
    {
        Console.WriteLine(finalResult.BestMove is not null ? $"bestmove {finalResult.BestMove}" : "bestmove 0000");
    }
}