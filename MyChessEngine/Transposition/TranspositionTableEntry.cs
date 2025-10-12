using MyChess.Models.Moves;

namespace MyChessEngine.Transposition;

public struct TranspositionTableEntry
{
    public ulong Hash { get; set; }
    public int Score { get; set; }
    public byte Depth { get; set; }
    public ChessMove? BestMove { get; set; }
    public NodeType NodeType { get; set; }
    public byte Age { get; set; }

    public bool IsEmpty => Hash == 0;
}