namespace MyChess.Rules.MoveGenerator;

public class RookMoveGenerator : LinearMoveGenerator
{
    public static readonly RookMoveGenerator Instance = new();

    private RookMoveGenerator() : base([-8, -1, 1, 8]) { }
}