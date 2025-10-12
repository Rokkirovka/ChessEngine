namespace MyChess.Hashing.PositionHistory;

using System.Collections.Generic;

public class RepetitionTable
{
    private readonly Dictionary<ulong, int> _positionCounts = new();
    private readonly Stack<ulong> _positionHistory = new();

    public void AddPosition(ulong hash)
    {
        if (_positionCounts.TryGetValue(hash, out int value))
            _positionCounts[hash] = ++value;
        else
            _positionCounts[hash] = 1;
        
        _positionHistory.Push(hash);
    }

    public void RemoveLastPosition()
    {
        if (_positionHistory.Count <= 0) return;
        var lastHash = _positionHistory.Pop();
        if (!_positionCounts.TryGetValue(lastHash, out var value)) return;
        _positionCounts[lastHash] = --value;
        if (value == 0)
            _positionCounts.Remove(lastHash);
    }

    public bool IsDrawByRepetition(ulong currentHash)
    {
        return _positionCounts.ContainsKey(currentHash) && _positionCounts[currentHash] >= 3;
    }

    public void Clear()
    {
        _positionCounts.Clear();
        _positionHistory.Clear();
    }

    public int GetRepetitionCount(ulong hash)
    {
        return _positionCounts.GetValueOrDefault(hash, 0);
    }
}