using MyChess.Core.Board;
using MyChess.Models;
using MyChess.Models.MoveHistoryItems;
using MyChess.Models.Moves;
using MyChess.Models.Pieces;
using MyChess.Services.MoveExecution.Interfaces;
using static MyChess.Models.ChessCell;

namespace MyChess.Services.MoveExecution.Strategies;

public class StandardMoveStrategy : IMoveStrategy
{
    public void Execute(ChessMove move, ChessBoard board, BoardState boardState)
    {
        board.MovePiece(move.From, move.To);
        boardState.EnPassantTarget = null;
        UpdateBoardState(move, board, boardState);
    }

    public void Undo(MoveHistoryItem historyItem, ChessBoard board, BoardState boardState)
    {
        var standardMove = (StandardMove)historyItem.Move;
        var standardMoveHistoryItem = (StandardMoveHistoryItem)historyItem;
        board.MovePiece(standardMove.To, standardMove.From);
        board.SetPiece(standardMove.To, standardMoveHistoryItem.CapturedPiece);
        boardState.RestoreFrom(historyItem.StateBeforeMove);
    }

    public MoveHistoryItem CreateHistoryItem(ChessMove move, BoardState stateBeforeMove, ChessBoard board)
    {
        if (board.GetPiece(move.To) is King) throw new ArgumentException("King cannot be captured.");
        return new StandardMoveHistoryItem(move, stateBeforeMove, board.GetPiece(move.To));
    }

    private void UpdateBoardState(ChessMove move, ChessBoard board, BoardState boardState)
    {
        var piece = board.GetPiece(move.To);
        switch (piece)
        {
            case Pawn:
                if (Math.Abs(move.From / 8 - move.To / 8) == 2)
                    boardState.EnPassantTarget = move.To;
                break;
            case King king:
                if (king.Color == ChessColor.White)
                {
                    boardState.DisableCastling(CastlingRights.WhiteQueenSide);
                    boardState.DisableCastling(CastlingRights.WhiteKingSide);
                }
                else
                {
                    boardState.DisableCastling(CastlingRights.BlackQueenSide);
                    boardState.DisableCastling(CastlingRights.BlackKingSide);
                }
                break;
            case Rook rook:
                if (rook.Color == ChessColor.White)
                {
                    if (move.From == (int)A1)
                        boardState.DisableCastling(CastlingRights.WhiteQueenSide);
                    if (move.From == (int)H1)
                        boardState.DisableCastling(CastlingRights.WhiteKingSide);
                }
                else
                {
                    if (move.From == (int)A8)
                        boardState.DisableCastling(CastlingRights.BlackQueenSide);
                    if (move.From == (int)H8)
                        boardState.DisableCastling(CastlingRights.BlackKingSide);
                }
                break;
        }
    }
}