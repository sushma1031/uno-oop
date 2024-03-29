﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnoModellingPractice.GameObjects
{
    public class Card
    {
        public CardColor Color { get; set; }
        public CardValue Value { get; set; }

        public Card(CardColor colour, CardValue val) {
            Color = colour;
            Value = val;
        }

        public string DisplayValue
        {
            get
            {
                if(Value == CardValue.Wild)
                {
                    return Value.ToString();
                }
                return Color.ToString() + " " + Value.ToString(); 
            }
        }
    }
}