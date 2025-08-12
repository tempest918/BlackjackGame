using System;
using System.Collections.Generic;

namespace BlackjackLogic
{
    // Deck class to represent a deck of cards
    public class Deck
    {
        public List<Card> Cards { get; set; }

        public Deck(int numDecks = 1)
        {
            // Initialize variables and create a new list of cards
            Cards = new List<Card>();
            string[] suits = { "Hearts", "Diamonds", "Clubs", "Spades" };
            string[] faces = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A" };
            int[] values = { 2, 3, 4, 5, 6, 7, 8, 9, 10, 10, 10, 10, 11 };

            for (int k = 0; k < numDecks; k++)
            {
                // Add all cards to the deck
                for (int i = 0; i < suits.Length; i++)
                {
                    for (int j = 0; j < faces.Length; j++)
                    {
                        Cards.Add(new Card(suits[i], faces[j], values[j]));
                    }
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
            if (Cards.Count == 0)
            {
                // Optionally, create a new shuffled deck if the current one is empty
                // For now, let's throw an exception or return null
                return null;
            }

            Card drawnCard = Cards[0];
            Cards.RemoveAt(0);
            return drawnCard;
        }
    }
}
