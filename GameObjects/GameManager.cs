﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnoModellingPractice.GameObjects
{
    public class GameManager
    {
        public List<Player> Players { get; set; }
        public CardDeck DrawPile { get; set; }
        public List<Card> DiscardPile { get; set; }

        public GameManager(int numPlayers)
        {
            Players = new List<Player>();
            DrawPile = new CardDeck();
            DrawPile.Shuffle();
            string humanPlayer = "";
            Console.Write("Name: ");
            humanPlayer = Console.ReadLine();
            if (string.IsNullOrEmpty(humanPlayer)) humanPlayer = "Human";
            Players.Add(new HumanPlayer(humanPlayer)
                {
                    Position = 0
                });
            for (int i = 1; i < numPlayers; i++)
            {
                Players.Add(new Player()
                {
                    Position = i
                });
            }

            int maxCards = 7 * numPlayers;
            int dealtCards = 0;

            while(dealtCards < maxCards)
            {
                for(int i = 0; i < numPlayers; i++)
                {
                    Players[i].Hand.Add(DrawPile.Cards.First());
                    DrawPile.Cards.RemoveAt(0);
                    dealtCards++;
                }
            }

            DiscardPile = new List<Card>();
            DiscardPile.Add(DrawPile.Cards.First());
            DrawPile.Cards.RemoveAt(0);

            while(DiscardPile.First().Value == CardValue.Wild || DiscardPile.First().Value == CardValue.DrawFour)
            {
                DiscardPile.Insert(0, DrawPile.Cards.First());
                DrawPile.Cards.RemoveAt(0);
            }
        }

        public void PlayGame()
        {
            int i = 0;
            bool isAscending = true;

            //First, let's show what each player starts with
            // foreach (var player in Players)
            // {
            //     player.ShowHand();
            // }
            Console.Write("Players: ");
            var j = 0;
            for (; j<Players.Count - 1; j++){
                Console.Write(Players[j] + ", ");
            }
            Console.WriteLine(Players[j]);
            Console.Write("Your Hand: ");
            Players[0].ShowHand();

            Console.WriteLine("Hit Enter to start!");
            Console.ReadLine();

            PlayerTurn currentTurn = new PlayerTurn()
            {
                Result = TurnResult.GameStart,
                Card = DiscardPile.First(),
                DeclaredColor = DiscardPile.First().Color
                // x => x.Color != CardColor.Wild 
                // && x.Value != CardValue.Reverse 
                // && x.Value != CardValue.Skip 
                // && x.Value != CardValue.DrawTwo
            };
            Console.Write("First Card Up: ");

            switch(currentTurn.Card.Color){
                    case CardColor.Red:
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;
                    case CardColor.Blue:
                        Console.ForegroundColor = ConsoleColor.Blue;
                        break;
                    case CardColor.Green:
                        Console.ForegroundColor = ConsoleColor.Green;
                        break;
                    case CardColor.Yellow:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;
                    default:
                        Console.ResetColor();
                        break;
                }
            Console.WriteLine(currentTurn.Card.DisplayValue);
            Console.ResetColor();

            while(!Players.Any(x => !x.Hand.Any()))
            {
                if(DrawPile.Cards.Count < 4)
                {
                    var currentCard = DiscardPile.First();
                    
                    //Take the discarded cards, shuffle them, and make them the new draw pile.
                    DrawPile.Cards = DiscardPile.Skip(1).ToList();
                    DrawPile.Shuffle();

                    //Reset the discard pile to only have the current card.
                    DiscardPile = new List<Card>();
                    DiscardPile.Add(currentCard);
                    
                    Console.WriteLine("Shuffling cards!");
                }

                var currentPlayer = Players[i];

                currentTurn = Players[i].PlayTurn(currentTurn, DrawPile);

                AddToDiscardPile(currentTurn);

                if (currentTurn.Result == TurnResult.Reversed)
                {
                    isAscending = !isAscending;
                }

                if (isAscending)
                {
                    i++;
                    if (i >= Players.Count) //Reset player counter
                    {
                        i = 0;
                    }
                }
                else
                {
                    i--;
                    if (i < 0)
                    {
                        i = Players.Count - 1;
                    }
                }        
            }

            var winningPlayer = Players.Where(x => !x.Hand.Any()).First();
            Console.WriteLine($"!! {winningPlayer} wins !!");
        }

        private void AddToDiscardPile(PlayerTurn currentTurn)
        {
            if (currentTurn.Result == TurnResult.PlayedCard
                    || currentTurn.Result == TurnResult.DrawTwo
                    || currentTurn.Result == TurnResult.Skip
                    || currentTurn.Result == TurnResult.WildCard
                    || currentTurn.Result == TurnResult.WildDrawFour
                    || currentTurn.Result == TurnResult.Reversed)
            {
                DiscardPile.Insert(0, currentTurn.Card);
            }
        }
    }
}