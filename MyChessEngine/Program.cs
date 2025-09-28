using MyChess.Models.Moves;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using MyChess.Core;

namespace MyChessEngine;

internal abstract class Program
{

    public static void Main()
    {
        // BenchmarkRunner.Run<EngineBenchmark>();
        Console.WriteLine(Tester.PerftTest(
            new ChessGame("r4rk1/1pp1qppp/p1np1n2/2b1p1B1/2B1P1b1/P1NP1N2/1PP1QPPP/R4RK1 w - - 0 10"), 
            4));
    }
}

[MemoryDiagnoser]
public class EngineBenchmark
{
    private ChessGame _game;

    [GlobalSetup]
    public void Setup()
    {
        _game = new ChessGame();
    }

    [Benchmark]
    public ChessMove? FirstMoveBenchmark()
    {
        var engine = new Engine(_game);
        return engine.EvaluatePosition(depth: 5).BestMove;
    }
}