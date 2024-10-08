using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;

namespace PongGame
{
    public class GamePaddle
    {
        public double X { get;  set; }
        public double Y { get;  set; }
        public double Speed { get; set; } = 5;
        public bool IsSpringing { get; set; }
        public double SpringDuration { get; set; } = 10;
        public double SpringPower { get; set; } = 1;
        public double SpringSpeedBoost { get; set; } = 2;
        public double CurrentSpring { get; set; }
        public Rectangle Rectangle { get; private set; }

        private Canvas GameCanvas;

        public GamePaddle(Rectangle rectangle, Canvas gameCanvas)
        {
            Rectangle = rectangle;
            GameCanvas = gameCanvas;
            X = Canvas.GetLeft(rectangle);
            Y = Canvas.GetTop(rectangle);
        }

        public void MoveUp()
        {
            Y = Math.Max(Y - Speed, 10);
            Canvas.SetTop(Rectangle, Y);
        }

        public void MoveDown()
        {
            Y = Math.Min(Y + Speed, GameCanvas.Height - Rectangle.Height - 10);
            Canvas.SetTop(Rectangle, Y);
        }

        public void AiMoveUp(double speed)
        {
            Y = Math.Max(Y - speed, 10);
            Canvas.SetTop(Rectangle, Y);
        }

        public void AiMoveDown(double speed)
        {
            Y = Math.Min(Y + speed, GameCanvas.Height - Rectangle.Height - 10);
            Canvas.SetTop(Rectangle, Y);
        }


        public void UpdateSpring()
        {
            if (IsSpringing)
            {
                if (CurrentSpring > 0)
                {
                    X += SpringPower;
                    Canvas.SetLeft(Rectangle, X);
                    CurrentSpring--;
                }
                else
                {
                    IsSpringing = false;
                    // Reset to original position
                    X = Rectangle.Name == "Paddle1" ? 32 : 758;
                    Canvas.SetLeft(Rectangle, X);
                }
            }
        }

        public void ActivateSpring()
        {
            if (!IsSpringing)
            {
                IsSpringing = true;
                CurrentSpring = SpringDuration;
            }
        }

        public void ResetPosition()
        {
            Y = (GameCanvas.Height / 2) - (Rectangle.Height / 2);
            Canvas.SetTop(Rectangle, Y);
        }
    }
}
