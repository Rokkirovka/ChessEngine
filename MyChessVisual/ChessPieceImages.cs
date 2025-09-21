using MyChess.Models;
using MyChess.Models.Pieces;

namespace MyChessVisual;

public static class ChessPieceImages
{
    private const string ResourcesPath = @"..\..\..\Resources\ChessPieces\";
    
    public static Bitmap GetImage(IChessPiece piece)
    {
        var colorPrefix = piece.Color == ChessColor.White ? "w" : "b";
        var pieceSuffix = GetPieceSuffix(piece);
        var imagePath = $"{ResourcesPath}{colorPrefix}{pieceSuffix}.png";
        return new Bitmap(imagePath);
    }
    
    private static string GetPieceSuffix(IChessPiece piece)
    {
        return piece switch
        {
            King => "K",
            Queen => "Q",
            Bishop => "B",
            Knight => "N",
            Rook => "R",
            Pawn => "P",
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}