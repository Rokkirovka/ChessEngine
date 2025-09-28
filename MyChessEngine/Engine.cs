using MyChess.Core;
using MyChess.Models;
using MyChess.Models.Moves;

namespace MyChessEngine;

public class Engine(ChessGame game)
{
    private int _score;
    private bool _isInitialized;
    
    public EngineResult EvaluatePosition(int depth = 5, int alpha = int.MinValue, int beta = int.MaxValue)
    {
        if (!_isInitialized)
        {
            _score = CalculateScore();
            _isInitialized = true;
        }
        
        var color = game.CurrentPlayer;
        var isMaximizing = color == ChessColor.White;

        if (game.IsCheckmate)
        {
            var mateScore = isMaximizing ? int.MinValue + 1 : int.MaxValue - 1;
            return new EngineResult(mateScore, null);
        }
    
        if (game.IsStalemate)
        {
            return new EngineResult(0, null);
        }
    
        if (depth == 0)
        {
            return new EngineResult(_score, null);
        }
        
        ChessMove? bestMove = null;

        foreach (var move in game.GetAllPossibleMoves())
        {
            var previousScore = _score;
            var movesWillChange = game.GetCellsWillChange(move).ToArray();
            
            RemoveCellsScore(movesWillChange);
            game.MakeMove(move);
            UpdateScore(movesWillChange);
            
            var result = EvaluatePosition(depth - 1, alpha, beta);
            
            game.UndoLastMove();
            _score = previousScore;

            if (isMaximizing)
            {
                if (result.Score > alpha)
                {
                    alpha = result.Score;
                    bestMove = move;
                }

                if (alpha >= beta)
                {
                    break;
                }
            }
            else
            {
                if (result.Score < beta)
                {
                    beta = result.Score;
                    bestMove = move;
                }
                if (beta <= alpha) break;
            }
        }
        
        var bestScore = isMaximizing ? alpha : beta;
        if (depth == 5) Console.WriteLine();
        return new EngineResult(bestScore, bestMove);
    }
    
    public void RemoveCellsScore(IEnumerable<int> changes)
    {
        foreach (var cell in changes)
        {
            var piece = game.GetPiece(cell);
            if (piece is null) continue;
            var color = piece.Color;
            
            var factor = -1;
            var tableIndex = cell;
            
            if (color == ChessColor.Black)
            {
                factor = 1;
                tableIndex = PieceSquareTables.MirrorSquare(cell);
            }
            
            _score += PieceSquareTables.GetPieceTable(piece)[tableIndex] * factor;
            _score += PieceSquareTables.GetPiecePrice(piece) * factor;
        }
    }

    public void UpdateScore(IEnumerable<int> changes)
    {
        foreach (var cell in changes)
        {
            var piece = game.GetPiece(cell);
            if (piece is null) continue;
            var color = piece.Color;
            
            var factor = 1;
            var tableIndex = cell;
            
            if (color == ChessColor.Black)
            {
                factor = -1;
                tableIndex = PieceSquareTables.MirrorSquare(cell);
            }
            
            _score += PieceSquareTables.GetPieceTable(piece)[tableIndex] * factor;
            _score += PieceSquareTables.GetPiecePrice(piece) * factor;
        }
    }

    private int CalculateScore()
    {
        var score = 0;

        if (game.IsCheckmate)
        {
            return game.CurrentPlayer == ChessColor.White ? int.MaxValue : int.MinValue;
        }

        for (var cell = 0; cell < 64; cell++)
        {
            var piece = game.GetPiece(cell);
            if (piece == null) continue;
            var factor = 1;
            var tableIndex = cell;

            var color = piece.Color;
            if (color == ChessColor.Black)
            {
                factor = -1;
                tableIndex = PieceSquareTables.MirrorSquare(cell);
            }

            score += PieceSquareTables.GetPieceTable(piece)[tableIndex] * factor;
            score += PieceSquareTables.GetPiecePrice(piece) * factor;
        }

        return score;
    }
}