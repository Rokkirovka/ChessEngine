using MyChess.Core;
using MyChess.Services.Fen;
using MyChessEngine.Core;
using MyChessEngine.Models;
using Xunit;

namespace EngineTests;

public class PuzzleTests
{
    [Theory]
    [InlineData("8/5pk1/4p1p1/3nP1Kp/7P/5PP1/5r2/Q4N2 w - - 3 59,f3f4 d5f4 g3f4 f2g2 f1g3 g2g3")]
    [InlineData("1r4k1/2Bp2pp/p3p1p1/npN1Pr1q/8/P2Q1bP1/1P3P1P/2R1R1K1 w - - 1 22,c7b8 h5h2 g1h2 f5h5 h2g1 h5h1")]
    [InlineData("1R4r1/p1Q3pk/P2p2np/8/8/7P/4q1P1/1R2b2K w - - 6 35,b8g8 e2f1 h1h2 e1g3 h2g3 f1f4")]
    [InlineData("4kbr1/p4p1p/2p1pBp1/1q6/2p5/6Q1/P4PPP/3R2K1 b - - 3 25,f8e7 g3c7 e7f6 c7c8 f6d8 c8d8")]
    [InlineData("7k/1pQ4p/5np1/p1P2p2/8/P6P/BP3PPK/R1Bqr3 w - - 1 29,c1g5 e1h1 h2g3 f6h5 g3h4 d1g4")]
    [InlineData("1rbq1k1r/p1p3pp/2nbPpn1/3N4/3P4/2P2Q2/PP3PPP/R1B1R1K1 b - - 2 18,c6e7 f3f6 g7f6 c1h6 f8g8 d5f6")]
    [InlineData("8/P5k1/3r4/5p2/1P3P1p/RR2P1Pp/4K2r/8 w - - 4 45,e2f3 d6g6 g3g4 g6g4 e3e4 g4g3")]
    [InlineData("6R1/1k6/p3B2p/1P6/3pNr1r/P3nn2/1PP4P/R1B3K1 w - - 1 33,g1f2 f3g1 f2e1 f4f1 e1d2 f1d1")]
    [InlineData("6R1/4pB1p/6pk/8/1n3PP1/7K/p6P/2r5 b - - 1 48,a2a1q h3h4 a1e5 g4g5 e5g5 f4g5")]
    [InlineData("7r/1p2kp1p/2p1pp2/pPP5/P2Pp3/4P2q/1QN5/R4R1K w - - 2 26,h1g1 h3g3 g1h1 g3h4 h1g1 h8g8")]
    public void MateIn3Puzzles(string line) => TestPuzzle(line, new SearchParameters {Depth = 7 });

    [Theory]
    [InlineData("8/6p1/3bp2p/3pNp1k/2pP1P1P/2P1P3/4K1P1/8 w - - 3 37,g2g3 d6e5 d4e5 h5g4 e2f2 g4h3")]
    public void EndgamePuzzles(string line) => TestPuzzle(line, new SearchParameters {Depth = 10});

    private static void TestPuzzle(string csvLine, SearchParameters parameters)
    {
        var parts = csvLine.Split(',');
        var fen = parts[0];
        var moves = parts[1].Split();
        var game = new ChessGame(fen);
        var engine = new ChessEngine();

        for (var i = 0; i < moves.Length; i += 2)
        {
            var playerMoveStr = moves[i];
            var expectedEngineMoveStr = moves[i + 1];
            var playerMove = FenParser.CreateMoveFromString(game.Board, playerMoveStr);
            game.MakeMove(playerMove);

            var result = engine.FindBestMoveWithIterativeDeepening(game, parameters);
            var engineBestMove = result.PrincipalVariation[0]!.ToString();

            game.MakeMove(FenParser.CreateMoveFromString(game.Board, engineBestMove));
            Assert.Equal(expectedEngineMoveStr, engineBestMove);
        }
    }
}