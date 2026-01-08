namespace MyChessEngine.Core.Evaluation.Moves.Components;

public class HistoryTableService
{
    private const int PieceCount = 12;
    private const int CellsCount = 64;
    
    private readonly int[,] _historyTable = new int[PieceCount, CellsCount];


    public void UpdateHistoryTable(int pieceIndex, int toCell, int depth)
    {
        var bonus = depth * depth;
        _historyTable[pieceIndex, toCell] += bonus;
    }

    public int GetScore(int pieceIndex, int toCell)
    {
        return _historyTable[pieceIndex, toCell];
    }
}