using MyChess.Core;
using MyChess.Models;
using MyChess.Models.Moves;
using MyChess.Models.Pieces;

namespace MyChess.Rules.MoveGenerator;

public class KnightMoveGenerator : IMoveGenerator
{
    private readonly int[] _jumps = [-17, -15, -10, -6, 6, 10, 15, 17];
    
    public static readonly KnightMoveGenerator Instance = new();
    
    private KnightMoveGenerator() { }
    public IEnumerable<ChessMove> GetPossibleMoves(ChessCell cell, ChessBoard board, BoardState boardState)
    {
        var piece = board.GetPiece(cell);
        if (piece is not Knight) yield break;

        foreach (var jump in _jumps)
        {
            var targetPos = (int)cell + jump;
            if (IsWithinBounds((int)cell, targetPos, board, piece.Color))
            {
                var move = new StandardMove(cell, (ChessCell)targetPos);
                yield return move;
            }
        }
    }

    private bool IsWithinBounds(int currentPos, int targetPos, ChessBoard board, ChessColor color)
    {
        if (targetPos < 0 || targetPos >= 64) return false;

        var currentFile = currentPos % 8;
        var targetFile = targetPos % 8;
        if (Math.Abs(currentFile - targetFile) > 2) return false;
        if (Math.Abs((currentPos / 8) - (targetPos / 8)) > 2) return false;

        var targetPiece = board.GetPiece((ChessCell)targetPos);
        return targetPiece == null || targetPiece.Color != color;
    }
}