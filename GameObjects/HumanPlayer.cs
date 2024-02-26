using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnoModellingPractice.GameObjects
{
    class HumanPlayer: Player{
        public String Name{ get; set; }

        public HumanPlayer(String name) : base() {
            Name = name;
        }

        public override PlayerTurn PlayTurn(PlayerTurn previousTurn, CardDeck drawPile){
            // Console.WriteLine($"Previous: {previousTurn.Card.DisplayValue}");
            PlayerTurn turn = new PlayerTurn();
            if (previousTurn.Result == TurnResult.Skip
                || previousTurn.Result == TurnResult.DrawTwo
                || previousTurn.Result == TurnResult.WildDrawFour)
            {
                return ProcessAttack(previousTurn.Card, drawPile);
            }
        
            else {
                Console.Write("Your Hand: ");
                ShowHand();
                string input = "";
                bool loop = true;
                do
                {
                    Console.Write(Name + ": (Choose a Card or type \"Draw\") ");
                    input = Console.ReadLine();
                    if(string.IsNullOrEmpty(input)){
                        continue;
                    }
                    if (!input.Equals("Draw")){
                        string[] CardComponents = input.Split(' ');
                        //Console.WriteLine(CardComponents[0]);
                        if(!input.Equals("Wild") && (!Enum.IsDefined(typeof(CardColor), CardComponents[0]) 
                        || !Enum.IsDefined(typeof(CardValue), CardComponents[1]))){
                            Console.WriteLine("Invalid card!");
                        }
                        else{
                            CardColor colour = GetCardColor(CardComponents[0]);
                            CardValue value;
                            if (CardComponents.Length > 1)
                            {
                                value = GetCardValue(CardComponents[1]);
                            } else {
                                value = GetCardValue("Wild");
                            }
                            Card chosenCard = new Card(colour, value);
                            Card match = GetCardFromHand(chosenCard);
                            if (match == null)
                            {
                                Console.WriteLine("Card not in hand!");
                            }
                            else if (ValidCard(chosenCard, previousTurn)){
                                turn.Card = chosenCard;
                                if (chosenCard.Color == CardColor.Wild)
                                {
                                    Console.Write("Color: ");
                                    string declaredColor = "";
                                    declaredColor = Console.ReadLine();
                                    turn.DeclaredColor = GetCardColor(declaredColor);
                                    turn.Result = TurnResult.WildCard;
                                }
                                else
                                {
                                    turn.DeclaredColor = turn.Card.Color;
                                }
                                Hand.Remove(match);
    
                                switch(chosenCard.Value){
                                    case CardValue.DrawFour:
                                        turn.Result = TurnResult.WildCard;
                                        break;
                                    case CardValue.DrawTwo:
                                        turn.Result = TurnResult.DrawTwo;
                                        break;
                                    case CardValue.Skip:
                                        turn.Result = TurnResult.Skip;
                                        break;
                                    case CardValue.Reverse:
                                        turn.Result = TurnResult.Reversed;
                                        break;
                                    case CardValue.Wild:
                                        turn.Result = TurnResult.WildCard;
                                        break;
                                }
                                loop = false;
                            } else {
                                Console.WriteLine("Invalid card!");
                            };
                        }
                    }else{
                        turn = DrawCard(previousTurn, drawPile);
                        loop = false;
                    }
                } while (loop);
                
            }
            DisplayTurn(turn);
            return turn;
        }

        protected override PlayerTurn DrawCard(PlayerTurn previousTurn, CardDeck drawPile)
        {
            PlayerTurn turn = new PlayerTurn();
            var drawnCard = drawPile.Draw(1);

            switch(drawnCard[0].Color){
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
            
            Console.WriteLine($"    Drawn: {drawnCard[0].DisplayValue}");
            Console.ResetColor();
            Hand.AddRange(drawnCard);

            switch(previousTurn.Card.Color){
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
            
            Console.WriteLine("     Previous Card: " + previousTurn.Card.DisplayValue);
            Console.ResetColor();

            if (ValidCard(drawnCard[0], previousTurn))
            {
                turn.Card = drawnCard[0];
                if (drawnCard[0].Color == CardColor.Wild)
                {
                    Console.Write("Colour: ");
                    string declaredColour = "";
                    declaredColour = Console.ReadLine();
                    turn.DeclaredColor = GetCardColor(declaredColour);
                }
                else
                {
                    turn.DeclaredColor = turn.Card.Color;
                }
                turn.Result = TurnResult.PlayedDraw;
                Hand.Remove(drawnCard[0]);
            }
            else
            {
                turn.Result = TurnResult.Draw;
                turn.Card = previousTurn.Card;
                turn.DeclaredColor = previousTurn.Card.Color;
            }

            return turn;
        }

        private CardColor GetCardColor(string input){
            foreach(CardColor color in Enum.GetValues(typeof(CardColor))){
                if (input.Equals(color.ToString()))
                    return color;
            }

            //this should not happen
            return CardColor.Wild;
        }

        private CardValue GetCardValue(string input)
        {
            foreach (CardValue value in Enum.GetValues(typeof(CardValue)))
            {
                if (input.Equals(value.ToString()))
                    return value;
            }

            //this should not happen
            return CardValue.Zero;
        }

        private Card GetCardFromHand(Card chosenCard){
            foreach (Card card in Hand){
                if (card.Color == chosenCard.Color && card.Value == chosenCard.Value)
                    return card;
            }
            return null;
        }

        private bool ValidCard(Card chosenCard, PlayerTurn previousTurn){
            if(previousTurn.Result == TurnResult.WildCard || previousTurn.Result == TurnResult.WildDrawFour){
                // Console.WriteLine($"Check {previousTurn.DeclaredColor.ToString()} == {chosenCard.Color.ToString()}");
                return chosenCard.Color == previousTurn.DeclaredColor;
            }
            if(previousTurn.Result == TurnResult.Attacked){
                return chosenCard.Color == previousTurn.DeclaredColor || chosenCard.Value == previousTurn.Card.Value;
            }
            if(chosenCard.Color == CardColor.Wild){
                return true;
            }
            if (!(chosenCard.Color == previousTurn.DeclaredColor || chosenCard.Value == previousTurn.Card.Value))
            {
                // Console.WriteLine($"{chosenCard.Color.ToString()} != {previousTurn.DeclaredColor.ToString()} || {chosenCard.Value.ToString()} != {previousTurn.Card.Value.ToString()}");
                return false;
            }

            return true;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}