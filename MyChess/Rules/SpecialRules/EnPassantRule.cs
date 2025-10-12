using MyChess.Core;
using MyChess.Models;
using MyChess.Models.Moves;
using MyChess.Rules.MoveGenerator;

namespace MyChess.Rules.SpecialRules;

public static class EnPassantRule
{
    public static IEnumerable<ChessMove> GetEnPassantMoves(int pawnPos, ChessBoard board, BoardState boardState)
    {
        if (boardState.EnPassantTarget == null) yield break;

        var enPassantSquare = boardState.EnPassantTarget.Value;
        var piece = board.GetPiece(pawnPos);
        if (piece == null) yield break;
        var color = piece.Color;

        var pawnAttacks = color == ChessColor.White
            ? WhitePawnMoveGenerator.WhitePawnAttackMasks[pawnPos]
            : BlackPawnMoveGenerator.BlackPawnAttackMasks[pawnPos];

        var dir = color == ChessColor.White ? -8 : 8;
        var targetSquare = enPassantSquare + dir;

        if ((pawnAttacks & (1UL << targetSquare)) != 0)
        {
            yield return new EnPassantMove((ChessCell)pawnPos, (ChessCell)targetSquare);
        }
    }
}