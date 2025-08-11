namespace BlackjackLogic
{
    // Card class to represent a single card
    public class Card
    {
        public string Suit { get; set; }
        public string Face { get; set; }
        public int Value { get; set; }

        public Card(string suit, string face, int value)
        {
            Suit = suit;
            Face = face;
            Value = value;
        }

        public string GetSuitSymbol()
        {
            return Suit switch
            {
                "Hearts" => "♥",
                "Diamonds" => "♦",
                "Clubs" => "♣",
                "Spades" => "♠",
                _ => ""
            };
        }
    }
}
