namespace BlackjackLogic
{
    public enum GameState
    {
        PlayerTurn,
        DealerTurn,
        HandOver
    }

    public enum HandResult
    {
        Win,
        Loss,
        Push, // Tie
        Blackjack
    }

    public class BlackjackGameLogic
    {
        public Player Player { get; private set; }
        public Dealer Dealer { get; private set; }
        public Deck Deck { get; private set; }
        public GameState CurrentState { get; private set; }
        public int CurrentBet { get; private set; }

        public BlackjackGameLogic()
        {
            Player = new Player("Player");
            Dealer = new Dealer();

            CurrentState = GameState.HandOver;
        }

        public void StartNewHand(int bet)
        {
            if (bet <= 0 || bet > Player.Money)
            {
                throw new ArgumentException("Invalid bet amount.");
            }

            CurrentBet = bet;
            Player.Money -= bet;

            Deck = new Deck();
            Deck.Shuffle();

            Player.ClearHand();
            Dealer.ClearHand();

            Player.DrawCard(Deck);
            Player.DrawCard(Deck);
            Dealer.DrawCard(Deck);
            Dealer.DrawCard(Deck);

            if (Player.CalculateScore() == 21)
            {
                CurrentState = GameState.HandOver;
            }
            else
            {
                CurrentState = GameState.PlayerTurn;
            }
        }

        public void PlayerHits()
        {
            if (CurrentState != GameState.PlayerTurn) return;

            Player.DrawCard(Deck);

            if (Player.CalculateScore() > 21)
            {
                CurrentState = GameState.HandOver;
            }
        }

        public void PlayerStays()
        {
            if (CurrentState != GameState.PlayerTurn) return;

            CurrentState = GameState.DealerTurn;
            while (Dealer.ShouldHit())
            {
                Dealer.DrawCard(Deck);
            }
            CurrentState = GameState.HandOver;
        }

        public HandResult DetermineHandResult()
        {
            if (CurrentState != GameState.HandOver)
            {
                // Or throw an exception, as this method should only be called at the end of a hand
                return HandResult.Loss;
            }

            int playerScore = Player.CalculateScore();
            int dealerScore = Dealer.CalculateScore();

            if (playerScore > 21)
            {
                return HandResult.Loss;
            }
            if (playerScore == 21 && Player.Hand.Count == 2)
            {
                Player.Money += (int)(CurrentBet * 2.5); // Blackjack pays 3:2, so player gets bet + 1.5*bet
                return HandResult.Blackjack;
            }
            if (dealerScore > 21 || playerScore > dealerScore)
            {
                Player.Money += CurrentBet * 2; // Win pays 1:1, player gets bet back + winnings
                return HandResult.Win;
            }
            if (playerScore < dealerScore)
            {
                return HandResult.Loss;
            }

            Player.Money += CurrentBet; // Push, player gets bet back
            return HandResult.Push;
        }
    }
}
