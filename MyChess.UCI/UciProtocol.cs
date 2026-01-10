using MyChess.Core;
using MyChess.Models;
using MyChess.Services.Fen;
using MyChessEngine.Core;
using MyChessEngine.Models;

namespace MyChess.UCI;

internal abstract class UciProtocol
{
    private static ChessGame? _game;
    private static ChessEngine _engine = new();
    private static readonly UciSearchReporter Reporter = new();
    private static readonly SearchCanceler Canceler = new();
    private const int DefaultDepth = 10;
    private const int MaxDepth = 32;

    private static async Task Main()
    {
        await RunUciProtocolAsync();
    }

    private static async Task RunUciProtocolAsync()
    {
        while (await Console.In.ReadLineAsync() is { } input)
        {
            if (string.IsNullOrEmpty(input)) continue;

            var tokens = input.Split(' ');

            var shouldExit = await ProcessCommandAsync(tokens);
            if (shouldExit) break;
        }
    }

    private static async Task<bool> ProcessCommandAsync(string[] tokens)
    {
        switch (tokens[0])
        {
            case "uci":
                HandleUci();
                break;

            case "isready":
                Console.WriteLine("readyok");
                break;

            case "position":
                HandlePosition(tokens);
                break;

            case "go":
                Canceler.Reset();
                _ = Task.Run(() => HandleGo(tokens));
                break;

            case "quit":
                Canceler.StopImmediately();
                await Task.Delay(50);
                return true;

            case "stop":
                Canceler.StopImmediately();
                break;

            case "ucinewgame":
                _game = new ChessGame();
                _engine = new ChessEngine();
                Canceler.Reset();
                break;
        }

        return false;
    }

    private static void HandleUci()
    {
        Console.WriteLine("id name Rokk");
        Console.WriteLine("id author Hevdanin");
        
        Console.WriteLine("uciok");
    }

    private static void HandlePosition(string[] tokens)
    {
        var index = 1;

        if (tokens[index] == "startpos")
        {
            _game = new ChessGame();
            index += 1;
        }
        else
        {
            var fen = string.Join(" ", tokens.Skip(2).Take(6));
            _game = new ChessGame(fen);
            index += 7;
        }

        if (index >= tokens.Length || tokens[index] != "moves") return;
        for (var i = index + 1; i < tokens.Length; i++)
        {
            var move = FenParser.CreateMoveFromString(_game.Board, tokens[i]);
            _game.MakeMove(move);
        }
    }

    private static void HandleGo(string[] tokens)
{
    var depth = DefaultDepth;
    int? moveTime = null;
    int? whiteTime = null;
    int? blackTime = null;
    int? whiteIncrement = null;
    int? blackIncrement = null;
    var depthSpecified = false;

    for (var i = 1; i < tokens.Length; i++)
    {
        switch (tokens[i])
        {
            case "depth" when i + 1 < tokens.Length:
                depth = int.Parse(tokens[i + 1]);
                depthSpecified = true;
                break;
            case "movetime" when i + 1 < tokens.Length:
                moveTime = int.Parse(tokens[i + 1]);
                break;
            case "wtime" when i + 1 < tokens.Length:
                whiteTime = int.Parse(tokens[i + 1]);
                break;
            case "btime" when i + 1 < tokens.Length:
                blackTime = int.Parse(tokens[i + 1]);
                break;
            case "winc" when i + 1 < tokens.Length:
                whiteIncrement = int.Parse(tokens[i + 1]);
                break;
            case "binc" when i + 1 < tokens.Length:
                blackIncrement = int.Parse(tokens[i + 1]);
                break;
        }
    }

    _game ??= new ChessGame();

    var hasTimeLimit = moveTime.HasValue || 
                       (whiteTime.HasValue && _game.CurrentPlayer == ChessColor.White) || 
                       (blackTime.HasValue && _game.CurrentPlayer == ChessColor.Black);

    if (!depthSpecified && hasTimeLimit) depth = MaxDepth;

    if (moveTime.HasValue) Canceler.StopByTimeLimit(TimeSpan.FromMilliseconds(moveTime.Value));
    else switch (_game.CurrentPlayer)
    {
        case ChessColor.White when whiteTime.HasValue:
            Canceler.StopByTimeLimit(TimeSpan.FromMilliseconds(TimeForMove(whiteTime.Value, whiteIncrement ?? 0)));
            break;
        case ChessColor.Black when blackTime.HasValue:
            Canceler.StopByTimeLimit(TimeSpan.FromMilliseconds(TimeForMove(blackTime.Value, blackIncrement ?? 0)));
            break;
    }

    _engine.FindBestMoveWithIterativeDeepening(
        _game,
        new SearchParameters { Depth = depth },
        Reporter,
        Canceler
    );
    
    Canceler.Reset();
}

    private static int TimeForMove(int baseTime, int incrementTime)
    {
        return (int)(baseTime / 20.0 + incrementTime / 2.0);
    }
}