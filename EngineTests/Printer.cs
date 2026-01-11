using MyChess.Core;
using MyChess.Services.Fen;
using MyChessEngine.Core;
using MyChessEngine.Models;
using Xunit;

namespace EngineTests;

public class Printer
{
    [Fact]
    public void Print()
    {
        var fen = "8/P5k1/3r4/5p2/1P3P1p/RR2P1Pp/4K2r/8 w - - 4 45";
        var game = new ChessGame(fen);
        var playerMove = FenParser.CreateMoveFromString(game.Board, "e2f3");
        game.MakeMove(playerMove);
        var engine = new ChessEngine();
        engine.FindBestMoveWithIterativeDeepening(game, new SearchParameters {Depth = 6, EnableDebugger = true});
    }
}