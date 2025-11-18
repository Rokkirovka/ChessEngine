namespace MyChessEngine.Models;

public record SearchParameters
{
    public required int Depth { get; set; }
    private int TimeLimitMs { get; set; }
    public bool UseQuiescenceSearch { get; set; } = true;
    public bool UseKillerMoves { get; set; } = true;
    public bool UseHistoryTable { get; set; } = true;
    public bool UseNullMovePruning { get; set; } = true;
    public bool UseTranspositionTable { get; set; } = true;
    public bool UseLateMoveReduction { get; set; } = true;
}