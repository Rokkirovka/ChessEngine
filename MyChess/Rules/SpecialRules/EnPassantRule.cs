using MyChess.Core;
using MyChess.Models;
using MyChess.Models.Moves;
using MyChess.Models.Pieces;

namespace MyChess.Rules.SpecialRules;

public static class EnPassantRule
{
    public static IEnumerable<ChessMove> GetEnPassantMoves(ChessCell pawnPos, ChessBoard board, BoardState boardState)
    {
        var pawn = board.GetPiece(pawnPos);
        if (pawn is not Pawn || !boardState.EnPassantTarget.HasValue) yield break;

        var targetPos = (int)boardState.EnPassantTarget.Value;
        var pawnFile = (int)pawnPos % 8;
        var targetFile = targetPos % 8;
        if (Math.Abs(pawnFile - targetFile) != 1) yield break;
        
        var pawnRank = (int)pawnPos / 8;
        var isValidRank = (pawn.Color == ChessColor.White && pawnRank == 3) || 
                          (pawn.Color == ChessColor.Black && pawnRank == 4);
        
        var newPos = boardState.EnPassantTarget.Value + (pawn.Color is ChessColor.White ? -8 : 8);
        var move = new EnPassantMove(pawnPos, newPos);
        if (isValidRank) yield return move;
    }
}