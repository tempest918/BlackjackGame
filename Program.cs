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
            Console.WriteLine($"{Face} of {Suit}");
        }
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
            string[] faces = ["2", "3", "4", "5", "6", "7", "8", "9", "10", "Jack", "Queen", "King", "Ace"];
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

    class BlackjackGame
    {
        // Deck property to store the deck of cards
        public Deck Deck { get; set; }

        // Constructor to initialize the deck
        public BlackjackGame()
        {
            Deck = new Deck();
        }
    }

    class Program
    {
        // Main method to test the Deck class
        static void Main(string[] args)
        {
            BlackjackGame game = new BlackjackGame();

            Console.WriteLine("Deck Order (Initial)");
            game.Deck.DisplayDeck();

            game.Deck.Shuffle();
            Console.WriteLine("\nDeck Order (After Shuffling)");
            game.Deck.DisplayDeck();
        }
    }
}