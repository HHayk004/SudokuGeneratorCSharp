﻿using System;
using System.Drawing;
using System.Windows.Forms;

namespace Sudoku
{
    public partial class Form1 : Form
    {
        private const int GridSize = 9;
        private const int MaxMistakes = 3;

        private Button[,] buttons = new Button[GridSize, GridSize];
        private int[,] solution;
        private int[,] puzzle;
        private int EmptyCellsCount;
        private int ElapsedSeconds = 0;
        private int Mistakes = 0;
        private Timer gameTimer;
        private bool isPaused = false;
        private int selectedDigit = 1;
        private Button selectedButton = null;

        private Label statusLabel;
        private TableLayoutPanel mainLayout;
        private TableLayoutPanel boardLayout;
        private TableLayoutPanel buttonLayout;
        private TableLayoutPanel digitSelectorLayout;

        private Panel gamePanel;
        private Panel menuPanel;
        private int missingPercentage = 20;

        public Form1()
        {
            InitializeComponent();
            InitializeMenu();
        }

        private void InitializeMenu()
        {
            this.Text = "Sudoku";
            this.ClientSize = new Size(600, 850);
            this.StartPosition = FormStartPosition.CenterScreen;

            menuPanel = new Panel { Dock = DockStyle.Fill };

            TableLayoutPanel layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 3,
                ColumnCount = 1
            };
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 30));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 70));

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

        private void InitializeGameLayout()
        {
            gamePanel = new Panel { Dock = DockStyle.Fill };
            this.Controls.Add(gamePanel);

            mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 4,
                ColumnCount = 1
            };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 600));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 100));

            gamePanel.Controls.Add(mainLayout);

            statusLabel = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Consolas", 12),
                Text = "Time: 0s | Mistakes: 0/3"
            };
            mainLayout.Controls.Add(statusLabel, 0, 0);

            boardLayout = new TableLayoutPanel
            {
                RowCount = GridSize,
                ColumnCount = GridSize,
                Dock = DockStyle.Fill
            };
            for (int i = 0; i < GridSize; ++i)
            {
                boardLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f / GridSize));
                boardLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / GridSize));
            }
            mainLayout.Controls.Add(boardLayout, 0, 1);

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

            // Create digit selector layout (3x3 grid for digits 1-9)
            digitSelectorLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,  // 3 columns for digits 1-9
                RowCount = 3,     // 3 rows for the buttons to fit nicely
            };

            // Ensure equal size distribution
            for (int i = 0; i < 3; i++)
            {
                digitSelectorLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f)); // Even distribution across columns
                digitSelectorLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33f)); // Even distribution across rows
            }

            // Create digit buttons (1 to 9)
            for (int i = 1; i <= 9; i++)
            {
                Button digitBtn = new Button
                {
                    Text = i.ToString(),
                    Dock = DockStyle.Fill,
                    Tag = i,
                    BackColor = Color.White,
                    Font = new Font("Arial", 16, FontStyle.Bold) // Adjust font size for better fit
                };

                // Attach click event handler to set selected digit
                digitBtn.Click += (s, e) =>
                {
                    selectedDigit = (int)((Button)s).Tag;
                    HighlightSelectedDigit();
                    TryPlaceDigit();
                };

                // Place buttons in 3x3 grid (row-major order)
                digitSelectorLayout.Controls.Add(digitBtn, (i - 1) % 3, (i - 1) / 3); // Add buttons to grid
            }

            // Add the digit selector layout to the main layout
            mainLayout.Controls.Add(digitSelectorLayout, 0, 3);  // Position this in the fourth row of mainLayout
        }

        private void HighlightSelectedDigit()
        {
            foreach (Control ctrl in digitSelectorLayout.Controls)
            {
                if (ctrl is Button btn)
                {
                    int val = (int)btn.Tag;
                    btn.BackColor = (val == selectedDigit) ? Color.LightBlue : Color.White;
                }
            }
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

            puzzle = generator.CreatePuzzle(solution, EmptyCellsCount);

            for (int row = 0; row < GridSize; row++)
            {
                for (int col = 0; col < GridSize; col++)
                {
                    Button btn = new Button
                    {
                        Dock = DockStyle.Fill,
                        Font = new Font("Consolas", 16),
                        Tag = new Point(row, col),
                        BackColor = Color.White
                    };

                    int value = puzzle[row, col];
                    if (value != 0)
                    {
                        btn.Text = value.ToString();
                        btn.Enabled = false;
                        btn.BackColor = Color.LightGray;
                    }
                    else
                    {
                        btn.Click += OnGridButtonClick;
                        btn.Text = "";
                    }

                    buttons[row, col] = btn;
                    boardLayout.Controls.Add(btn, col, row);
                }
            }
        }

        private void OnGridButtonClick(object sender, EventArgs e)
        {
            if (isPaused) return;

            Button btn = sender as Button;
            selectedButton = btn;

            // Highlight selected cell
            foreach (Button b in buttons)
                if (b.Enabled) b.BackColor = Color.White;

            btn.BackColor = Color.LightBlue;
        }

        private void TryPlaceDigit()
        {
            if (selectedButton == null) return;

            Point point = (Point)selectedButton.Tag;
            int row = point.X;
            int col = point.Y;

            if (selectedButton.Text == selectedDigit.ToString()) return;

            if (solution[row, col] == selectedDigit)
            {
                selectedButton.Text = selectedDigit.ToString();
                selectedButton.Enabled = false;
                selectedButton.BackColor = Color.White;
                selectedButton = null;
                EmptyCellsCount--;

                if (EmptyCellsCount == 0)
                    WinGame();
            }
            else
            {
                Mistakes++;
                selectedButton.BackColor = Color.LightPink;
                UpdateStatus();

                if (Mistakes >= MaxMistakes)
                    LoseGame();
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

        // Allow keyboard input for digits
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData >= Keys.D1 && keyData <= Keys.D9)
            {
                selectedDigit = keyData - Keys.D0;
                HighlightSelectedDigit();
                TryPlaceDigit();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
