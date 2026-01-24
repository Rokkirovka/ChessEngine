using MyChess.Core.Board;
using MyChess.Models;
using MyChess.Models.Moves;

namespace MyChess.Rules.MoveGenerator;

public class QueenMoveGenerator : IMoveGenerator
{
    public static readonly QueenMoveGenerator Instance = new();

    private QueenMoveGenerator() { }

    public IEnumerable<ChessMove> GetPossibleMoves(int pieceCell, BitBoard enemyPieces, BitBoard friendlyPieces)
    {
        var allPieces = enemyPieces | friendlyPieces;
        
        var rookAttacks = RookMoveGenerator.GetRookAttacks(pieceCell, (ulong)allPieces);
        var bishopAttacks = BishopMoveGenerator.GetBishopAttacks(pieceCell, (ulong)allPieces);
        var attacks = rookAttacks | bishopAttacks;
        
        var validTargets = attacks & ~friendlyPieces;
        var tempValidTargets = validTargets;
        
        while (tempValidTargets.Value != 0)
        {
            var index = tempValidTargets.GetLeastSignificantBitIndex();
            if (index == -1) break;
            tempValidTargets = tempValidTargets.ClearBit(index);
            yield return new StandardMove((ChessCell)pieceCell, (ChessCell)index);
        }
    }
}