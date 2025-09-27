using MyChess.Core;

namespace MyChessVisual
{
    public class GameInfoRenderer
    {
        private readonly ChessGame _game;
        private readonly Control _parentControl;
        private readonly Font _infoFont = new("Arial", 12, FontStyle.Bold);
        private readonly Font _statusFont = new("Arial", 14, FontStyle.Regular);
        
        private Label _playerLabel;
        private Label _evaluationLabel;
        private Label _statusLabel;
        private Label _moveNumberLabel;
        private Label _lastMoveLabel;
        private Button _restartButton;
        
        public GameInfoRenderer(ChessGame game, Control parentControl)
        {
            _game = game;
            _parentControl = parentControl;
            InitializeControls();
        }
        
        private void InitializeControls()
        {
            _playerLabel = new Label
            {
                Location = new Point(650, 50),
                Size = new Size(200, 30),
                Font = _infoFont,
                ForeColor = Color.Black
            };
            
            _evaluationLabel = new Label
            {
                Location = new Point(650, 90),
                Size = new Size(200, 30),
                Font = _infoFont,
                ForeColor = Color.DarkBlue
            };
            
            _statusLabel = new Label
            {
                Location = new Point(650, 130),
                Size = new Size(200, 40),
                Font = _statusFont,
                ForeColor = Color.Red
            };
            
            _moveNumberLabel = new Label
            {
                Location = new Point(650, 180),
                Size = new Size(200, 30),
                Font = _infoFont,
                ForeColor = Color.DarkGreen
            };
            
            _lastMoveLabel = new Label
            {
                Location = new Point(650, 220),
                Size = new Size(200, 30),
                Font = _infoFont,
                ForeColor = Color.Gray
            };
            
            _restartButton = new Button
            {
                Location = new Point(650, 270),
                Size = new Size(120, 30),
                Text = "Новая игра",
                Font = _infoFont
            };
            
            // Добавляем элементы на форму
            _parentControl.Controls.Add(_playerLabel);
            _parentControl.Controls.Add(_evaluationLabel);
            _parentControl.Controls.Add(_statusLabel);
            _parentControl.Controls.Add(_moveNumberLabel);
            _parentControl.Controls.Add(_lastMoveLabel);
            _parentControl.Controls.Add(_restartButton);
        }
        
        public void Clear()
        {
            _playerLabel.Text = string.Empty;
            _evaluationLabel.Text = string.Empty;
            _statusLabel.Text = string.Empty;
            _moveNumberLabel.Text = string.Empty;
            _lastMoveLabel.Text = string.Empty;
        }
    }
}