namespace BlackjackLogic
{
    public class Dealer : Player
    {
        public Dealer() : base("Dealer", 10000) // Give dealer effectively infinite money
        {
        }

        public bool ShouldHit()
        {
            // Dealer only ever has one hand at index 0
            return CalculateScore(0) < 17;
        }
    }
}
