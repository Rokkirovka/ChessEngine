using System;
using System.Linq;
using MyChess.Core;
using MyChess.Models.Moves;
using MyChessEngine.Core;
using MyChessEngine.Models;

namespace MyChess.UCI;

internal abstract class UciProtocol
{
    private static ChessGame? _game;
    private static ChessEngine _engine = new();
    private const int DefaultDepth = 5;

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
            var move = _game.CreateMoveFromString(tokens[i]);
            _game.MakeMove(move);
        }
    }

    private static void HandleGo(string[] tokens)
    {
        var depth = DefaultDepth;
        for (var i = 1; i < tokens.Length; i++)
        {
            if (tokens[i] == "depth" && i + 1 < tokens.Length)
            {
                depth = int.Parse(tokens[i + 1]);
            }
        }

        _game ??= new ChessGame();
        
        EngineResult? result = null;

        for (var currentDepth = 1; currentDepth <= depth; currentDepth++)
        {
            result = _engine.FindBestMove(_game, new SearchParameters { Depth = currentDepth });

            Console.Write($"info " +
                          $"score cp {result.Score} " +
                          $"depth {currentDepth} " +
                          $"nodes {result.NodesVisited} " +
                          $"pv " +
                          string.Join(" ", result.PrincipalVariation
                              .TakeWhile(move => move is not null)
                              .Select(move => move.ToString())) + "\n");
        }

        if (result!.BestMove is not null)
        {
            var uciMove = ConvertChessMoveToUci(result.BestMove);
            Console.WriteLine($"bestmove {uciMove}");
        }
        else
        {
            Console.WriteLine("bestmove 0000");
        }
    }

    private static string ConvertChessMoveToUci(ChessMove move)
    {
        return move.ToString();
    }
}