using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace HappyDungeon
{
    public class GeneralDisplay
    {

        private Game1 game;
        private SpriteBatch spriteBatch;

        private UI.Displays.BagDisplay bagDisplay;
        private UI.Displays.TitleScreenDisplay titleDisplay;
        private UI.Displays.SettingDisplay settingDisplay; 

        public GeneralDisplay(Game1 G)
        {
            game = G;

            spriteBatch = game.spriteBatch;

            InitDisplays();
        }

        private void InitDisplays()
        {
            bagDisplay = new UI.Displays.BagDisplay(game);

        }

        public void ResumeToRunning()
        {
            game.gameState = Globals.GameStates.Running;
        }

        public void LeftClickRelease(Vector2 CursorPos)
        {
            switch (game.gameState)
            {
                case Globals.GameStates.Bag:
                    bagDisplay.LeftClickRelease(CursorPos);
                    break;
                case Globals.GameStates.TitleScreen:
                    break;
                default:
                    break;
            }
        }

        public void LeftClickEvent(Vector2 CursorPos)
        {
            switch (game.gameState)
            {
                case Globals.GameStates.Bag:
                    bagDisplay.LeftClickEvent(CursorPos);
                    break;
                case Globals.GameStates.TitleScreen:
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Update cursor position mostly for on hover effects 
        /// </summary>
        /// <param name="CursorPos">Curosr position</param>
        public void UpdateCursorPos(Vector2 CursorPos)
        {
            switch (game.gameState)
            {
                case Globals.GameStates.Bag:
                    bagDisplay.UpdateOnhover(CursorPos);
                    break;
                case Globals.GameStates.TitleScreen:
                    break;
                default:
                    break;
            }
        }

        public void Update()
        {
            switch (game.gameState)
            {
                case Globals.GameStates.Bag:
                    bagDisplay.Update();
                    break;
                case Globals.GameStates.TitleScreen:
                    break;
                default:
                    break;
            }
        }

        public void Draw()
        {
            switch (game.gameState)
            {
                case Globals.GameStates.Bag:
                    bagDisplay.Draw();
                    break;
                case Globals.GameStates.TitleScreen:
                    break;
                default:
                    break; 
            }
        }



    }
}
