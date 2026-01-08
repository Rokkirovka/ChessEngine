using MyChess.Core;
using MyChess.Models;
using MyChess.Models.Moves;

namespace MyChess.Rules.MoveGenerator;

public class RookMoveGenerator : IMoveGenerator
{
    public static readonly RookMoveGenerator Instance = new();

    private static readonly ulong[] RookMagics;
    private static readonly ulong[,] RookAttacks;
    private static readonly int[] RookShifts;
    private static readonly ulong[] RookMasks;

    static RookMoveGenerator()
    {
        RookMagics = new ulong[64];
        RookAttacks = new ulong[64, 4096];
        RookShifts = new int[64];
        RookMasks = new ulong[64];
        
        InitializeRookMagics();
        InitializeRookAttackTable();
    }
    
    private RookMoveGenerator() { }

    private static void InitializeRookMagics()
    {
        ulong[] magics =
        [
            0x8a80104000800020, 0x140002000100040, 0x2801880a0017001, 0x100081001000420,
            0x200020010080420, 0x3001c0002010008, 0x8480008002000100, 0x2080088004402900,
            0x800098204000, 0x2024401000200040, 0x100802000801000, 0x120800800801000,
            0x208808088000400, 0x2802200800400, 0x2200800100020080, 0x801000060821100,
            0x80044006422000, 0x100808020004000, 0x12108a0010204200, 0x140848010000802,
            0x481828014002800, 0x8094004002004100, 0x4010040010010802, 0x20008806104,
            0x100400080208000, 0x2040002120081000, 0x21200680100081, 0x20100080080080,
            0x2000a00200410, 0x20080800400, 0x80088400100102, 0x80004600042881,
            0x4040008040800020, 0x440003000200801, 0x4200011004500, 0x188020010100100,
            0x14800401802800, 0x2080040080800200, 0x124080204001001, 0x200046502000484,
            0x480400080088020, 0x1000422010034000, 0x30200100110040, 0x100021010009,
            0x2002080100110004, 0x202008004008002, 0x20020004010100, 0x2048440040820001,
            0x101002200408200, 0x40802000401080, 0x4008142004410100, 0x2060820c0120200,
            0x1001004080100, 0x20c020080040080, 0x2935610830022400, 0x44440041009200,
            0x280001040802101, 0x2100190040002085, 0x80c0084100102001, 0x4024081001000421,
            0x20030a0244872, 0x12001008414402, 0x2006104900a0804, 0x1004081002402
        ];

        int[] bits =
        [
            12, 11, 11, 11, 11, 11, 11, 12,
            11, 10, 10, 10, 10, 10, 10, 11,
            11, 10, 10, 10, 10, 10, 10, 11,
            11, 10, 10, 10, 10, 10, 10, 11,
            11, 10, 10, 10, 10, 10, 10, 11,
            11, 10, 10, 10, 10, 10, 10, 11,
            11, 10, 10, 10, 10, 10, 10, 11,
            12, 11, 11, 11, 11, 11, 11, 12
        ];

        for (var i = 0; i < 64; i++)
        {
            RookMagics[i] = magics[i];
            RookShifts[i] = 64 - bits[i];
            RookMasks[i] = CalculateRookMask(i);
        }
    }

    private static ulong CalculateRookMask(int cell)
    {
        ulong mask = 0;
        var tr = cell / 8;
        var tf = cell % 8;

        for (var r = tr + 1; r <= 6; r++) mask |= 1UL << (r * 8 + tf);
        for (var r = tr - 1; r >= 1; r--) mask |= 1UL << (r * 8 + tf);
        for (var f = tf + 1; f <= 6; f++) mask |= 1UL << (tr * 8 + f);
        for (var f = tf - 1; f >= 1; f--) mask |= 1UL << (tr * 8 + f);

        return mask;
    }

    private static void InitializeRookAttackTable()
    {
        for (var square = 0; square < 64; square++)
        {
            var mask = RookMasks[square];
            var bitCount = CountBits(mask);
            var size = 1 << bitCount;

            for (var i = 0; i < size; i++)
            {
                var occupancy = SetOccupancy(i, bitCount, mask);
                var index = (int)((occupancy * RookMagics[square]) >> RookShifts[square]);
                RookAttacks[square, index] = CalculateRookAttacksSlow(square, occupancy);
            }
        }
    }

    private static ulong CalculateRookAttacksSlow(int cell, ulong occupancy)
    {
        ulong attacks = 0;
        var tr = cell / 8;
        var tf = cell % 8;

        for (var r = tr + 1; r <= 7; r++)
        {
            var square = r * 8 + tf;
            attacks |= 1UL << square;
            if ((occupancy & (1UL << square)) != 0) break;
        }

        for (var r = tr - 1; r >= 0; r--)
        {
            var square = r * 8 + tf;
            attacks |= 1UL << square;
            if ((occupancy & (1UL << square)) != 0) break;
        }

        for (var f = tf + 1; f <= 7; f++)
        {
            var square = tr * 8 + f;
            attacks |= 1UL << square;
            if ((occupancy & (1UL << square)) != 0) break;
        }

        for (var f = tf - 1; f >= 0; f--)
        {
            var square = tr * 8 + f;
            attacks |= 1UL << square;
            if ((occupancy & (1UL << square)) != 0) break;
        }

        return attacks;
    }

    public static BitBoard GetRookAttacks(int cell, ulong occupancy)
    {
        occupancy &= RookMasks[cell];
        occupancy *= RookMagics[cell];
        occupancy >>= RookShifts[cell];
        return (BitBoard)RookAttacks[cell, occupancy];
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
        var attacks = GetRookAttacks(pieceCell, (ulong)allPieces);
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