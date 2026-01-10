using MyChess.Core;
using MyChessEngine.Core.Evaluation.Position;
using Xunit;

namespace EngineTests;

public class PositionEvaluatorTests
{
    private readonly PositionEvaluator _positionEvaluator = new();
    [Theory]
    [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1")]
    [InlineData("3Kn2Q/4npB1/2P5/ppRB2p1/PPrb2P1/2p5/4NPb1/3kN2q w - - 0 1")]
    [InlineData("5nr1/Prp1p3/n1P1PP1K/p6P/P6p/N1p1pp1k/pRP1P3/5NR1 w - - 0 1")]
    [InlineData("R7/NBrqpp1p/P1B4n/4K1PP/4k1pp/p1b4N/nbRQPP1P/r7 w - - 0 1")]
    [InlineData("4N2N/PB3p2/2P1K1r1/1ppPP3/1PPpp3/2p1k1R1/pb3P2/4n2n w - - 0 1")]
    public void Evaluation_ShouldBeZero_InSymmetricPosition(string fen)
    {
        var board = new ChessGame(fen).Board;
        var actualEvaluation = _positionEvaluator.Evaluate(board);
        Assert.Equal(0, actualEvaluation, tolerance: 0.001);
    }
}