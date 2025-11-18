using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using MyChess.Core;
using MyChessEngine.Core;
using MyChessEngine.Core.Search;
using MyChessEngine.Models;

namespace MyChessEngine.Benchmarks;

[MemoryDiagnoser]
[SimpleJob(warmupCount: 2, iterationCount: 5)]
public class LmrBenchmark
{
    private ChessGame _game;
    private ChessEngine _engine;
    private SearchParameters _searchParamsWithLmr;
    private SearchParameters _searchParamsWithoutLmr;

    [GlobalSetup]
    public void Setup()
    {
        _game = new ChessGame("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
        _engine = new ChessEngine();
        
        _searchParamsWithLmr = new SearchParameters
        {
            Depth = 5,
            UseLateMoveReduction = true,
            UseTranspositionTable = true,
            UseNullMovePruning = true,
            UseQuiescenceSearch = true
        };
        
        _searchParamsWithoutLmr = new SearchParameters
        {
            Depth = 5,
            UseLateMoveReduction = false,
            UseTranspositionTable = true,
            UseNullMovePruning = true,
            UseQuiescenceSearch = true
        };
    }

    [Benchmark(Baseline = true)]
    public void WithoutLMR()
    {
        var tempGame = new ChessGame("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
        
        for (int i = 0; i < 3; i++)
        {
            var result = _engine.FindBestMove(tempGame, _searchParamsWithoutLmr);
            if (result.BestMove != null)
            {
                tempGame.MakeMove(result.BestMove);
            }
        }
    }

    [Benchmark]
    public void WithLMR()
    {
        var tempGame = new ChessGame("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
        
        for (int i = 0; i < 3; i++)
        {
            var result = _engine.FindBestMove(tempGame, _searchParamsWithLmr);
            if (result.BestMove != null)
            {
                tempGame.MakeMove(result.BestMove);
            }
        }
    }

    [Benchmark]
    public void SingleMove_WithoutLMR()
    {
        var tempGame = new ChessGame("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
        var result = _engine.FindBestMove(tempGame, _searchParamsWithoutLmr);
    }

    [Benchmark]
    public void SingleMove_WithLMR()
    {
        var tempGame = new ChessGame("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
        var result = _engine.FindBestMove(tempGame, _searchParamsWithLmr);
    }
}

public class ComplexPositionBenchmark
{
    private ChessGame _complexGame;
    private ChessEngine _engine;
    private SearchParameters _searchParamsWithLmr;
    private SearchParameters _searchParamsWithoutLmr;

    [GlobalSetup]
    public void Setup()
    {
        _complexGame = new ChessGame("r1bq1rk1/pp2bppp/2n1pn2/2pp4/3P4/2PBPN2/PP1N1PPP/R1BQ1RK1 w - - 0 1");
        _engine = new ChessEngine();
        
        _searchParamsWithLmr = new SearchParameters
        {
            Depth = 5,
            UseLateMoveReduction = true,
            UseTranspositionTable = true,
            UseNullMovePruning = true,
            UseQuiescenceSearch = true
        };
        
        _searchParamsWithoutLmr = new SearchParameters
        {
            Depth = 5,
            UseLateMoveReduction = false,
            UseTranspositionTable = true,
            UseNullMovePruning = true,
            UseQuiescenceSearch = true
        };
    }

    [Benchmark]
    public void ComplexPosition_WithoutLMR()
    {
        var result = _engine.FindBestMove(_complexGame, _searchParamsWithoutLmr);
    }

    [Benchmark]
    public void ComplexPosition_WithLMR()
    {
        var result = _engine.FindBestMove(_complexGame, _searchParamsWithLmr);
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Benchmarking LMR performance...");
        
        var summaryLmr = BenchmarkRunner.Run<LmrBenchmark>();
        var summaryComplex = BenchmarkRunner.Run<ComplexPositionBenchmark>();
        
        Console.WriteLine("Benchmarks completed!");
    }
}