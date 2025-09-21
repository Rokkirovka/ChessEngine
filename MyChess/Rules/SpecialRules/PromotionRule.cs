using MyChess.Core;
using MyChess.Models;
using MyChess.Models.Moves;
using MyChess.Models.Pieces;

namespace MyChess.Rules.SpecialRules;

public static class PromotionRule
{
    public static IEnumerable<ChessMove> GetPromotionMoves(ChessCell pawnPos, ChessBoard board, BoardState boardState)
    {
        var pawn = board.GetPiece(pawnPos);
        if (pawn is not Pawn) yield break;

        var pawnRank = (int)pawnPos / 8;
        var isValidRank = (pawn.Color == ChessColor.White && pawnRank == 1) || 
                          (pawn.Color == ChessColor.Black && pawnRank == 6);
        if (!isValidRank) yield break;
        
        var direction = pawn.Color == ChessColor.White ? -8 : 8;
        var targetCell = pawnPos + direction;
        
        var promotionPieces = new List<IChessPiece>
        {
            pawn.Color == ChessColor.White ? Bishop.White : Bishop.Black,
            pawn.Color == ChessColor.White ? Queen.White : Queen.Black,
            pawn.Color == ChessColor.White ? Knight.White : Knight.Black,
            pawn.Color == ChessColor.White ? Rook.White : Rook.Black
        };

        var leftCapturePos = (int)targetCell - 1;
        var leftCapturePiece = board.GetPiece((ChessCell)leftCapturePos);
        if (leftCapturePos is >= 0 and < 64 && (int)pawnPos % 8 - leftCapturePos % 8 == 1
            && leftCapturePiece is not null && leftCapturePiece.Color == 1 - pawn.Color)
            foreach (var piece in promotionPieces)
            {
                var move = new PromotionMove(pawnPos, (ChessCell)leftCapturePos, piece);
                yield return move;
            }

        var rightCapturePos = (int)targetCell + 1;
        var rightCapturePiece = board.GetPiece((ChessCell)rightCapturePos);
        if (rightCapturePos is >= 0 and < 64 && rightCapturePos % 8 - (int)pawnPos % 8 == 1
            && rightCapturePiece is not null && rightCapturePiece.Color == 1 - pawn.Color)
            foreach (var piece in promotionPieces)
            {
                var move = new PromotionMove(pawnPos, (ChessCell)rightCapturePos, piece);
                yield return move;
            }
        
        if ((int)targetCell < 0 || (int)targetCell >= 64 || board.GetPiece(targetCell) is not null) yield break;
 
        foreach (var piece in promotionPieces)
        {
            var move = new PromotionMove(pawnPos, pawnPos + direction, piece);
            yield return move;
        }
    }
}