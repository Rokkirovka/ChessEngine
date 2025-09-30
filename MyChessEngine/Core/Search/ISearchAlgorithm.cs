using MyChessEngine.Models;

namespace MyChessEngine.Core.Search;

public interface ISearchAlgorithm
{
    EngineResult Search(SearchParameters parameters);
    void StopSearch();
    SearchDiagnostics GetDiagnostics();
}