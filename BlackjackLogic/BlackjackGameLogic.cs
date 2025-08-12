namespace BlackjackLogic
{
    public enum GameState
    {
        AwaitingInsurance,
        PlayerTurn,
        DealerTurn,
        HandOver
    }

    public enum HandResult
    {
        Win,
        Loss,
        Push, // Tie
        Blackjack,
        InsuranceWin,
        InsuranceLoss
    }

    public class HandResultInfo
    {
        public HandResult MainHandResult { get; set; }
        public HandResult? InsuranceResult { get; set; }
    }

    public class BlackjackGameLogic
    {
        public Player Player { get; set; }
        public Dealer Dealer { get; set; }
        public Deck Deck { get; set; }
        public GameState CurrentState { get; set; }
        public int CurrentBet { get; set; }
        public int InsuranceBet { get; private set; }

        public bool DealerShowsAce => Dealer.Hand.Count > 1 && Dealer.Hand[1].Face == "A";

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

            InsuranceBet = 0; // Reset insurance bet

            if (DealerShowsAce)
            {
                CurrentState = GameState.AwaitingInsurance;
            }
            else if (Player.CalculateScore() == 21)
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

        public void ResolveInsurance(bool accepted)
        {
            if (CurrentState != GameState.AwaitingInsurance) return;

            if (accepted)
            {
                int insuranceCost = CurrentBet / 2;
                if (Player.Money < insuranceCost)
                {
                    throw new InvalidOperationException("Not enough money for insurance.");
                }
                Player.Money -= insuranceCost;
                InsuranceBet = insuranceCost;
            }

            // After resolving insurance, if player has blackjack, hand is over. Otherwise, it's player's turn.
            CurrentState = Player.CalculateScore() == 21 ? GameState.HandOver : GameState.PlayerTurn;
        }

        public void PlayerDoublesDown()
        {
            if (CurrentState != GameState.PlayerTurn || Player.Hand.Count != 2)
            {
                // Or throw an exception
                return;
            }

            if (Player.Money < CurrentBet)
            {
                throw new InvalidOperationException("Not enough money to double down.");
            }

            Player.Money -= CurrentBet;
            CurrentBet *= 2;

            Player.DrawCard(Deck);

            // The hand is over immediately after doubling down
            CurrentState = GameState.HandOver;
        }

        public HandResultInfo DetermineHandResult()
        {
            var result = new HandResultInfo();

            if (CurrentState != GameState.HandOver)
            {
                result.MainHandResult = HandResult.Loss; // Should not happen
                return result;
            }

            int playerScore = Player.CalculateScore();
            int dealerScore = Dealer.CalculateScore();

            // First, resolve insurance bet if one was made
            if (InsuranceBet > 0)
            {
                if (dealerScore == 21)
                {
                    Player.Money += InsuranceBet * 3; // Insurance pays 2:1, player gets bet back + winnings
                    result.InsuranceResult = HandResult.InsuranceWin;
                }
                else
                {
                    result.InsuranceResult = HandResult.InsuranceLoss;
                }
            }

            // Then, resolve the main hand
            if (playerScore > 21)
            {
                result.MainHandResult = HandResult.Loss;
            }
            else if (playerScore == 21 && Player.Hand.Count == 2 && dealerScore != 21)
            {
                Player.Money += (int)(CurrentBet * 2.5); // Blackjack pays 3:2
                result.MainHandResult = HandResult.Blackjack;
            }
            else if (dealerScore > 21 || playerScore > dealerScore)
            {
                Player.Money += CurrentBet * 2; // Win pays 1:1
                result.MainHandResult = HandResult.Win;
            }
            else if (playerScore < dealerScore)
            {
                result.MainHandResult = HandResult.Loss;
            }
            else
            {
                Player.Money += CurrentBet; // Push
                result.MainHandResult = HandResult.Push;
            }

            return result;
        }
    }
}
