using MyChessEngine.Core.Debug.Formatters;
using MyChessEngine.Core.Debug.Models;

namespace MyChessEngine.Core.Debug;

public class SearchDebuggerService
{
    private readonly SearchDebugger _debugger;
    private readonly ISearchTreeFormatter _textFormatter;
    private readonly ISearchTreeFormatter _jsonFormatter;
    
    public SearchDebugger Debugger => _debugger;
    
    public SearchDebuggerService()
    {
        _debugger = new SearchDebugger();
        _textFormatter = new TextTreeFormatter();
        _jsonFormatter = new JsonTreeFormatter();
    }
    
    public void SetEnabled(bool enabled)
    {
        _debugger.IsEnabled = enabled;
    }
    
    public void WriteToConsole(int maxDepth = int.MaxValue)
    {
        if (!_debugger.IsEnabled)
        {
            Console.WriteLine("Debugger is not enabled.");
            return;
        }
        
        var output = _textFormatter.Format(_debugger.RootNode, maxDepth);
        Console.WriteLine(output);
    }
    
    public void WriteToFile(string filePath, int maxDepth = int.MaxValue, bool asJson = false)
    {
        if (!_debugger.IsEnabled)
        {
            File.WriteAllText(filePath, "Debugger is not enabled.");
            return;
        }
        
        var formatter = asJson ? _jsonFormatter : _textFormatter;
        var output = formatter.Format(_debugger.RootNode, maxDepth);
        File.WriteAllText(filePath, output);
    }
    
    public string GetFormattedTree(int maxDepth = int.MaxValue, bool asJson = false)
    {
        if (!_debugger.IsEnabled)
            return "Debugger is not enabled.";
        
        var formatter = asJson ? _jsonFormatter : _textFormatter;
        return formatter.Format(_debugger.RootNode, maxDepth);
    }
    
    public bool WasBranchExamined(params string[] moves)
    {
        return _debugger.WasBranchExamined(moves);
    }
    
    public string? GetBranchPruningReason(params string[] moves)
    {
        return _debugger.GetBranchPruningReason(moves);
    }
    
    public SearchTreeNode? FindNodeByPath(string branchPath)
    {
        return _debugger.FindNodeByPath(branchPath);
    }
    
    public string GetNodeDetails(SearchTreeNode node)
    {
        return _textFormatter.FormatNode(node);
    }
}

