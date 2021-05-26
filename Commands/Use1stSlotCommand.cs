using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyDungeon
{
    class Use1stSlotCommand : ICommand
    {

        private Game1 game;

        public Use1stSlotCommand(Game1 G)
        {
            game = G;
        }
        public void execute()
        {
            game.spellSlots.UseItems(0);
        }
    }
}
