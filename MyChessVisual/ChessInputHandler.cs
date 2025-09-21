using MyChess.Models;
using MyChess.Core;
using MyChess.Models.Moves;
using MyChessEngine;

namespace MyChessVisual;

public class ChessInputHandler
{
    private ChessGame _chessGame;
    private Engine _engine;
    private ChessBoardRenderer _chessBoardRenderer;
    private ChessCell? _selectedCell;
    private ChessCell? _lastMoveFrom;
    private ChessCell? _lastMoveTo;
    private ChessMove[] _possibleMoves = [];

    public ChessInputHandler(ChessGame game, ChessBoardRenderer renderer)
    {
        _chessGame = game;
        _engine = new Engine(game);
        _chessBoardRenderer = renderer;
    }

    public void HandleSquareClick(object? sender, EventArgs e)
    {
        if (_chessGame.CurrentPlayer is ChessColor.Black
            || _chessGame.IsOver) return;

        if (sender is not PictureBox { Tag: ChessCell clickedCell } pictureBox) return;

        if (_selectedCell == clickedCell)
        {
            ClearSelection();
            return;
        }

        var piece = _chessGame.GetPiece(clickedCell);

        if (_selectedCell is null && piece is null) return;

        if (_selectedCell is null && piece != null && piece.Color == _chessGame.CurrentPlayer)
        {
            SelectPiece(clickedCell, pictureBox);
            return;
        }

        if (_selectedCell is null && piece != null && piece.Color != _chessGame.CurrentPlayer)
        {
            return;
        }

        if (piece != null && piece.Color == _chessGame.CurrentPlayer)
        {
            ChangeSelection(clickedCell, pictureBox);
            return;
        }

        _ = TryMakeMove((ChessCell)pictureBox.Tag);
    }

    private void SelectPiece(ChessCell cell, PictureBox pictureBox)
    {
        _selectedCell = cell;
        pictureBox.BackColor = Color.LightBlue;
        _possibleMoves = _chessGame.GetValidMoves(cell).ToArray();
        HighlightPossibleMoves();
    }

    private void ChangeSelection(ChessCell newCell, PictureBox pictureBox)
    {
        ClearSelection();
        SelectPiece(newCell, pictureBox);
    }

    private async Task TryMakeMove(ChessCell targetCell)
    {
        var move = _possibleMoves.FirstOrDefault(move => move.From == _selectedCell && move.To == targetCell);
        if (move is not null)
        {
            if (move is PromotionMove)
            {
                var promotionForm = new PromotionForm(_chessGame.CurrentPlayer);
                if (promotionForm.ShowDialog() == DialogResult.OK)
                {
                    move = new PromotionMove(move.From, move.To, promotionForm.SelectedPieceType);
                }
                else
                {
                    ClearSelection();
                    return;
                }
            }

            var movesWillChange = _chessGame.GetCellsWillChange(move).ToArray();
            _engine.RemoveCellsScore(movesWillChange);
            _chessGame.ForceMove(move);
            _engine.UpdateScore(movesWillChange);
            _lastMoveFrom = _selectedCell!.Value;
            _lastMoveTo = targetCell;
            UpdateBoardAfterMove();

            await Task.Run(() =>
            {
                var engineResult = _engine.EvaluatePosition();
                if (engineResult.BestMove is null) throw new Exception();
                _chessGame.ForceMove(engineResult.BestMove);
                _lastMoveFrom = engineResult.BestMove.From;
                _lastMoveTo = engineResult.BestMove.To;
            });

            UpdateBoardAfterMove();
        }

        ClearSelection();
    }

    private void HighlightPossibleMoves()
    {
        foreach (var move in _possibleMoves)
        {
            var row = (int)move.To / 8;
            var col = (int)move.To % 8;
            _chessBoardRenderer.HighlightSquare(row, col, Color.LightGreen);
        }
    }

    private void UpdateBoardAfterMove()
    {
        _chessBoardRenderer.UpdateAllSquares();
        HighlightLastMove();
    }

    private void HighlightLastMove()
    {
        if (_lastMoveFrom.HasValue)
        {
            var fromRow = (int)_lastMoveFrom.Value / 8;
            var fromCol = (int)_lastMoveFrom.Value % 8;
            _chessBoardRenderer.HighlightSquare(fromRow, fromCol, Color.LightYellow);
        }

        if (_lastMoveTo.HasValue)
        {
            var toRow = (int)_lastMoveTo.Value / 8;
            var toCol = (int)_lastMoveTo.Value % 8;
            _chessBoardRenderer.HighlightSquare(toRow, toCol, Color.LightYellow);
        }
    }

    private void ClearSelection()
    {
        _selectedCell = null;
        _possibleMoves = [];
        _chessBoardRenderer.UpdateAllSquares();
        HighlightLastMove();
    }
}