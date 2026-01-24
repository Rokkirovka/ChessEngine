using MyChess.Core.Board;
using MyChess.Models;
using MyChess.Models.Moves;
using MyChess.Models.Pieces;
using MyChess.Rules.MoveGenerator;
using MyChess.Rules.SpecialRules;
using MyChess.Services.MoveExecution;

namespace MyChess.Rules;

public static class GameRules
{
    private static readonly MoveExecutor MoveExecutor = new(new MoveStrategyFactory());

    public static bool IsValidMove(ChessMove move, ChessBoard board, BoardState boardState)
    {
        var color = boardState.CurrentMoveColor;
        MoveExecutor.ExecuteMove(move, board, boardState);
        var valid = !IsKingInCheck(color, board);
        MoveExecutor.UndoMove(board, boardState);
        return valid;
    }

    private static bool IsKingInCheck(ChessColor kingColor, ChessBoard board)
    {
        var kingPos = kingColor == ChessColor.White
            ? board.WhiteKing.GetLeastSignificantBitIndex()
            : board.BlackKing.GetLeastSignificantBitIndex();

        return IsSquareUnderAttack(kingPos, kingColor, board);
    }

    private static bool IsCellAttackedByDiagonal(int position, ChessBoard board, ChessColor cellColor)
    {
        var bitBoard = cellColor == ChessColor.Black
            ? board.WhiteBishops | board.WhiteQueens
            : board.BlackBishops | board.BlackQueens;

        return ((ulong)(BishopMoveGenerator.GetBishopAttacks(position,
            (ulong)(board.Occupancies[0] | board.Occupancies[1])) & bitBoard)) != 0;
    }

    private static bool IsCellAttackedByVerticalOrHorizontal(int position, ChessBoard board, ChessColor cellColor)
    {
        var bitBoard = cellColor == ChessColor.Black
            ? board.WhiteRooks | board.WhiteQueens
            : board.BlackRooks | board.BlackQueens;
        return ((ulong)(RookMoveGenerator.GetRookAttacks(position,
            (ulong)(board.Occupancies[0] | board.Occupancies[1])) & bitBoard)) != 0;
    }

    private static bool IsCellAttackedByKnight(int position, ChessBoard board, ChessColor cellColor)
    {
        var bitBoard = cellColor == ChessColor.Black ? board.WhiteKnights : board.BlackKnights;
        return ((ulong)(KnightMoveGenerator.KnightAttackMasks[position] & bitBoard)) != 0;
    }

    private static bool IsCellAttackedByPawn(int position, ChessBoard board, ChessColor cellColor)
    {
        var bitBoard = cellColor == ChessColor.Black ? board.WhitePawns : board.BlackPawns;
        if (cellColor == ChessColor.White)
            return ((ulong)(WhitePawnMoveGenerator.WhitePawnAttackMasks[position] & bitBoard)) != 0;
        return ((ulong)(BlackPawnMoveGenerator.BlackPawnAttackMasks[position] & bitBoard)) != 0;
    }

    private static bool IsCellAttackedByKing(int position, ChessBoard board, ChessColor cellColor)
    {
        var bitBoard = cellColor == ChessColor.Black ? board.WhiteKing : board.BlackKing;
        return ((ulong)(KingMoveGenerator.KingAttackMasks[position] & bitBoard)) != 0;
    }


    public static bool IsSquareUnderAttack(int square, ChessColor defenderColor, ChessBoard board)
    {
        return IsCellAttackedByKing(square, board, defenderColor)
               || IsCellAttackedByKnight(square, board, defenderColor)
               || IsCellAttackedByPawn(square, board, defenderColor)
               || IsCellAttackedByVerticalOrHorizontal(square, board, defenderColor)
               || IsCellAttackedByDiagonal(square, board, defenderColor);
    }

    public static bool IsCheckmate(ChessColor color, ChessBoard board, BoardState boardState)
    {
        if (!IsKingInCheck(color, board)) return false;
        return !HasAnyValidMove(color, board, boardState);
    }

    public static bool IsStalemate(ChessColor color, ChessBoard board, BoardState boardState)
    {
        if (IsKingInCheck(color, board)) return false;
        return !HasAnyValidMove(color, board, boardState);
    }

    private static bool HasAnyValidMove(ChessColor color, ChessBoard board, BoardState boardState)
    {
        var originalColor = boardState.CurrentMoveColor;
        boardState.CurrentMoveColor = color;

        try
        {
            for (var i = 0; i < 64; i++)
            {
                var piece = board.GetPiece(i);
                if (piece is null || piece.Color != color) continue;

                var friendlyPieces = color == ChessColor.White ? board.Occupancies[0] : board.Occupancies[1];
                var enemyPieces = color == ChessColor.White ? board.Occupancies[1] : board.Occupancies[0];

                var potentialMoves = piece
                    .GetMoveGenerator()
                    .GetPossibleMoves(i, enemyPieces, friendlyPieces);

                if (potentialMoves.Any(move => IsValidMove(move, board, boardState)))
                    return true;

                if (piece is King)
                {
                    if (CastlingRule.GetCastlingMoves(i, board, boardState)
                        .Any(move => IsValidMove(move, board, boardState)))
                        return true;
                }

                if (piece is Pawn)
                {
                    if (EnPassantRule.GetEnPassantMoves(i, board, boardState)
                        .Any(move => IsValidMove(move, board, boardState)))
                        return true;

                    if (PromotionRule.GetPromotionMoves(i, board, boardState)
                        .Any(move => IsValidMove(move, board, boardState)))
                        return true;
                }
            }

            return false;
        }
        finally
        {
            boardState.CurrentMoveColor = originalColor;
        }
    }
}