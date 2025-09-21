using MyChess.Core;
using MyChess.Models;
using MyChess.Models.Moves;
using MyChess.Models.Pieces;

namespace MyChess.Rules.MoveGenerator;

public abstract class LinearMoveGenerator(int[] directions) : IMoveGenerator
{
    public IEnumerable<ChessMove> GetPossibleMoves(ChessCell cell, ChessBoard board, BoardState boardState)
    {
        var piece = board.GetPiece(cell);
        if (piece is not Rook && piece is not Queen && piece is not Bishop) yield break;
        var color = piece.Color;

        foreach (var dir in directions)
        {
            for (var step = 1; step < 8; step++)
            {
                var previousPos = (int)cell + dir * (step - 1);
                var targetPos = (int)cell + dir * step;
                if (!IsWithinBounds(color, previousPos, targetPos, board, out var stop)) break;
                var move = new StandardMove(cell, (ChessCell)targetPos);
                yield return move;
                if (stop) break;
            }
        }
    }

    private bool IsWithinBounds(ChessColor color, int previousPos, int targetPos, ChessBoard board, out bool stop)
    {
        stop = false;
        if (targetPos < 0 || targetPos >= 64) return false;

        var currentFile = previousPos % 8;
        var targetFile = targetPos % 8;
        if (Math.Abs(currentFile - targetFile) > 1) return false;

        var targetPiece = board.GetPiece((ChessCell)targetPos);
        if (targetPiece == null) return true;

        stop = true;
        return targetPiece.Color != color;
    }
}