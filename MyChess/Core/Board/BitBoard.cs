using System.Numerics;

namespace MyChess.Core.Board;

public readonly record struct BitBoard(ulong Value = 0UL)
{
    public bool GetBit(int cell) => (Value & (1UL << cell)) != 0;
    public BitBoard SetBit(int cell) => new(Value | (1UL << cell));
    public BitBoard ClearBit(int cell) => new(Value & ~(1UL << cell));
    public bool HasBit(int cell) => (Value & (1UL << cell)) != 0;
    public int CountBits() => BitOperations.PopCount(Value);
    public int GetLeastSignificantBitIndex() => Value == 0 ? -1 : BitOperations.TrailingZeroCount(Value);

    public static BitBoard operator <<(BitBoard bb, int shift) => new(bb.Value << shift);
    public static BitBoard operator >> (BitBoard bb, int shift) => new(bb.Value >> shift);
    public static explicit operator ulong(BitBoard bb) => bb.Value;
    public static explicit operator BitBoard(ulong value) => new(value);
    public static BitBoard operator &(BitBoard left, BitBoard right) => new(left.Value & right.Value);
    public static BitBoard operator |(BitBoard left, BitBoard right) => new(left.Value | right.Value);
    public static BitBoard operator ~(BitBoard bb) => new(~bb.Value);
    public static BitBoard operator ^(BitBoard left, BitBoard right) => new(left.Value ^ right.Value);

    public override string ToString()
    {
        var result = "\n";

        for (var rank = 0; rank < 8; rank++)
        {
            for (var file = 0; file < 8; file++)
            {
                var square = rank * 8 + file;
                result += $" {(GetBit(square) ? 1 : 0)}";
            }

            result += "\n";
        }

        return result;
    }
}