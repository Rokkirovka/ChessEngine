using MyChess.Models;
using MyChess.Core;
using MyChess.Models.Moves;
using MyChessEngine.Core;

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
    private readonly ChessColor _humanPlayer;
    private const int AutoPlayDelay = 100;

    public ChessInputHandler(ChessGame game, ChessBoardRenderer renderer, bool autoPlayMode = true, ChessColor humanPlayer = ChessColor.Black)
    {
        _chessGame = game;
        _engine = new Engine(game);
        _chessBoardRenderer = renderer;
        _autoPlayMode = autoPlayMode;
        _humanPlayer = humanPlayer;
        
        if (_autoPlayMode)
        {
            StartAutoPlay();
        }
        else if (!IsHumanTurn())
        {
            MakeEngineMoveAsync();
        }
    }

    public void HandleSquareClick(object? sender, EventArgs e)
    {
        if (_autoPlayMode) return;
        if (!IsHumanTurn()) return;

        if (sender is not PictureBox { Tag: int clickedCell } pictureBox) return;
        
        if (_chessGame.IsOver) return;

        if (_selectedCell == clickedCell)
        {
            ClearSelection();
            return;
        }

        var piece = _chessGame.GetPiece(clickedCell);
        if (_selectedCell is null && piece is null) return;
        if (_selectedCell is null && piece != null && piece.Color != _humanPlayer) return;

        if (_selectedCell is null && piece != null && piece.Color == _humanPlayer)
        {
            SelectPiece(clickedCell, pictureBox);
            return;
        }

        if (piece != null && piece.Color == _humanPlayer)
        {
            ChangeSelection(clickedCell, pictureBox);
            return;
        }

        _ = MakeMove((int)pictureBox.Tag);
    }
    
    private bool IsHumanTurn()
    {
        return _chessGame.CurrentPlayer == _humanPlayer;
    }

    private bool IsEngineTurn()
    {
        return !_autoPlayMode && _chessGame.CurrentPlayer != _humanPlayer;
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
            var engineResult = _engine.FindBestMove();
            if (engineResult.BestMove is null) return;
            _chessGame.MakeMove(engineResult.BestMove);
            _lastMoveFrom = engineResult.BestMove.From;
            _lastMoveTo = engineResult.BestMove.To;
            UpdateBoardAfterMove();
        });
    }

    private async void MakeEngineMoveAsync()
    {
        if (_chessGame.IsOver || !IsEngineTurn()) return;

        await Task.Run(() =>
        {
            var engineResult = _engine.FindBestMove();
            if (engineResult.BestMove is null) return;
    
            _chessGame.MakeMove(engineResult.BestMove);
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
            
            _chessGame.MakeMove(move);
        
            _lastMoveFrom = _selectedCell!.Value;
            _lastMoveTo = targetCell;
            UpdateBoardAfterMove();

            if (!_chessGame.IsOver && IsEngineTurn())
            {
                await MakeEngineMove();
            }
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