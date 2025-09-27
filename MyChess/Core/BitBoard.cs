using MyChess.Models;

namespace MyChess.Core;

public class BitBoard(ulong value = 0UL)
{
    private ulong _value = value;

    public bool GetBit(int cell)
    {
        return (_value & (1UL << cell)) != 0;
    }

    public void SetBit(int cell)
    {
        _value |= 1UL << cell;
    }

    public bool PopBit(int cell)
    {
        var mask = 1UL << cell;
        var wasSet = (_value & mask) != 0;
        _value &= ~mask;
        return wasSet;
    }

    public int CountBits()
    {
        var count = 0;
        var bitBoard = _value;
        while (bitBoard != 0)
        {
            bitBoard &= bitBoard - 1;
            count++;
        }
        return count;
    }
    
    public int GetLeastSignificantBitIndex()
    {
        var count = 0;
        if (_value== 0) return -1;
        var ls1B = (_value & (ulong)-(long)_value) - 1;
        while (ls1B != 0)
        {
            ls1B &= ls1B - 1;
            count++;
        }
        return count;
    }
    
    public static BitBoard operator <<(BitBoard bb, int shift) => bb._value << shift;
    public static BitBoard operator >>(BitBoard bb, int shift) => bb._value >> shift;
    public static implicit operator ulong(BitBoard bb) => bb._value;
    public static implicit operator BitBoard(ulong value) => new() { _value = value };
    
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