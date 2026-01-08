using MyChess.Core;
using MyChessEngine.Core.Evaluation.Position.Interfaces;
using MyChessEngine.Utils;

namespace MyChessEngine.Core.Evaluation.Position.EvaluationComponents;

public class PawnStructureEvaluator : IEvaluationComponent
{
    public int Evaluate(ChessBoard board)
    {
        var score = 0;
        
        score += EvaluateWhitePawns(board);
        score += EvaluateBlackPawns(board);
        
        return score;
    }
    
    private static int EvaluateWhitePawns(ChessBoard board)
    {
        var score = 0;
        var whitePawns = board.WhitePawns;
        var tempWhitePawns = whitePawns;
        
        while (tempWhitePawns.Value != 0)
        {
            var cell = tempWhitePawns.GetLeastSignificantBitIndex();
            tempWhitePawns = tempWhitePawns.ClearBit(cell);
            
            var doublePawns = (board.WhitePawns & PieceSquareTables.FileMasks[cell]).CountBits();
            score += PieceSquareTables.DoublePawnPenalty * doublePawns;
            
            if ((ulong)(board.WhitePawns & PieceSquareTables.IsolatedMasks[cell]) == 0)
                score += PieceSquareTables.IsolatedPawnPenalty;
                
            if ((ulong)(board.BlackPawns & PieceSquareTables.WhitePassedMasks[cell]) == 0)
                score += PieceSquareTables.PassedPawnBonus[7 - cell / 8];
        }
        
        return score;
    }
    
    private static int EvaluateBlackPawns(ChessBoard board)
    {
        var score = 0;
        var blackPawns = board.BlackPawns;
        var tempBlackPawns = blackPawns;
        
        while (tempBlackPawns.Value != 0)
        {
            var cell = tempBlackPawns.GetLeastSignificantBitIndex();
            tempBlackPawns = tempBlackPawns.ClearBit(cell);
            
            var doublePawns = (board.BlackPawns & PieceSquareTables.FileMasks[cell]).CountBits();
            score -= PieceSquareTables.DoublePawnPenalty * doublePawns;
            
            if ((ulong)(board.BlackPawns & PieceSquareTables.IsolatedMasks[cell]) == 0)
                score -= PieceSquareTables.IsolatedPawnPenalty;
                
            if ((ulong)(board.WhitePawns & PieceSquareTables.BlackPassedMasks[cell]) == 0)
                score -= PieceSquareTables.PassedPawnBonus[cell / 8];
        }
        
        return score;
    }
}