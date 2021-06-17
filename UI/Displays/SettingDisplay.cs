using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace HappyDungeon.UI.Displays
{
    class SettingDisplay
    {

        private Game1 game;
        private SpriteBatch spriteBatch;

        private GeneralSprite background;
        private GeneralSprite[] texts;        // Too many, so unlike title, it's using an array 
        private GeneralSprite[] textsOnHover; 

        private Vector2 drawPosition = new Vector2(0, 0);
        private Vector2[] textLocations;

        // Text generator 
        private LargeBR textGen = new LargeBR();
        private LargeWR textOnHoverGen = new LargeWR();

        private Color defaultTint = Color.White;

        public SettingDisplay(Game1 G)
        {
            game = G;

            spriteBatch = game.spriteBatch;

            LoadAllSprites();
            SetupPositions();
        }

        private void LoadAllSprites()
        {
            ImageFile SB = TextureFactory.Instance.settingBackground;

            background = new GeneralSprite(SB.texture, SB.C, SB.R,
                Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.UI_MID);

            texts = new GeneralSprite[TextBridge.Instance.GetSettingOptions().Length];
            textsOnHover = new GeneralSprite[TextBridge.Instance.GetSettingOptions().Length];

            for (int i = 0; i < TextBridge.Instance.GetSettingOptions().Length; i++)
            {
                Texture2D TO = textGen.GetText(TextBridge.Instance.GetSettingOptions()[i], game.GraphicsDevice);
                Texture2D TOO = textOnHoverGen.GetText(TextBridge.Instance.GetSettingOptions()[i], game.GraphicsDevice);

                texts[i] = new GeneralSprite(TO, 1, 1, Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.UI_LAYER);
                textsOnHover[i] = new GeneralSprite(TOO, 1, 1, Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.UI_LAYER);
            }

        }

        private void SetupPositions()
        {
            textLocations = new Vector2[TextBridge.Instance.GetSettingOptions().Length];


            for (int i = 0; i < TextBridge.Instance.GetSettingOptions().Length; i++)
            {
                textLocations[i] = new Vector2(112 - texts[i].selfTexture.Width, 26 + 18 * i) * Globals.SCALAR; 
            }
        }

        // ================================================================================
        // ============================== Public methods ==================================
        // ================================================================================

        public void LeftClickEvent(Vector2 CursorPos)
        {

        }

        public void LeftClickRelease(Vector2 CursorPos)
        {

        }
        public void UpdateOnhover(Vector2 CursorPos)
        {

        }

        public void Draw()
        {
            background.Draw(spriteBatch, drawPosition, defaultTint);

            for (int i = 0; i < texts.Length; i++)
            {
                texts[i].Draw(spriteBatch, textLocations[i], defaultTint);
            }

        }

    }
}
