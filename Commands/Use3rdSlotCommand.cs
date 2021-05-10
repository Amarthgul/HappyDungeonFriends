using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyDungeon
{
    public class Use3rdSlotCommand : ICommand
    {
        private Game1 game;

        public Use3rdSlotCommand(Game1 G)
        {
            game = G;
        }
        public void execute()
        {
            
        }
    }
}
