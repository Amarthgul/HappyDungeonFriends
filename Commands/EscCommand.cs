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
        private System.Diagnostics.Stopwatch stopwatch;

        public EscCommand(Game1 G)
        {
            game = G;

            stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Restart();
        }
        public void execute()
        {
            if (stopwatch.ElapsedMilliseconds < Globals.KB_STATE_HOLD)
                return;

            switch (game.gameState)
            {
                case Globals.GameStates.Bag:
                    game.screenFX.SigTransitionStart(Globals.GameStates.Running);
                    break;
                case Globals.GameStates.TitleScreen:
                    game.Exit();
                    break;
                case Globals.GameStates.Running:
                    game.screenFX.SigTransitionStart(Globals.GameStates.Pause);
                    break;
                case Globals.GameStates.Setting:
                    game.screenFX.BackToLastState();
                    break;
                case Globals.GameStates.LoadAndSave:
                    game.screenFX.BackToLastState();
                    break;
                case Globals.GameStates.Pause:
                    game.screenFX.SigTransitionStart(Globals.GameStates.Running);
                    break;
                default:
                    break;
            }

            stopwatch.Restart();

        }
    }
}
