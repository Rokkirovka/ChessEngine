using MyChess.Core;
using MyChess.Models;
using MyChess.Models.Moves;
using MyChess.Models.Pieces;

namespace MyChess.Services
{
    public static class MoveFromStringFactory
    {
        private static ChessMove CreateMove(ChessBoard chessBoard, ChessCell from, ChessCell to, IChessPiece? promotionPiece = null)
        {
            if (IsCastlingMove(from, to))
            {
                return new CastlingMove(from, to);
            }

            if (IsEnPassantMove(chessBoard, from, to))
            {
                return new EnPassantMove(from, to);
            }

            if (promotionPiece != null && IsPromotionMove(from, to))
            {
                return new PromotionMove(from, to, promotionPiece);
            }

            return new StandardMove(from, to);
        }
        
        private static bool IsCastlingMove(ChessCell from, ChessCell to)
        {
            var fromIndex = (int)from;
            var toIndex = (int)to;
            var distance = Math.Abs(toIndex - fromIndex);

            var isHorizontalMove = fromIndex / 8 == toIndex / 8;
            var isKingMoveTwoSquares = distance == 2 && isHorizontalMove;
            
            return isKingMoveTwoSquares;
        }
        
        private static bool IsEnPassantMove(ChessBoard board, ChessCell from, ChessCell to)
        {
            if (board.GetPiece((int)from) is not Pawn) return false; 
            var fromIndex = (int)from;
            var toIndex = (int)to;

            var rowDiff = Math.Abs(toIndex / 8 - fromIndex / 8);
            var colDiff = Math.Abs(toIndex % 8 - fromIndex % 8);

            var isDiagonalMove = rowDiff == 1 && colDiff == 1;
            
            return isDiagonalMove && board.GetPiece((int)to) == null;
        }
        
        private static bool IsPromotionMove(ChessCell from, ChessCell to)
        {
            var fromRow = (int)from / 8;
            var toRow = (int)to / 8;

            var whitePromotion = fromRow == 6 && toRow == 7;
            var blackPromotion = fromRow == 1 && toRow == 0;
            
            return whitePromotion || blackPromotion;
        }

        public static ChessMove CreateMoveFromString(ChessBoard board, string moveString)
        {
            if (string.IsNullOrEmpty(moveString) || moveString.Length < 4)
                throw new ArgumentException("Invalid move string format");

            var fromStr = moveString.Substring(0, 2).ToUpper();
            var toStr = moveString.Substring(2, 2).ToUpper();
    
            // Определяем фигуру превращения, если она указана
            IChessPiece? promotionPiece = null;
            if (moveString.Length > 4)
            {
                var promotionChar = moveString[4].ToString().ToUpper();
        
                switch (promotionChar)
                {
                    case "Q":
                        promotionPiece = Queen.White;
                        break;
                    case "R":
                        promotionPiece =Rook.White;
                        break;
                    case "B":
                        promotionPiece = Bishop.White;
                        break;
                    case "N":
                        promotionPiece = Knight.White;
                        break;
                    default:
                        throw new ArgumentException($"Invalid promotion piece: {promotionChar}");
                }
            }
    
            if (!Enum.TryParse<ChessCell>(fromStr, out var from))
                throw new ArgumentException($"Invalid from cell: {fromStr}");
        
            if (!Enum.TryParse<ChessCell>(toStr, out var to))
                throw new ArgumentException($"Invalid to cell: {toStr}");
    
            return CreateMove(board, from, to, promotionPiece);
        }
    }
}