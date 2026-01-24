using MyChess.Core;
using MyChessEngine.Core;
using MyChessEngine.Models;
using Xunit;
using Xunit.Abstractions;

namespace EngineTests;

public class Printer(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void Print()
    {
        const string fen = "rnbqkb1r/ppp1pppp/5n2/3p4/3P1B2/2N5/PPP1PPPP/R2QKBNR b KQkq - 1 3";
        var game = new ChessGame(fen);
        var engine = new ChessEngine();
        var res = engine.FindBestMoveWithIterativeDeepening(game, new SearchParameters { Depth = 5, UseLateMoveReduction = false});
        testOutputHelper.WriteLine(res.BestMove!.ToString());
    }
}