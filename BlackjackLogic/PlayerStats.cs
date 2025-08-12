namespace BlackjackLogic
{
    public class PlayerStats
    {
        public int PlayerMoney { get; set; } = 100;
        public int HandsPlayed { get; set; } = 0;
        public int Wins { get; set; } = 0;
        public int Losses { get; set; } = 0;
        public int Pushes { get; set; } = 0;
        public int Blackjacks { get; set; } = 0;
        public int LargestPotWon { get; set; } = 0;

        public void Reset()
        {
            PlayerMoney = 100;
            HandsPlayed = 0;
            Wins = 0;
            Losses = 0;
            Pushes = 0;
            Blackjacks = 0;
            LargestPotWon = 0;
        }
    }
}
