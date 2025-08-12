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
        public List<int> Bets { get; set; }
        public int CurrentBet => Player.ActiveHandIndex < Bets.Count ? Bets[Player.ActiveHandIndex] : 0;
        public int InsuranceBet { get; private set; }

        public bool DealerShowsAce => Dealer.Hands.Count > 0 && Dealer.Hands[0].Count > 1 && Dealer.Hands[0][1].Face == "A";

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

            Bets = new List<int> { bet };
            Player.Money -= bet;

            Deck = new Deck();
            Deck.Shuffle();

            Player.ClearHands();
            Dealer.ClearHands();

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

            // If player busts and there's another hand to play, move to it
            if (Player.CalculateScore() > 21)
            {
                if (Player.ActiveHandIndex < Player.Hands.Count - 1)
                {
                    Player.ActiveHandIndex++;
                }
                else
                {
                    // All hands played, move to dealer's turn
                    CurrentState = GameState.DealerTurn;
                    while (Dealer.ShouldHit())
                    {
                        Dealer.DrawCard(Deck);
                    }
                    CurrentState = GameState.HandOver;
                }
            }
        }

        public void PlayerStays()
        {
            if (CurrentState != GameState.PlayerTurn) return;

            // Check if there are more hands to play
            if (Player.ActiveHandIndex < Player.Hands.Count - 1)
            {
                Player.ActiveHandIndex++;
            }
            else
            {
                // Last hand has been played, move to dealer's turn
                CurrentState = GameState.DealerTurn;
                while (Dealer.ShouldHit())
                {
                    Dealer.DrawCard(Deck);
                }
                CurrentState = GameState.HandOver;
            }
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

        public void PlayerSplits()
        {
            if (CurrentState != GameState.PlayerTurn || Player.CurrentHand.Count != 2 || Player.CurrentHand[0].Value != Player.CurrentHand[1].Value)
            {
                // Can only split a pair on the first turn of a hand
                return;
            }

            if (Player.Money < CurrentBet)
            {
                throw new InvalidOperationException("Not enough money to split.");
            }

            // Move the second card into a new hand
            var secondCard = Player.CurrentHand[1];
            Player.CurrentHand.RemoveAt(1);
            var newHand = new List<Card> { secondCard };
            Player.Hands.Add(newHand);

            // Add a bet for the new hand
            Player.Money -= CurrentBet;
            Bets.Add(CurrentBet);

            // Draw a new card for both hands
            Player.DrawCard(Deck); // Draws for the original hand (now active)
            Player.ActiveHandIndex++; // Move to the new hand
            Player.DrawCard(Deck); // Draws for the new hand
            Player.ActiveHandIndex = 0; // Return focus to the first hand
        }

        public void PlayerDoublesDown()
        {
            if (CurrentState != GameState.PlayerTurn || Player.CurrentHand.Count != 2)
            {
                // Or throw an exception
                return;
            }

            if (Player.Money < CurrentBet)
            {
                throw new InvalidOperationException("Not enough money to double down.");
            }

            Player.Money -= CurrentBet;
            Bets[Player.ActiveHandIndex] *= 2;

            Player.DrawCard(Deck);

            // After doubling, the turn for this hand is over. Move to the next hand or dealer's turn.
            PlayerStays();
        }

        public List<HandResultInfo> DetermineHandResult()
        {
            var results = new List<HandResultInfo>();
            if (CurrentState != GameState.HandOver)
            {
                // This case should ideally not be reached
                results.Add(new HandResultInfo { MainHandResult = HandResult.Loss });
                return results;
            }

            int dealerScore = Dealer.CalculateScore(0);
            HandResult? insuranceOutcome = null;

            // First, resolve insurance bet if one was made
            if (InsuranceBet > 0)
            {
                if (dealerScore == 21)
                {
                    Player.Money += InsuranceBet * 3; // Insurance pays 2:1, player gets bet back + winnings
                    insuranceOutcome = HandResult.InsuranceWin;
                }
                else
                {
                    insuranceOutcome = HandResult.InsuranceLoss;
                }
            }

            // Then, resolve each hand
            for (int i = 0; i < Player.Hands.Count; i++)
            {
                var handResult = new HandResultInfo();
                int playerScore = Player.CalculateScore(i);
                int currentHandBet = Bets[i];

                if (playerScore > 21)
                {
                    handResult.MainHandResult = HandResult.Loss;
                }
                else if (playerScore == 21 && Player.Hands[i].Count == 2 && dealerScore != 21)
                {
                    Player.Money += (int)(currentHandBet * 2.5); // Blackjack pays 3:2
                    handResult.MainHandResult = HandResult.Blackjack;
                }
                else if (dealerScore > 21 || playerScore > dealerScore)
                {
                    Player.Money += currentHandBet * 2; // Win pays 1:1
                    handResult.MainHandResult = HandResult.Win;
                }
                else if (playerScore < dealerScore)
                {
                    handResult.MainHandResult = HandResult.Loss;
                }
                else
                {
                    Player.Money += currentHandBet; // Push
                    handResult.MainHandResult = HandResult.Push;
                }
                results.Add(handResult);
            }

            // Assign insurance result to the first hand's info for display purposes
            if (results.Count > 0 && insuranceOutcome.HasValue)
            {
                results[0].InsuranceResult = insuranceOutcome;
            }

            return results;
        }
    }
}
