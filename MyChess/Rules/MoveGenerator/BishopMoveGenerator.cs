using MyChess.Core;
using MyChess.Models;
using MyChess.Models.Moves;

namespace MyChess.Rules.MoveGenerator;

public class BishopMoveGenerator : IMoveGenerator
{
    public static readonly BishopMoveGenerator Instance = new();
    
    private static readonly BitBoard[] BishopAttackMasks;

    static BishopMoveGenerator()
    {
        BishopAttackMasks = new BitBoard[64];
        InitializeBishopAttackMasks();
    }
    
    private BishopMoveGenerator()
    {
    }
    
    private static void InitializeBishopAttackMasks()
    {
        for (var cell = 0; cell < 64; cell++)
        {
            BishopAttackMasks[cell] = CalculateBishopAttackMask((ChessCell)cell);
        }
    }

    private static BitBoard CalculateBishopAttackMask(ChessCell cell)
    {
        ulong attacks = 0;
        int rank, file;
    
        var targetRank = (int)cell / 8;
        var targetFile = (int)cell % 8;
    
        for (rank = targetRank + 1, file = targetFile + 1; rank <= 6 && file <= 6; rank++, file++) 
            attacks |= 1UL << (rank * 8 + file);
        for (rank = targetRank - 1, file = targetFile + 1; rank >= 1 && file <= 6; rank--, file++) 
            attacks |= 1UL << (rank * 8 + file);
        for (rank = targetRank + 1, file = targetFile - 1; rank <= 6 && file >= 1; rank++, file--) 
            attacks |= 1UL << (rank * 8 + file);
        for (rank = targetRank - 1, file = targetFile - 1; rank >= 1 && file >= 1; rank--, file--) 
            attacks |= 1UL << (rank * 8 + file);
    
        return attacks;
    }
    
    public static BitBoard CalculateBishopAttacksWithBlocking(int cell, ulong occupancyMask)
    {
        ulong attacks = 0;
        var targetRank = cell / 8;
        var targetFile = cell % 8;
    
        for (int r = targetRank + 1, f = targetFile + 1; r <= 7 && f <= 7; r++, f++)
        {
            var square = r * 8 + f;
            var squareBit = 1UL << square;
            attacks |= squareBit;
            if ((squareBit & occupancyMask) != 0) break;
        }
    
        for (int r = targetRank - 1, f = targetFile + 1; r >= 0 && f <= 7; r--, f++)
        {
            var square = r * 8 + f;
            var squareBit = 1UL << square;
            attacks |= squareBit;
            if ((squareBit & occupancyMask) != 0) break;
        }
    
        for (int r = targetRank + 1, f = targetFile - 1; r <= 7 && f >= 0; r++, f--)
        {
            var square = r * 8 + f;
            var squareBit = 1UL << square;
            attacks |= squareBit;
            if ((squareBit & occupancyMask) != 0) break;
        }
    
        for (int r = targetRank - 1, f = targetFile - 1; r >= 0 && f >= 0; r--, f--)
        {
            var square = r * 8 + f;
            var squareBit = 1UL << square;
            attacks |= squareBit;
            if ((squareBit & occupancyMask) != 0) break;
        }
        
        return attacks;
    }

    public IEnumerable<ChessMove> GetPossibleMoves(int pieceCell, BitBoard enemyPieces, BitBoard friendlyPieces)
    {
        BitBoard allPieces = enemyPieces | friendlyPieces;
        var attacks = CalculateBishopAttacksWithBlocking(pieceCell, allPieces);
        BitBoard validTargets = attacks & ~friendlyPieces;
        
        for (var i = 0; i < 13; i++)
        {
            var index = validTargets.GetLeastSignificantBitIndex();
            if (index == -1) break;
            validTargets.PopBit(index);
            yield return new StandardMove((ChessCell)pieceCell, (ChessCell)index);
        }
    }
}