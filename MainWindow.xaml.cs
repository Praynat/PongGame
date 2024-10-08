using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PongGame
{
    public partial class MainWindow : Window
    {
        private Game game;

        public MainWindow()
        {
            InitializeComponent();

            // Initialize paddles and ball
            GamePaddle paddle1 = new GamePaddle(this.Paddle1, GameCanvas);
            GamePaddle paddle2 = new GamePaddle(this.Paddle2, GameCanvas);
            GameBall gameBall = new GameBall(this.Ball, GameCanvas);

            // Initialize game
            game = new Game(GameCanvas, paddle1, paddle2, gameBall, ScoreText1, ScoreText2,
                            LeftWallTop, LeftWallBottom, RightWallTop, RightWallBottom);

            // Attach event handlers after 'game' is initialized
            GameModeComboBox.SelectionChanged += GameModeComboBox_SelectionChanged;
            DifficultyComboBox.SelectionChanged += DifficultyComboBox_SelectionChanged;

            CompositionTarget.Rendering += GameLoop;
        }

        private void GameLoop(object sender, EventArgs e)
        {
            game.GameLoop();
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            // Hide the main menu and start the game
            MainMenuGrid.Visibility = Visibility.Collapsed;
            PauseMenuGrid.Visibility = Visibility.Collapsed;
            PauseButton.Visibility = Visibility.Visible;
            game.StartGame();
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            // Toggle the pause state
            game.PauseGame();

            if (game.IsPaused)
            {
                PauseMenuGrid.Visibility = Visibility.Visible;
                PauseButton.Visibility = Visibility.Collapsed;
                DifficultyComboBox.IsEnabled = false;
            }
            else
            {
                PauseMenuGrid.Visibility = Visibility.Collapsed;
                PauseButton.Visibility = Visibility.Visible;
                DifficultyComboBox.IsEnabled = true;
            }
        }

        private void ResetBallButton_Click(object sender, RoutedEventArgs e)
        {
            game.ResetBall(); // Reset the ball and paddles
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            // Exit to the main menu
            game.IsPaused = false; // Ensure the game is unpaused
            PauseMenuGrid.Visibility = Visibility.Collapsed; // Hide the pause menu
            MainMenuGrid.Visibility = Visibility.Visible; // Show the main menu
            PauseButton.Visibility = Visibility.Collapsed; // Hide the pause button

            // Reinitialize the score
            game.ScorePlayer1 = 0;
            game.ScorePlayer2 = 0;

            // Update the score display in the UI
            ScoreText1.Text = "0";
            ScoreText2.Text = "0";

            // Reset the ball and paddle positions
            game.ResetBall(); // Reset the ball position to the center

            // Update DifficultyPanel visibility based on the game mode
            DifficultyComboBox.IsEnabled = true;


            // Force focus on the DifficultyComboBox for interaction
            DifficultyComboBox.Focus();
        }


    

    // Gérer les événements clavier pour les mouvements des paddles
    protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            game.OnKeyDown(e.Key);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            game.OnKeyUp(e.Key);
        }

        private void GameModeComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (game == null)
                return;

            var selectedItem = GameModeComboBox.SelectedItem as System.Windows.Controls.ComboBoxItem;

            if (selectedItem != null)
            {
                string selectedContent = selectedItem.Content.ToString();
                if (selectedContent == "Two Players")
                {
                    game.IsTwoPlayer = true;
                    DifficultyPanel.Visibility = Visibility.Collapsed;
                }
                else if (selectedContent == "One Player")
                {
                    game.IsTwoPlayer = false;
                    DifficultyPanel.Visibility = Visibility.Visible;
                }
            }
        }

        private void DifficultyComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (game == null)
                return;

            var selectedItem = DifficultyComboBox.SelectedItem as System.Windows.Controls.ComboBoxItem;

            if (selectedItem != null)
            {
                switch (selectedItem.Content.ToString())
                {
                    case "Easy":
                        game.DifficultyLevel = 1;
                        break;
                    case "Medium":
                        game.DifficultyLevel = 2;
                        break;
                    case "Hard":
                        game.DifficultyLevel = 3;
                        break;
                }
            }
        }

    
    }
}
