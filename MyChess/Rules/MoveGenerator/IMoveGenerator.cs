using MyChess.Core;
using MyChess.Models.Moves;

namespace MyChess.Rules.MoveGenerator;

public interface IMoveGenerator
{
    IEnumerable<ChessMove> GetPossibleMoves(
        int pieceCell, 
        BitBoard enemyPieces,
        BitBoard friendlyPieces);
}