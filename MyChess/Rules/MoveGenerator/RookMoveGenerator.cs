using MyChess.Core;
using MyChess.Models;
using MyChess.Models.Moves;

namespace MyChess.Rules.MoveGenerator;

public class RookMoveGenerator : IMoveGenerator
{
    public static readonly RookMoveGenerator Instance = new();

    private static readonly BitBoard[] BishopAttackMasks;

    static RookMoveGenerator()
    {
        BishopAttackMasks = new BitBoard[64];
        InitializeRookAttackMasks();
    }
    
    private RookMoveGenerator()
    {
    }
    
    private static void InitializeRookAttackMasks()
    {
        for (var cell = 0; cell < 64; cell++)
        {
            BishopAttackMasks[cell] = CalculateRookAttackMask((ChessCell)cell);
        }
    }

    private static BitBoard CalculateRookAttackMask(ChessCell cell)
    {
        ulong attacks = 0;
        int rank, file;
    
        var targetRank = (int)cell / 8;
        var targetFile = (int)cell % 8;
    
        for (rank = targetRank + 1; rank <= 6; rank++) attacks |= 1UL << (rank * 8 + targetFile);
        for (rank = targetRank - 1; rank >= 1; rank--) attacks |= 1UL << (rank * 8 + targetFile);
        for (file = targetFile + 1; file <= 6; file++) attacks |= 1UL << (targetRank * 8 + file);
        for (file = targetFile - 1; file >= 1; file--) attacks |= 1UL << (targetRank * 8 + file);
    
        return attacks;
    }
    
    public static BitBoard CalculateRookAttacksWithBlocking(int cell, ulong occupancyMask)
    {
        ulong attacks = 0;
        var tr = cell / 8;
        var tf = cell % 8;

        for (var r = tr + 1; r <= 7; r++)
        {
            var square = r * 8 + tf;
            var squareBit = 1UL << square;
            attacks |= squareBit;
            if ((squareBit & occupancyMask) != 0) break;
        }

        for (var r = tr - 1; r >= 0; r--)
        {
            var square = r * 8 + tf;
            var squareBit = 1UL << square;
            attacks |= squareBit;
            if ((squareBit & occupancyMask) != 0) break;
        }

        for (var f = tf + 1; f <= 7; f++)
        {
            var square = tr * 8 + f;
            var squareBit = 1UL << square;
            attacks |= squareBit;
            if ((squareBit & occupancyMask) != 0) break;
        }

        for (var f = tf - 1; f >= 0; f--)
        {
            var square = tr * 8 + f;
            var squareBit = 1UL << square;
            attacks |= squareBit;
            if ((squareBit & occupancyMask) != 0) break;
        }

        return attacks;
    }

    public IEnumerable<ChessMove> GetPossibleMoves(int pieceCell, BitBoard enemyPieces, BitBoard friendlyPieces)
    {
        BitBoard allPieces = enemyPieces | friendlyPieces;
        var attacks = CalculateRookAttacksWithBlocking(pieceCell, allPieces);
        BitBoard validTargets = attacks & ~friendlyPieces;
        for (var i = 0; i < 14; i++)
        {
            var index = validTargets.GetLeastSignificantBitIndex();
            if (index == -1) break;
            validTargets.PopBit(index);
            yield return new StandardMove((ChessCell)pieceCell, (ChessCell)index);
        }
    }
}