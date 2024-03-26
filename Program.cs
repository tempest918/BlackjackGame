using System;
using System.Collections.Generic;

namespace BlackjackGame
{
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

    class Deck
    {
        public List<Card> Cards { get; set; }

        public Deck()
        {
            Cards = new List<Card>();
            string[] suits = ["Hearts", "Diamonds", "Clubs", "Spades"];
            string[] faces = ["2", "3", "4", "5", "6", "7", "8", "9", "10", "Jack", "Queen", "King", "Ace"];
            int[] values = [2, 3, 4, 5, 6, 7, 8, 9, 10, 10, 10, 10, 11];

            for (int i = 0; i < suits.Length; i++)
            {
                for (int j = 0; j < faces.Length; j++)
                {
                    Cards.Add(new Card(suits[i], faces[j], values[j]));
                }
            }
        }

        public void Shuffle()
        {
            Random rand = new();
            for (int i = 0; i < Cards.Count; i++)
            {
                int r = rand.Next(i, Cards.Count);
                (Cards[r], Cards[i]) = (Cards[i], Cards[r]);
            }
        }

        public Card Draw()
        {
            Card drawnCard = Cards[0];
            Cards.RemoveAt(0);
            return drawnCard;
        }

    public void DisplayDeck()
    {
        Console.WriteLine("Cards in deck:");
        foreach(Card card in Cards)
        {
            card.Display();
        }
    }
    }

    class BlackjackGame
    {
        public Deck Deck { get; set; }

        public BlackjackGame()
        {
            Deck = new Deck();
        }
    }

        class Program 
    {
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