using MyChess.Core;
using MyChessEngine.Core;
using MyChessEngine.Models;

namespace MyChessEngine.Examples;

public static class DebuggerExample
{
    public static void RunExample()
    {
        var game = new ChessGame();
        
        var searchParameters = new SearchParameters
        {
            Depth = 4,
            EnableDebugger = true,
            DebuggerMaxDepth = 4
        };
        
        var engine = new ChessEngine();
        
        var result = engine.FindBestMoveWithIterativeDeepening(game, searchParameters);
        
        Console.WriteLine($"Best move: {result.BestMove}");
        Console.WriteLine($"Score: {result.Score}");
        Console.WriteLine();
        
        var debuggerService = engine.GetDebuggerService();
        if (debuggerService != null)
        {
            Console.WriteLine("=== SEARCH TREE (Console Output) ===");
            debuggerService.WriteToConsole(maxDepth: 3);
            
            debuggerService.WriteToFile("search_tree.txt", maxDepth: 4, asJson: false);
            Console.WriteLine("\nSearch tree written to search_tree.txt");
            
            debuggerService.WriteToFile("search_tree.json", maxDepth: 4, asJson: true);
            Console.WriteLine("Search tree (JSON) written to search_tree.json");
            
            Console.WriteLine("\n=== BRANCH QUERIES ===");
            var wasExamined = debuggerService.WasBranchExamined("e2e4", "e7e5");
            Console.WriteLine($"Was e2e4 e7e5 examined? {wasExamined}");
            
            var pruningReason = debuggerService.GetBranchPruningReason("e2e4", "e7e5", "g8f6");
            Console.WriteLine($"Why was e2e4 e7e5 g8f6 pruned? {pruningReason ?? "Not pruned"}");
            
            var node = debuggerService.FindNodeByPath("root/e2e4");
            if (node != null)
            {
                Console.WriteLine($"\n=== NODE DETAILS ===");
                Console.WriteLine(debuggerService.GetNodeDetails(node));
            }
        }
    }
}

