using MyChess.Models;
using MyChess.Models.Pieces;

namespace MyChessVisual;

public partial class PromotionForm : Form
{
    private readonly ChessColor _color;
    public IChessPiece SelectedPieceType { get; private set; }

    public PromotionForm(ChessColor color)
    {
        _color = color;
        InitializeComponent();
        InitializePieceButtons();
    }

    private void InitializePieceButtons()
    {
        Text = "Выберите фигуру для промоции";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(240, 60);
        MaximizeBox = false;
        MinimizeBox = false;

        var whitePieces = new (IChessPiece, string) []
        {
            (Queen.White, "Q"),
            (Rook.White, "R"),
            (Bishop.White, "B"),
            (Knight.White, "N")
        };
        
        var blackPieces = new (IChessPiece, string) []
        {
            (Queen.Black, "Q"),
            (Rook.Black, "R"),
            (Bishop.Black, "B"),
            (Knight.Black, "N")
        };
        
        var pieces = _color is ChessColor.White ? whitePieces : blackPieces;

        for (var i = 0; i < pieces.Length; i++)
        {
            var button = new Button
            {
                Tag = pieces[i].Item1,
                Size = new Size(50, 50),
                Location = new Point(i * 60, 5),
                Image = ResizeImage(GetPieceImage(pieces[i].Item2), 50, 50),
                ImageAlign = ContentAlignment.MiddleCenter
            };
            button.Click += PieceButton_Click;
            Controls.Add(button);
        }
    }

    private Bitmap GetPieceImage(string pieceSuffix)
    {
        var imagePath = $@"..\..\..\Resources\ChessPieces\{(_color == ChessColor.White ? "w" : "b")}{pieceSuffix}.png";
        return new Bitmap(imagePath);
    }

    private void PieceButton_Click(object? sender, EventArgs e)
    {
        if (sender is Button button && button.Tag is IChessPiece piece)
        {
            SelectedPieceType = piece;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
    
    private Image ResizeImage(Image image, int width, int height)
    {
        return new Bitmap(image, width, height);
    }
}