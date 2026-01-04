using System.Diagnostics;

namespace MyChessEngine.Core;

public class SearchCanceler
{
    public bool ShouldStop;
    private readonly Stopwatch _stopwatch = new();
    
    public void StopImmediately() => ShouldStop = true;
    
    public void StopByTimeLimit(TimeSpan timeLimit)
    {
        _stopwatch.Restart();
        Task.Delay(timeLimit).ContinueWith(_ => ShouldStop = true);
    }
    
    public void Reset()
    {
        ShouldStop = false;
        _stopwatch.Reset();
    }
}