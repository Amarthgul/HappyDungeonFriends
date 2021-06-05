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
    /// <summary>
    /// Dependent on 2 things:
    /// headsup display, and items in bag 
    /// </summary>
    class BagDisplay
    {
        private Game1 game;
        private SpriteBatch spriteBatch;

        // ================================================================================
        // ============================= Parameters and data ==============================
        // ================================================================================



        // ================================================================================
        // =========================== Sprites and their stats ============================
        // ================================================================================

        private GeneralSprite bagViewBasic;
        private GeneralSprite bagViewUnderlay;

        private GeneralSprite onHoverNote; 

        private GeneralSprite primarySlot;
        private GeneralSprite[] itemSlots;
        private GeneralSprite[] bagSlots; 

        private Vector2 primaryLoc = new Vector2(112, 69) * Globals.SCALAR;
        private Vector2[] itemsLoc = new Vector2[] {
                new Vector2(137, 70) * Globals.SCALAR,
                new Vector2(158, 70) * Globals.SCALAR,
                new Vector2(179, 70) * Globals.SCALAR };
        private Vector2 bagLoc = new Vector2(112, 96) * Globals.SCALAR;

        private Vector2 drawPosition = new Vector2(0, 0);
        private Color defaultTint = Color.White;

        // ================================================================================
        // ============================== On hover regions ================================
        // ================================================================================
        private Rectangle primaryRange;
        private Rectangle[] itemsRange;
        private Rectangle bagRange;

        private bool primaryOnHover = false;
        private bool[] itemsOnHover = new bool[] { false, false, false };
        private bool bagOnHover = false;

        private Vector2 bagOnHoverIndexedLoc = new Vector2(0, 0); // Determining which slot will the hover not be in

        // Init 
        public BagDisplay(Game1 G)
        {
            game = G;

            spriteBatch = game.spriteBatch;

            LoadSprites();
            SetupRectangles();
        }

        /// <summary>
        /// Load and initlize all the sprites 
        /// </summary>
        private void LoadSprites()
        {
            ImageFile BDO = TextureFactory.Instance.BagViewBasic;
            ImageFile BDU = TextureFactory.Instance.BagViewUnderlay;
            ImageFile OHN = TextureFactory.Instance.SelectionNote; 

            bagViewBasic = new GeneralSprite(BDO.texture, BDO.C, BDO.R, 
                Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.UI_LAYER);
            bagViewUnderlay = new GeneralSprite(BDU.texture, BDU.C, BDU.R,
                Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.UI_UNDER);
            onHoverNote = new GeneralSprite(OHN.texture, OHN.C, OHN.R,
                Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.UI_ICONS);

            primarySlot = null; 
            itemSlots = new GeneralSprite[] { null, null, null };
            bagSlots = new GeneralSprite[Globals.BAG_SIZE];

            for (int i = 0; i < Globals.BAG_SIZE; i++) bagSlots[i] = null; 
        }

        /// <summary>
        /// Setup the rectangle ranges for onhover check 
        /// </summary>
        private void SetupRectangles()
        {
            primaryRange = new Rectangle(
                (int)primaryLoc.X, (int)primaryLoc.Y,
                Globals.OUT_UNIT, Globals.OUT_UNIT
                ) ;

            itemsRange = new Rectangle[] {
                new Rectangle(
                    (int)itemsLoc[0].X, (int)itemsLoc[0].Y,
                    Globals.OUT_UNIT, Globals.OUT_UNIT
                    ),
                new Rectangle(
                    (int)itemsLoc[1].X, (int)itemsLoc[1].Y,
                    Globals.OUT_UNIT, Globals.OUT_UNIT
                    ),
                new Rectangle(
                    (int)itemsLoc[2].X, (int)itemsLoc[2].Y,
                    Globals.OUT_UNIT, Globals.OUT_UNIT
                    )
                };

            bagRange = new Rectangle(
                112 * Globals.SCALAR, 96 * Globals.SCALAR,
                Globals.OUT_UNIT* 6, Globals.OUT_UNIT * 4
                );
        }

        private void ResetAllOnHover()
        {
            primaryOnHover = false;
            itemsOnHover = new bool[] { false, false, false };
            bagOnHover = false;
        }

        // ================================================================================
        // ============================== Public methods ==================================
        // ================================================================================

        public void UpdateOnhover(Vector2 CursorPos)
        {
            if (primaryRange.Contains(CursorPos))
                primaryOnHover = true;

            for (int i = 0; i < Globals.SLOT_SIZE; i++)
            {
                if (itemsRange[i].Contains(CursorPos))
                    itemsOnHover[i] = true;
            }

            if (bagRange.Contains(CursorPos))
            {
                bagOnHoverIndexedLoc = new Vector2(
                    Globals.OUT_UNIT * (int)(CursorPos.X / Globals.OUT_UNIT),
                    Globals.OUT_UNIT * (int)(CursorPos.Y / Globals.OUT_UNIT)
                    );
                bagOnHover = true;
            }
                
        }

        public void Update()
        {
            // Updates sprites in slots from the headsup 
            primarySlot = game.headsupDisplay.GetSprite(-1);
            for(int i = 0; i < Globals.SLOT_SIZE; i++)
            {
                itemSlots[i] = game.headsupDisplay.GetSprite(i);
            }


            for (int i = 0; i < Globals.BAG_SIZE; i++)
            {
                if (game.spellSlots.bag[i] != null)
                {
                    bagSlots[i] = game.spellSlots.bag[i].GetSprite();
                    bagSlots[i].layer = Globals.UI_SLOTS;
                }
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

            // Draw items in the bag 
            for (int i = 0; i < Globals.BAG_SIZE; i++)
            {
                if (bagSlots[i] != null)
                {
                    Vector2 ItemDrawPos = new Vector2(
                        (i / 6) * Globals.SCALAR,
                        (i % 6) * Globals.SCALAR
                        ) + bagLoc;
                    bagSlots[i].Draw(spriteBatch, ItemDrawPos, defaultTint);
                }
            }

            bagViewBasic.Draw(spriteBatch, drawPosition, defaultTint);


            // On hover selection notes
            if (primaryOnHover)
                onHoverNote.Draw(spriteBatch, primaryLoc, defaultTint);
            for (int i = 0; i < Globals.SLOT_SIZE; i++)
            {
                if (itemsOnHover[i])
                    onHoverNote.Draw(spriteBatch, itemsLoc[i], defaultTint);
            }
            if (bagOnHover)
                onHoverNote.Draw(spriteBatch, bagOnHoverIndexedLoc, defaultTint);

            // Revoke all on hover effects for next time 
            ResetAllOnHover();
        }

    }
}
