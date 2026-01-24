using MyChess.Core.Board;
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
        var bitBoard = new BitBoard(0UL);
        bitBoard = bitBoard.SetBit(cell);

        attacks |= (ulong)(bitBoard >> 8) | (ulong)(bitBoard << 8); 
    
        attacks |= (ulong)(bitBoard >> 9) & FileHExcludedMask;
        attacks |= (ulong)(bitBoard >> 7) & FileAExcludedMask; 
        attacks |= (ulong)(bitBoard >> 1) & FileHExcludedMask;
    
        attacks |= (ulong)(bitBoard << 9) & FileAExcludedMask;
        attacks |= (ulong)(bitBoard << 7) & FileHExcludedMask;
        attacks |= (ulong)(bitBoard << 1) & FileAExcludedMask; 

        return new BitBoard(attacks);
    }

    public IEnumerable<ChessMove> GetPossibleMoves(int pieceCell, BitBoard enemyPieces, BitBoard friendlyPieces)
    {
        var attacks = KingAttackMasks[pieceCell];
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