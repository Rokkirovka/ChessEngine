using MyChessEngine.Core.Debug.Models;

namespace MyChessEngine.Core.Debug.Formatters;

public class TextTreeFormatter : ISearchTreeFormatter
{
    private const string IndentString = "  ";
    private const string BranchPrefix = "├─ ";
    private const string LastBranchPrefix = "└─ ";
    private const string ContinuationPrefix = "│  ";
    private const string EmptyPrefix = "   ";
    
    public string Format(SearchTreeNode? rootNode, int maxDepth = int.MaxValue)
    {
        if (rootNode == null)
            return "No search tree available.";
        
        var output = new System.Text.StringBuilder();
        output.AppendLine("=== SEARCH TREE DEBUG OUTPUT ===");
        output.AppendLine();
        FormatNodeRecursive(rootNode, output, "", true, 0, maxDepth);
        return output.ToString();
    }
    
    public string FormatNode(SearchTreeNode node, int indentLevel = 0)
    {
        var indent = new string(' ', indentLevel * 2);
        return FormatNodeDetails(node, indent);
    }
    
    private void FormatNodeRecursive(SearchTreeNode node, System.Text.StringBuilder output, 
        string prefix, bool isLast, int depth, int maxDepth)
    {
        if (depth > maxDepth) return;
        
        var nodeLine = prefix + (isLast ? LastBranchPrefix : BranchPrefix) + FormatNodeDetails(node, "");
        output.AppendLine(nodeLine);
        
        var continuation = isLast ? EmptyPrefix : ContinuationPrefix;
        var newPrefix = prefix + continuation;
        
        var children = node.Children;
        for (int i = 0; i < children.Count; i++)
        {
            var child = children[i];
            var isLastChild = i == children.Count - 1;
            FormatNodeRecursive(child, output, newPrefix, isLastChild, depth + 1, maxDepth);
        }
    }
    
    private string FormatNodeDetails(SearchTreeNode node, string indent)
    {
        var parts = new List<string>();
        
        var moveStr = node.Move?.ToString() ?? "root";
        parts.Add($"Move: {moveStr}");
        
        parts.Add($"Depth: {node.Depth}");
        
        if (node.Score.HasValue)
        {
            parts.Add($"Score: {node.Score.Value}");
        }
        
        parts.Add($"α={node.Alpha} β={node.Beta}");
        
        parts.Add($"Type: {node.NodeType}");
        
        var optimizations = new List<string>();
        if (node.UsedNullMovePruning) optimizations.Add("NMP");
        if (node.UsedLateMoveReduction) optimizations.Add($"LMR({node.ReducedDepth})");
        if (node.UsedTranspositionTable) optimizations.Add("TT");
        if (node.UsedQuiescenceSearch) optimizations.Add("QS");
        
        if (optimizations.Count > 0)
        {
            parts.Add($"Opts: [{string.Join(", ", optimizations)}]");
        }
        
        if (node.WasPruned || node.PruningReason != PruningReason.None)
        {
            var pruningInfo = node.PruningReason?.ToString() ?? "Pruned";
            if (!string.IsNullOrEmpty(node.PruningDetails))
            {
                pruningInfo += $" ({node.PruningDetails})";
            }
            parts.Add($"PRUNED: {pruningInfo}");
        }
        
        if (node.WasBetaCutoff)
        {
            parts.Add("BETA-CUTOFF");
        }
        
        if (node.MoveIndex > 0)
        {
            parts.Add($"Move #{node.MoveIndex}");
        }
        
        var mainLine = string.Join(" | ", parts);
        
        var details = new List<string>();
        
        if (node.PositionHash.HasValue)
        {
            details.Add($"{indent}    Hash: 0x{node.PositionHash.Value:X16}");
        }
        
        if (!string.IsNullOrEmpty(node.BranchPath) && node.BranchPath != "root")
        {
            details.Add($"{indent}    Path: {node.BranchPath}");
        }
        
        var result = mainLine;
        if (details.Count > 0)
        {
            result += "\n" + string.Join("\n", details);
        }
        
        return result;
    }
}

