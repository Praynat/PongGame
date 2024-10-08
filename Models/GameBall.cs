using System;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows;

namespace PongGame
{
    public class GameBall
    {
        public double X { get; private set; }
        public double Y { get; private set; }
        public double SpeedX { get; set; } = 3;
        public double SpeedY { get; set; } = 3;
        public double Width { get; private set; }
        public double Height { get; private set; }
        public Ellipse Ellipse { get; private set; }

        private Canvas GameCanvas;

        public GameBall(Ellipse ellipse, Canvas gameCanvas)
        {
            Ellipse = ellipse;
            GameCanvas = gameCanvas;
            X = Canvas.GetLeft(ellipse);
            Y = Canvas.GetTop(ellipse);
            Width = ellipse.Width;
            Height = ellipse.Height;
        }

        public void Move()
        {
            X += SpeedX;
            Y += SpeedY;
            Canvas.SetLeft(Ellipse, X);
            Canvas.SetTop(Ellipse, Y);
        }

        public void ResetPosition()
        {
            X = (GameCanvas.Width / 2) - (Width / 2);
            Y = (GameCanvas.Height / 2) - (Height / 2);
            Canvas.SetLeft(Ellipse, X);
            Canvas.SetTop(Ellipse, Y);

            Random rnd = new Random();
            SpeedX = rnd.Next(0, 2) == 0 ? 3 : -3;
            SpeedY = rnd.Next(0, 2) == 0 ? 3 : -3;
        }

        public void InvertSpeedX()
        {
            SpeedX = -SpeedX;
        }

        public void InvertSpeedY()
        {
            SpeedY = -SpeedY;
        }

        public void SetSpeedLimit(double maxSpeed)
        {
            SpeedX = Math.Min(Math.Abs(SpeedX), maxSpeed) * Math.Sign(SpeedX);
            SpeedY = Math.Min(Math.Abs(SpeedY), maxSpeed) * Math.Sign(SpeedY);
        }
    }
}
