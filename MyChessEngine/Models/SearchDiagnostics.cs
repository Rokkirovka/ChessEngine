namespace MyChessEngine.Models;

public class SearchDiagnostics
{
    public int NodesVisited { get; set; }
    public int QuiescenceNodesVisited { get; set; }
    public int TotalNodes => NodesVisited + QuiescenceNodesVisited;
    public int Depth { get; set; }
    public TimeSpan TimeElapsed { get; set; }
    public int BetaCutoffs { get; set; }
    public int AlphaImprovements { get; set; }
    
    public int HashMoveCutoffs { get; set; }
    public int KillerMoveCutoffs { get; set; }
    public int CaptureCutoffs { get; set; }
    
    public override string ToString()
    {
        return $"Nodes: {TotalNodes} (Main: {NodesVisited}, Q: {QuiescenceNodesVisited}), " +
               $"Depth: {Depth}, Time: {TimeElapsed.TotalMilliseconds}ms, " +
               $"Cutoffs: {BetaCutoffs}, Improvements: {AlphaImprovements}";
    }
}