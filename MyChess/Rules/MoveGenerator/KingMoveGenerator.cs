using MyChess.Core;
using MyChess.Models;
using MyChess.Models.Moves;
using MyChess.Models.Pieces;
using MyChess.Rules.SpecialRules;

namespace MyChess.Rules.MoveGenerator;

public class KingMoveGenerator : IMoveGenerator
{
    private readonly int[] _directions = [-9, -8, -7, -1, 1, 7, 8, 9];
    
    public static readonly KingMoveGenerator Instance = new();
    
    private KingMoveGenerator() { }

    public IEnumerable<ChessMove> GetPossibleMoves(ChessCell cell, ChessBoard board, BoardState boardState)
    {
        var piece = board.GetPiece(cell);
        if (piece is not King) yield break;

        foreach (var dir in _directions)
        {
            var targetPos = (int)cell + dir;
            if (IsWithinBounds((int)cell, targetPos, dir, board, piece.Color))
            {
                var move = new StandardMove(cell, (ChessCell)targetPos);
               yield return move;
            }
        }
        
        foreach (var move in CastlingRule.GetCastlingMoves(cell, board, boardState)) yield return move;
    }

    private bool IsWithinBounds(int currentPos, int targetPos, int dir, ChessBoard board, ChessColor color)
    {
        if (targetPos < 0 || targetPos >= 64) return false;

        var currentFile = currentPos % 8;
        var targetFile = targetPos % 8;
        if (Math.Abs(dir) == 1 && Math.Abs(currentFile - targetFile) > 1)
            return false;
        if (Math.Abs(dir) == 9 && Math.Abs(currentFile - targetFile) > 1)
            return false;
        if (Math.Abs(dir) == 7 && Math.Abs(currentFile - targetFile) > 1)
            return false;

        var targetPiece = board.GetPiece((ChessCell)targetPos);
        return targetPiece == null || targetPiece.Color != color;
    }
}