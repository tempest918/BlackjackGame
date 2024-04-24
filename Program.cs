/*
 * File Name: Program.cs
 * Author: Anthony Barnes and Lou Craft
 * Date Created: 03/25/2024
 * Date Modified: 04/08/2024
 * Description: This is a simple blackjack game. The player will play against the dealer and try to get as close to 21 as possible without going over. The player will start with $100 and can bet any amount on each round. The player will win 1.5 times their bet if they get blackjack. 
 * The player will lose their bet if they go over 21 or have a lower score than the dealer. The game will continue until the player runs out of money or chooses to quit.
 * Modifications:
 * - created classes to represent cards, deck, player, and dealer
 * - added more comments
 * - added basic victory conditions
 * - added interactive menu with options to hit or stay
 * - reworked Ace logic
 * - added card art
 * - added logic to restart the game
 * - added player money and betting system
 * - changed winnings to switch statement
 * - fix Ace logic after renaming face string to "A"
 */
using System;
using System.Collections.Generic;

namespace BlackjackGame
{
    // Card class to represent a single card
    class Card(string suit, string face, int value)
    {
        public string Suit { get; set; } = suit;
        public string Face { get; set; } = face;
        public int Value { get; set; } = value;

        public void Display()
        {
            string suitSymbol = Suit switch
            {
                "Hearts" => "♥",
                "Diamonds" => "♦",
                "Clubs" => "♣",
                "Spades" => "♠",
                _ => ""  // should never happen
            };

            Console.WriteLine("┌─────┐");
            Console.WriteLine($"│{Face,2} {" ",-2}│");
            Console.WriteLine($"│  {suitSymbol}  │");
            Console.WriteLine($"│{" ",-2} {Face,2}│");
            Console.WriteLine("└─────┘");
            //Console.WriteLine($"{Face} of {Suit}");

        }

        // Deck class to represent a deck of cards
        class Deck
        {
            public List<Card> Cards { get; set; }

            public Deck()
            {
                // Initialize variables and create a new list of cards
                Cards = new List<Card>();
                string[] suits = ["Hearts", "Diamonds", "Clubs", "Spades"];
                string[] faces = ["2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A"];
                int[] values = [2, 3, 4, 5, 6, 7, 8, 9, 10, 10, 10, 10, 11];

                // Add all cards to the deck
                for (int i = 0; i < suits.Length; i++)
                {
                    for (int j = 0; j < faces.Length; j++)
                    {
                        Cards.Add(new Card(suits[i], faces[j], values[j]));
                    }
                }
            }

            // Shuffle the deck
            public void Shuffle()
            {
                Random rand = new();
                for (int i = 0; i < Cards.Count; i++)
                {
                    int r = rand.Next(i, Cards.Count);
                    (Cards[r], Cards[i]) = (Cards[i], Cards[r]);
                }
            }

            // Draw a card from the deck
            public Card Draw()
            {
                Card drawnCard = Cards[0];
                Cards.RemoveAt(0);
                return drawnCard;
            }

            // Display all cards in the deck
            public void DisplayDeck()
            {
                Console.WriteLine("Cards in deck:");
                foreach (Card card in Cards)
                {
                    card.Display();
                }
            }
        }

        class Player
        {
            public string Name { get; set; }
            public List<Card> Hand { get; set; }
            public int Score { get; set; }
            public int AceCount { get; set; }
            public int Money { get; set; }

            public Player(string name, int startingMoney = 100)
            {
                Name = name;
                Hand = new List<Card>();
                Score = 0;
                AceCount = 0;
                Money = startingMoney;
            }

            public void DrawCard(Deck deck)
            {
                Card drawnCard = deck.Draw();
                Hand.Add(drawnCard);
                if (drawnCard.Face == "A")
                {
                    AceCount++;
                }
            }

            // Calculate the score of the provided hand
            public int CalculateScore()
            {
                Score = 0;

                foreach (Card card in Hand)
                {
                    Score += card.Value;
                    if (card.Face == "A")
                    {
                        AceCount++;
                    }
                }

                while (Score > 21 && AceCount > 0)
                {
                    Score -= 10;
                    AceCount--;
                }

                return Score;
            }

            public void DisplayHand()
            {
                Console.WriteLine($"{Name}'s hand:");
                foreach (Card card in Hand)
                {
                    card.Display();
                }
                Console.WriteLine();
                Console.WriteLine($"Score: {CalculateScore()}");
                Console.WriteLine();
            }

            public void ClearHand()
            {
                Hand.Clear();
            }

            public void ResetScore()
            {
                Score = 0;
            }

            class Dealer : Player
            {
                public Dealer() : base("Dealer") { }

                public void DisplayPartialHand()
                {
                    Console.WriteLine($"{Name}'s hand:");

                    Console.WriteLine("┌─────┐");
                    Console.WriteLine("│* * *│");
                    Console.WriteLine("│ * * │");
                    Console.WriteLine("│* * *│");
                    Console.WriteLine("└─────┘");

                    Hand[1].Display();
                    Console.WriteLine();
                }
            }

            class BlackjackGame
            {
                // Deck property to store the deck of cards
                public Deck Deck { get; set; }

                // Constructor to initialize the game
                public BlackjackGame()
                {
                    //Console.Clear();
                    Console.WriteLine("Welcome to Blackjack!");
                    Console.WriteLine("----------------------\r\n");

                    StartGame();

                }

                // Method to start the game and execute the game loop
                public void StartGame()
                {
                    // Create a player and dealer object, and set the game state to not over
                    Player player = new Player("Player");
                    Dealer dealer = new Dealer();

                    bool gameOver = false;

                    while (!gameOver)
                    {
                        PlayRound(player, dealer);

                        // Reset the player and dealer hands and scores
                        player.ClearHand();
                        player.ResetScore();
                        dealer.ClearHand();
                        dealer.ResetScore();

                        // Check if the player is out of money
                        if (player.Money <= 0)
                        {
                            Console.WriteLine("Game Over! You're out of money.");
                            gameOver = true;
                        }
                        else
                        {
                            // Ask the player if they want to play another round
                            Console.WriteLine("Do you want to play another round? (y/n)");
                            string choice = Console.ReadLine();
                            if (choice.ToLower() != "y")
                            {
                                gameOver = true;
                            }

                        }
                    }

                    // Display the player's final money amount
                    if (player.Money > 0)
                    {
                        Console.WriteLine($"You finished with ${player.Money}.");
                    }
                    Console.WriteLine("Thanks for playing!");

                }

                // Method to play a round of blackjack
                public void PlayRound(Player player, Dealer dealer)
                {
                    // Create a new deck and set the handOver flag to false
                    Deck = new Deck();
                    bool handOver = false;

                    // Get the bet amount from the player
                    int bet = GetBetAmount(player);

                    // Shuffle the deck
                    Deck.Shuffle();

                    // Draw two cards for the player and dealer
                    player.DrawCard(Deck);
                    player.DrawCard(Deck);
                    dealer.DrawCard(Deck);
                    dealer.DrawCard(Deck);

                    // Display the player's hand
                    player.DisplayHand();

                    // Display the dealer's partial hand
                    dealer.DisplayPartialHand();

                    // Check if the player has blackjack
                    if (player.CalculateScore() == 21)
                    {
                        Console.WriteLine("Blackjack! You win!");
                        // player wins 1.5 times the bet when they get blackjack
                        player.Money += (int)(2.5 * bet);
                        handOver = true;
                    }
                    else
                    {
                        // Game Loop
                        while (!handOver)
                        {
                            Console.WriteLine("Choose an option:");
                            Console.WriteLine("1. Hit");
                            Console.WriteLine("2. Stay");

                            string choice = Console.ReadLine();
                            //Console.Clear();

                            switch (choice)
                            {
                                case "1":
                                    player.DrawCard(Deck);
                                    player.DisplayHand();
                                    dealer.DisplayPartialHand();
                                    if (player.CalculateScore() > 21)
                                        {
                                        Console.WriteLine("***BUST!***");
                                        Console.WriteLine();
                                        handOver = true;
                                    }
                                    break;
                                case "2":
                                    while (dealer.CalculateScore() < 17)
                                    {
                                        dealer.DrawCard(Deck);
                                    }
                                    player.DisplayHand();
                                    dealer.DisplayHand();

                                    handOver = true;
                                    break;
                                default:
                                    Console.WriteLine("Invalid choice. Please try again.");
                                    break;
                            }
                        }

                        // Check if the player has won
                        switch (HasPlayerWon(player, dealer))
                        {   case 0: 
                            //typical win
                                player.Money += 2 * bet;
                                break;
                            case 1:
                                //typical loss
                                player.Money -= bet;
                                break;
                            case 2:
                                //tie
                                player.Money += bet;
                                break;
                        }
                    }
                }
                private int GetBetAmount(Player player)
                {
                    // set up vars
                    int betAmount = 0;
                    bool validBet = false;

                    // loop until valid bet
                    while (!validBet)
                    {
                        // get bet amount from player
                        Console.WriteLine($"You have ${player.Money}. How much would you like to bet?\r\n");
                        string betInput = Console.ReadLine();

                        if (int.TryParse(betInput, out betAmount))
                        {
                            if (betAmount > player.Money)
                            {
                                Console.WriteLine("You don't have enough money to make that bet. Please try again.");
                            }
                            else
                            {
                                validBet = true;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid input. Please enter a valid number.");
                        }
                    }

                    return betAmount;
                }

                // Method to check if the player has won
                public int HasPlayerWon(Player player, Dealer dealer)
                {
                    if(player.CalculateScore() == 21 ||
                        dealer.CalculateScore() > 21 ||
                        player.CalculateScore() < 21 &&
                        player.CalculateScore() > dealer.CalculateScore())
                    {
                        Console.WriteLine("You Win!");
                        return 0;
                    }
                    else if (player.CalculateScore() > 21 ||
                            dealer.CalculateScore() == 21 ||
                            player.CalculateScore() < dealer.CalculateScore())
                    {
                        Console.WriteLine("You Lose!");
                        return 1;
                    }
                    else
                    {
                        Console.WriteLine("It's a tie!");
                        return 2;
                    }
                }
            }

            class Program
            {
                static void Main(string[] args)
                {

                    BlackjackGame game = new BlackjackGame();

                    // Stuff to test the Deck class
                    /*             
                        Console.WriteLine("Deck Order (Initial)");
                        game.Deck.DisplayDeck();

                        game.Deck.Shuffle();
                        Console.WriteLine("\nDeck Order (After Shuffling)");
                        game.Deck.DisplayDeck();
                    */

                }
            }
        }
    }
}