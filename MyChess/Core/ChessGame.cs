using MyChess.Models;
using MyChess.Models.Moves;
using MyChess.Models.Pieces;
using MyChess.Rules;
using MyChess.Services.MoveExecution;

namespace MyChess.Core;

public class ChessGame
{
    private readonly ChessBoard _board = new();
    private readonly BoardState _state = new();
    private readonly MoveStrategyFactory _strategyFactory = new();
    private readonly MoveExecutor _moveExecutor;

    public ChessGame()
    {
        _moveExecutor = new MoveExecutor(_strategyFactory);
        InitializeBoard();
    }

    public bool TryMakeMove(ChessMove move)
    {
        if (IsOver) return false;
        var piece = GetPiece(move.From);
        if (piece is null) return false;
        if (!_moveExecutor.TryExecuteMove(move, _board, _state)) return false;
        IsCheckmate = GameRules.IsCheckmate(_state.CurrentMoveColor, this);
        return true;
    }

    public void ForceMove(ChessMove move)
    {
        _moveExecutor.ForceMove(move, _board, _state);
        IsCheckmate = GameRules.IsCheckmate(_state.CurrentMoveColor, this);
        
    }
    
    public void UndoLastMove()
    {
        _moveExecutor.UndoMove(_board, _state);
        IsCheckmate = false;
    }

    public IEnumerable<ChessMove> GetValidMoves(ChessCell cell)
    {
        var piece = _board.GetPiece(cell);
        if (piece is null || piece.Color != CurrentPlayer) return [];
        var potentialMoves =  piece
            .GetMoveGenerator()
            .GetPossibleMoves(cell, _board, _state);
        return potentialMoves.Where(move => GameRules.IsValidMove(move, _board, _state));
    }

    public IEnumerable<ChessCell> GetCellsWillChange(ChessMove move)
    {
        var strategy = _strategyFactory.GetMoveStrategy(move);
        return strategy.GetCellsWillChange(move, _board, _state);
    }

    public ChessColor CurrentPlayer => _state.CurrentMoveColor;

    public IChessPiece? GetPiece(ChessCell cell) => _board.GetPiece(cell);
    
    public bool IsCheckmate { get; private set; }
    
    public bool IsStalemate { get; private set; }
    
    public bool IsOver => IsCheckmate || IsStalemate;

    public IEnumerable<ChessMove> GetAllPossibleMoves()
    {
        for (var i = 0; i < 64; i++)
        {
            var cell = (ChessCell)i;
            var piece = GetPiece(cell);
            if (piece?.Color == CurrentPlayer)
            {
                foreach (var move in GetValidMoves(cell)) 
                    yield return move;
            }
        }
    }

    public ChessBoard GetClonedBoard() => _board.Clone();

    private void InitializeBoard()
    {
        _board.SetPiece(ChessCell.A8, Rook.Black);
        _board.SetPiece(ChessCell.B8, Knight.Black);
        _board.SetPiece(ChessCell.C8, Bishop.Black);
        _board.SetPiece(ChessCell.D8, Queen.Black);
        _board.SetPiece(ChessCell.E8, King.Black);
        _board.SetPiece(ChessCell.F8, Bishop.Black);
        _board.SetPiece(ChessCell.G8, Knight.Black);
        _board.SetPiece(ChessCell.H8, Rook.Black);

        _board.SetPiece(ChessCell.A1, Rook.White);
        _board.SetPiece(ChessCell.B1, Knight.White);
        _board.SetPiece(ChessCell.C1, Bishop.White);
        _board.SetPiece(ChessCell.D1, Queen.White);
        _board.SetPiece(ChessCell.E1, King.White);
        _board.SetPiece(ChessCell.F1, Bishop.White);
        _board.SetPiece(ChessCell.G1, Knight.White);
        _board.SetPiece(ChessCell.H1, Rook.White);

        for (var col = 0; col < 8; col++)
        {
            _board.SetPiece((ChessCell)((int)ChessCell.A7 + col), Pawn.Black);
            _board.SetPiece((ChessCell)((int)ChessCell.A2 + col), Pawn.White);
        }
    }
}