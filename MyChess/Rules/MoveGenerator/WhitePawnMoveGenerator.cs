using MyChess.Core.Board;
using MyChess.Models.Moves;

namespace MyChess.Rules.MoveGenerator;

public class WhitePawnMoveGenerator : PawnMoveGenerator
{
    public static readonly WhitePawnMoveGenerator Instance = new();
    
    public static readonly BitBoard[] WhitePawnAttackMasks = PawnAttackMasks[0];
    
    public static readonly BitBoard[] WhitePawnMoveMasks = PawnMoveMasks[0];

    private WhitePawnMoveGenerator()
    {
    }

    public override IEnumerable<ChessMove> GetPossibleMoves(int pieceCell, BitBoard enemyPieces,
        BitBoard friendlyPieces)
    {
        return GetPossibleMoves(0, pieceCell, enemyPieces, friendlyPieces);
    }
}