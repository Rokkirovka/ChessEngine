using MyChess.Core;
using MyChess.Models;
using MyChess.Models.Moves;
using MyChess.Models.Pieces;

namespace MyChess.Rules.SpecialRules;

public static class CastlingRule
{
    public static IEnumerable<ChessMove> GetCastlingMoves(int kingPos, ChessBoard board, BoardState boardState)
    {
        var color = kingPos == board.FindKing(ChessColor.White) ? ChessColor.White : ChessColor.Black;
        
        if (color == ChessColor.White)
        {
            if (boardState.CastlingRights.HasFlag(CastlingRights.WhiteKingSide) && 
                CanCastle(true, color, board))
            {
                yield return new CastlingMove((ChessCell)kingPos, (ChessCell)kingPos + 2);
            }
            
            if (boardState.CastlingRights.HasFlag(CastlingRights.WhiteQueenSide) && 
                CanCastle(false, color, board))
            {
                yield return new CastlingMove((ChessCell)kingPos, (ChessCell)kingPos - 2);
            }
        }
        else
        {
            if (boardState.CastlingRights.HasFlag(CastlingRights.BlackKingSide) && 
                CanCastle(true, color, board))
            {
                yield return new CastlingMove((ChessCell)kingPos, (ChessCell)kingPos + 2); 
            }
            
            if (boardState.CastlingRights.HasFlag(CastlingRights.BlackQueenSide) && 
                CanCastle(false, color, board))
            {
                yield return new CastlingMove((ChessCell)kingPos, (ChessCell)kingPos - 2);
            }
        }
    }
    
    private static bool CanCastle(bool kingSide, ChessColor color, ChessBoard board)
    {
        int rookPos, checkSquare1, checkSquare2, checkSquare3, checkSquare4 = -1;
        var allPieces = (BitBoard)(board.Occupancies[0] | board.Occupancies[1]);
        
        if (color == ChessColor.White)
        {
            if (kingSide)
            {
                rookPos = 63;
                checkSquare1 = 60;
                checkSquare2 = 61; 
                checkSquare3 = 62;
            }
            else 
            {
                rookPos = 56;
                checkSquare1 = 60;
                checkSquare2 = 59; 
                checkSquare3 = 58;
                checkSquare4 = 57;
            }
        }
        else 
        {
            if (kingSide) 
            {
                rookPos = 7;
                checkSquare1 = 4; 
                checkSquare2 = 5; 
                checkSquare3 = 6; 
            }
            else
            {
                rookPos = 0; 
                checkSquare1 = 4; 
                checkSquare2 = 3; 
                checkSquare3 = 2; 
                checkSquare4 = 1; 
            }
        }
        
        var rookPiece = color == ChessColor.White ? Rook.White : Rook.Black;
        if (!board.BitBoards[rookPiece.Index].GetBit(rookPos)) return false;
            
        if (kingSide)
        {
            if (allPieces.GetBit(checkSquare2) || allPieces.GetBit(checkSquare3)) return false;
        }
        else
        {
            if (allPieces.GetBit(checkSquare2) || allPieces.GetBit(checkSquare3) || allPieces.GetBit(checkSquare4))
                return false;
        }

        return !GameRules.IsSquareUnderAttack(checkSquare1, color, board)
               && !GameRules.IsSquareUnderAttack(checkSquare3, color, board);
    }
}