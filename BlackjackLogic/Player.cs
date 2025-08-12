using System.Collections.Generic;
using System.Linq;

namespace BlackjackLogic
{
    public class Player
    {
        public string Name { get; set; }
        public List<List<Card>> Hands { get; set; }

        public int Money { get; set; }
        public int ActiveHandIndex { get; set; }

        public List<Card> CurrentHand => Hands.Count > ActiveHandIndex ? Hands[ActiveHandIndex] : null;

        public Player() { }

        public Player(string name, int startingMoney = 100)
        {
            Name = name;
            Hands = new List<List<Card>> { new List<Card>() };
            Money = startingMoney;
            ActiveHandIndex = 0;
        }

        public void DrawCard(Deck deck)
        {
            Card drawnCard = deck.Draw();
            if (drawnCard != null && CurrentHand != null)
            {
                CurrentHand.Add(drawnCard);
            }
        }

        public int CalculateScore(int handIndex = -1)
        {
            if (handIndex == -1) handIndex = ActiveHandIndex;
            if (Hands.Count <= handIndex) return 0;

            var hand = Hands[handIndex];
            int score = hand.Sum(card => card.Value);
            int aceCount = hand.Count(card => card.Face == "A");

            // Adjust for Aces if score is over 21
            while (score > 21 && aceCount > 0)
            {
                score -= 10;
                aceCount--;
            }

            return score;
        }

        public void ClearHands()
        {
            Hands = new List<List<Card>> { new List<Card>() };
            ActiveHandIndex = 0;
        }
    }
}