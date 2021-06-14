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
            switch (game.gameState)
            {
                case Globals.GameStates.Running:
                    new Commands.MoveCommand().Move(game, Globals.Direction.Up);
                    break;
                case Globals.GameStates.Setting:
                    break;
                case Globals.GameStates.TitleScreen:
                    break;
                case Globals.GameStates.Bag:
                    break;
                case Globals.GameStates.GameOver:
                    break;
                case Globals.GameStates.Conversation:
                    break;
                default:
                    break; 
            }

        }
    }
}
