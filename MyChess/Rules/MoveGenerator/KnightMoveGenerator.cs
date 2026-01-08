using MyChess.Core;
using MyChess.Models;
using MyChess.Models.Moves;

namespace MyChess.Rules.MoveGenerator;

public class KnightMoveGenerator : IMoveGenerator
{
    public static readonly KnightMoveGenerator Instance = new();

    public static readonly BitBoard[] KnightAttackMasks;

    private const ulong FileAExcludedMask = 18374403900871474942UL;

    private const ulong FileHExcludedMask = 9187201950435737471UL;

    private const ulong FilesHgExcludedMask = 4557430888798830399UL;

    private const ulong FilesAbExcludedMask = 18229723555195321596UL;

    static KnightMoveGenerator()
    {
        KnightAttackMasks = new BitBoard[64];
        InitializeKnightAttackMasks();
    }

    private KnightMoveGenerator()
    {
    }

    private static void InitializeKnightAttackMasks()
    {
        for (var cell = 0; cell < 64; cell++)
        {
            KnightAttackMasks[cell] = CalculateKnightAttackMask(cell);
        }
    }

    private static BitBoard CalculateKnightAttackMask(int cell)
    {
        ulong attacks = 0;
        var bitBoard = new BitBoard(0UL);
        bitBoard = bitBoard.SetBit(cell);

        attacks |= (ulong)(bitBoard >> 17) & FileHExcludedMask;
        attacks |= (ulong)(bitBoard >> 15) & FileAExcludedMask;
        attacks |= (ulong)(bitBoard >> 10) & FilesHgExcludedMask;
        attacks |= (ulong)(bitBoard >> 6) & FilesAbExcludedMask;

        attacks |= (ulong)(bitBoard << 17) & FileAExcludedMask;
        attacks |= (ulong)(bitBoard << 15) & FileHExcludedMask;
        attacks |= (ulong)(bitBoard << 10) & FilesAbExcludedMask;
        attacks |= (ulong)(bitBoard << 6) & FilesHgExcludedMask;

        return new BitBoard(attacks);
    }

    public IEnumerable<ChessMove> GetPossibleMoves(
        int pieceCell, BitBoard enemyPieces, BitBoard friendlyPieces)
    {
        var attacks = KnightAttackMasks[pieceCell];
        var validTargets = attacks & ~friendlyPieces;
        var tempValidTargets = validTargets;
        
        while (tempValidTargets.Value != 0)
        {
            var index = tempValidTargets.GetLeastSignificantBitIndex();
            if (index == -1) break;
            tempValidTargets = tempValidTargets.ClearBit(index);
            yield return new StandardMove((ChessCell)pieceCell, (ChessCell)index);
        }
    }
}