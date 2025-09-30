using MyChess.Core;
using MyChess.Models.Moves;
using MyChessEngine.Core;

namespace MyChess.UCI;

internal abstract class Program
{
    private static ChessGame? _game;
    private static Engine? _engine;

    private static void Main()
    {
        InitializeEngine();
        RunUciProtocol();
    }

    private static void InitializeEngine()
    {
        _game = new ChessGame();
        _engine = new Engine(_game);
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
                    InitializeEngine();
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

    static void HandlePosition(string[] tokens)
    {
        if (_game == null || _engine == null) return;

        var index = 1;
        
        if (tokens[index] == "startpos")
        {
            _game = new ChessGame();
            _engine.UpdateGame(_game);
            index += 1;
        }
        else if (tokens[index] == "fen")
        {
            var fen = string.Join(" ", tokens.Skip(2).Take(6));
            _game = new ChessGame(fen);
            _engine = new Engine(_game);
            index += 7;
        }

        if (index >= tokens.Length || tokens[index] != "moves") return;
        for (var i = index + 1; i < tokens.Length; i++)
        {
            ApplyMove(tokens[i]);
        }
    }

    private static void ApplyMove(string moveString)
    {
        if (_game == null || _engine == null) return;
        var move = ConvertUciMoveToChessMove(moveString);
        _game.MakeMove(move);
    }

    private static ChessMove ConvertUciMoveToChessMove(string moveString)
    {
        return _game!.CreateMoveFromString(moveString);
    }

    private static void HandleGo(string[] tokens)
    {
        if (_engine == null || _game == null) return;

        var depth = 5;
        
        for (var i = 1; i < tokens.Length; i++)
        {
            if (tokens[i] == "depth" && i + 1 < tokens.Length)
            {
                depth = int.Parse(tokens[i + 1]);
            }
        }

        var result = _engine.FindBestMove(depth);
        
        if (result.BestMove is not null)
        {
            var uciMove = ConvertChessMoveToUci(result.BestMove);
            Console.WriteLine($"bestmove {uciMove}");
        }
        else
        {
            Console.WriteLine("bestmove 0000");
        }
        
        _game.PrintBoard();
    }

    private static string ConvertChessMoveToUci(ChessMove move)
    {
        return move.ToString();
    }
}