namespace MyChessEngine.Transposition;

public enum NodeType : byte
{
    Exact,
    UpperBound,
    LowerBound
}