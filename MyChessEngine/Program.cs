using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using MyChess.Core;
using MyChessEngine.Utils;

namespace MyChessEngine;

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net80)]
public class ChessBenchmark
{
    private ChessGame _game = new ChessGame();

    [GlobalSetup]
    public void Setup()
    {
        _game = new ChessGame();
    }

    [Benchmark]
    [Arguments(4)]
    public ulong PerftTest(int depth)
    {
        return Tester.PerftTest(_game, depth);
    }
}

internal abstract class Program
{
    public static void Main()
    {
        Console.WriteLine(PieceSquareTables.BlackPassedMasks[45].ToString());
    }
}