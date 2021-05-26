using System;
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

        private Rectangle gameArea = new Rectangle(0, Globals.OUT_HEADSUP, Globals.OUT_FWIDTH, Globals.OUT_FHEIGHT);

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

            // The cursor always updates its position 
            Vector2 CurrentLocation = new Vector2(cursorX, cursorY);
            game.cursor.SetPosition(CurrentLocation);

            switch (game.gameState)
            {
                case Globals.GameStates.Running:
                    UpdateRunning(CurrentState, CurrentLocation);
                    break;
                case Globals.GameStates.RoomTransitioning:
                    UpdateTransitioning(CurrentState, CurrentLocation);
                    break; 
                default:
                    break; 
            }
                    

        }

        private void UpdateRunning(MouseState CurrentState, Vector2 CurrentLocation)
        {
            
            game.headsupDisplay.CheckOnHover(CurrentLocation);

            if (CurrentState.LeftButton == ButtonState.Pressed)
            {
                game.minimap.TabClick(CurrentLocation);
                game.headsupDisplay.CheckLeftClick(CurrentLocation);

                if (game.displayWholeMinimap != 1 && gameArea.Contains(CurrentLocation))
                {
                    game.mainChara.Attack();
                }
                    
            }
            else
            {
                game.minimap.TabClickRelease();
            }


            if (CurrentState.RightButton == ButtonState.Pressed)
            {
                game.mainChara.RightClickMove(CurrentLocation);

                // If it's not in UI area, then add ground click effect 
                if (!game.headsupDisplay.InsideUI(CurrentLocation))
                {
                    game.cursor.RightClick(CurrentLocation);
                }
            }
        }

        private void UpdateTransitioning(MouseState CurrentState, Vector2 CurrentLocation)
        {

        }
    }
}
