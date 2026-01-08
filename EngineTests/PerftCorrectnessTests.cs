using System.Diagnostics;
using MyChess.Core;
using Xunit;
using Xunit.Abstractions;

namespace EngineTests;

public class PerftCorrectnessTests(ITestOutputHelper output)
{
    private static readonly (int Depth, ulong ExpectedNodes, int Iterations)[] TestConfigs =
    [
        (1, 20, 10000), 
        (2, 400, 1000), 
        (3, 8902, 200), 
        (4, 197281, 5), 
        (5, 4865609, 1)
    ];

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public void Perft_StartingPosition_Depth_CorrectNodes(int depth)
    {
        var (_, expectedNodes, iterations) = TestConfigs.First(x => x.Depth == depth);
    
        var stopwatch = Stopwatch.StartNew();
        ulong totalNodes = 0;
    
        for (var i = 0; i < iterations; i++)
        {
            var game = new ChessGame();
            totalNodes += Tester.PerftTest(game, depth);
        }
        stopwatch.Stop();
    
        var actualNodes = totalNodes / (ulong)iterations;

        Assert.Equal(expectedNodes, actualNodes);
    
        output.WriteLine($"=== Depth {depth} ({iterations} iterations) ===");
        output.WriteLine($"Expected: {expectedNodes}");
        output.WriteLine($"Actual (avg): {actualNodes}");
        output.WriteLine($"Total time: {stopwatch.ElapsedMilliseconds} ms");
        output.WriteLine($"Avg time: {stopwatch.ElapsedMilliseconds / (double)iterations:F2} ms");
        output.WriteLine($"Speed: {CalculateSpeed(totalNodes, stopwatch.ElapsedMilliseconds)} nodes/ms");
        output.WriteLine($"PASSED: {actualNodes == expectedNodes}");
        output.WriteLine(string.Empty);
    }
    
    [Fact]
    public void Measure_RAW_Generation_Speed()
    {
        var game = new ChessGame();
        const int iterations = 1_000_000;
        var stopwatch = Stopwatch.StartNew();
    
        for (var i = 0; i < iterations; i++)
        {
            var moves = game.GetAllPossibleMoves();
            _ = moves.Count();
        }
    
        stopwatch.Stop();

        var speed = iterations * 20.0 / stopwatch.Elapsed.TotalSeconds;
        output.WriteLine($"Speed: {speed}/s");
    }
    
    private static string CalculateSpeed(ulong nodes, long milliseconds)
    {
        if (milliseconds == 0) return "∞";
        var speed = (double)nodes / milliseconds;
        
        if (speed >= 1000)
        {
            var speedK = speed / 1000;
            return $"{speedK:F1}K";
        }
        return $"{speed:F1}";
    }
}