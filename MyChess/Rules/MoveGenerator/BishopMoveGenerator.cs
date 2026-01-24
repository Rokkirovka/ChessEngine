using MyChess.Core.Board;
using MyChess.Models;
using MyChess.Models.Moves;

namespace MyChess.Rules.MoveGenerator;

public class BishopMoveGenerator : IMoveGenerator
{
    public static readonly BishopMoveGenerator Instance = new();

    private static readonly ulong[] BishopMagics;
    private static readonly ulong[,] BishopAttacks;
    private static readonly int[] BishopShifts;
    private static readonly ulong[] BishopMasks;

    static BishopMoveGenerator()
    {
        BishopMagics = new ulong[64];
        BishopAttacks = new ulong[64, 512];
        BishopShifts = new int[64];
        BishopMasks = new ulong[64];
        
        InitializeBishopMagics();
        InitializeBishopAttackTable();
    }
    
    private BishopMoveGenerator() { }

    private static void InitializeBishopMagics()
    {
        ulong[] magics =
        [
            0x40040844404084, 0x2004208a004208, 0x10190041080202, 0x108060845042010,
            0x581104180800210, 0x2112080446200010, 0x1080820820060210, 0x3c0808410220200,
            0x4050404440404, 0x21001420088, 0x24d0080801082102, 0x1020a0a020400,
            0x40308200402, 0x4011002100800, 0x401484104104005, 0x801010402020200,
            0x400210c3880100, 0x404022024108200, 0x810018200204102, 0x4002801a02003,
            0x85040820080400, 0x810102c808880400, 0xe900410884800, 0x8002020480840102,
            0x220200865090201, 0x2010100a02021202, 0x152048408022401, 0x20080002081110,
            0x4001001021004000, 0x800040400a011002, 0xe4004081011002, 0x1c004001012080,
            0x8004200962a00220, 0x8422100208500202, 0x2000402200300c08, 0x8646020080080080,
            0x80020a0200100808, 0x2010004880111000, 0x623000a080011400, 0x42008c0340209202,
            0x209188240001000, 0x400408a884001800, 0x110400a6080400, 0x1840060a44020800,
            0x90080104000041, 0x201011000808101, 0x1a2208080504f080, 0x8012020600211212,
            0x500861011240000, 0x180806108200800, 0x4000020e01040044, 0x300000261044000a,
            0x802241102020002, 0x20906061210001, 0x5a84841004010310, 0x4010801011c04,
            0xa010109502200, 0x4a02012000, 0x500201010098b028, 0x8040002811040900,
            0x28000010020204, 0x6000020202d0240, 0x8918844842082200, 0x4010011029020020
        ];

        int[] bits =
        [
            6, 5, 5, 5, 5, 5, 5, 6,
            5, 5, 5, 5, 5, 5, 5, 5,
            5, 5, 7, 7, 7, 7, 5, 5,
            5, 5, 7, 9, 9, 7, 5, 5,
            5, 5, 7, 9, 9, 7, 5, 5,
            5, 5, 7, 7, 7, 7, 5, 5,
            5, 5, 5, 5, 5, 5, 5, 5,
            6, 5, 5, 5, 5, 5, 5, 6
        ];

        for (var i = 0; i < 64; i++)
        {
            BishopMagics[i] = magics[i];
            BishopShifts[i] = 64 - bits[i];
            BishopMasks[i] = CalculateBishopMask(i);
        }
    }

    private static ulong CalculateBishopMask(int cell)
    {
        ulong mask = 0;
        var tr = cell / 8;
        var tf = cell % 8;

        for (int r = tr + 1, f = tf + 1; r <= 6 && f <= 6; r++, f++) mask |= 1UL << (r * 8 + f);
        for (int r = tr - 1, f = tf + 1; r >= 1 && f <= 6; r--, f++) mask |= 1UL << (r * 8 + f);
        for (int r = tr + 1, f = tf - 1; r <= 6 && f >= 1; r++, f--) mask |= 1UL << (r * 8 + f);
        for (int r = tr - 1, f = tf - 1; r >= 1 && f >= 1; r--, f--) mask |= 1UL << (r * 8 + f);

        return mask;
    }

    private static void InitializeBishopAttackTable()
    {
        for (var square = 0; square < 64; square++)
        {
            var mask = BishopMasks[square];
            var bitCount = CountBits(mask);
            var size = 1 << bitCount;

            for (var i = 0; i < size; i++)
            {
                var occupancy = SetOccupancy(i, bitCount, mask);
                var index = (int)((occupancy * BishopMagics[square]) >> BishopShifts[square]);
                BishopAttacks[square, index] = CalculateBishopAttacksSlow(square, occupancy);
            }
        }
    }

    private static ulong CalculateBishopAttacksSlow(int cell, ulong occupancy)
    {
        ulong attacks = 0;
        var tr = cell / 8;
        var tf = cell % 8;

        for (int r = tr + 1, f = tf + 1; r <= 7 && f <= 7; r++, f++)
        {
            var square = r * 8 + f;
            attacks |= 1UL << square;
            if ((occupancy & (1UL << square)) != 0) break;
        }

        for (int r = tr - 1, f = tf + 1; r >= 0 && f <= 7; r--, f++)
        {
            var square = r * 8 + f;
            attacks |= 1UL << square;
            if ((occupancy & (1UL << square)) != 0) break;
        }

        for (int r = tr + 1, f = tf - 1; r <= 7 && f >= 0; r++, f--)
        {
            var square = r * 8 + f;
            attacks |= 1UL << square;
            if ((occupancy & (1UL << square)) != 0) break;
        }

        for (int r = tr - 1, f = tf - 1; r >= 0 && f >= 0; r--, f--)
        {
            var square = r * 8 + f;
            attacks |= 1UL << square;
            if ((occupancy & (1UL << square)) != 0) break;
        }

        return attacks;
    }

    public static BitBoard GetBishopAttacks(int cell, ulong occupancy)
    {
        occupancy &= BishopMasks[cell];
        occupancy *= BishopMagics[cell];
        occupancy >>= BishopShifts[cell];
        return (BitBoard)BishopAttacks[cell, occupancy];
    }

    private static int CountBits(ulong x)
    {
        var count = 0;
        while (x != 0)
        {
            count++;
            x &= x - 1;
        }
        return count;
    }

    private static ulong SetOccupancy(int index, int bits, ulong attackMask)
    {
        ulong occupancy = 0;
        for (var count = 0; count < bits; count++)
        {
            var square = ((BitBoard)attackMask).GetLeastSignificantBitIndex();
            attackMask &= attackMask - 1;
            if ((index & (1 << count)) != 0)
                occupancy |= 1UL << square;
        }
        return occupancy;
    }

    public IEnumerable<ChessMove> GetPossibleMoves(int pieceCell, BitBoard enemyPieces, BitBoard friendlyPieces)
    {
        var allPieces = enemyPieces | friendlyPieces;
        var attacks = GetBishopAttacks(pieceCell, (ulong)allPieces);
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