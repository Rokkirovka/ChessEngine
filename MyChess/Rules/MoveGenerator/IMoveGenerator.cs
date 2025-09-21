using MyChess.Core;
using MyChess.Models;
using MyChess.Models.Moves;

namespace MyChess.Rules.MoveGenerator;

public interface IMoveGenerator
{
    IEnumerable<ChessMove> GetPossibleMoves(ChessCell cell, ChessBoard board, BoardState boardState);
}