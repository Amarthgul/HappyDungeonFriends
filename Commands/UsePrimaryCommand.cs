using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyDungeon
{
    public class UsePrimaryCommand : ICommand
    {

        private Game1 game;

        public UsePrimaryCommand(Game1 G)
        {
            game = G;
             
        }

        public void execute()
        {
            game.spellSlots.UsePrimary();
        }
    }
}
