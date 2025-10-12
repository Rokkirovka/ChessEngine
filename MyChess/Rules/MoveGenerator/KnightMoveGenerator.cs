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
        BitBoard bitBoard = new();
        bitBoard.SetBit(cell);

        attacks |= (bitBoard >> 17) & FileHExcludedMask;
        attacks |= (bitBoard >> 15) & FileAExcludedMask;
        attacks |= (bitBoard >> 10) & FilesHgExcludedMask;
        attacks |= (bitBoard >> 6) & FilesAbExcludedMask;

        attacks |= (bitBoard << 17) & FileAExcludedMask;
        attacks |= (bitBoard << 15) & FileHExcludedMask;
        attacks |= (bitBoard << 10) & FilesAbExcludedMask;
        attacks |= (bitBoard << 6) & FilesHgExcludedMask;

        return attacks;
    }

    public IEnumerable<ChessMove> GetPossibleMoves(
        int pieceCell, BitBoard enemyPieces, BitBoard friendlyPieces)
    {
        var attacks = KnightAttackMasks[pieceCell];
        var validTargets = (BitBoard)(attacks & ~friendlyPieces);
        for (var i = 0; i < 8; i++)
        {
            var index = validTargets.GetLeastSignificantBitIndex();
            if (index == -1) break;
            validTargets.PopBit(index);
            yield return new StandardMove((ChessCell)pieceCell, (ChessCell)index);
        }
    }
}