namespace BlackjackLogic
{
    public class Dealer : Player
    {
        public Dealer() : base("Dealer", 10000) // Give dealer effectively infinite money
        {
        }

        public bool ShouldHit()
        {
            return CalculateScore() < 17;
        }
    }
}
