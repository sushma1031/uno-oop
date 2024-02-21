using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnoModellingPractice.GameObjects
{
    public class Player
    {
        public List<Card> Hand { get; set; }
        public int Position { get; set; }

        public Player()
        {
            Hand = new List<Card>();
        }

        public virtual PlayerTurn PlayTurn(PlayerTurn previousTurn, CardDeck drawPile)
        {
            PlayerTurn turn = new PlayerTurn();
            if (previousTurn.Result == TurnResult.Skip
                || previousTurn.Result == TurnResult.DrawTwo
                || previousTurn.Result == TurnResult.WildDrawFour)
            {
                return ProcessAttack(previousTurn.Card, drawPile);
            }
            else if ((previousTurn.Result == TurnResult.WildCard 
                        || previousTurn.Result == TurnResult.Attacked 
                        || previousTurn.Result == TurnResult.Draw) 
                        && HasMatch(previousTurn.DeclaredColor))
            {
                turn = PlayMatchingCard(previousTurn.DeclaredColor);
            }
            else if (HasMatch(previousTurn.Card))
            {
                turn = PlayMatchingCard(previousTurn.Card);
            }
            else //Draw a card and see if it can play
            {
                turn = DrawCard(previousTurn, drawPile);
            }

            DisplayTurn(turn);
            return turn;
        }

        protected PlayerTurn DrawCard(PlayerTurn previousTurn, CardDeck drawPile)
        {
            PlayerTurn turn = new PlayerTurn();
            var drawnCard = drawPile.Draw(1);
            // Console.WriteLine($"Drawn: {drawnCard[0].DisplayValue}");
            Hand.AddRange(drawnCard);
            // Console.WriteLine("Previous: " + previousTurn.Card.DisplayValue);

            if (IsValidCard(drawnCard[0], previousTurn.Card))
            {
                turn = PlayMatchingCard(previousTurn.Card);
                turn.Result = TurnResult.PlayedDraw;
            }
            else
            {
                turn.Result = TurnResult.Draw;
                turn.Card = previousTurn.Card;
            }

            return turn;
        }

        protected void DisplayTurn(PlayerTurn currentTurn)
        {
            if (currentTurn.Result == TurnResult.Draw)
            {
                Console.WriteLine(this + " drew.");
            }
            else if(currentTurn.Result == TurnResult.PlayedDraw)
            {
                Console.WriteLine(this + " can play the drawn card!");
            }

            else if (currentTurn.Result == TurnResult.PlayedCard
                || currentTurn.Result == TurnResult.Skip
                || currentTurn.Result == TurnResult.DrawTwo 
                || currentTurn.Result == TurnResult.WildCard
                || currentTurn.Result == TurnResult.WildDrawFour
                || currentTurn.Result == TurnResult.Reversed
                || currentTurn.Result == TurnResult.PlayedDraw)
            {
                Console.WriteLine(this + ": " + currentTurn.Card.DisplayValue);
                if(currentTurn.Card.Color == CardColor.Wild)
                {
                    Console.WriteLine(this + " chooses new colour: " + currentTurn.DeclaredColor.ToString());
                }
                if(currentTurn.Result == TurnResult.Reversed)
                {
                    Console.WriteLine("Turn order reversed!");
                }
            }

            if (Hand.Count == 1)
            {
                Console.WriteLine(this + " shouts Uno!");
            }
        }

        protected PlayerTurn ProcessAttack(Card currentDiscard, CardDeck drawPile)
        {
            PlayerTurn turn = new PlayerTurn();
            turn.Result = TurnResult.Attacked;
            turn.Card = currentDiscard;
            turn.DeclaredColor = currentDiscard.Color;
            if(currentDiscard.Value == CardValue.Skip)
            {
                Console.WriteLine(this + " was skipped!");
                return turn;
            }
            else if(currentDiscard.Value == CardValue.DrawTwo)
            {
                Console.WriteLine(this + " must draw two cards!");
                Hand.AddRange(drawPile.Draw(2));
            }
            else if(currentDiscard.Value == CardValue.DrawFour)
            {
                Console.WriteLine(this + " must draw four cards!");
                Hand.AddRange(drawPile.Draw(4));
            }

            return turn;
        }

        private bool HasMatch(Card card)
        {
            return Hand.Any(x => x.Color == card.Color || x.Value == card.Value || x.Color == CardColor.Wild);
        }

        private bool HasMatch(CardColor color)
        {
            return Hand.Any(x => x.Color == color || x.Color == CardColor.Wild);
        }
        private bool IsValidCard(Card drawnCard, Card previous){
            return drawnCard.Color == previous.Color || drawnCard.Value == previous.Value || drawnCard.Color == CardColor.Wild;
        }
        private PlayerTurn PlayMatchingCard(CardColor color)
        {
            var turn = new PlayerTurn();
            turn.Result = TurnResult.PlayedCard;
            var matching = Hand.Where(x => x.Color == color || x.Color == CardColor.Wild).ToList();

            //Play the card that would cause the most damage to the next player.
            if (matching.All(x => x.Value == CardValue.DrawFour))
            {
                turn.Card = matching.First();
                turn.DeclaredColor = SelectDominantColor();
                turn.Result = TurnResult.WildCard;
                Hand.Remove(matching.First());

                return turn;
            }

            if (matching.Any(x => x.Value == CardValue.DrawTwo))
            {
                turn.Card = matching.First(x => x.Value == CardValue.DrawTwo);
                turn.Result = TurnResult.DrawTwo;
                turn.DeclaredColor = turn.Card.Color;
                Hand.Remove(turn.Card);

                return turn;
            }

            if (matching.Any(x => x.Value == CardValue.Skip))
            {
                turn.Card = matching.First(x => x.Value == CardValue.Skip);
                turn.Result = TurnResult.Skip;
                turn.DeclaredColor = turn.Card.Color;
                Hand.Remove(turn.Card);

                return turn;
            }

            if (matching.Any(x => x.Value == CardValue.Reverse))
            {
                turn.Card = matching.First(x => x.Value == CardValue.Reverse);
                turn.Result = TurnResult.Reversed;
                turn.DeclaredColor = turn.Card.Color;
                Hand.Remove(turn.Card);

                return turn;
            }

            var matchOnColor = matching.Where(x => x.Color == color);
            if (matchOnColor.Any())
            {
                turn.Card = matchOnColor.First();
                turn.DeclaredColor = turn.Card.Color;
                Hand.Remove(matchOnColor.First());

                return turn;
            }

            if (matching.Any(x => x.Value == CardValue.Wild))
            {
                turn.Card = matching.First(x => x.Value == CardValue.Wild);
                turn.DeclaredColor = SelectDominantColor();
                turn.Result = TurnResult.WildCard;
                Hand.Remove(turn.Card);

                return turn;
            }

            turn.Result = TurnResult.Draw;
            return turn;
        }

        private PlayerTurn PlayMatchingCard(Card currentDiscard)
        {
            var turn = new PlayerTurn();
            turn.Result = TurnResult.PlayedCard;
            var matching = Hand.Where(x => x.Color == currentDiscard.Color || x.Value == currentDiscard.Value || x.Color == CardColor.Wild).ToList();

            //Play the card that would cause the most damage to the next player.
            if (matching.All(x => x.Value == CardValue.DrawFour))
            {
                turn.Card = matching.First();
                turn.DeclaredColor = SelectDominantColor();
                turn.Result = TurnResult.WildCard;
                Hand.Remove(matching.First());

                return turn;
            }
            if (matching.Any(x=> x.Value == CardValue.DrawTwo))
            {
                turn.Card = matching.First(x => x.Value == CardValue.DrawTwo);
                turn.Result = TurnResult.DrawTwo;
                turn.DeclaredColor = turn.Card.Color;
                Hand.Remove(turn.Card);

                return turn;
            }

            if(matching.Any(x => x.Value == CardValue.Skip))
            {
                turn.Card = matching.First(x => x.Value == CardValue.Skip);
                turn.Result = TurnResult.Skip;
                turn.DeclaredColor = turn.Card.Color;
                Hand.Remove(turn.Card);

                return turn;
            }

            if (matching.Any(x => x.Value == CardValue.Reverse))
            {
                turn.Card = matching.First(x => x.Value == CardValue.Reverse);
                turn.Result = TurnResult.Reversed;
                turn.DeclaredColor = turn.Card.Color;
                Hand.Remove(turn.Card);

                return turn;
            }

            //At this point the player has a choice of sorts
            //Assuming they have a match on color AND a match on value, they can choose which to play
            //We choose the match with MORE possible plays from the hand.

            var matchOnColor = matching.Where(x => x.Color == currentDiscard.Color);
            var matchOnValue = matching.Where(x => x.Value == currentDiscard.Value);
            if(matchOnColor.Any() && matchOnValue.Any())
            {
                var correspondingColor = Hand.Where(x => x.Color == matchOnColor.First().Color);
                var correspondingValue = Hand.Where(x => x.Value == matchOnValue.First().Value);
                if(correspondingColor.Count() >= correspondingValue.Count())
                {
                    turn.Card = matchOnColor.First();
                    turn.DeclaredColor = turn.Card.Color;
                    Hand.Remove(matchOnColor.First());

                    return turn;
                }
                else //Match on value
                {
                    turn.Card = matchOnValue.First();
                    turn.DeclaredColor = turn.Card.Color;
                    Hand.Remove(matchOnValue.First());

                    return turn;
                }
                //Figure out which of these is better
            }
            else if(matchOnColor.Any())
            {
                turn.Card = matchOnColor.First();
                turn.DeclaredColor = turn.Card.Color;
                Hand.Remove(matchOnColor.First());

                return turn;
            }
            else if(matchOnValue.Any())
            {
                turn.Card = matchOnValue.First();
                turn.DeclaredColor = turn.Card.Color;
                Hand.Remove(matchOnValue.First());

                return turn;
            }

            //Play regular wilds last.  If a wild becomes our last card, we win on the next turn!
            if (matching.Any(x => x.Value == CardValue.Wild))
            {
                turn.Card = matching.First(x => x.Value == CardValue.Wild);
                turn.DeclaredColor = SelectDominantColor();
                turn.Result = TurnResult.WildCard;
                Hand.Remove(turn.Card);

                return turn;
            }

            //This should never happen
            turn.Result = TurnResult.Draw;
            return turn;
        }

        private CardColor SelectDominantColor()
        {
            if (!Hand.Any())
            {
                return CardColor.Wild;
            }
            var colors = Hand.GroupBy(x => x.Color).OrderByDescending(x => x.Count());
            return colors.First().First().Color;
        }

        private void SortHand()
        {
            this.Hand = this.Hand.OrderBy(x => x.Color).ThenBy(x => x.Value).ToList();
        }

        public void ShowHand()
        {
            SortHand();
            // Console.WriteLine("Player " + Position + "'s Hand: ");
            foreach (var card in Hand)
            {
                Console.Write(card.DisplayValue + "  ");
            }
            Console.WriteLine("");
        }

        public override string ToString()
        {
            return $"Player {Position}";  
        }
    }
}
