using MyChess.Models;
using MyChess.Models.Moves;
using MyChess.Models.Pieces;
using MyChess.Rules;
using MyChess.Rules.SpecialRules;
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

    public void MakeMove(ChessMove move)
    {
        _moveExecutor.ExecuteMove(move, _board, _state);
        IsCheckmate = GameRules.IsCheckmate(_state.CurrentMoveColor, this);
    }

    public void UndoLastMove()
    {
        _moveExecutor.UndoMove(_board, _state);
        IsCheckmate = false;
    }

    public IEnumerable<ChessMove> GetValidMoves(int cell)
    {
        var piece = _board.GetPiece(cell);
        if (piece is null || piece.Color != CurrentPlayer) yield break;

        var friendlyPieces = CurrentPlayer == ChessColor.White ? 
            _board.Occupancies[0] : _board.Occupancies[1];
        var enemyPieces = CurrentPlayer == ChessColor.White ? 
            _board.Occupancies[1] : _board.Occupancies[0];

        var potentialMoves = piece
            .GetMoveGenerator()
            .GetPossibleMoves(cell, enemyPieces, friendlyPieces)
            .Where(move => GameRules.IsValidMove(move, _board, _state));

        foreach (var move in potentialMoves)
        {
            yield return move;
        }

        if (piece is King)
        {
            var moves = CastlingRule
                .GetCastlingMoves(cell, _board, _state)
                .Where(move => GameRules.IsValidMove(move, _board, _state));
            
            foreach (var move in moves)
            {
                yield return move;
            }
        }
        
        if (piece is Pawn)
        {
            var moves = EnPassantRule
                .GetEnPassantMoves(cell, _board, _state)
                .Where(move => GameRules.IsValidMove(move, _board, _state));
            
            foreach (var move in moves)
            {
                yield return move;
            }
            
            moves = PromotionRule
                .GetPromotionMoves(cell, _board, _state)
                .Where(move => GameRules.IsValidMove(move, _board, _state));
            
            foreach (var move in moves)
            {
                yield return move;
            } 
        }
    }

    public IEnumerable<int> GetCellsWillChange(ChessMove move)
    {
        var strategy = _strategyFactory.GetMoveStrategy(move);
        return strategy.GetCellsWillChange(move, _board, _state);
    }

    public ChessColor CurrentPlayer => _state.CurrentMoveColor;

    public IChessPiece? GetPiece(int cell) => _board.GetPiece(cell);

    public bool IsCheckmate { get; private set; }

    public bool IsStalemate { get; private set; }

    public bool IsOver => IsCheckmate || IsStalemate;

    public IEnumerable<ChessMove> GetAllPossibleMoves()
    {
        for (var i = 0; i < 64; i++)
        {
            foreach (var move in GetValidMoves(i))
            {
                yield return move;
            }
        }
    }

    public ChessBoard GetClonedBoard() => _board.Clone();

    private void InitializeBoard()
    {
        _board.BitBoards =
        [
            new BitBoard(0x00FF000000000000),
            new BitBoard(0x4200000000000000),
            new BitBoard(0x2400000000000000),
            new BitBoard(0x8100000000000000),
            new BitBoard(0x0800000000000000),
            new BitBoard(0x1000000000000000),
            new BitBoard(0x000000000000FF00),
            new BitBoard(0x0000000000000042),
            new BitBoard(0x0000000000000024),
            new BitBoard(0x0000000000000081),
            new BitBoard(0x0000000000000008),
            new BitBoard(0x0000000000000010)
        ];
        
        _board.UpdateWhiteOccupancies();
        _board.UpdateBlackOccupancies();
    }
}