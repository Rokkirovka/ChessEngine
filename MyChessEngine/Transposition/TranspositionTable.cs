using MyChess.Models.Moves;
using MyChessEngine.Models;

namespace MyChessEngine.Transposition;

public class TranspositionTable
{
    private readonly TranspositionTableEntry[] _table;
    private readonly ulong _sizeMask;
    private byte _currentAge;
    private readonly int _bucketSize;

    public TranspositionTable(int sizeInEntries = 1 << 20, int bucketSize = 4)
    {
        if ((sizeInEntries & (sizeInEntries - 1)) != 0)
            throw new ArgumentException("Size must be power of two");
        
        if (bucketSize is < 1 or > 8)
            throw new ArgumentException("Bucket size must be between 1 and 8");

        _table = new TranspositionTableEntry[sizeInEntries * bucketSize];
        _sizeMask = (ulong)(sizeInEntries - 1);
        _currentAge = 0;
        _bucketSize = bucketSize;
    }

    public void Store(ulong hash, int score, int depth, ChessMove? bestMove, NodeType nodeType)
    {
        var bucketIndex = GetBucketIndex(hash);
        var bucketStart = bucketIndex * _bucketSize;

        for (var i = 0; i < _bucketSize; i++)
        {
            var entryIndex = bucketStart + i;
            if (_table[entryIndex].Hash != hash) continue;
            _table[entryIndex] = CreateEntry(hash, score, depth, bestMove, nodeType);
            return;
        }

        var replacementIndex = FindReplacementSlot(bucketStart, depth);
        _table[replacementIndex] = CreateEntry(hash, score, depth, bestMove, nodeType);
    }

    public bool TryGet(ulong hash, out TranspositionTableEntry entry, SearchContext context)
    {
        if (!context.Parameters.UseTranspositionTable)
        {
            entry = default;
            return false;
        }

        var bucketIndex = GetBucketIndex(hash);
        var bucketStart = bucketIndex * _bucketSize;

        for (var i = 0; i < _bucketSize; i++)
        {
            var candidate = _table[bucketStart + i];
            if (candidate.Hash != hash) continue;
            entry = candidate;
            return true;
        }

        entry = default;
        return false;
    }

    public void IncrementAge()
    {
        _currentAge++;
    }

    private int GetBucketIndex(ulong hash) => (int)(hash & _sizeMask);

    private int FindReplacementSlot(int bucketStart, int newDepth)
    {
        var worstSlot = bucketStart;
        var worstScore = byte.MaxValue;

        for (var i = 0; i < _bucketSize; i++)
        {
            var entryIndex = bucketStart + i;
            var entry = _table[entryIndex];

            if (entry.IsEmpty) return entryIndex;

            var score = CalculateReplacementScore(entry, newDepth);

            if (score >= worstScore) continue;
            worstScore = score;
            worstSlot = entryIndex;
        }

        return worstSlot;
    }

    private byte CalculateReplacementScore(TranspositionTableEntry entry, int newDepth)
    {
        byte score = 0;

        var ageDiff = (byte)(_currentAge - entry.Age);
        if (ageDiff > 10) ageDiff = 10;
        score += (byte)(ageDiff * 2);

        if (entry.Depth < newDepth) score += (byte)((newDepth - entry.Depth) * 3);
        
        return score;
    }

    private TranspositionTableEntry CreateEntry(ulong hash, int score, int depth, ChessMove? bestMove, NodeType nodeType)
    {
        return new TranspositionTableEntry
        {
            Hash = hash,
            Score = score,
            Depth = (byte)depth,
            BestMove = bestMove,
            NodeType = nodeType,
            Age = _currentAge
        };
    }
}