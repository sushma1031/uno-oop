using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnoModellingPractice.GameObjects
{
    public class PlayerTurn
    {
        public Card Card { get; set; }
        public CardColor DeclaredColor { get; set; }
        public TurnResult Result { get; set; }
    }
}