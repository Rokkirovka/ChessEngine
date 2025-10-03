namespace MyChessEngine.Models;

public record SearchParameters
{
    public required int Depth { get; set; }
    private int TimeLimitMs { get; init; }
    public bool UseQuiescenceSearch { get; private init; } = true;
    public bool UseKillerMoves { get; init; } = true;
    public bool UseHistoryTable { get; init; } = true;
    public bool UseNullMovePruning { get; init; } = true;
    
    public static SearchParameters FastSearch => new()
    {
        Depth = 3,
        UseQuiescenceSearch = true
    };
    
    public static SearchParameters DeepSearch => new()
    {
        Depth = 8,
        UseQuiescenceSearch = true
    };
    
    public static SearchParameters Tournament => new()
    {
        Depth = 15,
        TimeLimitMs = 30000,
        UseQuiescenceSearch = true
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
            TimeLimitMs = timeLimitMs
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
        
        var featuresStr = features.Count > 0 ? $" [{string.Join(", ", features)}]" : "";
        var timeStr = TimeLimitMs > 0 ? $", Time: {TimeLimitMs}ms" : "";
        
        return $"Depth: {Depth}{timeStr}{featuresStr}";
    }
}