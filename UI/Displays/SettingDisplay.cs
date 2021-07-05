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
        private const int OPTION_COUNT = 8;
        private const int SLIDER_COUNT = 3;
        private const int SLIDER_MA_POS = 232;
        private const int SLIDER_SUB_POS = 226;
        private const int ARROW_COUNT = 2;
        private const int ARROW_SIZE = 11;          //For rectangle 
        private const int DIFFICULTY_POS = 160; 

        private Game1 game;
        private SpriteBatch spriteBatch;

        // ================================================================================
        // ============================== Drawing of the elements =========================
        // ================================================================================
        private GeneralSprite background;
        private GeneralSprite titleMountainFG;
        private GeneralSprite titleMountainBG;
        private GeneralSprite backgroundPure;
        private GeneralSprite sliderBar;
        private GeneralSprite[] sliderBarOnHover;
        private GeneralSprite sliderBarRed;
        private GeneralSprite difficulty;
        private GeneralSprite[] arrows; 
        private GeneralSprite[] texts;        // Too many, so unlike title, it's using an array 
        private GeneralSprite[] textsOnHover;
        private GeneralSprite[] sliders;
        private GeneralSprite[] slidersRed; 
        private GeneralSprite[] volTexts;     // Percentage of volume 

        private Vector2 drawPosition = new Vector2(0, 0);
        private Vector2 difficultyPos; 
        private Vector2[] arrowPositions; 
        private Vector2[] sliderPositions;
        private Vector2[] textPositions;
        private Vector2[] percentagePos;
        private Vector2 sliderOffset = new Vector2(-1, 0) * Globals.SCALAR;

        private int[] sliderLength;

        // ================================================================================
        // ================================ On-hover and clicks ==========================
        // ================================================================================
        private Rectangle[] textRanges;
        private Rectangle[] sliderRanges;
        private Rectangle[] arrowRanges;
        private Vector2[] sliderAllowedRange;
        private bool[] textOnHoverFlag;
        private bool[] sliderSelected;
        private bool[] sliderOnHover;    // The on hover efect for sliders are actually slider bar highlight 

        private int difficultyIndex = 0; 

        private bool LMBSession = false;
        private Vector2 leftClickSessionStartPos;
        private Vector2 lastRecordSliderPos;

        private int sliderPosRecord;
        private int triggerDistance = (int)(1.0 * Globals.SCALAR);

        // --------------------------------------------------------------------------------
        // ------------------------ Switch for keyboard control ---------------------------
        private bool KBS = false;  // Keyboard session flag 
        private int KBIH = 0;      // Keyboard Index Horizontal 
        private int KBIV = 0;      // Keyboard Index Vertical 
        private Stopwatch KBVSW = new Stopwatch();  // Keyboard Vertical Stopwatch
        private Stopwatch KBHSW = new Stopwatch();  // Keyboard Horizontal Stopwatch
        private int KBInterval = 100;               // The one to use 
        private int KBIntervalShort = 50;           // Shorter one for volume control 
        private int KBIntervalLong = 100;           // Longer one for standard press event 

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
            sliderSelected = new bool[SLIDER_COUNT];
            sliderOnHover = new bool[SLIDER_COUNT];
            ResetSliderSelection();
            ResetTextOnHoverFlag(-1);

        }

        /// <summary>
        /// Load all sprites and their on-hover version, if have any 
        /// </summary>
        private void LoadAllSprites()
        {
            ImageFile SB = TextureFactory.Instance.titleBackground;
            ImageFile Sky = TextureFactory.Instance.skyBackground;
            ImageFile SSB = TextureFactory.Instance.settingSlideBar;
            ImageFile SSBR = TextureFactory.Instance.settingSlideBarRed;
            ImageFile AW = TextureFactory.Instance.settingArrow;
            ImageFile MB = TextureFactory.Instance.titleMountainBG;
            ImageFile MF = TextureFactory.Instance.titleMountainFG;

            background = new GeneralSprite(SB.texture, SB.C, SB.R,
                Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.UI_MID - 0.01f);
            backgroundPure = new GeneralSprite(Sky.texture, Sky.C, Sky.R,
                Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.UI_MID - 0.01f);
            sliderBar = new GeneralSprite(SSB.texture, SSB.C, SSB.R,
                Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.UI_MID);
            sliderBarRed = new GeneralSprite(SSBR.texture, SSBR.C, SSBR.R,
                Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.UI_MID);

            titleMountainFG = new GeneralSprite(MF.texture, MF.C, MF.R,
                Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.UI_MID + 0.01f);
            titleMountainBG = new GeneralSprite(MB.texture, MB.C, MB.R,
                Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.UI_MID);

            texts = new GeneralSprite[TextBridge.Instance.GetSettingOptions().Length];
            textsOnHover = new GeneralSprite[TextBridge.Instance.GetSettingOptions().Length];
            sliders = new GeneralSprite[SLIDER_COUNT];
            volTexts = new GeneralSprite[SLIDER_COUNT];

            for (int i = 0; i < OPTION_COUNT; i++)
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
            slidersRed = new GeneralSprite[] {
                new GeneralSprite(TextureFactory.Instance.settingSliderRed.texture, 1, 1,
                Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.UI_ICONS),
                new GeneralSprite(TextureFactory.Instance.settingSliderRed.texture, 1, 1,
                Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.UI_ICONS),
                new GeneralSprite(TextureFactory.Instance.settingSliderRed.texture, 1, 1,
                Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.UI_ICONS)
            };
            sliderBarOnHover = new GeneralSprite[] {
                new GeneralSprite(TextureFactory.Instance.settingSliderBarOH[0].texture, 1, 1, 
                Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.UI_SLOTS),
                new GeneralSprite(TextureFactory.Instance.settingSliderBarOH[1].texture, 1, 1,
                Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.UI_SLOTS),
                new GeneralSprite(TextureFactory.Instance.settingSliderBarOH[2].texture, 1, 1,
                Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.UI_SLOTS),
            };


            arrows = new GeneralSprite[ARROW_COUNT]
            {
                new GeneralSprite(AW.texture, AW.C, AW.R, Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.UI_ICONS),
                new GeneralSprite(AW.texture, AW.C, AW.R, Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.UI_ICONS),
            };
            arrows[0].colLimitation = 0;
            arrows[1].colLimitation = 1;

            RefreshDifficulty();
        }

        /// <summary>
        /// Setup all the draw position and on-hover rectangle ranges 
        /// </summary>
        private void SetupPositions()
        {
            textRanges = new Rectangle[OPTION_COUNT];
            percentagePos = new Vector2[SLIDER_COUNT];

            textPositions = new Vector2[OPTION_COUNT]
            {
                new Vector2(SLIDER_TEXT_POS - texts[0].selfTexture.Width, 26)* Globals.SCALAR,
                new Vector2(SLIDER_TEXT_POS - texts[1].selfTexture.Width, 44)* Globals.SCALAR,
                new Vector2(SLIDER_TEXT_POS - texts[2].selfTexture.Width, 62)* Globals.SCALAR,
                new Vector2(SLIDER_TEXT_POS - texts[3].selfTexture.Width, 104)* Globals.SCALAR,
                new Vector2(108 - texts[4].selfTexture.Width, 135)* Globals.SCALAR,
                new Vector2(144, 135)* Globals.SCALAR,
                new Vector2(108 - texts[6].selfTexture.Width, 160)* Globals.SCALAR,
                new Vector2(144, 160)* Globals.SCALAR
            };


            for (int i = 0; i < OPTION_COUNT; i++)
            {
                textRanges[i] = new Rectangle((int)(textPositions[i].X), (int)(textPositions[i].Y), 
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

            arrowPositions = new Vector2[ARROW_COUNT] {
                new Vector2(119, 104) * Globals.SCALAR,
                new Vector2(196, 104) * Globals.SCALAR
            };
            arrowRanges = new Rectangle[ARROW_COUNT] { 
                new Rectangle((int)(arrowPositions[0].X), (int)(arrowPositions[0].Y), 
                ARROW_SIZE * Globals.SCALAR, ARROW_SIZE * Globals.SCALAR),
                new Rectangle((int)(arrowPositions[1].X), (int)(arrowPositions[1].Y),
                ARROW_SIZE * Globals.SCALAR, ARROW_SIZE * Globals.SCALAR)
            };
            UpdateSliderRectangles();
            RefreshVolumePercentageText();
            
        }

        /// <summary>
        /// Change the displayed volume text depending on slider position,
        /// also re-adjusting their position 
        /// </summary>
        private void RefreshVolumePercentageText()
        {
            for (int i = 0; i < SLIDER_COUNT; i++)
            {
                int Percent = (int)(game.volumes[i] * 100.0);
                Texture2D PT = textGen.GetText($"{Percent}%", game.GraphicsDevice);
                volTexts[i] = new GeneralSprite(PT, 1, 1,
                Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.UI_LAYER);
            }

            percentagePos = new Vector2[SLIDER_COUNT]
            {
                new Vector2(SLIDER_MA_POS - volTexts[0].selfTexture.Width, 26)* Globals.SCALAR,
                new Vector2(SLIDER_SUB_POS - volTexts[1].selfTexture.Width, 44)* Globals.SCALAR,
                new Vector2(SLIDER_SUB_POS - volTexts[2].selfTexture.Width, 62)* Globals.SCALAR
            };
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
        private void ResetTextOnHoverFlag(int Excemption)
        {
            for (int i = 0; i < textOnHoverFlag.Length; i++)
            {
                if (i != Excemption)
                    textOnHoverFlag[i] = false;
            }

        }

        /// <summary>
        /// Set the arrow icons to normal for thoes not being hovered upon. 
        /// </summary>
        /// <param name="Excemption">Which to skip</param>
        private void ResetArrowOnHover(int Excemption)
        {
            for (int i = 0; i < ARROW_COUNT; i++)
            {
                if (i != Excemption)
                {
                    if (game.virgin)
                        arrows[i].rowLimitation = 0;
                    else
                        arrows[i].rowLimitation = 2;
                }
                    
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

        private void ResetSliderOnHover(int Excemption)
        {
            for (int i = 0; i < SLIDER_COUNT; i++)
            {
                if (i != Excemption)
                    sliderOnHover[i] = false;
            }
        }

        /// <summary>
        /// Refresh the displayed difficulty and also set the game difficulty to that. 
        /// </summary>
        private void RefreshDifficulty()
        {
            Globals.GameDifficulty CurrentDiff = Globals.DifficultyIter[Math.Abs(difficultyIndex) % Globals.DifficultyIter.Count]; 
            Texture2D DiffTexture = textGen.GetText(TextBridge.Instance.GetDifficultyOptions(CurrentDiff),
                game.GraphicsDevice);

            difficulty = new GeneralSprite(DiffTexture, 1, 1, Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.UI_LAYER);
            difficultyPos = new Vector2(DIFFICULTY_POS - DiffTexture.Width/2, 104) * Globals.SCALAR;

            game.difficulty = CurrentDiff; 
        }

        private void ExecuteCommand(int Index)
        {
            switch (Index)
            {
                case 4:  // Save
                    break;
                case 5:  // Load
                    break;
                case 6:  // Back
                    game.screenFX.BackToLastState();
                    break;
                case 7:  // Quit
                    game.Exit();
                    break;
                default:
                    break;
            }
        }

        private void DrawPeace()
        {
            backgroundPure.Draw(spriteBatch, drawPosition, defaultTint);
            titleMountainBG.Draw(spriteBatch, drawPosition, defaultTint);
            titleMountainFG.Draw(spriteBatch, drawPosition, defaultTint);
        }

        private void DrawReal()
        {
            background.Draw(spriteBatch, drawPosition, defaultTint);
            
        }

        private void RefreshKBS()
        {
            
        }

        /// <summary>
        /// Change KBIV accroding to mouse hovering. 
        /// </summary>
        /// <param name="Target">Which option to mark</param>
        private void ReverseKBS(int Target)
        {
            switch (Target)
            {

                default:
                    break;
            }
        }

        /// <summary>
        /// Check if KBIV has negative risk, if so, make it positive. 
        /// </summary>
        private void RestoreKBI()
        {
            if (KBIV < 4) KBIV += 4;
        }

        private void StartKBS()
        {
            KBS = true;
            RefreshKBS();
            SoundFX.Instance.PlayTitleOnHover();
            KBVSW.Restart();
        }

        // ================================================================================
        // ============================== Public methods ==================================
        // ================================================================================


        public void OptionMoveUp()
        {
            if (!KBS)
            {
                KBIV--;
                StartKBS();
            }
            else if (KBVSW.ElapsedMilliseconds > KBInterval)
            {
                KBIV--;
                SoundFX.Instance.PlayTitleOnHover();
                KBVSW.Restart();
            }
            RestoreKBI();
        }

        public void OptionMoveDown()
        {
            if (!KBS)
            {
                KBIV++;
                StartKBS();
            }
            else if (KBVSW.ElapsedMilliseconds > KBInterval)
            {
                KBIV++;
                SoundFX.Instance.PlayTitleOnHover();
                KBVSW.Restart();
            }
        }

        public void OptionMoveLeft()
        {
            if (!KBS)
            {
                KBIH--;
                StartKBS();
            }
            else if (KBVSW.ElapsedMilliseconds > KBInterval)
            {
                KBIH--;
                SoundFX.Instance.PlayTitleOnHover();
                KBVSW.Restart();
            }
        }

        public void OptionMoveRight()
        {
            if (!KBS)
            {
                KBIH++;
                StartKBS();
            }
            else if (KBVSW.ElapsedMilliseconds > KBInterval)
            {
                KBIH++;
                SoundFX.Instance.PlayTitleOnHover();
                KBVSW.Restart();
            }
        }


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

                        sliderPosRecord = (int)sliderPositions[i].X;
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
            if (!LMBSession) return; 

            if (sliderSelected.Any(x => x == true))
            {
                ResetSliderSelection();
                UpdateSliderRectangles();
            }

            // Adjusting difficulty 
            for (int i = 0; i < ARROW_COUNT; i++)
            {
                if (LMBSession && arrowRanges[i].Contains(CursorPos) && arrowRanges[i].Contains(leftClickSessionStartPos))
                {
                    SoundFX.Instance.PlayTitleClick();

                    if (i == 0) difficultyIndex--;
                    else difficultyIndex++;
                    RefreshDifficulty();
                }
            }

            for (int i = 0; i < texts.Length; i++)
            {
                if (textRanges[i].Contains(CursorPos) && textRanges[i].Contains(leftClickSessionStartPos))
                {
                    SoundFX.Instance.PlayTitleClick();
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
            bool hasSliderOnHover = false;

            for (int i = 0; i < texts.Length; i++)
            {
                if (textRanges[i].Contains(CursorPos))
                {
                    if (i > NON_ONHOVER_BOUND && !textOnHoverFlag[i])
                        SoundFX.Instance.PlayTitleOnHover();

                    textOnHoverFlag[i] = true;
                    hasOnHover = true;
                    ResetTextOnHoverFlag(i);
                }
            }
            for (int i = 0; i < ARROW_COUNT; i++)
            {
                if (arrowRanges[i].Contains(CursorPos))
                {
                    if ((arrows[i].rowLimitation != 1 && game.virgin) ||
                        (arrows[i].rowLimitation != 3 && !game.virgin))
                        SoundFX.Instance.PlayTitleOnHover();

                    if (game.virgin)
                        arrows[i].rowLimitation = 1;
                    else
                        arrows[i].rowLimitation = 3;
                    hasOnHover = true;
                    ResetArrowOnHover(i);
                }
            }

            // ----------------------------------------------------------------------
            // ------------------------- Update volume slider -----------------------
            for (int i = 0; i < SLIDER_COUNT; i++)
            {
                // -------------------- Hover logistics ----------------------------
                if (sliderRanges[i].Contains(CursorPos))
                {
                    hasSliderOnHover = true;
                    if (!sliderOnHover[i])
                    {
                        sliderOnHover[i] = true;
                        SoundFX.Instance.PlayTitleOnHover();
                        ResetSliderOnHover(i);
                    }
                }

                // -------------------- Click logistics ----------------------------
                if (sliderSelected[i] )
                {
                    sliderPositions[i].X = lastRecordSliderPos.X - (leftClickSessionStartPos.X - CursorPos.X);

                    // Let the slider follow the mouse movement 
                    if (Math.Abs(sliderPositions[i].X - sliderPosRecord) > triggerDistance) {
                        SoundFX.Instance.PlaySettingSliderClick();
                        sliderPosRecord = (int)sliderPositions[i].X;
                    }

                    // Avoid out of range 
                    if (sliderPositions[i].X < sliderAllowedRange[i].X)
                        sliderPositions[i].X = sliderAllowedRange[i].X;
                    if (sliderPositions[i].X > sliderAllowedRange[i].Y)
                        sliderPositions[i].X = sliderAllowedRange[i].Y;

                    game.volumes[i] = (sliderPositions[i].X - sliderAllowedRange[i].X) / (sliderLength[i] * Globals.SCALAR);
                    SoundFX.Instance.SetVolume(game.volumes);
                    RefreshVolumePercentageText();
                    UpdateSliderRectangles();
                }
            }


            // ------------------- Reset if no hover is detected -------------------
            if (!hasOnHover)
            {
                ResetTextOnHoverFlag(-1);
                ResetArrowOnHover(-1);
            }
            if (!hasSliderOnHover)
            {
                ResetSliderOnHover(-1);
            }
        }

        public void Update()
        {
            
        }

        public void Draw()
        {
            if (game.virgin)
                DrawPeace();
            else
                DrawReal();

            for (int i = 0; i < OPTION_COUNT; i++) {
                // volumes and difficulty can not be clicked, thus having no on hover effects 
                if (i > NON_ONHOVER_BOUND && textOnHoverFlag[i]) 
                    textsOnHover[i].Draw(spriteBatch, textPositions[i], defaultTint);
                else
                    texts[i].Draw(spriteBatch, textPositions[i], defaultTint);
            }


            // ----------------------------- Slider Bars --------------------------------
            if (game.virgin) {
                sliderBar.Draw(spriteBatch, drawPosition, defaultTint);
            }
            else {
                sliderBarRed.Draw(spriteBatch, drawPosition, defaultTint);
            }

            for (int i = 0; i < SLIDER_COUNT; i++) {
                if (sliderOnHover[i]) {
                    sliderBarOnHover[i].Draw(spriteBatch, drawPosition, defaultTint);
                }
            }
            
            
                
            
            
            

            for (int i = 0; i < SLIDER_COUNT; i++)  {
                if (game.virgin)
                    sliders[i].Draw(spriteBatch, sliderPositions[i] + sliderOffset, defaultTint);
                else
                    slidersRed[i].Draw(spriteBatch, sliderPositions[i] + sliderOffset, defaultTint);
                volTexts[i].Draw(spriteBatch, percentagePos[i], defaultTint);
            }

            for (int i = 0; i < ARROW_COUNT; i++) {
                arrows[i].Draw(spriteBatch, arrowPositions[i], defaultTint);
            }

            difficulty.Draw(spriteBatch, difficultyPos, defaultTint);

        }

    }
}
