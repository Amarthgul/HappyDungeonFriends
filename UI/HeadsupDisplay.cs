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

        private Game1 game;
        private SpriteBatch spriteBatch;

        private Vector2 primaryLocation = new Vector2(91, 3);
        private Vector2[] itemLocations = new Vector2[] {
            new Vector2(114, 1),
            new Vector2(135, 1),
            new Vector2(156, 1) };
        private GeneralSprite primarySlot;
        private GeneralSprite[] itemSlots;
        private GeneralSprite primaryMask;  // Draw on slot to make item more obvious 
        private GeneralSprite itemMask;
        private float maskOpacity = 0.5f; 

        private GeneralSprite uiFront;
        private GeneralSprite uiBack;
        private GeneralSprite health;
        private Texture2D bloodFill;
        private Texture2D currentBlood;

        private Vector2 drawPosition = new Vector2(0, 0);
        private Vector2 bloodPosition = new Vector2(868, 8);
        private Color defaultTint = Color.White;
        private Color bloodColor = new Color(148, 22, 20);
        private Color maskColor = new Color(50, 0, 0);
        private Color transparent = Color.Transparent;

        public HeadsupDisplay(Game1 G)
        {
            game = G;

            spriteBatch = game.spriteBatch;

            ImageFile UIF = TextureFactory.Instance.uiFront;
            ImageFile UIB = TextureFactory.Instance.uiBack;

            uiFront = new GeneralSprite(UIF.texture, UIF.C, UIF.R, Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.UI_LAYER);
            uiBack = new GeneralSprite(UIB.texture, UIB.C, UIB.R, Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.UI_LAYER);

            primarySlot = null;
            itemSlots = new GeneralSprite[] { null, null, null };

            SetUpHealthVessel();
            SetUpMasks();
        }

        private void SetUpHealthVessel()
        {
            bloodFill = TextureFactory.Instance.GenerateTexture(game.GraphicsDevice,
                BLOOD_TX_SIZE, BLOOD_TX_SIZE, pixel => bloodColor);
            currentBlood = TextureFactory.Instance.GenerateTexture(game.GraphicsDevice,
                BLOOD_TX_SIZE, BLOOD_TX_SIZE, pixel => transparent);

            health = new GeneralSprite(currentBlood, Globals.ONE_FRAME, Globals.ONE_FRAME, 
                Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.UI_LAYER);
        }

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

        public void SetPrimary(GeneralSprite PrimarySprite)
        {
            primarySlot = PrimarySprite;
        }

        public void SetItemSlot(GeneralSprite ItemSprite, int Index)
        {
            itemSlots[Index] = ItemSprite;
        }

        public void Update()
        {
            RedrawBloodVessel();

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

            health.Draw(spriteBatch, bloodPosition, defaultTint);

            if (primarySlot != null)
            {
                primaryMask.Draw(spriteBatch, primaryLocation * Globals.SCALAR, defaultTint);
                primarySlot.Draw(spriteBatch, primaryLocation * Globals.SCALAR, defaultTint);
            }
            for (int i = 0; i < Globals.SLOT_SIZE; i++)
            {
                if (itemSlots[i] != null)
                {
                    itemMask.Draw(spriteBatch, itemLocations[i] * Globals.SCALAR, defaultTint);
                    itemSlots[i].Draw(spriteBatch, itemLocations[i] * Globals.SCALAR, defaultTint);
                }
            }
                
            uiFront.Draw(spriteBatch, drawPosition, defaultTint);
        }
    }
}
