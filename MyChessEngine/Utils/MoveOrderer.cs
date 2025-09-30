using MyChess.Core;
using MyChess.Models;
using MyChess.Models.Moves;
using MyChess.Models.Pieces;
using MyChessEngine.Models;

namespace MyChessEngine.Utils;

public class MoveOrderer
{
    private readonly int[,] _historyTable = new int[12, 64];
    private readonly ChessMove[,] _killerMoves = new ChessMove[KillerSlots, MaxPly];
    private const int MaxPly = 128;
    private const int KillerSlots = 2;

    public int CalculateMoveScore(ChessGame game, ChessMove move, SearchParameters parameters)
    {
        var score = 0;
        var movingPiece = game.GetPiece(move.From);
        var targetPiece = game.GetPiece(move.To);

        if (movingPiece == null) return 0;

        if (targetPiece != null)
        {
            score += 1_000_000 + GetCaptureScore(movingPiece, targetPiece);
            return score;
        }

        if (parameters.UseKillerMoves && game.PlyCount < MaxPly)
        {
            for (var i = 0; i < KillerSlots; i++)
            {
                if (move.Equals(_killerMoves[i, game.PlyCount]))
                {
                    score += 900_000 - i * 1000;
                    return score;
                }
            }
        }

        if (parameters.UseHistoryHeuristic)
        {
            score += GetHistoryScore(movingPiece, move.To);
        }
        
        score += GetPositionalBonus(movingPiece, move.To);

        return score;
    }

    private static int GetCaptureScore(IChessPiece attacker, IChessPiece victim)
    {
        return PieceSquareTables
            .MostValuableVictimAndLessValuableAttacker
            [
                attacker.Index,
                victim.Index
            ];
    }

    private int GetHistoryScore(IChessPiece piece, int toSquare)
    {
        return _historyTable
        [
            piece.Index,
            toSquare
        ];
    }

    private static int GetPositionalBonus(IChessPiece piece, int toSquare)
    {
        var table = PieceSquareTables.GetPieceTable(piece);
        var fromScore = table
        [
            GetTableIndex(
                piece,
                piece.Color == ChessColor.White
                    ? toSquare
                    : PieceSquareTables.MirrorSquare(toSquare))
        ];
        return fromScore / 10;
    }

    private static int GetTableIndex(IChessPiece piece, int square)
    {
        return piece.Color == ChessColor.White
            ? square
            : PieceSquareTables.MirrorSquare(square);
    }

    public void UpdateHistory(IChessPiece piece, int toSquare, int depth)
    {
        var bonus = depth * depth;
        _historyTable[piece.Index, toSquare] += bonus;

        if (_historyTable[piece.Index, toSquare] <= 1_000_000) return;

        for (var i = 0; i < 12; i++)
        for (var j = 0; j < 64; j++)
            _historyTable[i, j] /= 2;
    }

    public void UpdateKiller(ChessMove move, int ply)
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

    public void ClearKillers()
    {
        for (var i = 0; i < KillerSlots; i++)
        {
            for (var j = 0; j < MaxPly; j++)
            {
                _killerMoves[i, j] = default!;
            }
        }
    }

    public void ClearHistory()
    {
        Array.Clear(_historyTable, 0, _historyTable.Length);
    }

    public MoveOrdererStats GetStats()
    {
        var stats = new MoveOrdererStats();

        for (var i = 0; i < 12; i++)
        for (var j = 0; j < 64; j++)
        {
            if (_historyTable[i, j] <= 0) continue;
            stats.HistoryEntries++;
            stats.MaxHistoryValue = Math.Max(stats.MaxHistoryValue, _historyTable[i, j]);
        }

        for (var i = 0; i < KillerSlots; i++)
        for (var j = 0; j < MaxPly; j++)
            if (_killerMoves[i, j].From != 0 || _killerMoves[i, j].To != 0)
                stats.KillerEntries++;

        return stats;
    }
}