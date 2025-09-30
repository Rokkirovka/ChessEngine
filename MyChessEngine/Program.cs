using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Jobs;
using MyChess.Core;
using MyChessEngine.Utils;

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net80)]
public class ChessBenchmark
{
    private ChessGame _game;

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
        BenchmarkRunner.Run<ChessBenchmark>();
    }
}