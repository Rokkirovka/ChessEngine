namespace MyChessEngine.Utils;

public class MoveOrdererStats
{
    public int HistoryEntries { get; set; }
    public int KillerEntries { get; set; }
    public int MaxHistoryValue { get; set; }

    public override string ToString()
    {
        return $"History: {HistoryEntries} entries, Max: {MaxHistoryValue} | Killers: {KillerEntries} entries";
    }
}