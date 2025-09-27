using MyChess.Core;
using MyChess.Models.Moves;

namespace MyChess.Rules.MoveGenerator;

public class BlackPawnMoveGenerator : PawnMoveGenerator
{
    public static readonly BlackPawnMoveGenerator Instance = new();
    
    public static readonly BitBoard[] BlackPawnAttackMasks = PawnAttackMasks[1];
    
    public static readonly BitBoard[] BlackPawnMoveMasks = PawnMoveMasks[1];

    private BlackPawnMoveGenerator()
    {
    }
    public override IEnumerable<ChessMove> GetPossibleMoves(int pieceCell, BitBoard enemyPieces, BitBoard friendlyPieces)
    {
        return GetPossibleMoves(1, pieceCell, enemyPieces, friendlyPieces);
    }
}