using MyChess.Core.Board;
using MyChess.Hashing;
using MyChess.Hashing.PositionHistory;
using MyChess.Models;
using MyChess.Models.Moves;
using MyChess.Models.Pieces;
using MyChess.Rules;
using MyChess.Rules.SpecialRules;
using MyChess.Services.Fen;
using MyChess.Services.MoveExecution;

namespace MyChess.Core;

public class ChessGame
{
    public readonly ChessBoard Board = new();
    public readonly BoardState State = new();
    private readonly MoveStrategyFactory _strategyFactory = new();
    private readonly MoveExecutor _moveExecutor;
    private readonly RepetitionTable _repetitionTable = new();
    private ulong _currentZobristHash;

    public int Ply;

    public ChessGame()
    {
        _moveExecutor = new MoveExecutor(_strategyFactory);
        InitializeBoard();
        UpdateZobristHash();
    }

    public ChessGame(string fen)
    {
        _moveExecutor = new MoveExecutor(_strategyFactory);
        InitializeFromFen(fen);
        UpdateZobristHash();
    }

    public void MakeMove(ChessMove move)
    {
        Ply++;
        _moveExecutor.ExecuteMove(move, Board, State);
        
        UpdateZobristHash();
        _repetitionTable.AddPosition(_currentZobristHash);
    }

    public void UndoLastMove()
    {
        Ply--;
        _repetitionTable.RemoveLastPosition();
        _moveExecutor.UndoMove(Board, State);
        UpdateZobristHash();
    }

    public ChessColor CurrentPlayer => State.CurrentMoveColor;

    public IChessPiece? GetPiece(int cell) => Board.GetPiece(cell);

    public bool IsCheckmate => GameRules.IsCheckmate(State.CurrentMoveColor, Board, State);
    public bool IsStalemate => GameRules.IsStalemate(State.CurrentMoveColor, Board, State);
    public bool IsDrawByRepetition => _repetitionTable.IsDrawByRepetition(_currentZobristHash);
    
    private void UpdateZobristHash()
    {
        _currentZobristHash = ZobristHasher.CalculateInitialHash(Board, State);
    }

    public bool IsKingInCheck()
    {
        return GameRules.IsSquareUnderAttack(Board.FindKing(State.CurrentMoveColor), State.CurrentMoveColor, Board);
    }

    private IEnumerable<ChessMove> GetAllPotentialMoves()
    {
        for (var i = 0; i < 64; i++)
        {
            var piece = Board.GetPiece(i);
            if (piece is null || piece.Color != CurrentPlayer) continue;

            var friendlyPieces = CurrentPlayer == ChessColor.White ? Board.Occupancies[0] : Board.Occupancies[1];
            var enemyPieces = CurrentPlayer == ChessColor.White ? Board.Occupancies[1] : Board.Occupancies[0];

            var potentialMoves = piece
                .GetMoveGenerator()
                .GetPossibleMoves(i, enemyPieces, friendlyPieces);

            foreach (var move in potentialMoves)
            {
                yield return move;
            }

            if (piece is King)
            {
                foreach (var move in CastlingRule.GetCastlingMoves(i, Board, State))
                {
                    yield return move;
                }
            }

            if (piece is Pawn)
            {
                foreach (var move in EnPassantRule.GetEnPassantMoves(i, Board, State))
                {
                    yield return move;
                }

                foreach (var move in PromotionRule.GetPromotionMoves(i, Board, State))
                {
                    yield return move;
                }
            }
        }
    }

    public IEnumerable<ChessMove> GetAllPossibleMoves()
    {
        return GetAllPotentialMoves().Where(move => GameRules.IsValidMove(move, Board, State));
    }

    private void InitializeBoard()
    {
        Board.BitBoards =
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

        Board.UpdateWhiteOccupancies();
        Board.UpdateBlackOccupancies();
    }

    private void InitializeFromFen(string fen)
    {
        var fenParts = fen.Split(' ');
        if (fenParts.Length < 4) throw new ArgumentException("Invalid FEN string");

        FenParser.SetupBoardFromFen(Board, fenParts[0]);
        FenParser.SetupCurrentPlayerFromFen(State, fenParts[1]);
        FenParser.SetupCastlingRightsFromFen(State, fenParts[2]);
        FenParser.SetupEnPassantFromFen(State, fenParts[3]);
    }

    public void PrintBoard()
    {
        Console.WriteLine("   a b c d e f g h");
        Console.WriteLine("   ________________");

        for (var rank = 0; rank < 8; rank++)
        {
            Console.Write($"{8 - rank} |");
            for (var file = 0; file < 8; file++)
            {
                var square = rank * 8 + file;
                var piece = GetPiece(square);

                var symbol = piece switch
                {
                    Pawn { Color: ChessColor.White } => "P",
                    Knight { Color: ChessColor.White } => "N",
                    Bishop { Color: ChessColor.White } => "B",
                    Rook { Color: ChessColor.White } => "R",
                    Queen { Color: ChessColor.White } => "Q",
                    King { Color: ChessColor.White } => "K",
                    Pawn { Color: ChessColor.Black } => "p",
                    Knight { Color: ChessColor.Black } => "n",
                    Bishop { Color: ChessColor.Black } => "b",
                    Rook { Color: ChessColor.Black } => "r",
                    Queen { Color: ChessColor.Black } => "q",
                    King { Color: ChessColor.Black } => "k",
                    _ => "."
                };

                Console.Write($"{symbol} ");
            }

            Console.WriteLine($"| {8 - rank}");
        }

        Console.WriteLine("   ----------------");
        Console.WriteLine("   a b c d e f g h");
        Console.WriteLine();

        Console.WriteLine($"Current player: {CurrentPlayer}");

        var castlingRights = State.CastlingRights;
        Console.Write("Castling rights: ");

        if (castlingRights.HasFlag(CastlingRights.WhiteKingSide)) Console.Write("K");
        if (castlingRights.HasFlag(CastlingRights.WhiteQueenSide)) Console.Write("Q");
        if (castlingRights.HasFlag(CastlingRights.BlackKingSide)) Console.Write("k");
        if (castlingRights.HasFlag(CastlingRights.BlackQueenSide)) Console.Write("q");

        if (castlingRights == 0) Console.Write("None");
        Console.WriteLine();

        var enPassantTarget = State.EnPassantTarget;
        if (enPassantTarget.HasValue)
        {
            var file = enPassantTarget.Value % 8;
            var rank = 7 - enPassantTarget.Value / 8;
            var fileChar = (char)('a' + file);
            var rankChar = (char)('1' + rank);
            Console.WriteLine($"En passant target: {fileChar}{rankChar}");
        }
        else
        {
            Console.WriteLine("En passant target: None");
        }

        if (IsCheckmate) Console.WriteLine("CHECKMATE!");
        else if (IsStalemate) Console.WriteLine("STALEMATE!");
        else if (IsDrawByRepetition) Console.WriteLine("DRAW BY THREEFOLD REPETITION!");
    }
}