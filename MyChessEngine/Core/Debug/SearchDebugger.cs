using MyChess.Models.Moves;
using MyChessEngine.Core.Debug.Models;
using MyChessEngine.Models;
using MyChessEngine.Transposition;

namespace MyChessEngine.Core.Debug;

public class SearchDebugger
{
    private SearchTreeNode? _rootNode;
    private SearchTreeNode? _currentNode;
    private readonly Stack<SearchTreeNode> _nodeStack = new();
    private bool _isEnabled;
    
    public bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            _isEnabled = value;
            if (!value)
            {
                Clear();
            }
        }
    }
    
    public SearchTreeNode? RootNode => _rootNode;
    
    public void StartSearch(SearchContext context)
    {
        if (!IsEnabled) return;
        
        Clear();
        _rootNode = new SearchTreeNode
        {
            Move = null,
            Depth = context.Parameters.Depth,
            Alpha = -1_000_000,
            Beta = 1_000_000,
            NodeType = NodeType.Exact,
            BranchPath = "root"
        };
        _currentNode = _rootNode;
        _nodeStack.Clear();
        _nodeStack.Push(_rootNode);
    }
    
    public SearchTreeNode EnterNode(ChessMove? move, int depth, int alpha, int beta, int moveIndex = 0)
    {
        if (!IsEnabled || _currentNode == null) return null!;
        
        var node = new SearchTreeNode
        {
            Move = move,
            Depth = depth,
            Alpha = alpha,
            Beta = beta,
            MoveIndex = moveIndex,
            Parent = _currentNode,
            BranchPath = _currentNode.BranchPath + (move is not null ? $"/{move}" : "")
        };
        
        _currentNode.Children.Add(node);
        _nodeStack.Push(node);
        _currentNode = node;
        
        return node;
    }
    
    public void ExitNode(int? score, Transposition.NodeType nodeType, bool wasBetaCutoff = false)
    {
        if (!IsEnabled || _currentNode == null) return;
        
        _currentNode.Score = score;
        _currentNode.NodeType = nodeType;
        _currentNode.WasBetaCutoff = wasBetaCutoff;
        
        if (_nodeStack.Count > 1)
        {
            _nodeStack.Pop();
            _currentNode = _nodeStack.Peek();
        }
    }
    
    public void MarkNullMovePruning(int? score)
    {
        if (!IsEnabled || _currentNode == null) return;
        _currentNode.UsedNullMovePruning = true;
        _currentNode.Score = score;
        _currentNode.PruningReason = PruningReason.NullMovePruning;
        _currentNode.PruningDetails = $"Null move pruning returned score: {score}";
    }
    
    public void MarkLateMoveReduction(int reducedDepth, bool wasReSearch)
    {
        if (!IsEnabled || _currentNode == null) return;
        _currentNode.UsedLateMoveReduction = true;
        _currentNode.ReducedDepth = reducedDepth;
        _currentNode.PruningDetails = wasReSearch 
            ? $"LMR: reduced depth {reducedDepth}, re-searched at full depth"
            : $"LMR: reduced depth {reducedDepth}, pruned";
    }
    
    public void MarkTranspositionTable(int score, Transposition.NodeType nodeType, ulong hash)
    {
        if (!IsEnabled || _currentNode == null) return;
        _currentNode.UsedTranspositionTable = true;
        _currentNode.Score = score;
        _currentNode.NodeType = nodeType;
        _currentNode.PositionHash = hash;
        _currentNode.PruningReason = PruningReason.TranspositionTableHit;
        _currentNode.PruningDetails = $"TT hit: {nodeType} score {score}";
    }
    
    public void MarkQuiescenceSearch()
    {
        if (!IsEnabled || _currentNode == null) return;
        _currentNode.UsedQuiescenceSearch = true;
    }
    
    public void MarkPruned(PruningReason reason, string? details = null)
    {
        if (!IsEnabled || _currentNode == null) return;
        _currentNode.WasPruned = true;
        _currentNode.PruningReason = reason;
        _currentNode.PruningDetails = details ?? reason.ToString();
    }
    
    public void MarkBetaCutoff(ChessMove move, int score)
    {
        if (!IsEnabled || _currentNode == null) return;
        _currentNode.WasBetaCutoff = true;
        _currentNode.PruningReason = PruningReason.AlphaBetaCutoff;
        _currentNode.PruningDetails = $"Beta cutoff at move {move}, score {score}";
    }
    
    public void SetPositionHash(ulong hash)
    {
        if (!IsEnabled || _currentNode == null) return;
        _currentNode.PositionHash = hash;
    }
    
    public SearchTreeNode? FindNodeByPath(string branchPath)
    {
        if (!IsEnabled || _rootNode == null) return null;
        
        if (branchPath == "root" || string.IsNullOrEmpty(branchPath))
            return _rootNode;
        
        var parts = branchPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length > 0 && parts[0] == "root")
        {
            parts = parts.Skip(1).ToArray();
        }
        
        return FindNodeByMoves(_rootNode, parts);
    }
    
    public bool WasBranchExamined(params string[] moves)
    {
        var path = "root/" + string.Join("/", moves);
        var node = FindNodeByPath(path);
        return node != null && !node.WasPruned;
    }
    
    public string? GetBranchPruningReason(params string[] moves)
    {
        var path = "root/" + string.Join("/", moves);
        var node = FindNodeByPath(path);
        
        if (node == null)
            return "Branch was never examined";
        
        if (node.WasPruned)
            return node.PruningDetails ?? node.PruningReason?.ToString() ?? "Unknown reason";
        
        return null;
    }
    
    public void Clear()
    {
        _rootNode = null;
        _currentNode = null;
        _nodeStack.Clear();
    }
    
    private SearchTreeNode? FindNodeByMoves(SearchTreeNode node, string[] moves)
    {
        if (moves.Length == 0) return node;
        
        var targetMove = moves[0];
        foreach (var child in node.Children)
        {
            if (child.Move is not null && child.Move.ToString() == targetMove)
            {
                if (moves.Length == 1)
                    return child;
                return FindNodeByMoves(child, moves.Skip(1).ToArray());
            }
        }
        
        return null;
    }
}

