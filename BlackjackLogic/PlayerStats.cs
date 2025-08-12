using System.Collections.Generic;

namespace BlackjackLogic
{
    public class PlayerStats
    {
        public int PlayerMoney { get; set; } = 100;
        public List<GameRunStats> History { get; set; } = new List<GameRunStats>();
        public GameRunStats CurrentRun { get; set; } = new GameRunStats();

        public void ArchiveAndReset()
        {
            // Set final money and end time for the current run
            CurrentRun.FinalMoney = PlayerMoney;
            CurrentRun.EndTime = System.DateTime.UtcNow;

            // Add the completed run to history
            if (CurrentRun.HandsPlayed > 0)
            {
                History.Add(CurrentRun);
            }

            // Start a new run
            CurrentRun = new GameRunStats();

            // Reset player's money for the new run
            PlayerMoney = 100;
        }
    }
}
