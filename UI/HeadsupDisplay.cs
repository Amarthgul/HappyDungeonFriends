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
        private const int AREA_HEIGHT_BOUND = 48 * Globals.SCALAR;
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

        private GeneralSprite goldIcon;
        private GeneralSprite bagIcon; 

        private GeneralSprite uiFront;
        private GeneralSprite uiBack;
        private GeneralSprite health;

        private UI.DigitsSmall digitsGenerator;
        private GeneralSprite healthText;
        private GeneralSprite healthTextDropS;

        private Texture2D bloodFill;
        private Texture2D currentBlood;

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
        //Location of some on hover info
        private Vector2 bloodTextLocation = new Vector2(234, 18);
        private Vector2 bloodTextShadowOffset = new Vector2(1, -1);

        private Rectangle primaryRange;
        private Rectangle[] itemRange;
        private Rectangle bagRange;
        private Rectangle bloodRange; 

        private Color defaultTint = Color.White;
        private Color dropShadow = Color.Black;
        private Color bloodColor = new Color(148, 22, 20);
        private Color maskColor = new Color(50, 0, 0);
        private Color transparent = Color.Transparent;

        // ================================================================================
        // ================================= Parameters ===================================
        // ================================================================================
        private int oldHealth;
        private int oldMaxHealth;
        private bool onHoverHP = false;

        public HeadsupDisplay(Game1 G)
        {
            game = G;

            spriteBatch = game.spriteBatch;

            oldHealth = game.mainChara.currentHealth;
            oldMaxHealth = game.mainChara.currentMaxHP;

            primaryState = 0;
            itemStates = new int[] { 0, 0, 0 };
            primarySlot = null;
            itemSlots = new GeneralSprite[] { null, null, null };

            SetUpSprites();
            SetUpHealthVessel();
            SetUpMasks();
            SetUpRanges();

            RedrawBloodVessel();
            UpdateHealthText();
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
            bloodTextLocation -= WidthOffset;
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
                Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.UI_LAYER);
            goldIcon = new GeneralSprite(GOB.texture, GOB.C, GOB.R,
                Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.UI_LAYER);
            bagIcon = new GeneralSprite(BOB.texture, BOB.C, BOB.R,
                0, BOB.C * BOB.R, Globals.UI_LAYER);

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

            health = new GeneralSprite(currentBlood, Globals.ONE_FRAME, Globals.ONE_FRAME, 
                Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.UI_LAYER);
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
                Globals.WHOLE_SHEET, Singleton, Globals.UI_LAYER);
            primaryMask.opacity = maskOpacity; 

            itemMask = new GeneralSprite(MaskTexture, Singleton, Singleton, 
                Globals.WHOLE_SHEET, Singleton, Globals.UI_LAYER);
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
        /// Depending on the percent of the character's current HP, 
        /// redraw the HP vessel, i.e. redetermine how high it is. 
        /// </summary>
        private void RedrawBloodVessel()
        {

            double PlayerHalthRatio = game.mainChara.currentHealth / (double)game.mainChara.currentMaxHP;
            int ClipRange = (int)(BLOOD_TX_SIZE * PlayerHalthRatio);
            int PasteDistance = BLOOD_TX_SIZE - ClipRange;

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
        }

        /// <summary>
        /// Set a sprite into primary slot. 
        /// </summary>
        /// <param name="PrimarySprite">Sprite to set to</param>
        public void SetPrimary(GeneralSprite PrimarySprite)
        {
            primarySlot = PrimarySprite;
            primaryState = PrimarySprite.rowLimitation; 
        }

        /// <summary>
        /// Set one sprite as one of the 3 usable slots.
        /// </summary>
        /// <param name="ItemSprite">The sprite to set to</param>
        /// <param name="Index">Indicating which slot is the target</param>
        public void SetItemSlot(GeneralSprite ItemSprite, int Index)
        {
            itemSlots[Index] = ItemSprite;
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
            if (onHoverHP)
            {
                healthTextDropS.Draw(spriteBatch, (bloodTextLocation + bloodTextShadowOffset) * Globals.SCALAR, dropShadow);
                healthText.Draw(spriteBatch, bloodTextLocation * Globals.SCALAR, defaultTint);
                onHoverHP = false;
            }

            // Set on hover effect off 
            bagIcon.rowLimitation = 0;
        }
    }
}
