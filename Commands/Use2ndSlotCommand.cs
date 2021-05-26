using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyDungeon
{
    public class Use2ndSlotCommand : ICommand
    {
        private Game1 game;

        public Use2ndSlotCommand(Game1 G)
        {
            game = G;
        }
        public void execute()
        {

            game.spellSlots.UseItems(1);
        }
    }
}
