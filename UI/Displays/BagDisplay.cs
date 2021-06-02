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

    class BagDisplay
    {
        private Game1 game;
        private SpriteBatch spriteBatch;

        private Vector2 drawPosition = new Vector2(0, 0);
        private Color defaultTint = Color.White;

        private GeneralSprite bagViewBasic;
        private GeneralSprite bagViewUnderlay; 

        public BagDisplay(Game1 G)
        {
            game = G;

            spriteBatch = game.spriteBatch;

            LoadSprites();
        }

        private void LoadSprites()
        {
            ImageFile BDO = TextureFactory.Instance.BagViewBasic;
            ImageFile BDU = TextureFactory.Instance.BagViewUnderlay;

            bagViewBasic = new GeneralSprite(BDO.texture, BDO.C, BDO.R, 
                Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.UI_LAYER);
            bagViewUnderlay = new GeneralSprite(BDU.texture, BDU.C, BDU.R,
                Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.UI_UNDER);
        }


        public void Draw()
        {

            bagViewUnderlay.Draw(spriteBatch, drawPosition, defaultTint);
            bagViewBasic.Draw(spriteBatch, drawPosition, defaultTint);
        }

    }
}
