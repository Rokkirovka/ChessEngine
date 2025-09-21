using MyChess.Core;
using MyChess.Models;
using MyChess.Models.Moves;
using MyChess.Models.Pieces;

namespace MyChess.Rules.SpecialRules;

public static class CastlingRule
{
    public static IEnumerable<ChessMove> GetCastlingMoves(ChessCell kingPos, ChessBoard board, BoardState boardState)
    {
        var king = board.GetPiece(kingPos);
        if (king is not King) yield break;

        if (GameRules.IsKingInCheck(king.Color, board)) yield break;

        var canKingSide = king.Color == ChessColor.White ? 
            boardState.WhiteKingSideCastling : boardState.BlackKingSideCastling;
        var canQueenSide = king.Color == ChessColor.White ? 
            boardState.WhiteQueenSideCastling : boardState.BlackQueenSideCastling;

        if (canKingSide && CheckKingSide(kingPos, board, king.Color))
        {
            var move = new CastlingMove(kingPos, (ChessCell)((int)kingPos + 2));
            yield return move;
        }

        if (canQueenSide && CheckQueenSide(kingPos, board, king.Color))
        {
            var move = new CastlingMove(kingPos, (ChessCell)((int)kingPos - 2));
            yield return move;
        }
    }

    private static bool CheckKingSide(ChessCell kingPos, ChessBoard board, ChessColor color)
    {
        var rookPos = (int)kingPos + 3;
        var kingTo = (int)kingPos + 2;
        var intermediate = (int)kingPos + 1;

        return board.GetPiece((ChessCell)rookPos) is Rook rook && 
               rook.Color == color &&
               board.GetPiece((ChessCell)intermediate) == null &&
               board.GetPiece((ChessCell)kingTo) == null &&
               !GameRules.IsSquareUnderAttack((ChessCell)intermediate, color, board) &&
               !GameRules.IsSquareUnderAttack((ChessCell)kingTo, color, board);
    }

    private static bool CheckQueenSide(ChessCell kingPos, ChessBoard board, ChessColor color)
    {
        var rookPos = (int)kingPos - 4;
        var kingTo = (int)kingPos - 2;
        var intermediate = (int)kingPos - 1;

        return board.GetPiece((ChessCell)rookPos) is Rook rook && 
               rook.Color == color &&
               board.GetPiece((ChessCell)((int)kingPos - 1)) == null &&
               board.GetPiece((ChessCell)((int)kingPos - 2)) == null &&
               board.GetPiece((ChessCell)((int)kingPos - 3)) == null &&
               !GameRules.IsSquareUnderAttack((ChessCell)intermediate, color, board) &&
               !GameRules.IsSquareUnderAttack((ChessCell)kingTo, color, board);
    }
}