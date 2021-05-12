using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyDungeon
{
    class AltDisplayCommand : ICommand
    {
        private Game1 game; 

        public AltDisplayCommand(Game1 G)
        {
            game = G;
        }
        public void execute()
        {

            game.headsupDisplay.AltDisplayOn();

        }
    }
}
