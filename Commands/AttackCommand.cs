using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HappyDungeon
{
    public class AttackCommand : ICommand
    {
        private Game1 game;


        public AttackCommand(Game1 G)
        {
            game = G;
        }
        public void execute()
        {
            
            game.mainChara.Attack();
        }
    }
}
