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
        private const int SLIDER_TEXT_POS = 102;
        private const int SLIDER_POS_X_MAIN = 120;
        private const int SLIDER_POS_X_AUG = 130;
        private const int SLIDER_LEN_0 = 80;
        private const int SLIDER_LEN_1 = 64;
        private const int SLIDER_LEN_2 = 64;
        private const int NON_ONHOVER_BOUND = 3;
        private const int OPTION_COUNT = 7; 

        private Game1 game;
        private SpriteBatch spriteBatch;

        // ================================================================================
        // ============================== Drawing of the elements =========================
        // ================================================================================
        private GeneralSprite background;
        private GeneralSprite backgroundPure; 
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

        private bool LMBSession = false;
        private Vector2 leftClickSessionStartPos;
        private Vector2 lastRecordSliderPos; 

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
            ImageFile Sky = TextureFactory.Instance.skyBackground;

            background = new GeneralSprite(SB.texture, SB.C, SB.R,
                Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.UI_MID);
            backgroundPure = new GeneralSprite(Sky.texture, Sky.C, Sky.R,
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
            textRanges = new Rectangle[OPTION_COUNT];
            textLocations = new Vector2[OPTION_COUNT]
            {
                new Vector2(SLIDER_TEXT_POS - texts[0].selfTexture.Width, 26)* Globals.SCALAR,
                new Vector2(SLIDER_TEXT_POS - texts[1].selfTexture.Width, 44)* Globals.SCALAR,
                new Vector2(SLIDER_TEXT_POS - texts[2].selfTexture.Width, 62)* Globals.SCALAR,
                new Vector2(SLIDER_TEXT_POS - texts[3].selfTexture.Width, 104)* Globals.SCALAR,
                new Vector2(108 - texts[4].selfTexture.Width, 135)* Globals.SCALAR,
                new Vector2(144, 135)* Globals.SCALAR,
                new Vector2(138 - texts[6].selfTexture.Width, 165)* Globals.SCALAR
            };

            for (int i = 0; i < OPTION_COUNT; i++)
            {
                textRanges[i] = new Rectangle((int)(textLocations[i].X), (int)(textLocations[i].Y), 
                    (texts[i].selfTexture.Width * Globals.SCALAR), (texts[i].selfTexture.Height * Globals.SCALAR));
            }

            sliderRanges = new Rectangle[3];
            sliderAllowedRange = new Vector2[3];
            sliderLength = new int[] { SLIDER_LEN_0, SLIDER_LEN_1, SLIDER_LEN_2 };
            sliderAllowedRange = new Vector2[] { 
                new Vector2(SLIDER_POS_X_MAIN, SLIDER_POS_X_MAIN + SLIDER_LEN_0) * Globals.SCALAR,
                new Vector2(SLIDER_POS_X_AUG, SLIDER_POS_X_AUG + SLIDER_LEN_1) * Globals.SCALAR,
                new Vector2(SLIDER_POS_X_AUG, SLIDER_POS_X_AUG + SLIDER_LEN_2) * Globals.SCALAR
            };
            sliderPositions = new Vector2[] {
                new Vector2(SLIDER_POS_X_MAIN + (game.volumes[0] / 1f) * SLIDER_LEN_0, 28) * Globals.SCALAR,
                new Vector2(SLIDER_POS_X_AUG + (game.volumes[0] / 1f) * SLIDER_LEN_1, 46) * Globals.SCALAR,
                new Vector2(SLIDER_POS_X_AUG + (game.volumes[0] / 1f) * SLIDER_LEN_2, 64) * Globals.SCALAR
            };
            UpdateSliderRectangles();
        }

        /// <summary>
        /// Update the rectange of all sliders for next selection
        /// </summary>
        private void UpdateSliderRectangles()
        {
            for (int i = 0; i < sliders.Length; i++)
            {
                sliderRanges[i] = new Rectangle((int)(sliderPositions[i].X), (int)(sliderPositions[i].Y),
                    sliders[i].selfTexture.Width * Globals.SCALAR,
                    sliders[i].selfTexture.Height * Globals.SCALAR);
            }
            
        }

        /// <summary>
        /// Release all on hover flag but one. Or release all if the Excemption is 
        /// not within the index bound. 
        /// </summary>
        /// <param name="Excemption">Index of the excemption</param>
        private void ResettextOnHoverFlag(int Excemption)
        {
            for (int i = 0; i < textOnHoverFlag.Length; i++)
            {
                if (i != Excemption)
                    textOnHoverFlag[i] = false;
            }

        }

        /// <summary>
        /// Release all the volume slider's selection flag 
        /// </summary>
        private void ResetSliderSelection()
        {
            for (int i = 0; i < sliderSelected.Length; i++)
            {
                sliderSelected[i] = false;
            }
        }

        private void ExecuteCommand(int Index)
        {

        }

        // ================================================================================
        // ============================== Public methods ==================================
        // ================================================================================

        /// <summary>
        /// Mark the start of a left click session
        /// </summary>
        /// <param name="CursorPos">Cursor position</param>
        public void LeftClickEvent(Vector2 CursorPos)
        {
            if (!LMBSession)
            {
                leftClickSessionStartPos = CursorPos;
                LMBSession = true;

                for (int i = 0; i < sliderRanges.Length; i++)
                {
                    if (sliderRanges[i].Contains(CursorPos) && !sliderSelected[i])
                    {
                        sliderSelected[i] = true;
                        lastRecordSliderPos = sliderPositions[i];
                    }
                }
            }
            
        }

        /// <summary>
        /// Mark the end of a left click session
        /// </summary>
        /// <param name="CursorPos">Cursor position</param>
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

            for (int i = 0; i < texts.Length; i++)
            {
                if (textRanges[i].Contains(CursorPos) && textRanges[i].Contains(leftClickSessionStartPos))
                {
                    ExecuteCommand(i);
                }
            }

            LMBSession = false;
        }

        /// <summary>
        /// Actually doe more than just on hover, for clicking on the volume slider it also updates the 
        /// slider to reflect the current sound volume levvel.
        /// </summary>
        /// <param name="CursorPos">Cursor position</param>
        public void UpdateOnhover(Vector2 CursorPos)
        {
            bool hasOnHover = false;

            for (int i = 0; i < texts.Length; i++)
            {
                if (textRanges[i].Contains(CursorPos))
                {
                    if (i > NON_ONHOVER_BOUND && !textOnHoverFlag[i])
                        SoundFX.Instance.PlayTitleOnHover();

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
                if (sliderSelected[i] )
                {
                    sliderPositions[i].X = lastRecordSliderPos.X - (leftClickSessionStartPos.X - CursorPos.X);

                    // Avoid out of range 
                    if (sliderPositions[i].X < sliderAllowedRange[i].X)
                        sliderPositions[i].X = sliderAllowedRange[i].X;
                    if (sliderPositions[i].X > sliderAllowedRange[i].Y)
                        sliderPositions[i].X = sliderAllowedRange[i].Y;
                }
            }
        }

        public void Update()
        {

        }

        public void Draw()
        {
            if (game.virgin)
                backgroundPure.Draw(spriteBatch, drawPosition, defaultTint);
            else
                background.Draw(spriteBatch, drawPosition, defaultTint);

            for (int i = 0; i < texts.Length; i++)
            {
                // volumes and difficulty can not be clicked, thus having no on hover effects 
                if (i > NON_ONHOVER_BOUND && textOnHoverFlag[i]) 
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
