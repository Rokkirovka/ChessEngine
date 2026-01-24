using MyChess.Core.Board;
using MyChess.Models;
using MyChess.Models.MoveHistoryItems;
using MyChess.Models.Moves;
using MyChess.Rules.SpecialRules;
using MyChess.Services.MoveExecution.Interfaces;

namespace MyChess.Services.MoveExecution.Strategies;

public class CastlingMoveStrategy : IMoveStrategy
{
    public bool CanExecute(ChessMove move, ChessBoard board, BoardState boardState)
    {
        return CastlingRule
            .GetCastlingMoves(move.From, board , boardState)
            .Contains(move);
    }

    public void Execute(ChessMove move, ChessBoard board, BoardState boardState)
    {
        var castlingMove = (CastlingMove)move;
        board.MovePiece(castlingMove.From, castlingMove.To);
        board.MovePiece(castlingMove.RookFrom, castlingMove.RookTo);
        boardState.EnPassantTarget = null;
        UpdateBoardState(castlingMove, board, boardState);
    }

    public void Undo(MoveHistoryItem historyItem, ChessBoard board, BoardState boardState)
    {
        var castlingMove = (CastlingMove)historyItem.Move;
        board.MovePiece(castlingMove.To, castlingMove.From);
        board.MovePiece(castlingMove.RookTo, castlingMove.RookFrom);
        boardState.RestoreFrom(historyItem.StateBeforeMove);
    }

    public MoveHistoryItem CreateHistoryItem(ChessMove move, BoardState stateBeforeMove, ChessBoard board)
    {
        return new CastlingMoveHistoryItem(move, stateBeforeMove);
    }

    private void UpdateBoardState(CastlingMove castlingMove, ChessBoard board, BoardState boardState)
    {
        var color = board.GetPiece(castlingMove.To)!.Color;
        if (color == ChessColor.White)
        {
            boardState.DisableCastling(CastlingRights.WhiteQueenSide);
            boardState.DisableCastling(CastlingRights.WhiteKingSide);
        }
        else
        {
            boardState.DisableCastling(CastlingRights.BlackQueenSide);
            boardState.DisableCastling(CastlingRights.BlackKingSide);
        }
    }
}