﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace HappyDungeon
{
    public class DisplayBagCommand : ICommand
    {
        private Game1 game;
        private Stopwatch stopwatch;

        public DisplayBagCommand(Game1 G)
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
                default:
                    game.gameState = Globals.GameStates.Bag;
                    break;
            }

            stopwatch.Restart();
            
        }

    }
}