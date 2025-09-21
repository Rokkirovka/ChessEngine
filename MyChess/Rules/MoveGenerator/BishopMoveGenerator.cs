namespace MyChess.Rules.MoveGenerator;

public class BishopMoveGenerator : LinearMoveGenerator
{
    public static readonly BishopMoveGenerator Instance = new();
    
    private BishopMoveGenerator() : base([-9, -7, 7, 9]) { }
}