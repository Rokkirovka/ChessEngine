using MyChess.Core;
using MyChess.Models;
using MyChess.Models.MoveHistoryItems;
using MyChess.Models.Moves;
using MyChess.Models.Pieces;
using MyChess.Services.MoveExecution.Interfaces;

namespace MyChess.Services.MoveExecution.Strategies;

public class StandardMoveStrategy : IMoveStrategy
{
    public bool CanExecute(ChessMove move, ChessBoard board, BoardState boardState)
    {
        var piece = board.GetPiece(move.From);
        return piece?.GetMoveGenerator()
            .GetPossibleMoves(move.From, board, boardState)
            .Contains(move) ?? false;
    }

    public void Execute(ChessMove castlingMove, ChessBoard board, BoardState boardState)
    {
        board.MovePiece(castlingMove.From, castlingMove.To);
        UpdateBoardState(castlingMove, board, boardState);
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
        return new StandardMoveHistoryItem(move, stateBeforeMove, board.GetPiece(move.To));
    }

    public IEnumerable<ChessCell> GetCellsWillChange(ChessMove move, ChessBoard board, BoardState boardState)
    {
        yield return move.From;
        yield return move.To;
    }

    private void UpdateBoardState(ChessMove move, ChessBoard board, BoardState boardState)
    {
        var piece = board.GetPiece(move.To);
        switch (piece)
        {
            case Pawn:
                boardState.EnPassantTarget = move.To;
                break;
            case King king:
                if (king.Color == ChessColor.White)
                {
                    boardState.WhiteKingSideCastling = false;
                    boardState.WhiteQueenSideCastling = false;
                }
                else
                {
                    boardState.BlackKingSideCastling = false;
                    boardState.BlackQueenSideCastling = false;
                }
                break;
            case Rook:
                boardState.BlackQueenSideCastling &= move.From != ChessCell.A8;
                boardState.BlackKingSideCastling &= move.From != ChessCell.H8;
                boardState.WhiteQueenSideCastling &= move.From != ChessCell.A1;
                boardState.WhiteKingSideCastling &= move.From != ChessCell.H1;
                break;
        }
    }
}