using MyChess.Core;
using MyChess.Models;
using MyChess.Models.Moves;
using MyChessEngine.Utils;

namespace MyChessEngine.Core.Evaluation;

public class Evaluator
{
    private readonly int[,] _historyTable = new int[12, 64];
    private readonly ChessMove[,] _killerMoves = new ChessMove[KillerSlots, MaxPly];
    private const int MaxPly = 256;
    private const int KillerSlots = 2;

    public static int EvaluatePosition(ChessBoard board)
    {
        var score = 0;

        for (var cell = 0; cell < 64; cell++)
        {
            var piece = board.GetPiece(cell);
            if (piece == null) continue;

            var factor = piece.Color == ChessColor.White ? 1 : -1;
            var tableIndex = piece.Color == ChessColor.White ? cell : PieceSquareTables.MirrorSquare(cell);

            score += PieceSquareTables.GetPieceTable(piece)[tableIndex] * factor;
            score += PieceSquareTables.GetPiecePrice(piece) * factor;
        }

        return score;
    }

    public int EvaluateMove(ChessGame game, ChessMove move)
    {
        var moveScore = 0;
        var movingPiece = game.GetPiece(move.From);
        if (movingPiece == null) return 0;
        var targetPiece = game.GetPiece(move.To);

        if (targetPiece is not null)
        {
            return 1_000_000 + PieceSquareTables
                .MostValuableVictimAndLessValuableAttacker
                [
                    movingPiece.Index,
                    targetPiece.Index
                ];
        }

        if (game.PlyCount < MaxPly)
        {
            for (var i = 0; i < KillerSlots; i++)
            {
                if (!move.Equals(_killerMoves[i, game.PlyCount])) continue;
                moveScore += 900_000 - i * 1000;
                return moveScore;
            }
        }

        moveScore += _historyTable[movingPiece.Index, move.To];

        return moveScore;
    }

    public void UpdateHistoryTable(int pieceIndex, int toCell, int depth)
    {
        var bonus = depth * depth;
        _historyTable[pieceIndex, toCell] += bonus;
    }
    
    public void UpdateKillerMoves(ChessMove move, int ply)
    {
        if (ply >= MaxPly) return;

        for (var i = 0; i < KillerSlots; i++)
        {
            if (!move.Equals(_killerMoves[i, ply])) continue;

            if (i > 0)
                (_killerMoves[0, ply], _killerMoves[i, ply]) 
                    = (_killerMoves[i, ply], _killerMoves[0, ply]);

            return;
        }

        for (var i = KillerSlots - 1; i > 0; i--)
        {
            _killerMoves[i, ply] = _killerMoves[i - 1, ply];
        }

        _killerMoves[0, ply] = move;
    }
}