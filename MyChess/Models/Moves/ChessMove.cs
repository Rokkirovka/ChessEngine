namespace MyChess.Models.Moves;

public abstract class ChessMove(ChessCell from, ChessCell to)
{
    public int From { get; } = (int)from;
    public int To { get; } = (int)to;
    
    public override bool Equals(object? obj)
    {
        if (obj is not ChessMove other) return false;
        return From == other.From && To == other.To;
    }

    public override string ToString()
    {
        return $"{(ChessCell)From}{(ChessCell)To}".ToLower();
    }

    public override int GetHashCode() => HashCode.Combine(From, To);
    
    public static bool operator ==(ChessMove left, ChessMove right) => Equals(left, right);
    public static bool operator !=(ChessMove left, ChessMove right) => !Equals(left, right);
}