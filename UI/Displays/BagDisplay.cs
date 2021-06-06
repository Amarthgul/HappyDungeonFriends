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

        // ================================================================================
        // ============================ Click modifications ===============================
        // ================================================================================
        private bool primarySelected;
        private bool[] slotSelected = new bool[Globals.SLOT_SIZE];
        private bool[] bagslotSelected = new bool[Globals.BAG_SIZE];

        private Vector2 selectionOffset = new Vector2();

        private bool leftClickInProcess = false;
        private Vector2 lastRecoredCursor = new Vector2(); // When left clicked being pressed 

        private int bagItemSelectionIndex;
        private IItem itemSelected;

        // Init 
        public BagDisplay(Game1 G)
        {
            game = G;

            spriteBatch = game.spriteBatch;

            LoadSprites();
            SetupRectangles();
            UnselectAll(); // Use as initliziation 
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
                );

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
                Globals.OUT_UNIT * 6, Globals.OUT_UNIT * 4
                );
        }

        /// <summary>
        /// Mark all bag select as false, both for init and recover 
        /// </summary>
        private void UnselectAll()
        {
            primarySelected = false;
            slotSelected = new bool[] { false, false, false };
            for (int i = 0; i < Globals.BAG_SIZE; i++)
            {
                bagslotSelected[i] = false;
            }
        }

        /// <summary>
        /// Mark all on hover effect as false. 
        /// Used to recover and refresh from on hover.
        /// </summary>
        private void ResetAllOnHover()
        {
            primaryOnHover = false;
            itemsOnHover = new bool[] { false, false, false };
            bagOnHover = false;
        }

        private void SpecialCasesPrimary(IItem Item)
        {
            if (Item is Torch && ((Torch)Item).torchOn)
            {
                ((Torch)Item).torchOn = false; 
                game.mainChara.primaryState = Globals.primaryTypes.None;
                game.fogOfWar.clairvoyantMode = false;
                game.fogOfWar.shakyMode = false;
            }
            
        }
        // ================================================================================
        // ============================== Public methods ==================================
        // ================================================================================

        /// <summary>
        /// Dealing with left click. Either marks the start of a left click session,
        /// or updating the left click seesion. 
        /// </summary>
        /// <param name="CursorPos">Position of the cursor</param>
        public void LeftClickEvent(Vector2 CursorPos)
        {
            if (leftClickInProcess) // Update the offset if something is already selected 
            {
                selectionOffset = CursorPos - lastRecoredCursor;
            }
            // Mark the start of LMB session
            else if (primaryRange.Contains(CursorPos) && primarySlot != null)
            { 
                primarySelected = true;
                primarySlot.layer = Globals.UI_ICONS;
                itemSelected = game.spellSlots.GetItem(-1);

                leftClickInProcess = true;
                lastRecoredCursor = CursorPos;

            }
            else if (bagRange.Contains(CursorPos))
            {
                bagItemSelectionIndex = (int)((CursorPos.X - bagLoc.X) / Globals.OUT_UNIT)
                    + (int)((CursorPos.Y - bagLoc.Y) / Globals.OUT_UNIT) * Globals.BAG_COL;
                if (bagSlots[bagItemSelectionIndex] == null) return;
                itemSelected = game.spellSlots.GetBagItem(bagItemSelectionIndex);

                bagslotSelected[bagItemSelectionIndex] = true;
                bagSlots[bagItemSelectionIndex].layer = Globals.UI_ICONS;

                leftClickInProcess = true;
                lastRecoredCursor = CursorPos;
            }
            else { 
                for (int i = 0; i < Globals.SLOT_SIZE; i++)
                {
                    if (itemsRange[i].Contains(CursorPos) && itemSlots[i] != null)
                    {
                        slotSelected[i] = true;
                        itemSlots[i].layer = Globals.UI_ICONS;
                        itemSelected = game.spellSlots.GetItem(i);

                        leftClickInProcess = true;
                        lastRecoredCursor = CursorPos;
                    }
                }
            }
            
        }

        /// <summary>
        /// Dealing with LMB release, mostly likely to be moving items from and between 
        /// primary slot, useable slots, and bag slots. 
        /// BTW I fucking hate this, duplications everywhere. 
        /// </summary>
        /// <param name="CursorPos">Where the cursor is released</param>
        public void LeftClickRelease(Vector2 CursorPos)
        {
            if (!leftClickInProcess) return;
            // Quit if it's not yet a left click session

            // ---------------------- Try to put something in primary --------------------------
            if (primaryRange.Contains(CursorPos)) 
            {
                if (game.spellSlots.CanPutInPrimary(itemSelected))
                {
                    if (slotSelected.Contains(true)) // If it's from usable slot 
                    {
                        int TBP = slotSelected.ToList().FindIndex(x => x == true);

                        game.spellSlots.PutIntoUsable(game.spellSlots.GetItem(-1), TBP);
                    }
                    else if (bagslotSelected.Contains(true))
                    {
                        int TBP = bagslotSelected.ToList().FindIndex(x => x == true);
                        game.spellSlots.PutIntoBag(null, TBP);
                    } 

                    // Finally, put the item into primary 
                    game.spellSlots.PutIntoPrimary(itemSelected);
                }
            }
            // -------------------- Trying to drop/replace it in bag ---------------------------
            else if (bagRange.Contains(CursorPos)) 
            {
                int TBP = (int)((CursorPos.X - bagLoc.X) / Globals.OUT_UNIT)
                    + (int)((CursorPos.Y - bagLoc.Y) / Globals.OUT_UNIT) * Globals.BAG_COL;

                if (primarySelected) // If it's from the primary 
                {
                    if (game.spellSlots.bag[TBP] == null)
                    {
                        game.spellSlots.PutIntoPrimary(null);
                        game.spellSlots.PutIntoBag(itemSelected, TBP);
                        SpecialCasesPrimary(itemSelected);
                    }
                    else if (game.spellSlots.CanPutInPrimary(game.spellSlots.GetBagItem(TBP)))
                    {
                        game.spellSlots.PutIntoPrimary(game.spellSlots.GetBagItem(TBP));
                        game.spellSlots.PutIntoBag(itemSelected, TBP);
                        SpecialCasesPrimary(itemSelected);
                    }
                } 
                else if (slotSelected.Contains(true)) // Move from usable slots 
                {
                    int TBE = slotSelected.ToList().FindIndex(x => x == true);
                    if (game.spellSlots.GetBagItem(TBP) == null)
                    {
                        game.spellSlots.PutIntoUsable(null, TBE);
                        game.spellSlots.PutIntoBag(itemSelected, TBP);
                    }
                    else if (game.spellSlots.CanPutInUsable(game.spellSlots.GetBagItem(TBP)))
                    {
                        game.spellSlots.PutIntoUsable(game.spellSlots.GetBagItem(TBP), TBE);
                        game.spellSlots.PutIntoBag(itemSelected, TBP);
                    }
                } 
                else if (bagslotSelected.Contains(true)) // move between bag items 
                {
                    game.spellSlots.PutIntoBag(game.spellSlots.GetBagItem(TBP), bagItemSelectionIndex);
                    game.spellSlots.PutIntoBag(itemSelected, TBP);
                }

            }
            // ---------------------- Drop things into usable slots ---------------------------
            else
            {
                for (int i = 0; i < Globals.SLOT_SIZE; i++)
                {
                    if (itemsRange[i].Contains(CursorPos) && game.spellSlots.CanPutInUsable(itemSelected))
                    {
                        if (primarySelected && game.spellSlots.CanPutInPrimary(game.spellSlots.GetItem(i)))
                        {   // If that thing can exchange with primary 
                            game.spellSlots.PutIntoPrimary(game.spellSlots.GetItem(i));
                            game.spellSlots.PutIntoUsable(itemSelected, i);
                            SpecialCasesPrimary(itemSelected);
                        }
                        else if (slotSelected.Contains(true)) // Move inbetween usable slots 
                        {
                            int TBE = slotSelected.ToList().FindIndex(x => x == true);
                            game.spellSlots.PutIntoUsable(game.spellSlots.GetItem(i), TBE);
                            game.spellSlots.PutIntoUsable(itemSelected, i);
                        } else if (bagslotSelected.Contains(true))
                        {
                            int TBE = bagslotSelected.ToList().FindIndex(x => x == true);
                            if (game.spellSlots.CanPutInUsable(game.spellSlots.GetBagItem(TBE)))
                            {
                                
                                if (game.spellSlots.GetItem(i) == null) {
                                    game.spellSlots.PutIntoBag(null, TBE);
                                }
                                else {
                                    game.spellSlots.PutIntoBag(game.spellSlots.GetItem(i), TBE);
                                }
                                game.spellSlots.PutIntoUsable(itemSelected, i);
                            }

                        }
                        
                    }
                }
            }

            // Released all holds of click seesion 
            UnselectAll();
            leftClickInProcess = false;
            selectionOffset = new Vector2(0, 0);
        }

        /// <summary>
        /// Marks onhover effect for the draw method 
        /// </summary>
        /// <param name="CursorPos">Position of the cursor</param>
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
                else
                {
                    bagSlots[i] = null; 
                }
            }
        }

        /// <summary>
        /// Draw UI, items, stats
        /// </summary>
        public void Draw()
        {
            bagViewUnderlay.Draw(spriteBatch, drawPosition, defaultTint);

            // Draw the items in use 
            if (primarySlot != null) {
                if (primarySelected) {
                    primarySlot.Draw(spriteBatch, primaryLoc + selectionOffset, defaultTint);
                }
                else {
                    primarySlot.Draw(spriteBatch, primaryLoc, defaultTint);
                }
            }
            
            // Draw items in usable slots 
            for (int i = 0; i < Globals.SLOT_SIZE; i++)
            {
                if (itemSlots[i] != null) {
                    if (slotSelected[i])  {
                        itemSlots[i].Draw(spriteBatch, itemsLoc[i] + selectionOffset, defaultTint);
                    }
                    else {
                        itemSlots[i].Draw(spriteBatch, itemsLoc[i], defaultTint);
                    }
                } 
            }

            // Draw items in the bag 
            for (int i = 0; i < Globals.BAG_SIZE; i++)
            {
                if (bagSlots[i] != null)
                {
                    Vector2 ItemDrawPos = new Vector2(
                        (i % 6) * Globals.OUT_UNIT,
                        (i / 6) * Globals.OUT_UNIT
                        ) + bagLoc; // Determines position to draw 
                    if (bagslotSelected[i])  {
                        bagSlots[i].Draw(spriteBatch, ItemDrawPos + selectionOffset, defaultTint);
                    }
                    else {
                        bagSlots[i].Draw(spriteBatch, ItemDrawPos, defaultTint);
                    }
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
