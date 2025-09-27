using MyChess.Models;
using MyChess.Core;
using MyChess.Models.Moves;
using MyChessEngine;

namespace MyChessVisual;

public class ChessInputHandler
{
    private readonly ChessGame _chessGame;
    private readonly Engine _engine;
    private readonly ChessBoardRenderer _chessBoardRenderer;
    private int? _selectedCell;
    private int? _lastMoveFrom;
    private int? _lastMoveTo;
    private List<ChessMove> _possibleMoves = [];
    private readonly bool _autoPlayMode;
    private const int AutoPlayDelay = 100;

    public ChessInputHandler(ChessGame game, ChessBoardRenderer renderer, bool autoPlayMode = true)
    {
        _chessGame = game;
        _engine = new Engine(game);
        _chessBoardRenderer = renderer;
        _autoPlayMode = autoPlayMode;
        
        if (_autoPlayMode)
        {
            StartAutoPlay();
        }
    }

    public void HandleSquareClick(object? sender, EventArgs e)
    {
        if (_autoPlayMode) return;

        if (sender is not PictureBox { Tag: int clickedCell } pictureBox) return;
        
        if (_chessGame.IsOver) return;

        if (_selectedCell == clickedCell)
        {
            ClearSelection();
            return;
        }

        var piece = _chessGame.GetPiece(clickedCell);
        if (_selectedCell is null && piece is null) return;
        if (_selectedCell is null && piece != null && piece.Color != _chessGame.CurrentPlayer) return;

        if (_selectedCell is null && piece != null && piece.Color == _chessGame.CurrentPlayer)
        {
            SelectPiece(clickedCell, pictureBox);
            return;
        }

        if (piece != null && piece.Color == _chessGame.CurrentPlayer)
        {
            ChangeSelection(clickedCell, pictureBox);
            return;
        }

        _ = MakeMove((int)pictureBox.Tag);
    }
    
    private async void StartAutoPlay()
    {
        while (!_chessGame.IsOver && _autoPlayMode)
        {
            await MakeEngineMove();
            await Task.Delay(AutoPlayDelay);

            if (_chessGame.IsOver) continue;
            await MakeEngineMove();
            await Task.Delay(AutoPlayDelay);
        }
    }
    
    private async Task MakeEngineMove()
    {
        await Task.Run(() =>
        {
            var engineResult = _engine.EvaluatePosition();
            if (engineResult.BestMove is null) return;
            var movesWillChange = _chessGame.GetCellsWillChange(engineResult.BestMove).ToArray();
            _engine.RemoveCellsScore(movesWillChange);
            _chessGame.MakeMove(engineResult.BestMove);
            _engine.UpdateScore(movesWillChange);
            _lastMoveFrom = engineResult.BestMove.From;
            _lastMoveTo = engineResult.BestMove.To;
            UpdateBoardAfterMove();
        });
    }

    private void SelectPiece(int cell, PictureBox pictureBox)
    {
        _selectedCell = cell;
        pictureBox.BackColor = Color.LightBlue;
        _possibleMoves = _chessGame.GetValidMoves(cell).ToList();
        HighlightPossibleMoves();
    }

    private void ChangeSelection(int newCell, PictureBox pictureBox)
    {
        ClearSelection();
        SelectPiece(newCell, pictureBox);
    }

    private async Task MakeMove(int targetCell)
    {
        var move = _possibleMoves.FirstOrDefault(move => move.From == _selectedCell && move.To == targetCell);
        if (move is not null)
        {
            if (move is PromotionMove)
            {
                var promotionForm = new PromotionForm(_chessGame.CurrentPlayer);
                if (promotionForm.ShowDialog() == DialogResult.OK)
                {
                    move = new PromotionMove((ChessCell)move.From, (ChessCell)move.To, promotionForm.SelectedPieceType);
                }
                else
                {
                    ClearSelection();
                    return;
                }
            }

            var movesWillChange = _chessGame.GetCellsWillChange(move).ToArray();
            _engine.RemoveCellsScore(movesWillChange);
            _chessGame.MakeMove(move);
            _engine.UpdateScore(movesWillChange);
            _lastMoveFrom = _selectedCell!.Value;
            _lastMoveTo = targetCell;
            UpdateBoardAfterMove();

            await Task.Run(() =>
            {
                var engineResult = _engine.EvaluatePosition();
                if (engineResult.BestMove is null) throw new Exception();
                _chessGame.MakeMove(engineResult.BestMove);
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
            var row = move.To / 8;
            var col = move.To % 8;
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
            var fromRow = _lastMoveFrom.Value / 8;
            var fromCol = _lastMoveFrom.Value % 8;
            _chessBoardRenderer.HighlightSquare(fromRow, fromCol, Color.LightYellow);
        }

        if (_lastMoveTo.HasValue)
        {
            var toRow = _lastMoveTo.Value / 8;
            var toCol = _lastMoveTo.Value % 8;
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