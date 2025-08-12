namespace BlackjackLogic
{
    public class GameRunStats
    {
        public DateTime EndTime { get; set; }
        public int HandsPlayed { get; set; } = 0;
        public int Wins { get; set; } = 0;
        public int Losses { get; set; } = 0;
        public int Pushes { get; set; } = 0;
        public int Blackjacks { get; set; } = 0;
        public int LargestPotWon { get; set; } = 0;
        public int FinalMoney { get; set; } = 0;
    }
}
