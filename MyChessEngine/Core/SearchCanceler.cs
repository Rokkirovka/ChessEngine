using System.Diagnostics;

namespace MyChessEngine.Core;

public class SearchCanceler
{
    public bool MustStop;
    private readonly Stopwatch _stopwatch = new();
    private TimeSpan _timeLimit;
    
    public void StopImmediately() => MustStop = true;
    
    public bool ShouldStop
    {
        get
        {
            if (MustStop) return true;
            if (!_stopwatch.IsRunning) return false;
            return _stopwatch.Elapsed > _timeLimit * 0.85;
        }
    }
    
    public void StopByTimeLimit(TimeSpan timeLimit)
    {
        _timeLimit = timeLimit;
        _stopwatch.Restart();
        Task.Delay(timeLimit).ContinueWith(_ => MustStop = true);
    }
    
    public void Reset()
    {
        MustStop = false;
        _stopwatch.Reset();
    }
}