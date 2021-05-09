using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HappyDungeon
{
    public class DisplayMapCommand : ICommand
    {
        private Game1 game;


        public DisplayMapCommand(Game1 G)
        {
            game = G;
        }
        public void execute()
        {
            game.displayWholeMinimap = 1; 
        }
    }
}
