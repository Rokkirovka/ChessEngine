using MyChess.Core;
using MyChess.Models;
using MyChess.Models.Moves;
using MyChess.Models.Pieces;
using MyChess.Rules.SpecialRules;

namespace MyChess.Rules.MoveGenerator;

public class PawnMoveGenerator : IMoveGenerator
{
    public static readonly PawnMoveGenerator Instance = new();
    
    private PawnMoveGenerator() { }
    public IEnumerable<ChessMove> GetPossibleMoves(ChessCell cell, ChessBoard board, BoardState boardState)
    {
        var piece = board.GetPiece(cell);
        if (piece is not Pawn) yield break;

        var direction = piece.Color == ChessColor.White ? -8 : 8;
        var startRank = piece.Color == ChessColor.White ? 6 : 1;
        var currentRank = (int)cell / 8;

        var forwardPos = (int)cell + direction;
        if (IsWithinBounds((int)cell,forwardPos) && board.GetPiece((ChessCell)forwardPos) == null)
        {
            var move = new StandardMove(cell, (ChessCell)forwardPos);
            yield return move;
            
            var doubleForwardPos = (int)cell + 2 * direction;
            if (currentRank == startRank && board.GetPiece((ChessCell)doubleForwardPos) == null)
            {
                var doubleMove = new StandardMove(cell, (ChessCell)doubleForwardPos);
                yield return doubleMove;
            }
        }

        int[] captureDirections = [direction + 1, direction - 1];
        foreach (var captureDir in captureDirections)
        {
            var capturePos = (int)cell + captureDir;
            if (IsWithinBounds((int)cell, capturePos))
            {
                var targetPiece = board.GetPiece((ChessCell)capturePos);
                if (targetPiece != null && targetPiece.Color != piece.Color)
                {
                    var move = new StandardMove(cell, (ChessCell)capturePos);
                    yield return move;
                }
            }
        }

        foreach (var move in EnPassantRule.GetEnPassantMoves(cell, board, boardState)) yield return move;
        foreach (var move in PromotionRule.GetPromotionMoves(cell, board, boardState)) yield return move;
    }

    private bool IsWithinBounds(int currentPos, int targetPos)
    {
        if (currentPos is < 0 or >= 64) return false;
        var currentFile = currentPos % 8;
        var targetFile = targetPos % 8;
        var targetRank = targetPos / 8;
        if (targetRank is 0 or 7) return false;
        return Math.Abs(currentFile - targetFile) <= 1;
    }
}