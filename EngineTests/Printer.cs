using MyChess.Core;
using MyChessEngine.Core.Evaluation.Position;
using Xunit;
using Xunit.Abstractions;

namespace EngineTests;

public class Printer(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void Print()
    {
        var fen = "rnbqkbnr/pppp1ppp/8/4N3/8/8/PPPPPPPP/RNBQKB1R b KQkq - 0 2";
        var board = new ChessGame(fen).Board;
        testOutputHelper.WriteLine(new PositionEvaluator().Evaluate(board).ToString());
    }
}