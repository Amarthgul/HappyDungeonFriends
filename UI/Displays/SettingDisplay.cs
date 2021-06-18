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

        private const int SLIDER_POS_X_MAIN = 120;
        private const int SLIDER_POS_X_AUG = 130;
        private const int SLIDER_LEN_0 = 80;
        private const int SLIDER_LEN_1 = 64;
        private const int SLIDER_LEN_2 = 64;

        private Game1 game;
        private SpriteBatch spriteBatch;

        // ================================================================================
        // ============================== Drawing of the elements =========================
        // ================================================================================
        private GeneralSprite background;
        private GeneralSprite[] texts;        // Too many, so unlike title, it's using an array 
        private GeneralSprite[] textsOnHover;
        private GeneralSprite[] sliders; 

        private Vector2 drawPosition = new Vector2(0, 0);
        private Vector2[] sliderPositions;
        private Vector2[] textLocations;

        // ================================================================================
        // ================================ On-hover and clicks ==========================
        // ================================================================================
        private Rectangle[] textRanges;
        private Rectangle[] sliderRanges;
        private Vector2[] sliderAllowedRange;
        private bool[] textOnHoverFlag;
        private bool[] sliderSelected; 

        private Vector2 lastRecordPosition; 

        private int[] sliderLength; 

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

            textOnHoverFlag = new bool[texts.Length];
            sliderSelected = new bool[3];
            ResetSliderSelection();
            ResettextOnHoverFlag(-1);
        }

        private void LoadAllSprites()
        {
            ImageFile SB = TextureFactory.Instance.settingBackground;

            background = new GeneralSprite(SB.texture, SB.C, SB.R,
                Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.UI_MID);

            texts = new GeneralSprite[TextBridge.Instance.GetSettingOptions().Length];
            textsOnHover = new GeneralSprite[TextBridge.Instance.GetSettingOptions().Length];
            sliders = new GeneralSprite[3];

            for (int i = 0; i < TextBridge.Instance.GetSettingOptions().Length; i++)
            {
                Texture2D TO = textGen.GetText(TextBridge.Instance.GetSettingOptions()[i], game.GraphicsDevice);
                Texture2D TOO = textOnHoverGen.GetText(TextBridge.Instance.GetSettingOptions()[i], game.GraphicsDevice);

                texts[i] = new GeneralSprite(TO, 1, 1, Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.UI_LAYER);
                textsOnHover[i] = new GeneralSprite(TOO, 1, 1, Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.UI_LAYER);
            }
            sliders = new GeneralSprite[] {
                new GeneralSprite(TextureFactory.Instance.settingSlider.texture, 1, 1,
                Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.UI_ICONS),
                new GeneralSprite(TextureFactory.Instance.settingSlider.texture, 1, 1,
                Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.UI_ICONS),
                new GeneralSprite(TextureFactory.Instance.settingSlider.texture, 1, 1,
                Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.UI_ICONS)
            };
            
        }

        private void SetupPositions()
        {
            textLocations = new Vector2[TextBridge.Instance.GetSettingOptions().Length];
            textRanges = new Rectangle[TextBridge.Instance.GetSettingOptions().Length];

            for (int i = 0; i < TextBridge.Instance.GetSettingOptions().Length; i++)
            {
                textLocations[i] = new Vector2(112 - texts[i].selfTexture.Width, 26 + 18 * i) * Globals.SCALAR;
                textRanges[i] = new Rectangle((int)(textLocations[i].X), (int)(textLocations[i].Y), 
                    (texts[i].selfTexture.Width * Globals.SCALAR), (texts[i].selfTexture.Height * Globals.SCALAR));
            }

            sliderRanges = new Rectangle[3];
            sliderAllowedRange = new Vector2[3];
            sliderLength = new int[] { SLIDER_LEN_0, SLIDER_LEN_1, SLIDER_LEN_2 };
            sliderPositions = new Vector2[] {
                new Vector2(SLIDER_POS_X_MAIN + (game.volumes[0] / 1) * sliderLength[0], 28) * Globals.SCALAR,
                new Vector2(SLIDER_POS_X_AUG + (game.volumes[0] / 1) * sliderLength[1], 46) * Globals.SCALAR,
                new Vector2(SLIDER_POS_X_AUG + (game.volumes[0] / 1) * sliderLength[2], 64) * Globals.SCALAR
            };
            sliderAllowedRange = new Vector2[] { 
                new Vector2(SLIDER_POS_X_MAIN, SLIDER_POS_X_MAIN + SLIDER_LEN_0) * Globals.SCALAR,
                new Vector2(SLIDER_POS_X_AUG, SLIDER_POS_X_MAIN + SLIDER_LEN_1) * Globals.SCALAR,
                new Vector2(SLIDER_POS_X_AUG, SLIDER_POS_X_MAIN + SLIDER_LEN_2) * Globals.SCALAR
            };
            UpdateSliderRectangles();
        }

        private void UpdateSliderRectangles()
        {
            for (int i = 0; i < sliders.Length; i++)
            {
                sliderRanges[i] = new Rectangle((int)(sliderPositions[i].X), (int)(sliderPositions[i].Y),
                    sliders[i].selfTexture.Width * Globals.SCALAR,
                    sliders[i].selfTexture.Height * Globals.SCALAR);
            }
            
        }

        private void ResettextOnHoverFlag(int Excemption)
        {
            for (int i = 0; i < textOnHoverFlag.Length; i++)
            {
                if (i != Excemption)
                    textOnHoverFlag[i] = false;
            }

        }

        private void ResetSliderSelection()
        {
            for (int i = 0; i < sliderSelected.Length; i++)
            {
                sliderSelected[i] = false;

            }
        }

        // ================================================================================
        // ============================== Public methods ==================================
        // ================================================================================

        public void LeftClickEvent(Vector2 CursorPos)
        {
            lastRecordPosition = CursorPos;
            for (int i = 0; i < sliderRanges.Length; i++)
            {
                if (sliderRanges[i].Contains(CursorPos))
                {
                    sliderSelected[i] = true;
                }
            }
        }

        public void LeftClickRelease(Vector2 CursorPos)
        {

            if (sliderSelected.Any(x => x == true))
            {
                for (int i = 0; i < sliderSelected.Length; i++)
                {
                    if (sliderSelected[i])
                    {
                        game.volumes[i] = (sliderPositions[i].X - sliderAllowedRange[i].X) / (sliderLength[i] * Globals.SCALAR);
                    }
                }
                SoundFX.Instance.SetVolume(game.volumes);
                ResetSliderSelection();
                UpdateSliderRectangles();
            }
        }

        public void UpdateOnhover(Vector2 CursorPos)
        {
            bool hasOnHover = false;

            for (int i = 0; i < texts.Length; i++)
            {
                if (textRanges[i].Contains(CursorPos))
                {
                    textOnHoverFlag[i] = true;
                    hasOnHover = true;
                    ResettextOnHoverFlag(i);

                }
            }

            if (!hasOnHover)
            {
                ResettextOnHoverFlag(-1);
            }

            // ----------------------------------------------------------------------
            // ------------------------- Update slider click ------------------------
            for (int i = 0; i < sliderSelected.Length; i++)
            {
                if (sliderSelected[i] &&
                    CursorPos.X > sliderAllowedRange[i].X &&
                    CursorPos.X < sliderAllowedRange[i].Y)
                {
                    sliderPositions[i].X = CursorPos.X; 
                }
            }
        }

        public void Update()
        {

        }

        public void Draw()
        {
            background.Draw(spriteBatch, drawPosition, defaultTint);

            for (int i = 0; i < texts.Length; i++)
            {
                if (textOnHoverFlag[i])
                    textsOnHover[i].Draw(spriteBatch, textLocations[i], defaultTint);
                else
                    texts[i].Draw(spriteBatch, textLocations[i], defaultTint);
            }

            for (int i = 0; i < sliders.Length; i++)
            {
                sliders[i].Draw(spriteBatch, sliderPositions[i], defaultTint);
            }

        }

    }
}
