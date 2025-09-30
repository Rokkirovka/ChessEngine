using MyChess.Core;
using MyChess.Models;
using MyChess.Models.Moves;
using MyChessEngine.Utils;

namespace MyChessEngine.Core.Evaluation;

public class StandardEvaluator : IEvaluator
{
    private ChessGame _game;

    public int Score { get; set; }

    public StandardEvaluator(ChessGame game)
    {
        _game = game;
        Initialize(game);
    }

    public void Initialize(ChessGame game)
    {
        _game = game;
        Score = CalculateScore();
    }

    public void UpdatePosition(ChessMove move)
    {
        var changes = _game.GetCellsWillChange(move).ToArray();
        RemoveCellsScore(changes);
        _game.MakeMove(move);
        UpdateScore(changes);
    }

    public void RemoveCellsScore(IEnumerable<int> cells)
    {
        foreach (var cell in cells)
        {
            var piece = _game.GetPiece(cell);
            if (piece is null) continue;

            var factor = piece.Color == ChessColor.White ? 1 : -1;
            var tableIndex = GetTableIndex(cell, piece.Color);

            Score -= PieceSquareTables.GetPieceTable(piece)[tableIndex] * factor;
            Score -= PieceSquareTables.GetPiecePrice(piece) * factor;
        }
    }

    public void UpdateScore(IEnumerable<int> cells)
    {
        foreach (var cell in cells)
        {
            var piece = _game.GetPiece(cell);
            if (piece is null) continue;

            var factor = piece.Color == ChessColor.White ? 1 : -1;
            var tableIndex = GetTableIndex(cell, piece.Color);

            Score += PieceSquareTables.GetPieceTable(piece)[tableIndex] * factor;
            Score += PieceSquareTables.GetPiecePrice(piece) * factor;
        }
    }

    public int Evaluate()
    {
        return Score;
    }

    private int CalculateScore()
    {
        var score = 0;

        if (_game.IsCheckmate)
        {
            return _game.CurrentPlayer == ChessColor.White ? int.MinValue : int.MaxValue;
        }

        for (var cell = 0; cell < 64; cell++)
        {
            var piece = _game.GetPiece(cell);
            if (piece == null) continue;

            var factor = piece.Color == ChessColor.White ? 1 : -1;
            var tableIndex = GetTableIndex(cell, piece.Color);

            score += PieceSquareTables.GetPieceTable(piece)[tableIndex] * factor;
            score += PieceSquareTables.GetPiecePrice(piece) * factor;
        }

        return score;
    }

    private static int GetTableIndex(int cell, ChessColor color)
    {
        return color == ChessColor.White ? cell : PieceSquareTables.MirrorSquare(cell);
    }

    public int EvaluateMove(ChessMove move)
    {
        var moveScore = 0;
        var movingPiece = _game.GetPiece(move.From);
        var targetPiece = _game.GetPiece(move.To);

        if (targetPiece is not null)
        {
            moveScore += PieceSquareTables
                .MostValuableVictimAndLessValuableAttacker
                [
                    movingPiece!.Index, 
                    targetPiece.Index
                ];
        }

        return moveScore;
    }
}