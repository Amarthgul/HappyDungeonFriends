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
            game.mainChara.ChangeDirection(Globals.Direction.Left);
            if (!game.mainChara.isMoving)
                game.mainChara.Move();
        }
    }
}
