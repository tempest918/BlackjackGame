using System.Collections.Generic;
using System.Linq;

namespace BlackjackLogic
{
    public class Player
    {
        public string Name { get; set; }
        public List<Card> Hand { get; set; }
        public int Money { get; set; }

        public Player(string name, int startingMoney = 100)
        {
            Name = name;
            Hand = new List<Card>();
            Money = startingMoney;
        }

        public void DrawCard(Deck deck)
        {
            Card drawnCard = deck.Draw();
            if (drawnCard != null)
            {
                Hand.Add(drawnCard);
            }
        }

        public int CalculateScore()
        {
            int score = Hand.Sum(card => card.Value);
            int aceCount = Hand.Count(card => card.Face == "A");

            // Adjust for Aces if score is over 21
            while (score > 21 && aceCount > 0)
            {
                score -= 10;
                aceCount--;
            }

            return score;
        }

        public void ClearHand()
        {
            Hand.Clear();
        }
    }
}