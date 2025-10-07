using System.Collections.Generic;
using MyChess.Core;
using MyChess.Models;
using MyChess.Models.Moves;

namespace MyChess.Rules.MoveGenerator;

public class KingMoveGenerator : IMoveGenerator
{
    public static readonly KingMoveGenerator Instance = new();
    
    public static readonly BitBoard[] KingAttackMasks;

    private const ulong FileAExcludedMask = 18374403900871474942UL;

    private const ulong FileHExcludedMask = 9187201950435737471UL;

    static KingMoveGenerator()
    {
        KingAttackMasks = new BitBoard[64];
        InitializeKnightAttackMasks();
    }
    private KingMoveGenerator() { }
    
    private static void InitializeKnightAttackMasks()
    {
        for (var cell = 0; cell < 64; cell++)
        {
            KingAttackMasks[cell] = CalculateKingAttackMask(cell);
        }
    }

    private static BitBoard CalculateKingAttackMask(int cell)
    {
        ulong attacks = 0;
        BitBoard bitBoard = new();
        bitBoard.SetBit(cell);

        attacks |= (bitBoard >> 8) | (bitBoard << 8); 
    
        attacks |= (bitBoard >> 9) & FileHExcludedMask;
        attacks |= (bitBoard >> 7) & FileAExcludedMask; 
        attacks |= (bitBoard >> 1) & FileHExcludedMask;
    
        attacks |= (bitBoard << 9) & FileAExcludedMask;
        attacks |= (bitBoard << 7) & FileHExcludedMask;
        attacks |= (bitBoard << 1) & FileAExcludedMask; 

        return attacks;
    }

    public IEnumerable<ChessMove> GetPossibleMoves(int pieceCell, BitBoard enemyPieces, BitBoard friendlyPieces)
    {
        var attacks = KingAttackMasks[pieceCell];
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