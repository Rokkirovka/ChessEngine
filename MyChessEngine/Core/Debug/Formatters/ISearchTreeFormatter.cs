using MyChessEngine.Core.Debug.Models;

namespace MyChessEngine.Core.Debug.Formatters;

public interface ISearchTreeFormatter
{
    string Format(SearchTreeNode? rootNode, int maxDepth = int.MaxValue);
    
    string FormatNode(SearchTreeNode node, int indentLevel = 0);
}

