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
    class GeneralDisplay
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

        public void Update()
        {
            switch (game.gameState)
            {
                case Globals.GameStates.Bag:
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
                    break;
                case Globals.GameStates.TitleScreen:
                    break;
                default:
                    break; 
            }
        }



    }
}
