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
        public PlayerStats Stats { get; private set; }

        public bool DealerShowsAce => Dealer.Hands.Count > 0 && Dealer.Hands[0].Count > 1 && Dealer.Hands[0][1].Face == "A";


        public BlackjackGameLogic()
        {
            Stats = PersistenceService.LoadStats();
            Player = new Player("Player", Stats.PlayerMoney);
            Dealer = new Dealer();

            CurrentState = GameState.HandOver;
        }

        public void Reset()
        {
            Stats = PersistenceService.LoadStats();
            Player = new Player("Player", Stats.PlayerMoney);
            Dealer = new Dealer();

            CurrentState = GameState.HandOver;
        }

        public void StartNewHand(int bet, int numberOfDecks)
        {
            if (bet <= 0 || bet > Player.Money)
            {
                throw new ArgumentException("Invalid bet amount.");
            }

            Bets = new List<int> { bet };
            Player.Money -= bet;

            Deck = new Deck(numberOfDecks);
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
            }
        }

        public void DealerHits()
        {
            if (CurrentState != GameState.DealerTurn) return;

            if (Dealer.ShouldHit())
            {
                Dealer.DrawCard(Deck);
            }

            if (!Dealer.ShouldHit())
            {
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

            var currentBet = Bets[Player.ActiveHandIndex];
            if (Player.Money < currentBet)
            {
                throw new InvalidOperationException("Not enough money to split.");
            }

            // Move the second card into a new hand, inserting it after the current hand
            var handToSplit = Player.CurrentHand;
            var secondCard = handToSplit[1];
            handToSplit.RemoveAt(1);

            var newHand = new List<Card> { secondCard };
            Player.Hands.Insert(Player.ActiveHandIndex + 1, newHand);

            // Add a bet for the new hand, also inserting it
            Player.Money -= currentBet;
            Bets.Insert(Player.ActiveHandIndex + 1, currentBet);

            // Draw a new card for both the original hand and the new hand
            handToSplit.Add(Deck.Draw());
            newHand.Add(Deck.Draw());

            // ActiveHandIndex remains on the first split hand, player plays it out.
            // PlayerStays or a bust will naturally move to the next hand.
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
            bool dealerHasBlackjack = Dealer.Hands.Count > 0 && Dealer.Hands[0].Count == 2 && dealerScore == 21;

            for (int i = 0; i < Player.Hands.Count; i++)
            {
                var handResult = new HandResultInfo();
                int playerScore = Player.CalculateScore(i);
                int currentHandBet = Bets[i];
                bool playerHasBlackjack = Player.Hands[i].Count == 2 && playerScore == 21;

                if (playerScore > 21)
                {
                    handResult.MainHandResult = HandResult.Loss;
                }
                else if (playerHasBlackjack && !dealerHasBlackjack)
                {
                    Player.Money += (int)(currentHandBet * 2.5); // Blackjack pays 3:2
                    handResult.MainHandResult = HandResult.Blackjack;
                }
                else if (dealerHasBlackjack && !playerHasBlackjack)
                {
                    handResult.MainHandResult = HandResult.Loss; // Dealer's Blackjack wins
                }
                else if (dealerScore > 21)
                {
                    Player.Money += currentHandBet * 2; // Win pays 1:1
                    handResult.MainHandResult = HandResult.Win;
                }
                else if (playerScore > dealerScore)
                {
                    Player.Money += currentHandBet * 2; // Win pays 1:1
                    handResult.MainHandResult = HandResult.Win;
                }
                else if (playerScore < dealerScore)
                {
                    handResult.MainHandResult = HandResult.Loss;
                }
                else // Scores are equal
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

        public void UpdateStatsAndSave(List<HandResultInfo> results)
        {
            Stats.CurrentRun.HandsPlayed += results.Count;
            int totalWinnings = 0;

            foreach (var result in results)
            {
                int betAmount = Bets[results.IndexOf(result)];
                switch (result.MainHandResult)
                {
                    case HandResult.Win:
                        Stats.CurrentRun.Wins++;
                        totalWinnings += betAmount;
                        break;
                    case HandResult.Loss:
                        Stats.CurrentRun.Losses++;
                        break;
                    case HandResult.Push:
                        Stats.CurrentRun.Pushes++;
                        break;
                    case HandResult.Blackjack:
                        Stats.CurrentRun.Blackjacks++;
                        totalWinnings += (int)(betAmount * 1.5);
                        break;
                }
            }

            if (totalWinnings > Stats.CurrentRun.LargestPotWon)
            {
                Stats.CurrentRun.LargestPotWon = totalWinnings;
            }

            Stats.PlayerMoney = Player.Money;
            PersistenceService.SaveStats(Stats);
        }
    }
}
