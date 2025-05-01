using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Sudoku
{
    public partial class Form1 : Form
    {
        private const int GridSize = 9;
        private const int MaxMistakes = 3;

        private TextBox[,] textBoxes = new TextBox[GridSize, GridSize];
        private int[,] solution;
        private int EmptyCellsCount;
        private int ElapsedSeconds = 0;
        private int Mistakes = 0;
        private Timer gameTimer;
        private bool isPaused = false;

        private Label statusLabel;
        private TableLayoutPanel mainLayout;
        private TableLayoutPanel boardLayout;
        private TableLayoutPanel buttonLayout;

        private Panel gamePanel;
        private Panel menuPanel;
        private int missingPercentage = 20; // default (easy)

        public Form1()
        {
            InitializeComponent();
            InitializeMenu();
        }

        // ============================
        //         MENU SETUP
        // ============================
        private void InitializeMenu()
        {
            this.Text = "Sudoku";
            this.ClientSize = new Size(500, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            menuPanel = new Panel
            {
                Dock = DockStyle.Fill
            };

            TableLayoutPanel layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 3,
                ColumnCount = 1
            };

            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 30)); // Spacer
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));    // Title
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 70)); // Buttons

            Label title = new Label
            {
                Text = "Choose Difficulty",
                Dock = DockStyle.Fill,
                Font = new Font("Consolas", 16, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };
            layout.Controls.Add(title, 0, 1);

            FlowLayoutPanel difficultyButtons = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                Dock = DockStyle.Fill,
                WrapContents = false,
                Anchor = AnchorStyles.None,
                AutoSize = true
            };

            Button easyBtn = new Button { Text = "Easy (20%)", Width = 200, Height = 40 };
            easyBtn.Click += (s, e) => StartGameWithDifficulty(20);
            Button mediumBtn = new Button { Text = "Medium (40%)", Width = 200, Height = 40 };
            mediumBtn.Click += (s, e) => StartGameWithDifficulty(40);
            Button hardBtn = new Button { Text = "Hard (60%)", Width = 200, Height = 40 };
            hardBtn.Click += (s, e) => StartGameWithDifficulty(60);

            difficultyButtons.Controls.Add(easyBtn);
            difficultyButtons.Controls.Add(mediumBtn);
            difficultyButtons.Controls.Add(hardBtn);

            layout.Controls.Add(difficultyButtons, 0, 2);
            menuPanel.Controls.Add(layout);
            this.Controls.Add(menuPanel);
        }


        private void StartGameWithDifficulty(int percentMissing)
        {
            missingPercentage = percentMissing;
            menuPanel.Visible = false;

            if (gamePanel != null)
                this.Controls.Remove(gamePanel);

            InitializeGameLayout();
            StartGame();
            StartTimer();
        }

        // ============================
        //         GAME SETUP
        // ============================
        private void InitializeGameLayout()
        {
            gamePanel = new Panel { Dock = DockStyle.Fill };
            this.Controls.Add(gamePanel);

            mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 3,
                ColumnCount = 1
            };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30)); // status label
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 90));  // board
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50)); // buttons

            gamePanel.Controls.Add(mainLayout);

            // Status label
            statusLabel = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Consolas", 12),
                Text = "Time: 0s | Mistakes: 0/3"
            };
            mainLayout.Controls.Add(statusLabel, 0, 0);

            // Board layout
            boardLayout = new TableLayoutPanel
            {
                RowCount = GridSize,
                ColumnCount = GridSize,
                Dock = DockStyle.Fill
            };

            for (int i = 0; i < GridSize; i++)
            {
                boardLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f / GridSize));
                boardLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / GridSize));
            }

            mainLayout.Controls.Add(boardLayout, 0, 1);

            // Button layout
            buttonLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1
            };
            buttonLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            buttonLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

            Button resetButton = new Button { Text = "Reset", Dock = DockStyle.Fill };
            resetButton.Click += (s, e) => ReturnToMenu();

            Button pauseButton = new Button { Text = "Pause", Dock = DockStyle.Fill };
            pauseButton.Click += TogglePause;

            buttonLayout.Controls.Add(resetButton, 0, 0);
            buttonLayout.Controls.Add(pauseButton, 1, 0);

            mainLayout.Controls.Add(buttonLayout, 0, 2);
        }

        private void StartGame()
        {
            boardLayout.Controls.Clear();
            ElapsedSeconds = 0;
            Mistakes = 0;
            UpdateStatus();

            BoardGenerator generator = new BoardGenerator();
            solution = generator.GetBoard(GridSize);

            int totalCells = GridSize * GridSize;
            EmptyCellsCount = (missingPercentage * totalCells) / 100;

            int[,] puzzle = generator.CreatePuzzle(solution, EmptyCellsCount);

            for (int row = 0; row < GridSize; row++)
            {
                for (int col = 0; col < GridSize; col++)
                {
                    TextBox textBox = new TextBox
                    {
                        Dock = DockStyle.Fill,
                        TextAlign = HorizontalAlignment.Center,
                        Font = new Font("Consolas", 16),
                        MaxLength = 1,
                        Tag = new Point(row, col)
                    };

                    int value = puzzle[row, col];
                    if (value != 0)
                    {
                        textBox.Text = value.ToString();
                        textBox.Enabled = false;
                        textBox.BackColor = Color.LightGray;
                    }
                    else
                    {
                        textBox.KeyPress += OnKeyPressDigitOnly;
                        textBox.TextChanged += OnUserInputChanged;
                    }

                    textBoxes[row, col] = textBox;
                    boardLayout.Controls.Add(textBox, col, row);
                }
            }
        }

        private void OnKeyPressDigitOnly(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && (e.KeyChar < '1' || e.KeyChar > '9'))
            {
                e.Handled = true;
            }
        }

        private void OnUserInputChanged(object sender, EventArgs e)
        {
            if (isPaused) return;

            TextBox textBox = sender as TextBox;
            if (textBox == null || string.IsNullOrEmpty(textBox.Text))
            {
                textBox.BackColor = Color.White;
                return;
            }

            Point point = (Point)textBox.Tag;
            int row = point.X;
            int col = point.Y;
            int expected = solution[row, col];

            if (int.TryParse(textBox.Text, out int userValue))
            {
                if (userValue != expected)
                {
                    Mistakes++;
                    textBox.BackColor = Color.LightPink;
                }
                else
                {
                    textBox.BackColor = Color.White;
                    textBox.Enabled = false;
                    --EmptyCellsCount;
                }
            }

            UpdateStatus();

            if (Mistakes >= MaxMistakes)
            {
                LoseGame();
            }
            else if (EmptyCellsCount == 0)
            {
                WinGame();
            }
        }

        private void UpdateStatus()
        {
            statusLabel.Text = $"Time: {ElapsedSeconds}s | Mistakes: {Mistakes}/{MaxMistakes}";
        }

        private void StartTimer()
        {
            if (gameTimer != null)
            {
                gameTimer.Stop();
                gameTimer.Dispose();
            }

            gameTimer = new Timer();
            gameTimer.Interval = 1000;
            gameTimer.Tick += (s, e) =>
            {
                if (!isPaused)
                {
                    ElapsedSeconds++;
                    UpdateStatus();
                }
            };
            gameTimer.Start();
        }

        private void TogglePause(object sender, EventArgs e)
        {
            isPaused = !isPaused;
            Button pauseBtn = sender as Button;
            pauseBtn.Text = isPaused ? "Resume" : "Pause";

            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize; j++)
                {
                    if (!textBoxes[i, j].Enabled)
                    {
                        textBoxes[i, j].Text = isPaused ? null : solution[i, j].ToString();
                    }
                }
            }
        }

        private void LoseGame()
        {
            gameTimer.Stop();
            MessageBox.Show("Game Over! You've made 3 Mistakes.", "Sudoku", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            ReturnToMenu();
        }

        private void WinGame()
        {
            gameTimer.Stop();
            MessageBox.Show("Congratulations! You Win.", "Sudoku", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ReturnToMenu();
        }

        private void ReturnToMenu()
        {
            gameTimer?.Stop();
            this.Controls.Remove(gamePanel);
            menuPanel.Visible = true;
        }
    }
}
