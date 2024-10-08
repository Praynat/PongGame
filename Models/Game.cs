using System;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Input;

namespace PongGame
{
    public class Game
    {
        public bool IsGameRunning { get;  set; }
        public bool IsPaused { get;  set; }
        public bool IsTwoPlayer { get; set; }
        public int DifficultyLevel { get; set; } = 1;
        public int ScorePlayer1 { get;  set; }
        public int ScorePlayer2 { get;  set; }
        public GamePaddle Paddle1 { get;  set; }
        public GamePaddle Paddle2 { get;  set; }
        public GameBall Ball { get;  set; }

        private Random rnd = new Random();

        private Canvas GameCanvas;
        private TextBlock ScoreText1;
        private TextBlock ScoreText2;
        private Rectangle LeftWallTop;
        private Rectangle LeftWallBottom;
        private Rectangle RightWallTop;
        private Rectangle RightWallBottom;

        private bool upPressed = false;
        private bool downPressed = false;
        private bool wPressed = false;
        private bool sPressed = false;

        private double maxBallSpeed = 100.0;

        public Game(Canvas gameCanvas, GamePaddle paddle1, GamePaddle paddle2, GameBall ball,
                    TextBlock scoreText1, TextBlock scoreText2,
                    Rectangle leftWallTop, Rectangle leftWallBottom,
                    Rectangle rightWallTop, Rectangle rightWallBottom)
        {
            GameCanvas = gameCanvas;
            Paddle1 = paddle1;
            Paddle2 = paddle2;
            Ball = ball;
            ScoreText1 = scoreText1;
            ScoreText2 = scoreText2;
            LeftWallTop = leftWallTop;
            LeftWallBottom = leftWallBottom;
            RightWallTop = rightWallTop;
            RightWallBottom = rightWallBottom;
        }

        public void StartGame()
        {
            IsGameRunning = true;
            IsPaused = false;
            ResetBall();
        }

        public void PauseGame()
        {
            IsPaused = !IsPaused;
        }

        public void ResetBall()
        {
            Ball.ResetPosition();
            Paddle1.ResetPosition();
            Paddle2.ResetPosition();
        }

        public void GameLoop()
        {
            if (IsGameRunning && !IsPaused)
            {
                MovePaddles();
                Ball.Move();
                CheckCollisions();
            }

            Paddle1.UpdateSpring();
            Paddle2.UpdateSpring();
        }

        private void MovePaddles()
        {
            // AI controls left paddle (Player 1)
            if (!IsTwoPlayer)
            {
                MoveAIPaddle();
            }
            else
            {
                // Player 1's paddle (controlled by player in two-player mode)
                if (wPressed)
                    Paddle1.MoveUp();
                if (sPressed)
                    Paddle1.MoveDown();
            }

            // Player 2's paddle (always controlled by the player)
            if (upPressed)
                Paddle2.MoveUp();
            if (downPressed)
                Paddle2.MoveDown();
        }

        private void MoveAIPaddle()
        {
            // Contrôle du paddle de l'IA (Player 1) sur la gauche
            double paddleCenterY = Canvas.GetTop(Paddle1.Rectangle) + (Paddle1.Rectangle.Height / 2);
            double ballCenterY = Canvas.GetTop(Ball.Ellipse) + (Ball.Ellipse.Height / 2);

            // Ajuster la vitesse du paddle en fonction de la difficulté
            double aiPaddleSpeed = Paddle1.Speed;

            switch (DifficultyLevel)
            {
                case 1:
                    aiPaddleSpeed *= 2.0;
                    break;
                case 2:
                    aiPaddleSpeed *= 3.5;
                    break;
                case 3:
                    aiPaddleSpeed *= 5.0;
                    break;
            }

            // Réduire la marge d'erreur pour un meilleur suivi
            int errorMargin = DifficultyLevel == 1 ? 10 : DifficultyLevel == 2 ? 5 : 3;
            ballCenterY += rnd.Next(-errorMargin, errorMargin); // Introduire une légère imprécision pour simuler un comportement humain

            // Calculer la différence entre la position de la balle et celle du paddle
            double difference = ballCenterY - paddleCenterY;

            // Appliquer un facteur de lissage pour un mouvement plus fluide
            double smoothingFactor = 0.15; // Valeur ajustable pour affiner le mouvement

            if (Math.Abs(difference) > 2) // Déplacer le paddle seulement lorsque c'est nécessaire
            {
                double newY = Canvas.GetTop(Paddle1.Rectangle) + (aiPaddleSpeed * Math.Sign(difference) * smoothingFactor);

                // S'assurer que le paddle ne dépasse pas les limites du canvas
                newY = Math.Max(0, Math.Min(newY, GameCanvas.Height - Paddle1.Rectangle.Height));

                // Mettre à jour la position du paddle sur le canvas
                Canvas.SetTop(Paddle1.Rectangle, newY);
            }

            // Corriger les chevauchements potentiels entre la balle et le paddle
            CorrectBallPaddleOverlap();
        }

        private void CorrectBallPaddleOverlap()
        {
            // Get the ball's current position
            double ballLeft = Canvas.GetLeft(Ball.Ellipse);
            double ballRight = ballLeft + Ball.Ellipse.Width;
            double ballTop = Canvas.GetTop(Ball.Ellipse);
            double ballBottom = ballTop + Ball.Ellipse.Height;

            // Predict the ball's next position based on its speed
            double nextBallLeft = ballLeft + Ball.SpeedX;
            double nextBallRight = nextBallLeft + Ball.Ellipse.Width;
            double nextBallTop = ballTop + Ball.SpeedY;
            double nextBallBottom = nextBallTop + Ball.Ellipse.Height;

            // Get the paddle's current position
            double paddleLeft = Canvas.GetLeft(Paddle1.Rectangle);
            double paddleRight = paddleLeft + Paddle1.Rectangle.Width;
            double paddleTop = Canvas.GetTop(Paddle1.Rectangle);
            double paddleBottom = paddleTop + Paddle1.Rectangle.Height;

            // Check if the ball's next position intersects with the paddle
            bool isBallGoingToIntersect = nextBallRight >= paddleLeft && nextBallLeft <= paddleRight &&
                                          nextBallBottom >= paddleTop && nextBallTop <= paddleBottom;

            // If the ball is about to pass through the paddle, invert its horizontal speed
            if (isBallGoingToIntersect)
            {
                Ball.InvertSpeedX();
            }
        }




        private void CheckCollisions()
        {
            SetMaxBallSpeed();

            // Ball collision with top and bottom walls
            if (Ball.Y <= 0 || (Ball.Y + Ball.Height) >= GameCanvas.Height)
            {
                Ball.InvertSpeedY();
            }

            // Ball collision with paddles
            if (Ball.X <= Paddle1.X + Paddle1.Rectangle.Width &&
                Ball.Y + Ball.Height >= Paddle1.Y &&
                Ball.Y <= Paddle1.Y + Paddle1.Rectangle.Height)
            {
                Ball.SpeedX = Math.Abs(Ball.SpeedX);

                double hitPosition = (Ball.Y + Ball.Height / 2) - (Paddle1.Y + Paddle1.Rectangle.Height / 2);
                double maxAngle = 2;
                Ball.SpeedY += hitPosition / (Paddle1.Rectangle.Height / 2) * maxAngle;

                if (Paddle1.IsSpringing)
                {
                    Ball.SpeedX *= Paddle1.SpringSpeedBoost;
                    Ball.SpeedY *= Paddle1.SpringSpeedBoost;
                }
            }

            if (Ball.X + Ball.Width >= Paddle2.X &&
                Ball.Y + Ball.Height >= Paddle2.Y &&
                Ball.Y <= Paddle2.Y + Paddle2.Rectangle.Height)
            {
                Ball.SpeedX = -Math.Abs(Ball.SpeedX);

                double hitPosition = (Ball.Y + Ball.Height / 2) - (Paddle2.Y + Paddle2.Rectangle.Height / 2);
                double maxAngle = 2;
                Ball.SpeedY += hitPosition / (Paddle2.Rectangle.Height / 2) * maxAngle;

                if (Paddle2.IsSpringing)
                {
                    Ball.SpeedX *= Paddle2.SpringSpeedBoost;
                    Ball.SpeedY *= Paddle2.SpringSpeedBoost;
                }
            }

            // Ball collision with left and right walls (scoring)
            if (Ball.X <= 0)
            {
                if (Ball.Y + Ball.Height >= Canvas.GetTop(LeftWallTop) + LeftWallTop.Height &&
                    Ball.Y <= Canvas.GetTop(LeftWallBottom))
                {
                    ScorePlayer2++;
                    ScoreText2.Text = ScorePlayer2.ToString();
                    ResetBall();
                }
                else
                {
                    Ball.InvertSpeedX();
                }
            }

            if (Ball.X + Ball.Width >= GameCanvas.Width)
            {
                if (Ball.Y + Ball.Height >= Canvas.GetTop(RightWallTop) + RightWallTop.Height &&
                    Ball.Y <= Canvas.GetTop(RightWallBottom))
                {
                    ScorePlayer1++;
                    ScoreText1.Text = ScorePlayer1.ToString();
                    ResetBall();
                }
                else
                {
                    Ball.InvertSpeedX();
                }
            }

            Ball.SetSpeedLimit(maxBallSpeed);
        }

        private void SetMaxBallSpeed()
        {
            switch (DifficultyLevel)
            {
                case 1:
                    maxBallSpeed = 20;
                    break;
                case 2:
                    maxBallSpeed = 40;
                    break;
                case 3:
                    maxBallSpeed = 80;
                    break;
                default:
                    maxBallSpeed = 100;
                    break;
            }
        }

        // Methods to handle key presses
        public void OnKeyDown(Key key)
        {
            if (key == Key.Up)
                upPressed = true;
            if (key == Key.Down)
                downPressed = true;
            if (IsTwoPlayer)
            {
                if (key == Key.W)
                    wPressed = true;
                if (key == Key.S)
                    sPressed = true;
                if (key == Key.LeftShift)
                {
                    Paddle1.ActivateSpring();
                }
            }
            if (key == Key.Space)
            {
                Paddle2.ActivateSpring();
            }
        }

        public void OnKeyUp(Key key)
        {
            if (key == Key.Up)
                upPressed = false;
            if (key == Key.Down)
                downPressed = false;
            if (key == Key.W)
                wPressed = false;
            if (key == Key.S)
                sPressed = false;
        }
    }
}
