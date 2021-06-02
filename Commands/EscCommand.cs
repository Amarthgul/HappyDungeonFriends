using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace HappyDungeon
{
    public class EscCommand : ICommand
    {
        private Game1 game;
        private Stopwatch stopwatch;

        public EscCommand(Game1 G)
        {
            game = G;

            stopwatch = new Stopwatch();
            stopwatch.Restart();
        }
        public void execute()
        {
            if (stopwatch.ElapsedMilliseconds < Globals.KB_STATE_HOLD)
                return;

            switch (game.gameState)
            {
                case Globals.GameStates.Bag:
                    game.gameState = Globals.GameStates.Running;
                    break;
                case Globals.GameStates.TitleScreen:
                    game.Exit();
                    break;
                case Globals.GameStates.Running:
                    game.Exit();
                    break;
                default:
                    break;
            }

            stopwatch.Restart();

        }
    }
}
