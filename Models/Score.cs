using System.Windows.Controls;

namespace PongGame.Models
{
    public class Score
    {
        public int Player1 { get; private set; }
        public int Player2 { get; private set; }

        private TextBlock player1Text;
        private TextBlock player2Text;

        public Score(TextBlock p1Text, TextBlock p2Text)
        {
            player1Text = p1Text;
            player2Text = p2Text;
            Player1 = 0;
            Player2 = 0;
            UpdateUI();
        }

        public void Player1Scored()
        {
            Player1++;
            UpdateUI();
        }

        public void Player2Scored()
        {
            Player2++;
            UpdateUI();
        }

        private void UpdateUI()
        {
            player1Text.Text = Player1.ToString();
            player2Text.Text = Player2.ToString();
        }

        public void Reset()
        {
            Player1 = 0;
            Player2 = 0;
            UpdateUI();
        }
    }
}
