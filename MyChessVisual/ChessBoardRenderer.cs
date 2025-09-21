using MyChess.Models;
using MyChess.Core;

namespace MyChessVisual;

public class ChessBoardRenderer
{
    private const int BoardPadding = 20;
    private const int BoardSize = 8;
    private const int SquareSize = 60;
    
    private readonly PictureBox[,] _chessBoard;
    private readonly ChessGame _game;
    
    public ChessBoardRenderer(ChessGame game, Control parentControl)
    {
        _game = game;
        _chessBoard = new PictureBox[BoardSize, BoardSize];
        InitializeBoard(parentControl);
    }
    
    private void InitializeBoard(Control parentControl)
    {
        for (var row = 0; row < BoardSize; row++)
        {
            for (var col = 0; col < BoardSize; col++)
            {
                _chessBoard[row, col] = new PictureBox
                {
                    Width = SquareSize,
                    Height = SquareSize,
                    Location = new Point(col * SquareSize + BoardPadding, row * SquareSize + BoardPadding),
                    BackColor = GetSquareColor(row, col),
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Tag = (ChessCell)(row * BoardSize + col)
                };
                parentControl.Controls.Add(_chessBoard[row, col]);
            }
        }
        UpdateAllSquares();
    }
    
    public void UpdateAllSquares()
    {
        for (var row = 0; row < BoardSize; row++)
        {
            for (var col = 0; col < BoardSize; col++)
            {
                UpdateSquare(row, col);
            }
        }
    }
    
    public void UpdateSquare(int row, int col)
    {
        _chessBoard[row, col].Image = GetPieceImage(row, col);
        _chessBoard[row, col].BackColor = GetSquareColor(row, col);
    }
    
    public void HighlightSquare(int row, int col, Color color)
    {
        _chessBoard[row, col].BackColor = color;
    }
    
    private Color GetSquareColor(int row, int col)
    {
        return (row + col) % 2 == 0 ? Color.White : Color.Gray;
    }
    
    private Bitmap GetPieceImage(int row, int col)
    {
        var piece = _game.GetPiece((ChessCell)(row * BoardSize + col));
        return piece == null 
            ? new Bitmap(SquareSize, SquareSize) 
            : ChessPieceImages.GetImage(piece);
    }
    
    public IEnumerable<PictureBox> GetAllSquares()
    {
        for (var row = 0; row < BoardSize; row++)
        {
            for (var col = 0; col < BoardSize; col++)
            {
                yield return _chessBoard[row, col];
            }
        }
    }
}