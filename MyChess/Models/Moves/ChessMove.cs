namespace MyChess.Models.Moves;

public abstract class ChessMove(ChessCell from, ChessCell to)
{
    public ChessCell From { get; } = from;
    public ChessCell To { get; } = to;
    
    public override bool Equals(object? obj)
    {
        if (obj is not ChessMove other) return false;
        return From == other.From && To == other.To;
    }

    public override string ToString()
    {
        return From + "-" + To;
    }

    public override int GetHashCode() => HashCode.Combine(From, To);
    
    public static bool operator ==(ChessMove left, ChessMove right) => Equals(left, right);
    public static bool operator !=(ChessMove left, ChessMove right) => !Equals(left, right);
}