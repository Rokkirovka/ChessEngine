using MyChess.Core.Board;
using MyChess.Models;
using MyChess.Models.MoveHistoryItems;
using MyChess.Models.Moves;
using MyChess.Models.Pieces;
using MyChess.Rules.SpecialRules;
using MyChess.Services.MoveExecution.Interfaces;

namespace MyChess.Services.MoveExecution.Strategies;

public class PromotionMoveStrategy : IMoveStrategy
{
    public bool CanExecute(ChessMove move, ChessBoard board, BoardState boardState)
    {
        return PromotionRule
            .GetPromotionMoves(move.From, board, boardState)
            .Contains(move);
    }

    public void Execute(ChessMove move, ChessBoard board, BoardState boardState)
    {
        var promotionMove = (PromotionMove)move;
        board.MovePiece(promotionMove.From, promotionMove.To);
        board.SetPiece(promotionMove.To, promotionMove.PromotionPiece);
        boardState.EnPassantTarget = null;
    }

    public void Undo(MoveHistoryItem historyItem, ChessBoard board, BoardState boardState)
    {
        var promotionMove = (PromotionMove)historyItem.Move;
        var promotionMoveHistoryItem = (PromotionMoveHistoryItem)historyItem;
        board.MovePiece(promotionMove.To, promotionMove.From);
        var pawn = boardState.CurrentMoveColor == ChessColor.Black 
            ? Pawn.White : Pawn.Black;
        board.SetPiece(promotionMove.From, pawn);
        board.SetPiece(promotionMove.To, promotionMoveHistoryItem.CapturedPiece);
        boardState.RestoreFrom(historyItem.StateBeforeMove);
    }

    public MoveHistoryItem CreateHistoryItem(ChessMove move, BoardState stateBeforeMove, ChessBoard board)
    {
        return new PromotionMoveHistoryItem(move, stateBeforeMove, board.GetPiece(move.To));
    }
}