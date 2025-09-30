using MyChess.Core;
using MyChess.Models;
using MyChess.Models.Moves;
using MyChessEngine.Core.Evaluation;
using MyChessEngine.Models;
using MyChessEngine.Utils;

namespace MyChessEngine.Core.Search;

public class AlphaBetaSearch : ISearchAlgorithm
{
    private readonly IEvaluator _evaluator;
    private readonly MoveOrderer _moveOrderer;
    private ChessGame _game;
    private bool _searchStopped;
    
    private int _nodesVisited;
    private int _qNodesVisited;

    public AlphaBetaSearch(ChessGame game, IEvaluator evaluator)
    {
        _game = game;
        _evaluator = evaluator;
        _evaluator.Initialize(game);
        _moveOrderer = new MoveOrderer();
    }

    public void Initialize(ChessGame game)
    {
        _game = game;
        _evaluator.Initialize(game);
    }

    public void UpdatePosition(ChessMove move)
    {
        _game.MakeMove(move);
        _evaluator.UpdatePosition(move);
    }

    public EngineResult Search(SearchParameters parameters)
    {
        _searchStopped = false;
        _nodesVisited = 0;
        _qNodesVisited = 0;

        var color = _game.CurrentPlayer;
        var isMaximizing = color == ChessColor.White;

        ChessMove? bestMove = null;
        var bestScore = isMaximizing ? int.MinValue : int.MaxValue;

        var moves = _game.GetAllPossibleMoves()
            .OrderByDescending(move => _moveOrderer.CalculateMoveScore(_game, move, parameters));

        var alpha = int.MinValue;
        var beta = int.MaxValue;

        foreach (var move in moves)
        {
            if (_searchStopped) break;

            var previousScore = _evaluator.Score;
            var changes = _game.GetCellsWillChange(move).ToArray();
            
            _evaluator.RemoveCellsScore(changes);
            _game.MakeMove(move);
            _evaluator.UpdateScore(changes);

            var score = Search(
                depth: parameters.Depth - 1,
                alpha: alpha,
                beta: beta,
                isMaximizing: !isMaximizing,
                parameters: parameters);

            _game.UndoLastMove();
            _evaluator.Score = previousScore;

            if (isMaximizing)
            {
                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = move;
                    alpha = Math.Max(alpha, score);
                }
            }
            else
            {
                if (score < bestScore)
                {
                    bestScore = score;
                    bestMove = move;
                    beta = Math.Min(beta, score);
                }
            }

            if (alpha >= beta) break;
        }

        return new EngineResult(bestScore, bestMove);
    }

    private int Search(int depth, int alpha, int beta, bool isMaximizing, SearchParameters parameters)
    {
        _nodesVisited++;

        if (_game.IsCheckmate)
        {
            return isMaximizing ? -100000 - depth : 100000 + depth;
        }

        if (_game.IsStalemate) return 0;

        if (depth == 0) 
        {
            return parameters.UseQuiescenceSearch 
                ? QuiescenceSearch(alpha, beta, isMaximizing, parameters) 
                : _evaluator.Score;
        }

        var moves = _game.GetAllPossibleMoves()
            .OrderByDescending(move => _moveOrderer.CalculateMoveScore(_game, move, parameters))
            .ToArray();

        var bestScore = isMaximizing ? int.MinValue : int.MaxValue;

        foreach (var move in moves)
        {
            if (_searchStopped) break;

            var previousScore = _evaluator.Score;
            var changes = _game.GetCellsWillChange(move).ToArray();
            
            _evaluator.RemoveCellsScore(changes);
            _game.MakeMove(move);
            _evaluator.UpdateScore(changes);

            var score = Search(depth - 1, alpha, beta, !isMaximizing, parameters);

            _game.UndoLastMove();
            _evaluator.Score = previousScore;

            if (isMaximizing)
            {
                bestScore = Math.Max(bestScore, score);
                alpha = Math.Max(alpha, bestScore);
            }
            else
            {
                bestScore = Math.Min(bestScore, score);
                beta = Math.Min(beta, bestScore);
            }

            if (alpha >= beta) 
            {
                if (_game.GetPiece(move.To) is null)
                {
                    var movingPiece = _game.GetPiece(move.From);
                    if (movingPiece != null)
                    {
                        _moveOrderer.UpdateKiller(move, _game.PlyCount);
                        _moveOrderer.UpdateHistory(movingPiece, move.To, depth);
                    }
                }
                break;
            }
        }

        return bestScore;
    }

    private int QuiescenceSearch(int alpha, int beta, bool isMaximizing, SearchParameters parameters)
    {
        _qNodesVisited++;

        var standPat = _evaluator.Score;

        if (isMaximizing)
        {
            if (standPat >= beta) return beta;
            if (standPat > alpha) alpha = standPat;
        }
        else
        {
            if (standPat <= alpha) return alpha;
            if (standPat < beta) beta = standPat;
        }

        var captureMoves = _game.GetAllPossibleMoves()
            .Where(move => _game.GetPiece(move.To) is not null)
            .OrderByDescending(move => _moveOrderer.CalculateMoveScore(_game, move, parameters))
            .ToArray();

        foreach (var move in captureMoves)
        {
            if (_searchStopped) break;

            var previousScore = _evaluator.Score;
            var changes = _game.GetCellsWillChange(move).ToArray();
            
            _evaluator.RemoveCellsScore(changes);
            _game.MakeMove(move);
            _evaluator.UpdateScore(changes);

            var score = QuiescenceSearch(alpha, beta, !isMaximizing, parameters);

            _game.UndoLastMove();
            _evaluator.Score = previousScore;

            if (isMaximizing)
            {
                if (score > alpha) alpha = score;
                if (alpha >= beta) return beta;
            }
            else
            {
                if (score < beta) beta = score;
                if (beta <= alpha) return alpha;
            }
        }

        return isMaximizing ? alpha : beta;
    }

    public void StopSearch()
    {
        _searchStopped = true;
    }

    public SearchDiagnostics GetDiagnostics()
    {
        return new SearchDiagnostics
        {
            NodesVisited = _nodesVisited,
            QuiescenceNodesVisited = _qNodesVisited
        };
    }
}