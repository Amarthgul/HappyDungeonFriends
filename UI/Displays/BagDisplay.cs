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
        private const int HOVERING_HORIZONTAL_SWITCH = 144 * Globals.SCALAR;
        private const int HOVERING_VERTICAL_TOLERANCE = 16 * Globals.SCALAR;
        private const int HOVERING_WIDTH = 84 * Globals.SCALAR;

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
        // TODO: add description sprites

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
        private bool[] bagOnHoverSFX = new bool[Globals.BAG_SIZE];

        private Vector2 bagOnHoverIndexedLoc = new Vector2(0, 0); // Determining which slot will the hover not be in

        // Hovering descriptions 
        private Vector2 deltaTrigger = new Vector2(.5f, .5f) * Globals.SCALAR;
        private Vector2 lastHoveringPos = new Vector2();
        private Vector2 hoveringDrawPos;
        private GeneralSprite hoveringDescriptionText;
        private GeneralSprite hoveringDescriptionBox;
        private Stopwatch hoveringSW = new Stopwatch();
        private Stopwatch hoveringPreCastSW = new Stopwatch();
        private int hoveringUpdateInterval = 100;
        private int hoveringPreCastPeriod = 500;
        private bool isStationary = false;
        private bool hoveringPreCasting = false;
        private bool hoveringDisplay = false;

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

        // --------------------------------------------------------------------------------
        // ------------------------ Switch for keyboard control ---------------------------
        private bool KBS = false;  // Keyboard session flag 
        private int KBIH = 0;      // Keyboard Index Horizontal 
        private int KBIV = 0;      // Keyboard Index Vertical 
        private Stopwatch KBVSW = new Stopwatch();  // Keyboard Vertical Stopwatch
        private Stopwatch KBHSW = new Stopwatch();  // Keyboard Horizontal Stopwatch
        private Stopwatch optionProtectionSW = new Stopwatch(); // Avoid repetitive triggering of Enter key
        private int KBInterval = 200;               // The one to use 
        private int optionConfirmProtection = 500;  // Cooldown for next valid Enter key confirm

        // Init 
        public BagDisplay(Game1 G)
        {
            game = G;

            spriteBatch = game.spriteBatch;

            LoadSprites();
            SetupRectangles();
            UnselectAll(); // Use as initliziation 

            hoveringSW.Restart();
            optionProtectionSW.Restart();
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

            hoveringDescriptionText = new GeneralSprite(TextureFactory.Instance.bagOnHoverBoxTop.texture,
                1, 1, Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.UI_TEXT_LAYER);
            hoveringDescriptionBox = new GeneralSprite(TextureFactory.Instance.bagOnHoverBoxTop.texture,
                1, 1, Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.UI_TEXT_SHADOW);
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
        /// Reset certain on hover effects. 
        /// 0 for reset all;
        /// 1 for reset all but primary;
        /// 2 for reset all but items;
        /// 3 for reset all but bag view;
        /// </summary>
        /// <param name="Mode">Different mode flag</param>
        private void ResetOnHover(int Mode)
        {
            switch (Mode)
            {
                case 0:
                    itemsOnHover = new bool[] { false, false, false };
                    bagOnHover = false;
                    primaryOnHover = false;
                    ResetBagOnHoverSFX(-1);
                    break;
                case 1:
                    itemsOnHover = new bool[] { false, false, false };
                    bagOnHover = false;
                    break;
                case 2:
                    primaryOnHover = false;
                    bagOnHover = false;
                    break;
                case 3:
                    itemsOnHover = new bool[] { false, false, false };
                    primaryOnHover = false;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Reset the sound effect flags for bag slots. 
        /// With either 1 or no exemption. 
        /// </summary>
        /// <param name="Excemption">Excluded slot index, negative for no excemption</param>
        private void ResetBagOnHoverSFX(int Excemption)
        {
            for (int i = 0; i < Globals.BAG_SIZE; i++)
            {
                if (i != Excemption)
                    bagOnHoverSFX[i] = false;
            }
        }

        /// <summary>
        /// Check if the mouse is staying relatively stationary fora while.
        /// The triggering time is defined as `hoveringPreCastPeriod`. 
        /// </summary>
        /// <param name="CursorPos">Current position of the cursor</param>
        /// <returns>True if it barely moves in the past half second or so</returns>
        private bool CheckStationary(Vector2 CursorPos)
        {
            if (hoveringSW.ElapsedMilliseconds < hoveringUpdateInterval)
                return isStationary;
            else
            {
                bool Result = false;

                Vector2 CurrentDelta = CursorPos - lastHoveringPos;
                if (Math.Abs(CurrentDelta.X) < deltaTrigger.X && Math.Abs(CurrentDelta.Y) < deltaTrigger.Y)
                {
                    if (hoveringPreCasting)
                    {
                        if (hoveringPreCastSW.ElapsedMilliseconds > hoveringPreCastPeriod)
                        {
                            Result = true;
                        }
                    }
                    else
                    {
                        hoveringPreCasting = true;
                        hoveringPreCastSW.Restart();
                    }
                }
                else
                {
                    hoveringPreCasting = false;
                }

                lastHoveringPos = CursorPos; 

                return Result;
            }
        }

        /// <summary>
        /// Initilize the hovering decription display. get the textures and set their positions. 
        /// </summary>
        /// <param name="TargetItem">What item to display</param>
        /// <param name="DrawPos">Possibly where to draw it</param>
        private void InitHoveringDisplay(IItem TargetItem, Vector2 DrawPos)
        {
            GenerateDescription Gen = new GenerateDescription(game.GraphicsDevice, TargetItem);

            hoveringDrawPos = new Vector2(DrawPos.X + 6 * Globals.SCALAR, DrawPos.Y);

            hoveringDescriptionText.selfTexture = Gen.GetTexture();
            hoveringDescriptionText.positionOffset = Gen.TextPositionOffset() * Globals.SCALAR;
            hoveringDescriptionText.Refresh();

            hoveringDescriptionBox.selfTexture = Gen.GetBox();
            hoveringDescriptionBox.Refresh();

            if (DrawPos.X > HOVERING_HORIZONTAL_SWITCH)
                hoveringDrawPos.X = DrawPos.X - HOVERING_WIDTH; 
            if (DrawPos.Y + hoveringDescriptionBox.selfTexture.Height > 
                Globals.OUT_FHEIGHT - HOVERING_VERTICAL_TOLERANCE)
                hoveringDrawPos.Y = DrawPos.Y - hoveringDescriptionBox.selfTexture.Height * Globals.SCALAR;
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

        /// <summary>
        /// Mark highlight depending on which option is being selected by keyboard. 
        /// </summary>
        private void RefreshKBS()
        {
            
        }

        /// <summary>
        /// Change KBIV and KBIH accroding to mouse hovering. 
        /// 
        /// </summary>
        /// <param name="Target">Which option to mark</param>
        private void ReverseKBS(int Target)
        {
            

        }

        /// <summary>
        /// Check if KBIs have negative risk, if so, make it positive. 
        /// </summary>
        private void RestoreKBI()
        {
            
        }

        /// <summary>
        /// Start a KBS session
        /// </summary>
        /// <param name="Vertical">if it's up and down key being pressed</param>
        private void StartKBS(bool Vertical)
        {
            KBS = true;
            RefreshKBS();
            SoundFX.Instance.PlayTitleOnHover();
            if (Vertical)
                KBVSW.Restart();
            else
                KBHSW.Restart();
        }

        // ================================================================================
        // ============================== Public methods ==================================
        // ================================================================================

        public void OptionMoveUp()
        {
            RestoreKBI();
            if (!KBS)
            {
                StartKBS(true);
            }
            else if (KBVSW.ElapsedMilliseconds > KBInterval)
            {
                KBIV--;
                RefreshKBS();
                SoundFX.Instance.PlayTitleOnHover();
                KBVSW.Restart();
            }
        }

        public void OptionMoveDown()
        {
            if (!KBS)
            {
                StartKBS(true);
            }
            else if (KBVSW.ElapsedMilliseconds > KBInterval)
            {
                KBIV++;
                RefreshKBS();
                SoundFX.Instance.PlayTitleOnHover();
                KBVSW.Restart();
            }
        }

        public void OptionMoveLeft()
        {
            RestoreKBI();
            if (!KBS)
            {
                StartKBS(false);
            }
            else if (KBHSW.ElapsedMilliseconds > KBInterval)
            {
                KBIH--;
                RefreshKBS();
                SoundFX.Instance.PlayTitleOnHover();
                KBHSW.Restart();
            }
        }

        public void OptionMoveRight()
        {
            if (!KBS)
            {
                StartKBS(false);
            }
            else if (KBHSW.ElapsedMilliseconds > KBInterval)
            {
                KBIH++;
                RefreshKBS();
                SoundFX.Instance.PlayTitleOnHover();
                KBHSW.Restart();
            }
        }

        public void OptionConfirm()
        {
            
        }

        /// <summary>
        /// Dealing with left click. Either marks the start of a left click session,
        /// or updating the left click seesion. 
        /// </summary>
        /// <param name="CursorPos">Position of the cursor</param>
        public void LeftClickEvent(Vector2 CursorPos)
        {
            bool Selected = false;

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
                Selected = true; 

                leftClickInProcess = true;
                lastRecoredCursor = CursorPos;

            }
            else if (bagRange.Contains(CursorPos))
            {
                bagItemSelectionIndex = (int)((CursorPos.X - bagLoc.X) / Globals.OUT_UNIT)
                    + (int)((CursorPos.Y - bagLoc.Y) / Globals.OUT_UNIT) * Globals.BAG_COL;
                if (bagSlots[bagItemSelectionIndex] == null) return;
                itemSelected = game.spellSlots.GetBagItem(bagItemSelectionIndex);
                Selected = true;

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
                        Selected = true;

                        leftClickInProcess = true;
                        lastRecoredCursor = CursorPos;
                    }
                }
            }

            if (Selected) {
                SoundFX.Instance.PlayBagLMBSelect();
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
            bool Released = false;

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
                        Released = true;
                    }
                    else if (game.spellSlots.CanPutInPrimary(game.spellSlots.GetBagItem(TBP)))
                    {
                        game.spellSlots.PutIntoPrimary(game.spellSlots.GetBagItem(TBP));
                        game.spellSlots.PutIntoBag(itemSelected, TBP);
                        SpecialCasesPrimary(itemSelected);
                        Released = true;
                    }
                } 
                else if (slotSelected.Contains(true)) // Move from usable slots 
                {
                    int TBE = slotSelected.ToList().FindIndex(x => x == true);
                    if (game.spellSlots.GetBagItem(TBP) == null)
                    {
                        game.spellSlots.PutIntoUsable(null, TBE);
                        game.spellSlots.PutIntoBag(itemSelected, TBP);
                        Released = true;
                    }
                    else if (game.spellSlots.CanPutInUsable(game.spellSlots.GetBagItem(TBP)))
                    {
                        game.spellSlots.PutIntoUsable(game.spellSlots.GetBagItem(TBP), TBE);
                        game.spellSlots.PutIntoBag(itemSelected, TBP);
                        Released = true;
                    }
                } 
                else if (bagslotSelected.Contains(true)) // move between bag items 
                {
                    game.spellSlots.PutIntoBag(game.spellSlots.GetBagItem(TBP), bagItemSelectionIndex);
                    game.spellSlots.PutIntoBag(itemSelected, TBP);
                    Released = true;
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
                            Released = true;
                        }
                        else if (slotSelected.Contains(true)) // Move inbetween usable slots 
                        {
                            int TBE = slotSelected.ToList().FindIndex(x => x == true);
                            game.spellSlots.PutIntoUsable(game.spellSlots.GetItem(i), TBE);
                            game.spellSlots.PutIntoUsable(itemSelected, i);
                            Released = true;
                        } 
                        else if (bagslotSelected.Contains(true))
                        {
                            int TBE = bagslotSelected.ToList().FindIndex(x => x == true);
                            if (game.spellSlots.CanPutInUsable(game.spellSlots.GetBagItem(TBE)))
                            {
                                
                                if (game.spellSlots.GetItem(i) == null) {
                                    game.spellSlots.PutIntoBag(null, TBE);
                                    Released = true;
                                }
                                else {
                                    game.spellSlots.PutIntoBag(game.spellSlots.GetItem(i), TBE);
                                    Released = true;
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

            if (Released) {
                SoundFX.Instance.PlayBagLMBRelease();
            }
        }

        /// <summary>
        /// Marks onhover effect for the draw method, also checks hovering descriptions. 
        /// </summary>
        /// <param name="CursorPos">Position of the cursor</param>
        public void UpdateOnhover(Vector2 CursorPos)
        {
            bool OnHoverSFX = false;
            bool OnHoverDetected = false;

            isStationary = CheckStationary(CursorPos);

            if (primaryRange.Contains(CursorPos))
            {
                if (!primaryOnHover) OnHoverSFX = true;

                // Check for whether or not to display the item description 
                if (isStationary && !hoveringDisplay)  {
                    hoveringDisplay = true;
                    InitHoveringDisplay(game.spellSlots.GetItem(-1), CursorPos);
                }
                else if (! isStationary) {
                    hoveringDisplay = false;
                }

                primaryOnHover = true;
                OnHoverDetected = true;
                ResetOnHover(1);
            }
            // Bag
            else if (bagRange.Contains(CursorPos))
            {
                int TBP = (int)((CursorPos.X - bagLoc.X) / Globals.OUT_UNIT)
                    + (int)((CursorPos.Y - bagLoc.Y) / Globals.OUT_UNIT) * Globals.BAG_COL;
                bagOnHoverIndexedLoc = new Vector2(
                    Globals.OUT_UNIT * (int)(CursorPos.X / Globals.OUT_UNIT),
                    Globals.OUT_UNIT * (int)(CursorPos.Y / Globals.OUT_UNIT)
                    );
                bagOnHover = true;

                // Play on hover sound 
                if (!bagOnHoverSFX[TBP]) {
                    OnHoverSFX = true;
                    bagOnHoverSFX[TBP] = true;
                    ResetBagOnHoverSFX(TBP);
                }

                // Check for whether or not to display the item description 
                if (isStationary && !hoveringDisplay && game.spellSlots.GetBagItem(TBP) != null) {
                    hoveringDisplay = true;
                    InitHoveringDisplay(game.spellSlots.GetBagItem(TBP), CursorPos);
                }
                else if (!isStationary) {
                    hoveringDisplay = false;
                }

                OnHoverDetected = true;
                ResetOnHover(3);
            }
            // The 3 slots on top
            else
            {
                for (int i = 0; i < Globals.SLOT_SIZE; i++){
                    if (itemsRange[i].Contains(CursorPos)) {
                        if (!itemsOnHover[i]) OnHoverSFX = true;

                        // Check for whether or not to display the item description 
                        if (isStationary && !hoveringDisplay && game.spellSlots.GetItem(i) != null){
                            hoveringDisplay = true;
                            InitHoveringDisplay(game.spellSlots.GetItem(i), CursorPos);
                        }
                        else if (!isStationary) {
                            hoveringDisplay = false;
                        }

                        itemsOnHover[i] = true;
                        OnHoverDetected = true;
                        ResetOnHover(2);
                    }   
                }
            }

            if (!OnHoverDetected) {
                ResetOnHover(0);
            }

            if (OnHoverSFX) { // There can only be one on hover at a time 
                SoundFX.Instance.PlayBagItemOnhover();
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

            if (hoveringDisplay)
            {
                hoveringDescriptionText.Draw(spriteBatch, hoveringDrawPos, defaultTint);
                hoveringDescriptionBox.Draw(spriteBatch, hoveringDrawPos, defaultTint);
            }
                

        }

    }
}
