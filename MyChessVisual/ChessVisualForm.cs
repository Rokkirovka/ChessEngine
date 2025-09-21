using MyChess.Core;

namespace MyChessVisual;

public partial class ChessVisualForm : Form
{
    private readonly ChessGame _game = new();
    private ChessBoardRenderer _boardRenderer;
    private ChessInputHandler _inputHandler;
    
    public ChessVisualForm()
    {
        InitializeComponent();
        InitializeChessComponents();
        SetFixedSize();
    }
    
    private void SetFixedSize()
    {
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        this.MaximizeBox = false;
    }
    
    private void InitializeChessComponents()
    {
        ClientSize = new Size(16 * 60 + 40, 8 * 60 + 40);
        
        _boardRenderer = new ChessBoardRenderer(_game, this);
        _inputHandler = new ChessInputHandler(_game, _boardRenderer);
        
        foreach (var square in _boardRenderer.GetAllSquares())
        {
            square.Click += _inputHandler.HandleSquareClick;
        }
    }
}