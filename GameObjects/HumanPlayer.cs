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

        // public override PlayerTurn PlayTurn(PlayerTurn previousTurn, CardDeck drawPile){
        //     PlayerTurn turn = new PlayerTurn();
        //     return turn;
        // }

        public override string ToString()
        {
            return Name;
        }
    }
}