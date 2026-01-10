using MyChess.Models.Moves;
using MyChessEngine.Transposition;

namespace MyChessEngine.Core.Debug.Models;

public class SearchTreeNode
{
    public required ChessMove? Move { get; init; }
    public required int Depth { get; init; }
    public int? Score { get; set; }
    public int Alpha { get; set; }
    public int Beta { get; set; }
    public NodeType NodeType { get; set; }
    public List<SearchTreeNode> Children { get; } = new();
    public SearchTreeNode? Parent { get; set; }
    
    public bool UsedNullMovePruning { get; set; }
    public bool UsedLateMoveReduction { get; set; }
    public bool UsedTranspositionTable { get; set; }
    public bool UsedQuiescenceSearch { get; set; }
    
    public PruningReason? PruningReason { get; set; }
    public string? PruningDetails { get; set; }
    
    public int MoveIndex { get; set; }
    public bool WasPruned { get; set; }
    public bool WasBetaCutoff { get; set; }
    public int? ReducedDepth { get; set; }
    public ulong? PositionHash { get; set; }
    
    public string BranchPath { get; set; } = string.Empty;
}

public enum PruningReason
{
    None,
    AlphaBetaCutoff,
    NullMovePruning,
    LateMoveReduction,
    TranspositionTableHit,
    TerminalNode,
    SearchCancelled,
    QuiescenceStandPat
}

