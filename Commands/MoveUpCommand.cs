using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;


namespace HappyDungeon
{
    public class MoveUpCommand : ICommand
    {
        private Game1 game;

        public MoveUpCommand(Game1 G)
        {
            game = G;
        }
        public void execute()
        {
            new Commands.MoveCommand().Move(game, Globals.Direction.Up);

        }
    }
}
