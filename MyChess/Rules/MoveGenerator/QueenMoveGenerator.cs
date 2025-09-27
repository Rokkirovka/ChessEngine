using MyChess.Core;
using MyChess.Models;
using MyChess.Models.Moves;

namespace MyChess.Rules.MoveGenerator;

public class QueenMoveGenerator : IMoveGenerator
{
    public static readonly QueenMoveGenerator Instance = new();

    private QueenMoveGenerator()
    {
    }

    private BitBoard CalculateQueenAttacksWithBlocking(ChessCell cell, ulong occupancyMask)
    {
        ulong attacks = 0;
        var targetRank = (int)cell / 8;
        var targetFile = (int)cell % 8;

        for (var r = targetRank + 1; r <= 7; r++)
        {
            var square = r * 8 + targetFile;
            var squareBit = 1UL << square;
            attacks |= squareBit;
            if ((squareBit & occupancyMask) != 0) break;
        }

        for (var r = targetRank - 1; r >= 0; r--)
        {
            var square = r * 8 + targetFile;
            var squareBit = 1UL << square;
            attacks |= squareBit;
            if ((squareBit & occupancyMask) != 0) break;
        }

        for (var f = targetFile + 1; f <= 7; f++)
        {
            var square = targetRank * 8 + f;
            var squareBit = 1UL << square;
            attacks |= squareBit;
            if ((squareBit & occupancyMask) != 0) break;
        }

        for (var f = targetFile - 1; f >= 0; f--)
        {
            var square = targetRank * 8 + f;
            var squareBit = 1UL << square;
            attacks |= squareBit;
            if ((squareBit & occupancyMask) != 0) break;
        }

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
        var attacks = CalculateQueenAttacksWithBlocking((ChessCell)pieceCell, allPieces);
        BitBoard validTargets = attacks & ~friendlyPieces;
        for (var i = 0; i < 27; i++)
        {
            var index = validTargets.GetLeastSignificantBitIndex();
            if (index == -1) break;
            validTargets.PopBit(index);
            yield return new StandardMove((ChessCell)pieceCell, (ChessCell)index);
        }
    }
}