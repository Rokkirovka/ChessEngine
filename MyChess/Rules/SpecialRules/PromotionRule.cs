using MyChess.Core;
using MyChess.Models;
using MyChess.Models.Moves;
using MyChess.Models.Pieces;
using MyChess.Rules.MoveGenerator;

namespace MyChess.Rules.SpecialRules;

public static class PromotionRule
{
    private const ulong SecondRankMask = 71776119061217280UL;
    private const ulong SeventhRankMask = 65280UL;
    public static IEnumerable<ChessMove> GetPromotionMoves(int pawnPos, ChessBoard board, BoardState boardState)
    {
        var piece = board.GetPiece(pawnPos);
        if (piece is null) yield break;
        var color = piece.Color;

        if (color == ChessColor.White)
        {
            if ((SeventhRankMask & (1UL << pawnPos)) == 0) yield break;
            var attacks = WhitePawnMoveGenerator.WhitePawnAttackMasks[pawnPos];
            var moves = WhitePawnMoveGenerator.WhitePawnMoveMasks[pawnPos];
            var validTargets = (BitBoard)((attacks & board.Occupancies[1]) | (moves & ~(board.Occupancies[0] | board.Occupancies[1])));
            for (var i = 0; i < 3; i++)
            {
                var index = validTargets.GetLeastSignificantBitIndex();
                if (index == -1) break;
                validTargets.PopBit(index);
                yield return new PromotionMove((ChessCell)pawnPos, (ChessCell)index, Queen.White);
                yield return new PromotionMove((ChessCell)pawnPos, (ChessCell)index, Rook.White);
                yield return new PromotionMove((ChessCell)pawnPos, (ChessCell)index, Bishop.White);
                yield return new PromotionMove((ChessCell)pawnPos, (ChessCell)index, Knight.White);
            }
        }
        else
        {
            if ((SecondRankMask & (1UL << pawnPos)) == 0) yield break;
            var attacks = BlackPawnMoveGenerator.BlackPawnAttackMasks[pawnPos];
            var moves = BlackPawnMoveGenerator.BlackPawnMoveMasks[pawnPos];
            var validTargets = (BitBoard)((attacks & board.Occupancies[0]) | (moves & ~(board.Occupancies[0] | board.Occupancies[1])));
            for (var i = 0; i < 3; i++)
            {
                var index = validTargets.GetLeastSignificantBitIndex();
                if (index == -1) break;
                validTargets.PopBit(index);
                yield return new PromotionMove((ChessCell)pawnPos, (ChessCell)index, Queen.Black);
                yield return new PromotionMove((ChessCell)pawnPos, (ChessCell)index, Rook.Black);
                yield return new PromotionMove((ChessCell)pawnPos, (ChessCell)index, Bishop.Black);
                yield return new PromotionMove((ChessCell)pawnPos, (ChessCell)index, Knight.Black);
            }
        }
    }
}