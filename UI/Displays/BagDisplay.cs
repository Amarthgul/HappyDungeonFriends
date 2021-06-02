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



        // ================================================================================
        // =========================== Sprites and their stats ============================
        // ================================================================================

        private GeneralSprite bagViewBasic;
        private GeneralSprite bagViewUnderlay;

        private GeneralSprite primarySlot;
        private GeneralSprite[] itemSlots;
        private Vector2 primaryLoc = new Vector2(112, 69) * Globals.SCALAR;
        private Vector2[] itemsLoc = new Vector2[] {
                new Vector2(137, 70) * Globals.SCALAR,
                new Vector2(158, 70) * Globals.SCALAR,
                new Vector2(177, 70) * Globals.SCALAR };

        private Vector2 drawPosition = new Vector2(0, 0);
        private Color defaultTint = Color.White;


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

            primarySlot = null; 
            itemSlots = new GeneralSprite[] { null, null, null }; 
        }

        public void Update()
        {
            primarySlot = game.headsupDisplay.GetSprite(-1);
            for(int i = 0; i < Globals.SLOT_SIZE; i++)
            {
                itemSlots[i] = game.headsupDisplay.GetSprite(i);
            }

        }

        public void Draw()
        {
            bagViewUnderlay.Draw(spriteBatch, drawPosition, defaultTint);

            // Draw the items in use 
            if (primarySlot != null)
                primarySlot.Draw(spriteBatch, primaryLoc, defaultTint);
            for (int i = 0; i < Globals.SLOT_SIZE; i++)
            {
                if (itemSlots[i] != null)
                    itemSlots[i].Draw(spriteBatch, itemsLoc[i], defaultTint);
            }

            bagViewBasic.Draw(spriteBatch, drawPosition, defaultTint);
        }

    }
}
