using MyChess.Models.Moves;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using MyChess.Core;

namespace MyChessEngine;

class Program
{

    public static void Main()
    {
        BenchmarkRunner.Run<EngineBenchmark>();
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