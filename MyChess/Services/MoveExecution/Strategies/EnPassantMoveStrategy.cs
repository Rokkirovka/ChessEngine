using MyChess.Core;
using MyChess.Models;
using MyChess.Models.MoveHistoryItems;
using MyChess.Models.Moves;
using MyChess.Models.Pieces;
using MyChess.Rules.SpecialRules;
using MyChess.Services.MoveExecution.Interfaces;

namespace MyChess.Services.MoveExecution.Strategies;

public class EnPassantMoveStrategy : IMoveStrategy
{
    public bool CanExecute(ChessMove move, ChessBoard board, BoardState boardState)
    {
        return EnPassantRule
            .GetEnPassantMoves(move.From, board, boardState)
            .Contains(move);
    }

    public void Execute(ChessMove move, ChessBoard board, BoardState boardState)
    {
        board.MovePiece(move.From, move.To);
        board.RemovePiece(boardState.EnPassantTarget!.Value);
        boardState.EnPassantTarget = null;
    }

    public void Undo(MoveHistoryItem historyItem, ChessBoard board, BoardState boardState)
    {
        var enPassantMove = (EnPassantMove)historyItem.Move;
        board.MovePiece(enPassantMove.To, enPassantMove.From);
        var pawn = boardState.CurrentMoveColor == ChessColor.White 
            ? Pawn.White : Pawn.Black;
        board.SetPiece(historyItem.StateBeforeMove.EnPassantTarget!.Value, pawn);
        boardState.RestoreFrom(historyItem.StateBeforeMove);
    }

    public MoveHistoryItem CreateHistoryItem(ChessMove move, BoardState stateBeforeMove, ChessBoard board)
    {
        return new EnPassantMoveHistoryItem(move, stateBeforeMove);
    }

    public IEnumerable<int> GetCellsWillChange(ChessMove move, ChessBoard board, BoardState boardState)
    {
        yield return move.From;
        yield return move.To;
        yield return boardState.EnPassantTarget!.Value;
    }
}