using MyChess.Models;
using MyChess.Models.Moves;
using MyChess.Models.Pieces;
using MyChess.Rules;
using MyChess.Rules.SpecialRules;
using MyChess.Services;
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

    public ChessGame(string fen)
    {
        _moveExecutor = new MoveExecutor(_strategyFactory);
        InitializeFromFen(fen);
    }

    public void MakeMove(ChessMove move)
    {
        _moveExecutor.ExecuteMove(move, _board, _state);
        IsCheckmate = GameRules.IsCheckmate(_state.CurrentMoveColor, this);
        IsStalemate = GameRules.IsStalemate(_state.CurrentMoveColor, this);
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

    private void InitializeFromFen(string fen)
    {
        var fenParts = fen.Split(' ');
        if (fenParts.Length < 4) throw new ArgumentException("Invalid FEN string");

        SetupBoardFromFen(fenParts[0]);
        SetupCurrentPlayerFromFen(fenParts[1]);
        SetupCastlingRightsFromFen(fenParts[2]);
        SetupEnPassantFromFen(fenParts[3]);
    }

    private void SetupBoardFromFen(string boardFen)
    {
        var ranks = boardFen.Split('/');
        if (ranks.Length != 8) throw new ArgumentException("Invalid board position in FEN");

        for (var rankIndex = 0; rankIndex < 8; rankIndex++)
        {
            var rank = ranks[rankIndex];
            var squareIndex = rankIndex * 8;

            foreach (var chr in rank)
            {
                if (char.IsDigit(chr))
                {
                    var emptySquares = chr - '0';
                    squareIndex += emptySquares;
                }
                else
                {
                    var piece = GetPieceFromFenChar(chr);
                    _board.SetPiece(squareIndex, piece);
                    squareIndex++;
                }
            }
        }
    }

    private IChessPiece GetPieceFromFenChar(char c)
    {
        return c switch
        {
            'P' => Pawn.White,
            'N' => Knight.White,
            'B' => Bishop.White,
            'R' => Rook.White,
            'Q' => Queen.White,
            'K' => King.White,
            'p' => Pawn.Black,
            'n' => Knight.Black,
            'b' => Bishop.Black,
            'r' => Rook.Black,
            'q' => Queen.Black,
            'k' => King.Black,
            _ => throw new ArgumentException($"Invalid FEN character: {c}")
        };
    }

    private void SetupCurrentPlayerFromFen(string activeColor)
    {
        _state.CurrentMoveColor = activeColor.ToLower() == "w" ? ChessColor.White : ChessColor.Black;
    }

    private void SetupCastlingRightsFromFen(string castlingRights)
    {
        if (!castlingRights.Contains('K')) _state.DisableCastling(CastlingRights.WhiteKingSide);
        if (!castlingRights.Contains('Q')) _state.DisableCastling(CastlingRights.WhiteQueenSide);
        if (!castlingRights.Contains('k')) _state.DisableCastling(CastlingRights.BlackKingSide);
        if (!castlingRights.Contains('q')) _state.DisableCastling(CastlingRights.BlackQueenSide);
    }

    private void SetupEnPassantFromFen(string enPassantSquare)
    { 
        _state.EnPassantTarget = enPassantSquare == "-" ? null : SquareNotationToIndex(enPassantSquare);
    }
    
    private int? SquareNotationToIndex(string square)
    {
        if (square.Length != 2)
            return null;

        var fileChar = square[0];
        var rankChar = square[1];

        if (fileChar < 'a' || fileChar > 'h' || rankChar < '1' || rankChar > '8')
            return null;

        var file = fileChar - 'a';
        var rank = 8 - (rankChar - '0');
        return rank * 8 + file;
    }

    public ChessMove CreateMoveFromString(string moveString)
    {
        return MoveFromStringFactory.CreateMoveFromString(_board, moveString);
    }
}