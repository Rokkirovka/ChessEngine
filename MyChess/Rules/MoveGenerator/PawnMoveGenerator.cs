using System.Collections.Generic;
using MyChess.Core;
using MyChess.Models;
using MyChess.Models.Moves;

namespace MyChess.Rules.MoveGenerator;

public abstract class PawnMoveGenerator : IMoveGenerator
{
    protected static readonly BitBoard[][] PawnAttackMasks = new BitBoard[2][];
    protected static readonly BitBoard[][] PawnMoveMasks = new BitBoard[2][];

    private const ulong FileAExcludedMask = 18374403900871474942UL;
    private const ulong FileHExcludedMask = 9187201950435737471UL;
    private const ulong SecondRankMask = 71776119061217280UL;
    private const ulong SeventhRankMask = 65280UL;
    private const ulong EighthRankExcludedMask = 18446744073709551360UL;
    private const ulong FirstRankExcludedMask = 72057594037927935UL;
    

    static PawnMoveGenerator()
    {
        PawnAttackMasks[0] = new BitBoard[64];
        PawnAttackMasks[1] = new BitBoard[64];
        PawnMoveMasks[0] = new BitBoard[64];
        PawnMoveMasks[1] = new BitBoard[64];
        InitializePawnAttackMasks();
        InitializePawnMoveMasks();
    }

    private static void InitializePawnAttackMasks()
    {
        for (var cell = 0; cell < 64; cell++)
        {
            PawnAttackMasks[0][cell] = CalculatePawnAttackMask(ChessColor.White, cell);
            PawnAttackMasks[1][cell] = CalculatePawnAttackMask(ChessColor.Black, cell);
        }
    }

    private static void InitializePawnMoveMasks()
    {
        for (var cell = 0; cell < 64; cell++)
        {
            PawnMoveMasks[0][cell] = CalculatePawnMoveMask(ChessColor.White, cell);
            PawnMoveMasks[1][cell] = CalculatePawnMoveMask(ChessColor.Black, cell);
        }
    }

    private static BitBoard CalculatePawnAttackMask(ChessColor color, int cell)
    {
        ulong attacks = 0;
        BitBoard bitBoard = new();
        bitBoard.SetBit(cell);

        if (color is ChessColor.White)
        {
            attacks |= (bitBoard >> 7) & FileAExcludedMask;
            attacks |= (bitBoard >> 9) & FileHExcludedMask;
        }
        else
        {
            attacks |= (bitBoard << 7) & FileHExcludedMask;
            attacks |= (bitBoard << 9) & FileAExcludedMask;
        }

        return attacks;
    }

    private static BitBoard CalculatePawnMoveMask(ChessColor color, int cell)
    {
        ulong moves = 0;
        BitBoard bitBoard = new();
        bitBoard.SetBit(cell);

        if (color is ChessColor.White)
        {
            moves |= bitBoard >> 8;
        }
        else
        {
            moves |= bitBoard << 8;
        }

        return moves;
    }

    protected static IEnumerable<ChessMove> GetPossibleMoves(int color, int pieceCell, BitBoard enemyPieces,
        BitBoard friendlyPieces)
    {
        BitBoard allPieces = enemyPieces | friendlyPieces;
        var direction = color == 0 ? -8 : +8;

        var targetSquare = pieceCell + direction;
        var doubleTarget = targetSquare + direction;
        
        var isStartingRank = color == 0
            ? (SecondRankMask & (1UL << pieceCell)) != 0
            : (SeventhRankMask & (1UL << pieceCell)) != 0;

        if (isStartingRank && doubleTarget is >= 0 and < 64 && !allPieces.GetBit(doubleTarget) &&
            !allPieces.GetBit(targetSquare))
        {
            yield return new StandardMove((ChessCell)pieceCell, (ChessCell)doubleTarget);
        }

        var attacks = PawnAttackMasks[color][pieceCell];
        var moves = PawnMoveMasks[color][pieceCell];
        var validTargets = (BitBoard)((attacks & enemyPieces) | (moves & ~allPieces));

        for (var i = 0; i < 4; i++)
        {
            var index = validTargets.GetLeastSignificantBitIndex();
            if (index == -1) break;
            validTargets.PopBit(index);
            if (index < 8 || index > 55) break;
            yield return new StandardMove((ChessCell)pieceCell, (ChessCell)index);
        }
    }

    public abstract IEnumerable<ChessMove> GetPossibleMoves(
        int pieceCell, BitBoard enemyPieces, BitBoard friendlyPieces);
}