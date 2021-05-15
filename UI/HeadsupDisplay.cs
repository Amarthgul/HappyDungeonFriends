using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace HappyDungeon
{
    public class HeadsupDisplay : IUIElement
    {
        private const int BLOOD_TX_SIZE = 35;
        private const int PANEL_HOVER_HEIGHT = 17 * Globals.SCALAR;
        private const int AREA_HEIGHT_BOUND = 48 * Globals.SCALAR;
        private const int BLOOD_HOVER_RAD = 23 * Globals.SCALAR; // Radius of the blood area, used for UI area cauculation 
        private const int BAG_SIZE = 13; 

        private Game1 game;
        private SpriteBatch spriteBatch;

        // ================================================================================
        // =========================== Sprites and their stats ============================
        // ================================================================================
        
        private GeneralSprite primarySlot;
        private GeneralSprite[] itemSlots;
        private GeneralSprite primaryMask;  // Draw on slot to make item more obvious 
        private GeneralSprite itemMask;
        private float maskOpacity = 0.5f;

        private GeneralSprite goldCountText;
        private GeneralSprite goldCountTextShadow;
        private GeneralSprite goldIcon;
        private GeneralSprite bagIcon; 

        private GeneralSprite uiFront;
        private GeneralSprite uiBack;
        private GeneralSprite health;
        private GeneralSprite healthSignifier;

        private UI.DigitsSmall digitsGenerator;
        private UI.LargeBR fontLBRGenerator; 
        private GeneralSprite healthText;
        private GeneralSprite healthTextDropS;

        private GeneralSprite[] altDisplays; 

        private Texture2D bloodFill;
        private Texture2D bloodSig; 
        private Texture2D currentBlood;
        private Texture2D currentSig; 

        // ================================================================================
        // ================= Draw locations, on hover and click check =====================
        // ================================================================================

        private int primaryState;
        private int[] itemStates; 

        // Location of UI panel and some elements in it
        private Vector2 drawPosition = new Vector2(0, 0);
        private Vector2 bloodPosition = new Vector2(217, 2 );
        private Vector2 bagLocation = new Vector2(175, 1);
        //Location of the slots 
        private Vector2 primaryLocation = new Vector2(91, 3);
        private Vector2[] itemLocations = new Vector2[] {
            new Vector2(114, 1),
            new Vector2(135, 1),
            new Vector2(156, 1) };
        //Location of some on-hover info
        private Vector2 bloodTextLocation = new Vector2(234, 18);
        private Vector2 bloodTextLocationUpdate; 
        private Vector2 textShadowOffset = new Vector2(1, -1);
        // Positions of the texts when pressing alt key 
        private Vector2[] altDisplayPositions = new Vector2[] {
                new Vector2(96, 19),
                new Vector2(118, 16),
                new Vector2(140, 16),
                new Vector2(161, 16),
                new Vector2(178, 14),
             };
        private Vector2 altGoldCountLocation = new Vector2(76, 14); 

        private Rectangle primaryRange;   // On hover and click 
        private Rectangle[] itemRange;
        private Rectangle bagRange;
        private Rectangle bloodRange; 

        private Color defaultTint = Color.White;
        private Color dropShadow = Color.Black;
        private Color bloodColor = new Color(148, 22, 20);
        private Color maskColor = new Color(50, 0, 0);
        private Color healthSig = Color.White; 
        private Color transparent = Color.Transparent;

        // ================================================================================
        // ================================= Parameters ===================================
        // ================================================================================
        private int oldHealth;
        private int oldMaxHealth;
        private int oldGoldCount;
        private bool healthSignifierOn = false;
        private bool onHoverHP = false;
        private bool altDisplayOn = false; 

        public HeadsupDisplay(Game1 G)
        {
            game = G;

            spriteBatch = game.spriteBatch;

            oldHealth = game.mainChara.currentHealth;
            oldMaxHealth = game.mainChara.currentMaxHP;
            oldGoldCount = -1; // force an update at the start 

            primaryState = 0;
            itemStates = new int[] { 0, 0, 0 };
            primarySlot = null;
            itemSlots = new GeneralSprite[] { null, null, null };

            SetUpSprites();
            SetUpHealthVessel();
            SetUpMasks();
            SetUpRanges();
            RedrawBloodVessel();
            // Text and auxiliary displays 
            UpdateHealthText();
            SetUpAltDisplays();
            CheckGoldChange();
        }

        /// <summary>
        /// Re-render the texture of the digits representing character health.  
        /// </summary>
        private void UpdateHealthText()
        {
            digitsGenerator = new UI.DigitsSmall();

            Texture2D HPT = digitsGenerator.GetText((oldHealth.ToString() + "/" + oldMaxHealth.ToString()),
                game.GraphicsDevice);
            healthText = new GeneralSprite(HPT, 1, 1, Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.UI_TEXT_LAYER);

            digitsGenerator.useBlack = true;
            HPT = digitsGenerator.GetText((oldHealth.ToString() + "/" + oldMaxHealth.ToString()),
                game.GraphicsDevice);
            healthTextDropS = new GeneralSprite(HPT, 1, 1, Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.UI_TEXT_SHADOW);

            Vector2 WidthOffset = new Vector2(HPT.Width / 2, 0);
            bloodTextLocationUpdate = bloodTextLocation - WidthOffset;
        }

        /// <summary>
        /// Putting these in init would be too tideous, so there they are. 
        /// </summary>
        private void SetUpSprites()
        {

            ImageFile UIF = TextureFactory.Instance.uiFront;
            ImageFile UIB = TextureFactory.Instance.uiBack;
            ImageFile GOB = TextureFactory.Instance.goldOnBar;
            ImageFile BOB = TextureFactory.Instance.bagOnBar;

            uiFront = new GeneralSprite(UIF.texture, UIF.C, UIF.R,
                Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.UI_LAYER);
            uiBack = new GeneralSprite(UIB.texture, UIB.C, UIB.R,
                Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.UI_UNDER);

            goldIcon = new GeneralSprite(GOB.texture, GOB.C, GOB.R,
                Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.UI_ICONS);
            bagIcon = new GeneralSprite(BOB.texture, BOB.C, BOB.R,
                0, BOB.C * BOB.R, Globals.UI_ICONS);

        }

        /// <summary>
        /// Setup the texture and sprite for top-right corner HP.  
        /// </summary>
        private void SetUpHealthVessel()
        {
            bloodFill = TextureFactory.Instance.GenerateTexture(game.GraphicsDevice,
                BLOOD_TX_SIZE, BLOOD_TX_SIZE, pixel => bloodColor);
            currentBlood = TextureFactory.Instance.GenerateTexture(game.GraphicsDevice,
                BLOOD_TX_SIZE, BLOOD_TX_SIZE, pixel => transparent);

            bloodSig = TextureFactory.Instance.GenerateTexture(game.GraphicsDevice,
                BLOOD_TX_SIZE, BLOOD_TX_SIZE, pixel => healthSig);
            currentSig = TextureFactory.Instance.GenerateTexture(game.GraphicsDevice,
                BLOOD_TX_SIZE, BLOOD_TX_SIZE, pixel => transparent);

            health = new GeneralSprite(currentBlood, Globals.ONE_FRAME, Globals.ONE_FRAME, 
                Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.UI_MID);

            healthSignifier = new GeneralSprite(currentSig, Globals.ONE_FRAME, Globals.ONE_FRAME,
                Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.UI_SIG);
        }

        /// <summary>
        /// Setup the masks for items in the slots.
        /// </summary>
        private void SetUpMasks()
        {
            Texture2D MaskTexture = TextureFactory.Instance.GenerateTexture(game.GraphicsDevice, 
                Globals.ORIG_UNIT, Globals.ORIG_UNIT, pixel => maskColor);
            int Singleton = 1; 

            primaryMask = new GeneralSprite(MaskTexture, Singleton, Singleton, 
                Globals.WHOLE_SHEET, Singleton, Globals.UI_MID);
            primaryMask.opacity = maskOpacity; 

            itemMask = new GeneralSprite(MaskTexture, Singleton, Singleton, 
                Globals.WHOLE_SHEET, Singleton, Globals.UI_MID);
            itemMask.opacity = maskOpacity;
        }

        /// <summary>
        /// The range of some areas that can provide feedbacks. 
        /// </summary>
        private void SetUpRanges()
        {
            primaryRange = new Rectangle(
                (int)primaryLocation.X * Globals.SCALAR,
                (int)primaryLocation.Y * Globals.SCALAR,
                Globals.OUT_UNIT, Globals.OUT_UNIT
                );

            bagRange = new Rectangle(
                (int)bagLocation.X * Globals.SCALAR,
                (int)bagLocation.Y * Globals.SCALAR,
                BAG_SIZE * Globals.SCALAR, BAG_SIZE * Globals.SCALAR
                );

            bloodRange = new Rectangle(
                (int)bloodPosition.X * Globals.SCALAR,
                (int)bloodPosition.Y * Globals.SCALAR,
                BLOOD_TX_SIZE * Globals.SCALAR, BLOOD_TX_SIZE * Globals.SCALAR
                );
        }

        /// <summary>
        /// Setup where the alt displays shall be and what to draw.
        /// It's set this way in case someone want to implement customized hotkeys. 
        /// </summary>
        private void SetUpAltDisplays()
        {
            int AllSlotSize = 4; 
            fontLBRGenerator = new UI.LargeBR();

            Texture2D PM = fontLBRGenerator.GetText("Q", game.GraphicsDevice);
            Texture2D Slot1 = fontLBRGenerator.GetText("W", game.GraphicsDevice);
            Texture2D Slot2 = fontLBRGenerator.GetText("E", game.GraphicsDevice);
            Texture2D Slot3 = fontLBRGenerator.GetText("R", game.GraphicsDevice);
            Texture2D BagText = fontLBRGenerator.GetText("B", game.GraphicsDevice);

            altDisplays = new GeneralSprite[] {
                new GeneralSprite(PM, 1, 1, Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.UI_ALT_TEXT),
                new GeneralSprite(Slot1, 1, 1, Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.UI_ALT_TEXT),
                new GeneralSprite(Slot2, 1, 1, Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.UI_ALT_TEXT),
                new GeneralSprite(Slot3, 1, 1, Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.UI_ALT_TEXT),
                new GeneralSprite(BagText, 1, 1, Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.UI_ALT_TEXT),
            };

            // Decrease opacity for all 
            for (int i = 0; i < AllSlotSize; i++)
            {
                altDisplays[i].opacity = 0.5f; 
            }

        }

        /// <summary>
        /// Depending on the percent of the character's current HP, 
        /// redraw the HP vessel, i.e. redetermine how high it is. 
        /// </summary>
        private void RedrawBloodVessel()
        {
            double PlayerHealthRatio = game.mainChara.currentHealth / (double)game.mainChara.currentMaxHP;
            double PlayerSigRatio = game.mainChara.pastHealth / (double)game.mainChara.currentMaxHP; 
            int ClipRange = (int)(BLOOD_TX_SIZE * PlayerHealthRatio);
            int ClipRangeSig = (int)(BLOOD_TX_SIZE * PlayerSigRatio);
            int PasteDistance = BLOOD_TX_SIZE - ClipRange;
            int PasteDistanceSiig = BLOOD_TX_SIZE - ClipRangeSig;

            if (ClipRange > 0) // Avoid 0 health error 
            {
                // Clear current blood 
                currentBlood = TextureFactory.Instance.GenerateTexture(game.GraphicsDevice,
                    BLOOD_TX_SIZE, BLOOD_TX_SIZE, pixel => transparent);
                // Copy part of the blood 
                Rectangle SourceRectangle = new Rectangle(0, 0, BLOOD_TX_SIZE, ClipRange);
                Color[] data = new Color[SourceRectangle.Width * SourceRectangle.Height];
                bloodFill.GetData<Color>(0, SourceRectangle, data, 0, data.Length);
                // Paste into the current blood vessel texture 
                currentBlood.SetData(0, new Rectangle(0, PasteDistance, BLOOD_TX_SIZE, ClipRange), data, 0, data.Length);
                // Set the new texture for blood vessel sprite
                health.selfTexture = currentBlood;

                // ------------------------------------------------------------------------------------
                // Do the same again, but for the signifier 
                if(game.mainChara.pastHealth > 0)
                {
                    currentSig = TextureFactory.Instance.GenerateTexture(game.GraphicsDevice,
                    BLOOD_TX_SIZE, BLOOD_TX_SIZE, pixel => transparent);
                    // Copy part of the blood 
                    SourceRectangle = new Rectangle(0, 0, BLOOD_TX_SIZE, ClipRangeSig);
                    data = new Color[SourceRectangle.Width * SourceRectangle.Height];
                    bloodSig.GetData<Color>(0, SourceRectangle, data, 0, data.Length);
                    // Paste into the current blood vessel texture 
                    currentSig.SetData(0, new Rectangle(0, PasteDistanceSiig, BLOOD_TX_SIZE, ClipRangeSig), data, 0, data.Length);
                    // Set the new texture for blood vessel sprite
                    healthSignifier.selfTexture = currentSig;
                }

            }
            else // 0 health display 
            {
                health.selfTexture = TextureFactory.Instance.GenerateTexture(game.GraphicsDevice,
                    BLOOD_TX_SIZE, BLOOD_TX_SIZE, pixel => transparent); 
            }
                
        }

        /// <summary>
        /// Redraw the blood vessel may take a lot of effort, so only call it
        /// when it is confirmed that the player's HP has changed. 
        /// </summary>
        private void CheckHealthChange()
        {
            if(oldHealth != game.mainChara.currentHealth || oldMaxHealth != game.mainChara.currentMaxHP)
            {
                oldHealth = game.mainChara.currentHealth;
                oldMaxHealth = game.mainChara.currentMaxHP;

                UpdateHealthText();

                RedrawBloodVessel();
            }
            if (game.mainChara.pastHealth != -1)
                healthSignifierOn = true;
            else healthSignifierOn = false;
        }

        /// <summary>
        /// Check if the gold count changes, if so, redraw the count text sprite.
        /// </summary>
        private void CheckGoldChange()
        {
            if (oldGoldCount == game.goldCount)
                return;

            oldGoldCount = game.goldCount;

            digitsGenerator = new UI.DigitsSmall();
            Texture2D GC = digitsGenerator.GetText(oldGoldCount.ToString(), game.GraphicsDevice);
            goldCountText = new GeneralSprite(GC, 1, 1, Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.UI_TEXT_LAYER);

            digitsGenerator.useBlack = true;
            GC = digitsGenerator.GetText(oldGoldCount.ToString(), game.GraphicsDevice);
            goldCountTextShadow = new GeneralSprite(GC, 1, 1, Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.UI_TEXT_SHADOW);
        }

        // ================================================================================
        // ============================== Public methods ==================================
        // ================================================================================

        /// <summary>
        /// Check if a location is within UI aera.
        /// Consists of 3 parts: if it's in minimap, if it's in panel, and if it's in blood vessel. 
        /// The blood vessel is calculated as a circle. 
        /// </summary>
        /// <param name="Location">Location to check</param>
        /// <returns>True if it's inside UI area</returns>
        public bool InsideUI(Vector2 Location)
        {
            Rectangle MiniMapArea = new Rectangle(2 * Globals.SCALAR, 3 * Globals.SCALAR,
                58 * Globals.SCALAR, 36 * Globals.SCALAR);
            Vector2 BloodCenter = new Vector2(232, 22) * Globals.SCALAR;
            int Distance = (int)Math.Sqrt( Math.Pow(Location.X - BloodCenter.X, 2) + 
                Math.Pow(Location.Y - BloodCenter.Y, 2));

            return ( Location.Y < PANEL_HOVER_HEIGHT || MiniMapArea.Contains(Location) || Distance < BLOOD_HOVER_RAD); 
        }

        /// <summary>
        /// Call to turn on alt display. 
        /// </summary>
        public void AltDisplayOn()
        {
            altDisplayOn = true;
        }

        /// <summary>
        /// Set a sprite into primary slot. 
        /// </summary>
        /// <param name="PrimarySprite">Sprite to set to</param>
        public void SetPrimary(GeneralSprite PrimarySprite)
        {
            // If it's removing the primary 
            if(PrimarySprite == null)
            {
                primarySlot = PrimarySprite;
                altDisplays[0].opacity = 0.0f; 
            }
            // If it's setting up the primary 
            else
            {
                primarySlot = PrimarySprite;
                primarySlot.layer = Globals.UI_SLOTS;
                primaryState = PrimarySprite.rowLimitation;
                altDisplays[0].opacity = 1.0f;
            }
            
        }

        /// <summary>
        /// Set one sprite as one of the 3 usable slots.
        /// </summary>
        /// <param name="ItemSprite">The sprite to set to</param>
        /// <param name="Index">Indicating which slot is the target</param>
        public void SetItemSlot(GeneralSprite ItemSprite, int Index)
        {
            itemSlots[Index] = ItemSprite;
            itemSlots[Index].layer = Globals.UI_SLOTS; 
        }

        /// <summary>
        /// Check if the cursor location is on top some functioning icons,
        /// if so, give some visual indication that they can click. 
        /// </summary>
        /// <param name="CursorLoc">Position of the cursor in game screen</param>
        public void CheckOnHover(Vector2 CursorLoc)
        {
            if (CursorLoc.Y > AREA_HEIGHT_BOUND)
                return;

            if (bagRange.Contains(CursorLoc))
            {
                bagIcon.rowLimitation = 1;
            }

            if (primarySlot != null && primaryRange.Contains(CursorLoc))
            {
                primarySlot.rowLimitation = primaryState + 1; 
            }

            if (bloodRange.Contains(CursorLoc))
            {
                onHoverHP = true;
            }

        }

        /// <summary>
        /// Check if a left click would do anything.
        /// </summary>
        /// <param name="CursorLoc">Position of the cursor in game screen</param>
        public void CheckLeftClick(Vector2 CursorLoc)
        {
            if (CursorLoc.Y > AREA_HEIGHT_BOUND)
                return;

            if (primarySlot != null && primaryRange.Contains(CursorLoc))
            {
                game.spellSlots.UsePrimary();
            }

        }

        public void Update()
        {
            CheckHealthChange();
            CheckGoldChange();

            if (primarySlot != null)
            {
                primarySlot.Update();
            }
            foreach(GeneralSprite itemSprite in itemSlots){
                if (itemSprite != null)
                {
                    itemSprite.Update();
                }
            }
        }

        public void Draw()
        {
            uiBack.Draw(spriteBatch, drawPosition, defaultTint);

            if (healthSignifierOn) // When taking damage, signifying is taking damage 
                healthSignifier.Draw(spriteBatch, bloodPosition * Globals.SCALAR, defaultTint);
            health.Draw(spriteBatch, bloodPosition * Globals.SCALAR, defaultTint);

            if (primarySlot != null)
            {
                primaryMask.Draw(spriteBatch, primaryLocation * Globals.SCALAR, defaultTint);
                primarySlot.Draw(spriteBatch, primaryLocation * Globals.SCALAR, defaultTint);

                primarySlot.rowLimitation = primaryState; // Turn off on hover affect, if there are any 
            }
            for (int i = 0; i < Globals.SLOT_SIZE; i++)
            {
                if (itemSlots[i] != null)
                {
                    itemMask.Draw(spriteBatch, itemLocations[i] * Globals.SCALAR, defaultTint);
                    itemSlots[i].Draw(spriteBatch, itemLocations[i] * Globals.SCALAR, defaultTint);
                }
            }
                
            // Draw the front panel 
            uiFront.Draw(spriteBatch, drawPosition, defaultTint);

            // Draw the bag, this thing should always be there 
            bagIcon.Draw(spriteBatch, new Vector2(0, 0), defaultTint);

            // If she carries any gold, draw it as a coin notation
            if (game.goldCount > 0)
            {
                goldIcon.Draw(spriteBatch, new Vector2(0, 0), defaultTint);
            }

            // On hover show the amount of HP she still has 
            if (onHoverHP || altDisplayOn)
            {
                healthTextDropS.Draw(spriteBatch, (bloodTextLocationUpdate + textShadowOffset) * Globals.SCALAR, dropShadow);
                healthText.Draw(spriteBatch, bloodTextLocationUpdate * Globals.SCALAR, defaultTint);
                onHoverHP = false;
            }

            // When Alt is being pressed, display hotkeys and stats 
            if (altDisplayOn)
            {
                goldCountTextShadow.Draw(spriteBatch, (altGoldCountLocation + textShadowOffset) * Globals.SCALAR, defaultTint);
                goldCountText.Draw(spriteBatch, altGoldCountLocation * Globals.SCALAR, defaultTint);

                for (int i = 0; i < altDisplays.Length; i++)
                {

                    altDisplays[i].Draw(spriteBatch, altDisplayPositions[i] * Globals.SCALAR, defaultTint);
                }
                altDisplayOn = false;
            }


            // Set on hover effect off 
            bagIcon.rowLimitation = 0;
        }
    }
}
