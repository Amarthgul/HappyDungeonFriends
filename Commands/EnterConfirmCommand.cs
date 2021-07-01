using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;


namespace HappyDungeon
{
    public class EnterConfirmCommand : ICommand
    {
        private Game1 game;

        public EnterConfirmCommand(Game1 G)
        {
            game = G;
        }
        public void execute()
        {
            if (Globals.Displays.Contains(game.gameState))
            {
                game.generalDisplay.OptionConfirm();
            }
        }
    }
}
