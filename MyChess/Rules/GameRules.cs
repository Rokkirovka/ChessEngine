using MyChess.Core;
using MyChess.Models;
using MyChess.Models.Moves;
using MyChess.Models.Pieces;
using MyChess.Services.MoveExecution;

namespace MyChess.Rules;

public static class GameRules
{
    private static readonly int[] DiagonalDirections = [-9, -7, 7, 9];
    private static readonly int[] VerticalAndHorizontalDirections = [-8, -1, 1, 8];
    private static readonly int[] KnightJumps = [-17, -15, -10, -6, 6, 10, 15, 17];
    private static readonly int[] KingMoves = [-9, -8, -7, -1, 1, 7, 8, 9];
    private static readonly MoveExecutor MoveExecutor = new(new MoveStrategyFactory());

    public static bool IsValidMove(ChessMove move, ChessBoard board, BoardState boardState)
    {
        var color = boardState.CurrentMoveColor;
        MoveExecutor.ForceMove(move, board, boardState);
        var valid = !IsKingInCheck(color, board);
        MoveExecutor.UndoMove(board, boardState);
        return valid;
    }

    public static bool IsKingInCheck(ChessColor color, ChessBoard board)
    {
        var kingPos = board.FindKing(color);
        return IsSquareUnderAttack(kingPos, color, board);
    }

    private static bool IsCellAttackedByDiagonal(ChessCell position, ChessBoard board, ChessColor cellColor)
    {
        foreach (var dir in DiagonalDirections)
        {
            for (var step = 1; step < 8; step++)
            {
                var previousPos = (int)position + dir * (step - 1);
                var targetPos = (int)position + dir * step;
                if (!IsWithinBounds(previousPos, targetPos)) break;
                var piece = board.GetPiece((ChessCell)targetPos);
                if (piece is null) continue;
                if (piece.Color != cellColor && piece is Bishop or Queen) return true;
                break;
            }
        }

        return false;
    }

    private static bool IsCellAttackedByVerticalOrHorizontal(ChessCell position, ChessBoard board, ChessColor cellColor)
    {
        foreach (var dir in VerticalAndHorizontalDirections)
        {
            for (var step = 1; step < 8; step++)
            {
                var previousPos = (int)position + dir * (step - 1);
                var targetPos = (int)position + dir * step;
                if (!IsWithinBounds(previousPos, targetPos)) break;
                var piece = board.GetPiece((ChessCell)targetPos);
                if (piece is null) continue;
                if (piece.Color != cellColor && piece is Rook or Queen) return true;
                break;
            }
        }

        return false;
    }

    private static bool IsCellAttackedByKnight(ChessCell position, ChessBoard board, ChessColor cellColor)
    {
        foreach (var jump in KnightJumps)
        {
            if ((int)position + jump < 0 || (int)position + jump >= 64 ||
                ((int)position + jump) % 8 - (int)position % 8 > 2) continue;
            var piece = board.GetPiece(position + jump);
            if (piece is null || piece.Color == cellColor) continue;
            if (piece is Knight) return true;
        }

        return false;
    }
    
    private static bool IsCellAttackedByPawn(ChessCell position, ChessBoard board, ChessColor cellColor)
    {
        if (cellColor == ChessColor.White)
        {
            if (IsWithinBounds((int)position,(int)position - 9) 
                && board.GetPiece(position - 9) is Pawn { Color: ChessColor.Black } || 
                IsWithinBounds((int)position,(int)position - 7) 
                && board.GetPiece(position - 7) is Pawn { Color: ChessColor.Black }) return true;
        }
        else
        {
            if (IsWithinBounds((int)position,(int)position + 9) 
                && board.GetPiece(position + 9) is Pawn { Color: ChessColor.White } || 
                IsWithinBounds((int)position,(int)position + 7) 
                && board.GetPiece(position + 7) is Pawn { Color: ChessColor.White }) return true;
        }

        return false;
    }
    
    private static bool IsCellAttackedByKing(ChessCell position, ChessBoard board, ChessColor cellColor)
    {
        foreach (var dir in KingMoves)
        {
            if (!IsWithinBounds((int)position,(int)position + dir)) continue;
            var piece = board.GetPiece(position + dir);
            if (piece is null || piece.Color == cellColor) continue;
            if (piece is King) return true;
        }

        return false;
    }
    

    public static bool IsSquareUnderAttack(ChessCell defenderPos, ChessColor defenderColor, ChessBoard board)
    {
        return IsCellAttackedByVerticalOrHorizontal(defenderPos, board, defenderColor)
               || IsCellAttackedByDiagonal(defenderPos, board, defenderColor)
               || IsCellAttackedByKnight(defenderPos, board, defenderColor)
               || IsCellAttackedByKing(defenderPos, board, defenderColor)
               || IsCellAttackedByPawn(defenderPos, board, defenderColor);
    }

    public static bool IsCheckmate(ChessColor color, ChessGame game)
    {
        if (!IsKingInCheck(color, game.GetClonedBoard())) return false;
        var moves = game.GetAllPossibleMoves();
        return !moves.Any();
    }

    public static bool IsWithinBounds(int previousPos, int targetPos)
    {
        if (targetPos < 0 || targetPos >= 64) return false;
        var currentFile = previousPos % 8;
        var targetFile = targetPos % 8;
        return Math.Abs(currentFile - targetFile) <= 1;
    }
}