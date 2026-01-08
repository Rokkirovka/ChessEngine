using MyChess.Models.Moves;

namespace MyChessEngine.Core.Evaluation.Moves.Components;

public class KillerMovesService
{
    private const int MaxPly = 256;
    private const int KillerSlots = 2;

    private readonly ChessMove[,] _killerMoves = new ChessMove[KillerSlots, MaxPly];


    public void UpdateKillerMoves(ChessMove move, int ply)
    {
        if (ply >= MaxPly) return;
        for (var i = 0; i < KillerSlots; i++)
        {
            if (!move.Equals(_killerMoves[i, ply])) continue;
            if (i > 0) (_killerMoves[0, ply], _killerMoves[i, ply]) = (_killerMoves[i, ply], _killerMoves[0, ply]);
            return;
        }

        for (var i = KillerSlots - 1; i > 0; i--) _killerMoves[i, ply] = _killerMoves[i - 1, ply];
        _killerMoves[0, ply] = move;
    }

    public int? TryGetScore(ChessMove move, int ply)
    {
        if (ply >= MaxPly) return null;
        for (var i = 0; i < KillerSlots; i++)
        {
            if (!move.Equals(_killerMoves[i, ply])) continue;
            return 900_000 - i * 1000;
        }

        return null;
    }
}