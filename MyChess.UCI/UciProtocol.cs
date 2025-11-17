using MyChess.Core;
using MyChess.Models;
using MyChess.Models.Moves;
using MyChess.Services.Fen;
using MyChessEngine.Core;
using MyChessEngine.Models;

namespace MyChess.UCI;

internal abstract class UciProtocol
{
    private static ChessGame? _game;
    private static ChessEngine _engine = new();
    private const int DefaultDepth = 6;

    private static void Main()
    {
        RunUciProtocol();
    }

    private static void RunUciProtocol()
    {
        while (true)
        {
            var input = Console.ReadLine();
            if (string.IsNullOrEmpty(input)) continue;

            var tokens = input.Split(' ');

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
                    HandleGo(tokens);
                    break;

                case "quit":
                    return;

                case "stop":
                    break;

                case "ucinewgame":
                    _game = new ChessGame();
                    _engine = new ChessEngine();
                    break;
            }
        }
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

        for (var i = 1; i < tokens.Length; i++)
        {
            switch (tokens[i])
            {
                case "depth" when i + 1 < tokens.Length:
                    depth = int.Parse(tokens[i + 1]);
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

        var timeForMove = CalculateTimeForMove(_game.CurrentPlayer,
            moveTime, whiteTime, blackTime, whiteIncrement, blackIncrement);

        EngineResult? result = null;
        var searchStarted = DateTime.Now;

        result = _engine.FindBestMove(_game, new SearchParameters {Depth = depth});

        if (result?.BestMove is not null)
        {
            var uciMove = ConvertChessMoveToUci(result.BestMove);
            Console.WriteLine($"bestmove {uciMove}");
        }
        else
        {
            Console.WriteLine("bestmove 0000");
        }
    }
    
    private static int CalculateTimeForMove(ChessColor currentColor, 
        int? moveTime, int? whiteTime, int? blackTime, 
        int? whiteIncrement, int? blackIncrement)
    {
        if (moveTime.HasValue) return moveTime.Value;

        var timeLeft = currentColor == ChessColor.White ? 
            whiteTime ?? 60000 : blackTime ?? 60000;
        var increment = currentColor == ChessColor.White ? 
            whiteIncrement ?? 0 : blackIncrement ?? 0;
    
        var baseTime = timeLeft / 20;
        var bonusTime = increment / 2;
    
        var calculatedTime = baseTime + bonusTime;
    
        calculatedTime = Math.Min(calculatedTime, (int)(timeLeft * 0.8));
    
        return calculatedTime;
    }

    private static string ConvertChessMoveToUci(ChessMove move)
    {
        return move.ToString();
    }
}