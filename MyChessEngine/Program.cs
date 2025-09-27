using MyChess.Models.Moves;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using MyChess.Core;

namespace MyChessEngine;

class Program
{

    public static void Main(string[] args)
    {
        BenchmarkRunner.Run<EngineBenchmark>();
        // var bb = new BitBoard();
        // for (int i = 0; i <= 55; i++)
        // {
        //     bb.SetBit(i);
        // }
        // Console.WriteLine(bb.ToString());
        // Console.WriteLine((ulong)bb);
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
        return engine.EvaluatePosition(depth: 4).BestMove;
    }
}