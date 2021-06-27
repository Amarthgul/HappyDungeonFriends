using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;


namespace HappyDungeon.UI.Displays
{
    class GameOverDisplay
    {
        private const int OPTION_COUNT = 2; 

        private Game1 game;
        private SpriteBatch spriteBatch;

        private GeneralSprite deadBG;
        private GeneralSprite[] options;
        private GeneralSprite[] optionOnHover;

        private Vector2 drawPosition = new Vector2(0, 0);
        private Vector2 optionPos; 

        // Text generator 
        private LargeBR textGen = new LargeBR();
        private LargeWR textOnHoverGen = new LargeWR();

        
        private Color defaultTint = Color.White;

        public GameOverDisplay(Game1 G)
        {
            game = G;

            spriteBatch = game.spriteBatch;

            LoadSprites();

        }

        private void LoadSprites()
        {
            ImageFile DBG = TextureFactory.Instance.gameOverDeadBG;

            deadBG = new GeneralSprite(DBG.texture, DBG.C, DBG.R,
                Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.UI_UNDER);
        }

        public void LeftClickEvent(Vector2 CursorPos)
        {

        }

        public void LeftClickRelease(Vector2 CursorPos)
        {

        }

        public void UpdateOnhover(Vector2 CursorPos)
        {

        }

        public void Update()
        {

        }

        public void Draw()
        {
            deadBG.Draw(spriteBatch, drawPosition, defaultTint);


        }

    }
}
