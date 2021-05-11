﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace HappyDungeon 
{
    /// <summary>
    /// Jesus fuck, this thing is the real game changer. 
    /// It feels much much better after having a mouse control. 
    /// </summary>
    class MouseController : IController
    {
        private Game1 game; 

        private int cursorX;
        private int cursorY;

        public MouseController(Game1 G)
        {
            game = G;
        }

        public void Update()
        {

            MouseState CurrentState = Mouse.GetState();
            cursorX = CurrentState.X;
            cursorY = CurrentState.Y;

            Vector2 CurrentLocation = new Vector2(cursorX, cursorY);

            game.cursor.SetPosition(CurrentLocation);
            game.headsupDisplay.CheckOnHover(CurrentLocation);

            if (CurrentState.LeftButton == ButtonState.Pressed)
            {
                game.headsupDisplay.CheckLeftClick(CurrentLocation);
            }
            if (CurrentState.RightButton == ButtonState.Pressed)
            {
                game.mainChara.RightClickMove(CurrentLocation);
            }

        }
    }
}