﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;


namespace HappyDungeon
{
    public class MoveLeftCommand : ICommand
    {

        private Game1 game;

        public MoveLeftCommand(Game1 G)
        {
            game = G;
        }
        public void execute()
        {
            if (game.gameState == Globals.GameStates.Running)
                new Commands.MoveCommand().Move(game, Globals.Direction.Left);
            else if (Globals.Displays.Contains(game.gameState))
                game.generalDisplay.OptionMoveLeft();
        }
    }
}
