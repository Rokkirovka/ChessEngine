namespace MyChessEngine.Models;

public record SearchParameters
{
    public int Depth { get; init; } = 5;
    private int TimeLimitMs { get; init; }
    public bool UseQuiescenceSearch { get; init; } = true;
    public bool UseKillerMoves { get; init; } = true;
    public bool UseHistoryHeuristic { get; init; } = true;
    public bool UseMoveOrdering { get; init; } = true;
    private bool UseTranspositionTable { get; init; }
    private bool UseNullMovePruning { get; init; }
    public int NullMoveReduction { get; init; } = 2;
    private bool UseInternalIterativeDeepening { get; init; }

    public static SearchParameters Default => new();
    
    public static SearchParameters FastSearch => new()
    {
        Depth = 3,
        UseQuiescenceSearch = true,
        UseTranspositionTable = false
    };
    
    public static SearchParameters DeepSearch => new()
    {
        Depth = 8,
        UseQuiescenceSearch = true,
        UseTranspositionTable = true,
        UseNullMovePruning = true
    };
    
    public static SearchParameters Tournament => new()
    {
        Depth = 15,
        TimeLimitMs = 30000,
        UseQuiescenceSearch = true,
        UseTranspositionTable = true,
        UseNullMovePruning = true,
        UseInternalIterativeDeepening = true
    };
    
    public static SearchParameters Debug => new()
    {
        Depth = 4,
        UseQuiescenceSearch = true
    };
    
    public static SearchParameters WithTimeLimit(int timeLimitMs, int defaultDepth = 10)
    {
        return new SearchParameters
        {
            Depth = defaultDepth,
            TimeLimitMs = timeLimitMs,
            UseQuiescenceSearch = true,
            UseTranspositionTable = true
        };
    }
    
    public static SearchParameters WithDepth(int depth)
    {
        return new SearchParameters
        {
            Depth = depth,
            UseQuiescenceSearch = depth > 0
        };
    }
    
    public override string ToString()
    {
        var features = new List<string>();
        
        if (UseQuiescenceSearch) features.Add("QSearch");
        if (UseTranspositionTable) features.Add("TT");
        if (UseNullMovePruning) features.Add("NullMove");
        if (UseInternalIterativeDeepening) features.Add("IID");
        
        var featuresStr = features.Count > 0 ? $" [{string.Join(", ", features)}]" : "";
        var timeStr = TimeLimitMs > 0 ? $", Time: {TimeLimitMs}ms" : "";
        
        return $"Depth: {Depth}{timeStr}{featuresStr}";
    }
}