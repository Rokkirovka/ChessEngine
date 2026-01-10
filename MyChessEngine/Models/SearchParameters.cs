namespace MyChessEngine.Models;

public record SearchParameters
{
    public required int Depth { get; set; }
    public bool UseQuiescenceSearch { get; init; } = true;
    public bool UseKillerMoves { get; init; } = true;
    public bool UseHistoryTable { get; init; } = true;
    public bool UseNullMovePruning { get; init; } = true;
    public bool UseTranspositionTable { get; init; } = true;
    public bool UseLateMoveReduction { get; init; } = true;
    public bool EnableDebugger { get; init; } = false;
    public int DebuggerMaxDepth { get; init; } = int.MaxValue;
}