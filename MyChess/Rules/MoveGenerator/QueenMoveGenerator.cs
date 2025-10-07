using System.Collections.Generic;
using MyChess.Core;
using MyChess.Models;
using MyChess.Models.Moves;

namespace MyChess.Rules.MoveGenerator;

public class QueenMoveGenerator : IMoveGenerator
{
    public static readonly QueenMoveGenerator Instance = new();

    private QueenMoveGenerator() { }

    public IEnumerable<ChessMove> GetPossibleMoves(int pieceCell, BitBoard enemyPieces, BitBoard friendlyPieces)
    {
        BitBoard allPieces = enemyPieces | friendlyPieces;
        
        var rookAttacks = RookMoveGenerator.GetRookAttacks(pieceCell, allPieces);
        var bishopAttacks = BishopMoveGenerator.GetBishopAttacks(pieceCell, allPieces);
        var attacks = rookAttacks | bishopAttacks;
        
        BitBoard validTargets = attacks & ~friendlyPieces;
        
        while (validTargets != 0)
        {
            var index = validTargets.GetLeastSignificantBitIndex();
            if (index == -1) break;
            validTargets.PopBit(index);
            yield return new StandardMove((ChessCell)pieceCell, (ChessCell)index);
        }
    }
}