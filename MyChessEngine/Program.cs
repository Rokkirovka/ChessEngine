using MyChess.Core;
using MyChessEngine.Core;
using MyChessEngine.Models;

namespace MyChessEngine;

internal abstract class Program
{
    public static void Main()
    {
        var engine = new Engine
        (new ChessGame(),
            new SearchParameters
            {
                UseKillerMoves = true,
                UseHistoryHeuristic = true,
                UseQuiescenceSearch = true
            }
        );
        engine.FindBestMove();
        Console.WriteLine(engine.GetDiagnostics().NodesVisited);
    }
}