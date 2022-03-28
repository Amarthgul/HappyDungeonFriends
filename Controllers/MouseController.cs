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
                case Globals.GameStates.Bag:
                    UpdateDisplays(CurrentState, CurrentLocation);
                    break;
                case Globals.GameStates.TitleScreen:
                    UpdateDisplays(CurrentState, CurrentLocation);
                    break;
                case Globals.GameStates.Setting:
                    UpdateDisplays(CurrentState, CurrentLocation);
                    break;
                case Globals.GameStates.GameOver:
                    UpdateDisplays(CurrentState, CurrentLocation);
                    break;
                case Globals.GameStates.LoadAndSave:
                    UpdateDisplays(CurrentState, CurrentLocation);
                    break;
                case Globals.GameStates.Pause:
                    UpdateDisplays(CurrentState, CurrentLocation);
                    break;
                default:
                    break; 
            }
                    

        }

        // ================================================================================
        // ============================= Private methods ==================================
        // ================================================================================

        /// <summary>
        /// Updates during normal game run
        /// </summary>
        /// <param name="CurrentState">Mouse state</param>
        /// <param name="CurrentLocation">Position, for easier access</param>
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

        /// <summary>
        /// For during the room transitioning 
        /// </summary>
        /// <param name="CurrentState">meh</param>
        /// <param name="CurrentLocation">meh</param>
        private void UpdateTransitioning(MouseState CurrentState, Vector2 CurrentLocation)
        {
            // Nothing to do here
        }

        /// <summary>
        /// Update when in the bag view 
        /// </summary>
        /// <param name="CurrentState">Mouse state</param>
        /// <param name="CurrentLocation">Position, for easier access</param>
        private void UpdateDisplays(MouseState CurrentState, Vector2 CurrentLocation)
        {
            game.generalDisplay.UpdateCursorPos(CurrentLocation);

            // For left click 
            if (CurrentState.LeftButton == ButtonState.Pressed)
            {
                game.generalDisplay.LeftClickEvent(CurrentLocation);
            }
            else
            {
                game.generalDisplay.LeftClickRelease(CurrentLocation);
            }

        }

    }
}
