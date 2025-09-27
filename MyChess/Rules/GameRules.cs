using MyChess.Core;
using MyChess.Models;
using MyChess.Models.Moves;
using MyChess.Models.Pieces;
using MyChess.Rules.MoveGenerator;
using MyChess.Services.MoveExecution;

namespace MyChess.Rules;

public static class GameRules
{
    private static readonly MoveExecutor MoveExecutor = new(new MoveStrategyFactory());

    public static bool IsValidMove(ChessMove move, ChessBoard board, BoardState boardState)
    {
        var color = boardState.CurrentMoveColor;
        MoveExecutor.ExecuteMove(move, board, boardState);
        var valid = !IsKingInCheck(color, board);
        MoveExecutor.UndoMove(board, boardState);
        return valid;
    }

    private static bool IsKingInCheck(ChessColor kingColor, ChessBoard board)
    {
        var kingPos = kingColor == ChessColor.White
            ? board.BitBoards[5].GetLeastSignificantBitIndex()
            : board.BitBoards[11].GetLeastSignificantBitIndex();

        return IsSquareUnderAttack(kingPos, kingColor, board);
    }

    private static bool IsCellAttackedByDiagonal(int position, ChessBoard board, ChessColor cellColor)
    {
        var bitBoard = cellColor == ChessColor.Black 
            ? board.BitBoards[2]  | board.BitBoards[4]
            : board.BitBoards[8] | board.BitBoards[10];

        return (BishopMoveGenerator.CalculateBishopAttacksWithBlocking(position, 
            board.Occupancies[0] | board.Occupancies[1]) & bitBoard) != 0;
    }

    private static bool IsCellAttackedByVerticalOrHorizontal(int position, ChessBoard board, ChessColor cellColor)
    {
        var bitBoard = cellColor == ChessColor.Black 
            ? board.BitBoards[3]  | board.BitBoards[4]
            : board.BitBoards[9] | board.BitBoards[10];
        return (RookMoveGenerator.CalculateRookAttacksWithBlocking(position, 
            board.Occupancies[0] | board.Occupancies[1]) & bitBoard) != 0;
    }

    private static bool IsCellAttackedByKnight(int position, ChessBoard board, ChessColor cellColor)
    {
        var bitBoard = cellColor == ChessColor.Black ? board.BitBoards[1] : board.BitBoards[7];
        return (KnightMoveGenerator.KnightAttackMasks[position] & bitBoard) != 0;
    }

    private static bool IsCellAttackedByPawn(int position, ChessBoard board, ChessColor cellColor)
    {
        var bitBoard = cellColor == ChessColor.Black ? board.BitBoards[0] : board.BitBoards[6];
        if (cellColor == ChessColor.White)
            return (WhitePawnMoveGenerator.WhitePawnAttackMasks[position] & bitBoard) != 0;
        return (BlackPawnMoveGenerator.BlackPawnAttackMasks[position] & bitBoard) != 0;
    }

    private static bool IsCellAttackedByKing(int position, ChessBoard board, ChessColor cellColor)
    {
        var bitBoard = cellColor == ChessColor.Black ? board.BitBoards[5] : board.BitBoards[11];
        return (KingMoveGenerator.KingAttackMasks[position] & bitBoard) != 0;
    }


    public static bool IsSquareUnderAttack(int square, ChessColor defenderColor, ChessBoard board)
    {
        return IsCellAttackedByKing(square, board, defenderColor)
               || IsCellAttackedByKnight(square, board, defenderColor)
               || IsCellAttackedByPawn(square, board, defenderColor)
               || IsCellAttackedByVerticalOrHorizontal(square, board, defenderColor)
               || IsCellAttackedByDiagonal(square, board, defenderColor);
    }

    public static bool IsCheckmate(ChessColor color, ChessGame game)
    {
        if (!IsKingInCheck(color, game.GetClonedBoard())) return false;
        var moves = game.GetAllPossibleMoves();
        return !moves.Any();
    }
}