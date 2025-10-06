using MyChess.Core;
using MyChess.Models;
using MyChess.Models.Moves;
using MyChess.Rules.MoveGenerator;
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

        BitBoard whitePawns = (ulong)board.BitBoards[0];
        while (whitePawns != 0)
        {
            var cell = whitePawns.GetLeastSignificantBitIndex();
            whitePawns.PopBit(cell);
            var doublePawns = ((BitBoard)(board.BitBoards[0] & PieceSquareTables.FileMasks[cell])).CountBits();
            score += PieceSquareTables.DoublePawnPenalty * doublePawns;
            if ((board.BitBoards[0] & PieceSquareTables.IsolatedMasks[cell]) == 0)
                score += PieceSquareTables.IsolatedPawnPenalty;
            if ((board.BitBoards[6] & PieceSquareTables.WhitePassedMasks[cell]) == 0)
                score += PieceSquareTables.PassedPawnBonus[7 - cell / 8];
        }
        
        BitBoard blackPawns = (ulong)board.BitBoards[6];
        while (blackPawns != 0)
        {
            var cell = blackPawns.GetLeastSignificantBitIndex();
            blackPawns.PopBit(cell);
            var doublePawns = ((BitBoard)(board.BitBoards[6] & PieceSquareTables.FileMasks[cell])).CountBits();
            score -= PieceSquareTables.DoublePawnPenalty * doublePawns;
            if ((board.BitBoards[6] & PieceSquareTables.IsolatedMasks[cell]) == 0)
                score -= PieceSquareTables.IsolatedPawnPenalty;
            if ((board.BitBoards[0] & PieceSquareTables.BlackPassedMasks[cell]) == 0)
                score -= PieceSquareTables.PassedPawnBonus[cell / 8];
        }
        
        BitBoard whiteRooks = (ulong)board.BitBoards[3];
        while (whiteRooks != 0)
        {
            var cell = whiteRooks.GetLeastSignificantBitIndex();
            whiteRooks.PopBit(cell);
            if ((board.BitBoards[0] & PieceSquareTables.FileMasks[cell]) == 0)
                score += PieceSquareTables.SemiOpenFileScore;
            if ((board.BitBoards[0] & board.BitBoards[6] & PieceSquareTables.FileMasks[cell]) == 0)
                score += PieceSquareTables.OpenFileScore;
            score += RookMoveGenerator.GetRookAttacks(cell, board.Occupancies[0] | board.Occupancies[1]).CountBits() * PieceSquareTables.MobilityBonus;
        }
        
        BitBoard blackRooks = (ulong)board.BitBoards[9];
        while (blackRooks != 0)
        {
            var cell = blackRooks.GetLeastSignificantBitIndex();
            blackRooks.PopBit(cell);
            if ((board.BitBoards[6] & PieceSquareTables.FileMasks[cell]) == 0)
                score -= PieceSquareTables.SemiOpenFileScore;
            if ((board.BitBoards[0] & board.BitBoards[6] & PieceSquareTables.FileMasks[cell]) == 0)
                score -= PieceSquareTables.OpenFileScore;
            score -= RookMoveGenerator.GetRookAttacks(cell, board.Occupancies[0] | board.Occupancies[1]).CountBits() * PieceSquareTables.MobilityBonus;
        }
        
        BitBoard whiteBishops = (ulong)board.BitBoards[2];
        while (whiteBishops != 0)
        {
            var cell = whiteBishops.GetLeastSignificantBitIndex();
            whiteBishops.PopBit(cell);
            score += BishopMoveGenerator.GetBishopAttacks(cell, board.Occupancies[0] | board.Occupancies[1]).CountBits() * PieceSquareTables.MobilityBonus;
        }
        
        BitBoard blackBishops = (ulong)board.BitBoards[8];
        while (blackBishops != 0)
        {
            var cell = blackBishops.GetLeastSignificantBitIndex();
            blackBishops.PopBit(cell);
            score -= BishopMoveGenerator.GetBishopAttacks(cell, board.Occupancies[0] | board.Occupancies[1]).CountBits() * PieceSquareTables.MobilityBonus;
        }
        
        BitBoard whiteQueens = (ulong)board.BitBoards[4];
        while (whiteQueens != 0)
        {
            var cell = whiteQueens.GetLeastSignificantBitIndex();
            whiteQueens.PopBit(cell);
            score += ((BitBoard)(BishopMoveGenerator.GetBishopAttacks(cell, board.Occupancies[0] | board.Occupancies[1])
                | RookMoveGenerator.GetRookAttacks(cell, board.Occupancies[0] | board.Occupancies[1]))).CountBits() * PieceSquareTables.MobilityBonus;
        }
        
        BitBoard blackQueens = (ulong)board.BitBoards[10];
        while (blackQueens != 0)
        {
            var cell = blackQueens.GetLeastSignificantBitIndex();
            blackQueens.PopBit(cell);
            score -= ((BitBoard)(BishopMoveGenerator.GetBishopAttacks(cell, board.Occupancies[0] | board.Occupancies[1])
                                 | RookMoveGenerator.GetRookAttacks(cell, board.Occupancies[0] | board.Occupancies[1]))).CountBits() * PieceSquareTables.MobilityBonus;
        }
        
        BitBoard whiteKing = (ulong)board.BitBoards[5];
        while (whiteKing != 0)
        {
            var cell = whiteKing.GetLeastSignificantBitIndex();
            whiteKing.PopBit(cell);
            if ((board.BitBoards[0] & PieceSquareTables.FileMasks[cell]) == 0)
                score -= PieceSquareTables.SemiOpenFileScore;
            if ((board.BitBoards[0] & board.BitBoards[6] & PieceSquareTables.FileMasks[cell]) == 0)
                score -= PieceSquareTables.OpenFileScore;
            score += ((BitBoard)(KingMoveGenerator.KingAttackMasks[cell] & board.Occupancies[0])).CountBits() * PieceSquareTables.KingShieldBonus;
        }
        
        BitBoard blackKing = (ulong)board.BitBoards[11];
        while (blackKing != 0)
        {
            var cell = blackKing.GetLeastSignificantBitIndex();
            blackKing.PopBit(cell);
            if ((board.BitBoards[0] & PieceSquareTables.FileMasks[cell]) == 0)
                score += PieceSquareTables.SemiOpenFileScore;
            if ((board.BitBoards[0] & board.BitBoards[6] & PieceSquareTables.FileMasks[cell]) == 0)
                score += PieceSquareTables.OpenFileScore;
            score -= ((BitBoard)(KingMoveGenerator.KingAttackMasks[cell] & board.Occupancies[1])).CountBits() * PieceSquareTables.KingShieldBonus;
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

        if (game.Ply < MaxPly)
        {
            for (var i = 0; i < KillerSlots; i++)
            {
                if (!move.Equals(_killerMoves[i, game.Ply])) continue;
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