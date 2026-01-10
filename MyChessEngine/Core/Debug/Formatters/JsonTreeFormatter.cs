using System.Text.Json;
using MyChessEngine.Core.Debug.Models;

namespace MyChessEngine.Core.Debug.Formatters;

public class JsonTreeFormatter : ISearchTreeFormatter
{
    private readonly JsonSerializerOptions _options;
    
    public JsonTreeFormatter()
    {
        _options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }
    
    public string Format(SearchTreeNode? rootNode, int maxDepth = int.MaxValue)
    {
        if (rootNode == null)
            return "{}";
        
        var dto = ConvertToDto(rootNode, maxDepth);
        return JsonSerializer.Serialize(dto, _options);
    }
    
    public string FormatNode(SearchTreeNode node, int indentLevel = 0)
    {
        var dto = ConvertToDto(node, int.MaxValue);
        return JsonSerializer.Serialize(dto, _options);
    }
    
    private SearchTreeNodeDto ConvertToDto(SearchTreeNode node, int maxDepth, int currentDepth = 0)
    {
        if (currentDepth > maxDepth)
            return new SearchTreeNodeDto { Move = node.Move?.ToString(), Depth = node.Depth, Truncated = true };
        
        return new SearchTreeNodeDto
        {
            Move = node.Move?.ToString(),
            Depth = node.Depth,
            Score = node.Score,
            Alpha = node.Alpha,
            Beta = node.Beta,
            NodeType = node.NodeType.ToString(),
            UsedNullMovePruning = node.UsedNullMovePruning,
            UsedLateMoveReduction = node.UsedLateMoveReduction,
            UsedTranspositionTable = node.UsedTranspositionTable,
            UsedQuiescenceSearch = node.UsedQuiescenceSearch,
            PruningReason = node.PruningReason?.ToString(),
            PruningDetails = node.PruningDetails,
            MoveIndex = node.MoveIndex,
            WasPruned = node.WasPruned,
            WasBetaCutoff = node.WasBetaCutoff,
            ReducedDepth = node.ReducedDepth,
            PositionHash = node.PositionHash?.ToString("X16"),
            BranchPath = node.BranchPath,
            Children = node.Children.Select(c => ConvertToDto(c, maxDepth, currentDepth + 1)).ToList()
        };
    }
    
    private class SearchTreeNodeDto
    {
        public string? Move { get; set; }
        public int Depth { get; set; }
        public int? Score { get; set; }
        public int Alpha { get; set; }
        public int Beta { get; set; }
        public string? NodeType { get; set; }
        public bool UsedNullMovePruning { get; set; }
        public bool UsedLateMoveReduction { get; set; }
        public bool UsedTranspositionTable { get; set; }
        public bool UsedQuiescenceSearch { get; set; }
        public string? PruningReason { get; set; }
        public string? PruningDetails { get; set; }
        public int MoveIndex { get; set; }
        public bool WasPruned { get; set; }
        public bool WasBetaCutoff { get; set; }
        public int? ReducedDepth { get; set; }
        public string? PositionHash { get; set; }
        public string? BranchPath { get; set; }
        public List<SearchTreeNodeDto>? Children { get; set; }
        public bool Truncated { get; set; }
    }
}

