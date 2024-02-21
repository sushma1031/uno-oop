using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnoModellingPractice.GameObjects
{
    class HumanPlayer: Player{
        public String Name{ get; set; }

        public HumanPlayer(String name) : base() {
            Name = name;
        }

        public override PlayerTurn PlayTurn(PlayerTurn previousTurn, CardDeck drawPile){
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
                        if(!Enum.IsDefined(typeof(CardColor), CardComponents[0]) 
                        || !Enum.IsDefined(typeof(CardValue), CardComponents[1])){
                            Console.WriteLine("Invalid card!");
                        }
                        else{
                            CardColor colour = GetCardColor(CardComponents[0]);
                            CardValue value = GetCardValue(CardComponents[1]);
                            Card chosenCard = new Card(colour, value);
                            Card match = GetCardFromHand(chosenCard);
                            if (match == null)
                            {
                                Console.WriteLine("Card not in hand!");
                            }
                            else if (ValidCard(chosenCard, previousTurn, Hand)){
                                turn.Card = chosenCard;
                                turn.DeclaredColor = turn.Card.Color;
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

        private bool ValidCard(Card chosenCard, PlayerTurn previousTurn , List<Card> Hand){
            if (previousTurn.Result == TurnResult.WildCard && chosenCard.Color != previousTurn.DeclaredColor)
            {
                // Console.WriteLine($"{previousTurn.DeclaredColor.ToString()} != {chosenCard.Color.ToString()}");
                return false;
            }
            if (!(chosenCard.Color == previousTurn.DeclaredColor || chosenCard.Value == previousTurn.Card.Value))
            {
                // Console.WriteLine($"{previousTurn.DeclaredColor.ToString()} != {chosenCard.Color.ToString()} || {chosenCard.Value.ToString()} != {previousTurn.Card.Value.ToString()}");
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