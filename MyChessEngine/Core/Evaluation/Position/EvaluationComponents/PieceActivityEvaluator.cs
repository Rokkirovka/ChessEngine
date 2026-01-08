using MyChess.Core;
using MyChess.Rules.MoveGenerator;
using MyChessEngine.Core.Evaluation.Position.Interfaces;
using MyChessEngine.Utils;

namespace MyChessEngine.Core.Evaluation.Position.EvaluationComponents;

public class PieceActivityEvaluator : IEvaluationComponent
{
    public int Evaluate(ChessBoard board)
    {
        var score = 0;

        score += EvaluateRooks(board);
        score += EvaluateBishops(board);
        score += EvaluateQueens(board);

        return score;
    }

    private static int EvaluateRooks(ChessBoard board)
    {
        var score = 0;

        score += EvaluateWhiteRooks(board);
        score += EvaluateBlackRooks(board);

        return score;
    }

    private static int EvaluateWhiteRooks(ChessBoard board)
    {
        var score = 0;
        var whiteRooks = board.WhiteRooks;
        var tempWhiteRooks = whiteRooks;

        while (tempWhiteRooks.Value != 0)
        {
            var cell = tempWhiteRooks.GetLeastSignificantBitIndex();
            tempWhiteRooks = tempWhiteRooks.ClearBit(cell);

            if ((ulong)(board.WhitePawns & PieceSquareTables.FileMasks[cell]) == 0)
                score += PieceSquareTables.SemiOpenFileScore;

            if ((ulong)(board.WhitePawns & board.BlackPawns & PieceSquareTables.FileMasks[cell]) == 0)
                score += PieceSquareTables.OpenFileScore;

            score += RookMoveGenerator.GetRookAttacks(cell, (ulong)(board.Occupancies[0] | board.Occupancies[1]))
                .CountBits() * PieceSquareTables.MobilityBonus;
        }

        return score;
    }

    private static int EvaluateBlackRooks(ChessBoard board)
    {
        var score = 0;
        var blackRooks = board.BlackRooks;
        var tempBlackRooks = blackRooks;

        while (tempBlackRooks.Value != 0)
        {
            var cell = tempBlackRooks.GetLeastSignificantBitIndex();
            tempBlackRooks = tempBlackRooks.ClearBit(cell);

            if ((ulong)(board.BlackPawns & PieceSquareTables.FileMasks[cell]) == 0)
                score -= PieceSquareTables.SemiOpenFileScore;

            if ((ulong)(board.WhitePawns & board.BlackPawns & PieceSquareTables.FileMasks[cell]) == 0)
                score -= PieceSquareTables.OpenFileScore;

            score -= RookMoveGenerator.GetRookAttacks(cell, (ulong)(board.Occupancies[0] | board.Occupancies[1]))
                .CountBits() * PieceSquareTables.MobilityBonus;
        }

        return score;
    }

    private static int EvaluateBishops(ChessBoard board)
    {
        var score = 0;

        score += EvaluateWhiteBishops(board);
        score += EvaluateBlackBishops(board);

        return score;
    }

    private static int EvaluateWhiteBishops(ChessBoard board)
    {
        var score = 0;
        var whiteBishops = board.WhiteBishops;
        var tempWhiteBishops = whiteBishops;

        while (tempWhiteBishops.Value != 0)
        {
            var cell = tempWhiteBishops.GetLeastSignificantBitIndex();
            tempWhiteBishops = tempWhiteBishops.ClearBit(cell);

            score += BishopMoveGenerator.GetBishopAttacks(cell, (ulong)(board.Occupancies[0] | board.Occupancies[1]))
                .CountBits() * PieceSquareTables.MobilityBonus;
        }

        return score;
    }

    private static int EvaluateBlackBishops(ChessBoard board)
    {
        var score = 0;
        var blackBishops = board.BlackBishops;
        var tempBlackBishops = blackBishops;

        while (tempBlackBishops.Value != 0)
        {
            var cell = tempBlackBishops.GetLeastSignificantBitIndex();
            tempBlackBishops = tempBlackBishops.ClearBit(cell);

            score -= BishopMoveGenerator.GetBishopAttacks(cell, (ulong)(board.Occupancies[0] | board.Occupancies[1]))
                .CountBits() * PieceSquareTables.MobilityBonus;
        }

        return score;
    }

    private static int EvaluateQueens(ChessBoard board)
    {
        var score = 0;

        score += EvaluateWhiteQueens(board);
        score += EvaluateBlackQueens(board);

        return score;
    }

    private static int EvaluateWhiteQueens(ChessBoard board)
    {
        var score = 0;
        var whiteQueens = board.WhiteQueens;
        var tempWhiteQueens = whiteQueens;

        while (tempWhiteQueens.Value != 0)
        {
            var cell = tempWhiteQueens.GetLeastSignificantBitIndex();
            tempWhiteQueens = tempWhiteQueens.ClearBit(cell);

            score += (BishopMoveGenerator.GetBishopAttacks(cell, (ulong)(board.Occupancies[0] | board.Occupancies[1]))
                      | RookMoveGenerator.GetRookAttacks(cell, (ulong)(board.Occupancies[0] | board.Occupancies[1])))
                .CountBits() * PieceSquareTables.MobilityBonus;
        }

        return score;
    }

    private static int EvaluateBlackQueens(ChessBoard board)
    {
        var score = 0;
        var blackQueens = board.BlackQueens;
        var tempBlackQueens = blackQueens;

        while (tempBlackQueens.Value != 0)
        {
            var cell = tempBlackQueens.GetLeastSignificantBitIndex();
            tempBlackQueens = tempBlackQueens.ClearBit(cell);

            score -= (BishopMoveGenerator.GetBishopAttacks(cell, (ulong)(board.Occupancies[0] | board.Occupancies[1]))
                      | RookMoveGenerator.GetRookAttacks(cell, (ulong)(board.Occupancies[0] | board.Occupancies[1])))
                .CountBits() * PieceSquareTables.MobilityBonus;
        }

        return score;
    }
}