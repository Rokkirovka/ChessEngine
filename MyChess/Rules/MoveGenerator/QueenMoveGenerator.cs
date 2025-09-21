namespace MyChess.Rules.MoveGenerator;

public class QueenMoveGenerator : LinearMoveGenerator
{
    public static readonly QueenMoveGenerator Instance = new();

    private QueenMoveGenerator() : base([-8, -1, 1, 8, -9, -7, 7, 9]) { }
}