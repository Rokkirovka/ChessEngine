using MyChess.Core;
using MyChess.Rules.MoveGenerator;
using MyChessEngine.Core.Evaluation.Position.Interfaces;
using MyChessEngine.Utils;

namespace MyChessEngine.Core.Evaluation.Position.EvaluationComponents;

public class KingSafetyEvaluator : IEvaluationComponent
{
    public int Evaluate(ChessBoard board)
    {
        var score = 0;
        
        score += EvaluateWhiteKing(board);
        score += EvaluateBlackKing(board);
        
        return score;
    }
    
    private static int EvaluateWhiteKing(ChessBoard board)
    {
        var score = 0;
        var whiteKing = board.WhiteKing;
        var tempWhiteKing = whiteKing;
        
        while (tempWhiteKing.Value != 0)
        {
            var cell = tempWhiteKing.GetLeastSignificantBitIndex();
            tempWhiteKing = tempWhiteKing.ClearBit(cell);
            
            if ((board.BitBoards[0] & PieceSquareTables.FileMasks[cell]).Value == 0)
                score -= PieceSquareTables.SemiOpenFileScore;
                
            if ((board.BitBoards[0] & board.BitBoards[6] & PieceSquareTables.FileMasks[cell]).Value == 0)
                score -= PieceSquareTables.OpenFileScore;
                
            score += (KingMoveGenerator.KingAttackMasks[cell] & board.Occupancies[0]).CountBits() * PieceSquareTables.KingShieldBonus;
        }
        
        return score;
    }
    
    private static int EvaluateBlackKing(ChessBoard board)
    {
        var score = 0;
        var blackKing = board.BlackKing;
        var tempBlackKing = blackKing;
        
        while (tempBlackKing.Value != 0)
        {
            var cell = tempBlackKing.GetLeastSignificantBitIndex();
            tempBlackKing = tempBlackKing.ClearBit(cell);
            
            if ((board.BitBoards[0] & PieceSquareTables.FileMasks[cell]).Value == 0)
                score += PieceSquareTables.SemiOpenFileScore;
                
            if ((board.BitBoards[0] & board.BitBoards[6] & PieceSquareTables.FileMasks[cell]).Value == 0)
                score += PieceSquareTables.OpenFileScore;
                
            score -= (KingMoveGenerator.KingAttackMasks[cell] & board.Occupancies[1]).CountBits() * PieceSquareTables.KingShieldBonus;
        }
        
        return score;
    }
}