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
    class TitleScreenDisplay
    {
        private const int OPTION_COUNT = 4; 
        private const float BG_BASE_LAYER = Globals.UI_UNDER;
        private const float SKY_LAYER = BG_BASE_LAYER + 0.001f; 
        private const float CLOUD_BG = BG_BASE_LAYER + 0.002f;
        private const float CLOUD_MG = BG_BASE_LAYER + 0.003f;
        private const float CLOUD_FG = BG_BASE_LAYER + 0.004f;
        private const float MOUNT_BG = BG_BASE_LAYER + 0.005f;
        private const float MOUNT_FG = BG_BASE_LAYER + 0.006f;

        private Game1 game;
        private SpriteBatch spriteBatch;

        // ================================================================================
        // =========================== Sprites and their stats ============================
        // ================================================================================
        private GeneralSprite title; 
        private GeneralSprite background;
        private GeneralSprite backgroundSky;
        private GeneralSprite titleCloudFG;
        private GeneralSprite titleCloudMG;
        private GeneralSprite titleCloudBG;
        private GeneralSprite titleMountainFG;
        private GeneralSprite titleMountainBG;

        private GeneralSprite campaignText;
        private GeneralSprite campaignTextOnHover;
        private GeneralSprite adventureText;
        private GeneralSprite adventureTextOnHover;
        private GeneralSprite loadSavedText;
        private GeneralSprite loadSavedTextOnHover;
        private GeneralSprite settingText;
        private GeneralSprite settingTextOnHover;

        private Vector2 drawPosition = new Vector2(0, 0);
        private Vector2 campaignLoc = new Vector2(Globals.ORIG_FWIDTH / 2, 112) * Globals.SCALAR;
        private Vector2 adventureLoc = new Vector2(Globals.ORIG_FWIDTH / 2, 128) * Globals.SCALAR;
        private Vector2 loadSavedLoc = new Vector2(Globals.ORIG_FWIDTH / 2, 144) * Globals.SCALAR;
        private Vector2 settingLoc = new Vector2(Globals.ORIG_FWIDTH / 2, 160) * Globals.SCALAR;

        private Vector2 cloudPosFG = new Vector2(0, 0);
        private Vector2 cloudPosMG = new Vector2(0, 0);
        private Vector2 cloudPosBG = new Vector2(0, 0);
        private Vector2 mountPosFG = new Vector2(0, 0);
        private Vector2 mountPosBG = new Vector2(0, 0);
        private Vector2 duplicate = new Vector2(-Globals.OUT_FWIDTH, 0);
        private float cloudSpeedFG = 0.4f * Globals.SCALAR;
        private float cloudSpeedMG = 0.2f * Globals.SCALAR;
        private float cloudSpeedBG = 0.1f * Globals.SCALAR;
        private float mountainSpeedFG = 0.2f * Globals.SCALAR;
        private float mountainSpeedBG = 0.05f * Globals.SCALAR;
        private int animteInterval = 50;
        private System.Diagnostics.Stopwatch animateSW = new System.Diagnostics.Stopwatch();

        private bool campaignAvailable = true; // Decides if it gets gray out 

        // Text generator 
        private LargeBR textGen = new LargeBR();
        private LargeWR textOnHoverGen = new LargeWR();

        // ================================================================================
        // ========================= Click and on hover logics  ===========================
        // ================================================================================
        // Could have put them all in an array, but it'll be harder to read and maintain 
        private bool onHoverCampaign = false;
        private bool onHoverAdventure = false;
        private bool onHoverLoadSaved = false;
        private bool onHoeverSetting = false; 

        private Rectangle rectCampaign;
        private Rectangle rectAdventure;
        private Rectangle rectLoadSaved; 
        private Rectangle rectSetting;

        private bool LMBSessionOn = false;
        private Vector2 onClickPosition = new Vector2();

        // --------------------------------------------------------------------------------
        // ------------------------ Switch for keyboard control ---------------------------
        private bool KBS = false; // Keyboard session flag 
        private int KBI = 0;      // Option index 
        private System.Diagnostics.Stopwatch KBSW = new System.Diagnostics.Stopwatch();
        private int KBInterval = 100; 

        private bool transitProtection = false;
        private System.Diagnostics.Stopwatch transitProtSW = new System.Diagnostics.Stopwatch();

        private Color defaultTint = Color.White;

        public TitleScreenDisplay(Game1 G)
        {
            game = G;

            spriteBatch = game.spriteBatch;
            campaignAvailable = Globals.VERSION >= 1.0f;

            LoadAllSprites();
            SetupPositions();

            animateSW.Restart();
        }

        /// <summary>
        /// Load all background sprites and creates all text sprites
        /// </summary>
        private void LoadAllSprites()
        {
            ImageFile TBG = TextureFactory.Instance.titleBackground;
            ImageFile SKY = TextureFactory.Instance.skyBackground;
            ImageFile TT = TextureFactory.Instance.titleText;

            ImageFile CF = TextureFactory.Instance.titleCloudFG;
            ImageFile CM = TextureFactory.Instance.titleCloudMG;
            ImageFile CB = TextureFactory.Instance.titleCloudBG;
            ImageFile MB = TextureFactory.Instance.titleMountainBG;
            ImageFile MF = TextureFactory.Instance.titleMountainFG; 

            Texture2D CT = textGen.GetText(TextBridge.Instance.GetTitleScreenOption(0), game.GraphicsDevice);
            Texture2D CTO = textOnHoverGen.GetText(TextBridge.Instance.GetTitleScreenOption(0), game.GraphicsDevice);
            Texture2D AT = textGen.GetText(TextBridge.Instance.GetTitleScreenOption(1), game.GraphicsDevice);
            Texture2D ATO = textOnHoverGen.GetText(TextBridge.Instance.GetTitleScreenOption(1), game.GraphicsDevice);
            Texture2D LT = textGen.GetText(TextBridge.Instance.GetTitleScreenOption(2), game.GraphicsDevice);
            Texture2D LTO = textOnHoverGen.GetText(TextBridge.Instance.GetTitleScreenOption(2), game.GraphicsDevice);
            Texture2D ST = textGen.GetText(TextBridge.Instance.GetTitleScreenOption(3), game.GraphicsDevice);
            Texture2D STO = textOnHoverGen.GetText(TextBridge.Instance.GetTitleScreenOption(3), game.GraphicsDevice);

            background = new GeneralSprite(TBG.texture, TBG.C, TBG.R, 
                Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.UI_MID);
            backgroundSky = new GeneralSprite(SKY.texture, SKY.C, SKY.R,
                Globals.WHOLE_SHEET, Globals.ONE_FRAME, SKY_LAYER);
            title = new GeneralSprite(TT.texture, TT.C, TT.R,
                Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.UI_TEXT_LAYER);

            titleCloudFG = new GeneralSprite(CF.texture, CF.C, CF.R,
                Globals.WHOLE_SHEET, Globals.ONE_FRAME, CLOUD_FG);
            titleCloudMG = new GeneralSprite(CM.texture, CM.C, CM.R,
                Globals.WHOLE_SHEET, Globals.ONE_FRAME, CLOUD_MG);
            titleCloudBG = new GeneralSprite(CB.texture, CB.C, CB.R,
                Globals.WHOLE_SHEET, Globals.ONE_FRAME, CLOUD_BG);
            titleMountainFG = new GeneralSprite(MF.texture, MF.C, MF.R,
                Globals.WHOLE_SHEET, Globals.ONE_FRAME, MOUNT_FG);
            titleMountainBG = new GeneralSprite(MB.texture, MB.C, MB.R,
                Globals.WHOLE_SHEET, Globals.ONE_FRAME, MOUNT_BG);

            campaignText = new GeneralSprite(CT, 1, 1, Globals.WHOLE_SHEET, 1, Globals.UI_LAYER);
            campaignTextOnHover = new GeneralSprite(CTO, 1, 1, Globals.WHOLE_SHEET, 1, Globals.UI_LAYER);
            adventureText = new GeneralSprite(AT, 1, 1, Globals.WHOLE_SHEET, 1, Globals.UI_LAYER);
            adventureTextOnHover = new GeneralSprite(ATO, 1, 1, Globals.WHOLE_SHEET, 1, Globals.UI_LAYER);
            loadSavedText = new GeneralSprite(LT, 1, 1, Globals.WHOLE_SHEET, 1, Globals.UI_LAYER);
            loadSavedTextOnHover = new GeneralSprite(LTO, 1, 1, Globals.WHOLE_SHEET, 1, Globals.UI_LAYER);
            settingText = new GeneralSprite(ST, 1, 1, Globals.WHOLE_SHEET, 1, Globals.UI_LAYER);
            settingTextOnHover = new GeneralSprite(STO, 1, 1, Globals.WHOLE_SHEET, 1, Globals.UI_LAYER);


        }

        /// <summary>
        /// Setup the position and rectangles for draw and on hover check.
        /// </summary>
        private void SetupPositions()
        {
            // Align them to the middle, used this way in case other languages are used 
            campaignLoc.X -= (campaignText.selfTexture.Width * Globals.SCALAR) / 2;
            adventureLoc.X -= (adventureText.selfTexture.Width * Globals.SCALAR) / 2;
            loadSavedLoc.X -= (loadSavedText.selfTexture.Width * Globals.SCALAR) / 2;
            settingLoc.X -= (settingText.selfTexture.Width * Globals.SCALAR) / 2;

            rectCampaign = new Rectangle((int)campaignLoc.X, (int)campaignLoc.Y,
                campaignText.selfTexture.Width * Globals.SCALAR, 
                campaignText.selfTexture.Height * Globals.SCALAR);
            rectAdventure = new Rectangle((int)adventureLoc.X, (int)adventureLoc.Y,
                adventureText.selfTexture.Width * Globals.SCALAR, 
                adventureText.selfTexture.Height * Globals.SCALAR);
            rectLoadSaved = new Rectangle((int)loadSavedLoc.X, (int)loadSavedLoc.Y,
                loadSavedText.selfTexture.Width * Globals.SCALAR,
                loadSavedText.selfTexture.Height * Globals.SCALAR);
            rectSetting = new Rectangle((int)settingLoc.X, (int)settingLoc.Y,
                settingText.selfTexture.Width* Globals.SCALAR,
                settingText.selfTexture.Height* Globals.SCALAR);
        }

        /// <summary>
        /// Reset on hover flags for the texts. 
        /// With either 1 excemption or no excemption. 
        /// </summary>
        /// <param name="Excemption">Which to skip</param>
        private void ResetOnHover(int Excemption)
        {
            switch (Excemption)
            {
                case 0:
                    onHoverCampaign = false;
                    onHoverAdventure = false;
                    onHoverLoadSaved = false;
                    onHoeverSetting = false;
                    break;
                case 1:
                    onHoverAdventure = false;
                    onHoeverSetting = false;
                    onHoverLoadSaved = false;
                    break;
                case 2:
                    onHoverCampaign = false;
                    onHoeverSetting = false;
                    onHoverLoadSaved = false;
                    break;
                case 3:
                    onHoverCampaign = false;
                    onHoverAdventure = false;
                    onHoeverSetting = false;
                    break;
                case 4:
                    onHoverLoadSaved = false;
                    onHoverCampaign = false;
                    onHoverAdventure = false;
                    break;
                default:
                    break;
            }
        }

        private void DrawPeace()
        {
            backgroundSky.Draw(spriteBatch, drawPosition, defaultTint);

            titleCloudBG.Draw(spriteBatch, cloudPosBG, defaultTint);
            titleCloudMG.Draw(spriteBatch, cloudPosMG, defaultTint);
            titleCloudFG.Draw(spriteBatch, cloudPosFG, defaultTint);
            titleMountainBG.Draw(spriteBatch, mountPosBG, defaultTint);
            titleMountainFG.Draw(spriteBatch, mountPosFG, defaultTint);

            titleCloudBG.Draw(spriteBatch, cloudPosBG + duplicate, defaultTint);
            titleCloudMG.Draw(spriteBatch, cloudPosMG + duplicate, defaultTint);
            titleCloudFG.Draw(spriteBatch, cloudPosFG + duplicate, defaultTint);
            titleMountainBG.Draw(spriteBatch, mountPosBG + duplicate, defaultTint);
            titleMountainFG.Draw(spriteBatch, mountPosFG + duplicate, defaultTint);
        }

        private void DrawReal()
        {
            background.Draw(spriteBatch, drawPosition, defaultTint);
        }

        /// <summary>
        /// Flag certain option as being selected. 
        /// </summary>
        /// <returns>True if a valid selection is made</returns>
        private bool RefreshKBS()
        {
            bool Result = true;

            switch (Math.Abs(KBI) % OPTION_COUNT)
            {
                case 0:
                    onHoverCampaign = true;
                    ResetOnHover(1);
                    if (!campaignAvailable)
                        Result = false;
                    break;
                case 1:
                    onHoverAdventure = true;
                    ResetOnHover(2);
                    break;
                case 2:
                    onHoverLoadSaved = true;
                    ResetOnHover(3);
                    break;
                case 3:
                    onHoeverSetting = true;
                    ResetOnHover(4);
                    break;
                default:
                    break;
            }

            return Result; 
        }

        /// <summary>
        /// Change KBI accroding to mouse hovering. 
        /// </summary>
        /// <param name="Target">Which option to mark</param>
        private void ReverseKBS(int Target)
        {
            switch (Target)
            {

                case 1:
                    if(campaignAvailable)
                        KBI = 0;
                    break;
                case 2:
                    KBI = 1;
                    break;
                case 3:
                    KBI = 2;
                    break;
                case 4:
                    KBI = 3;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Check if KBI has negative risk, if so, make it positive. 
        /// </summary>
        private void RestoreKBI()
        {
            if (KBI < 4) KBI += 4;
        }


        // ================================================================================
        // ============================== Public methods ==================================
        // ================================================================================

        public void OptionMoveUp()
        {
            if (!KBS)
            {
                KBS = true;
                while (!RefreshKBS()) KBI--;
                SoundFX.Instance.PlayTitleOnHover();
                KBSW.Restart();
            }
            else if (KBSW.ElapsedMilliseconds > KBInterval)
            {
                KBI--;
                while (!RefreshKBS()) KBI--;
                SoundFX.Instance.PlayTitleOnHover();
                KBSW.Restart();
            }
            RestoreKBI();
        }

        public void OptionMoveDown()
        {
            if (!KBS)
            {
                KBS = true;
                while (!RefreshKBS()) KBI++;
                SoundFX.Instance.PlayTitleOnHover();
                KBSW.Restart();
            }
            else if (KBSW.ElapsedMilliseconds > KBInterval)
            {
                KBI++;
                while (!RefreshKBS()) KBI++;
                SoundFX.Instance.PlayTitleOnHover();
                KBSW.Restart();
            }
        }

        public void OptionConfirm()
        {
            if (!KBS) return; // Do nothing if it's currently selected with keyboard 

            Vector2 FakeClick = new Vector2(); 

            switch (Math.Abs(KBI) % OPTION_COUNT)
            {
                case 0:
                    FakeClick = new Vector2(campaignLoc.X + 1, campaignLoc.Y + 1);
                    onClickPosition = FakeClick;
                    LeftClickRelease(FakeClick);
                    break;
                case 1:
                    FakeClick = new Vector2(adventureLoc.X + 1, adventureLoc.Y + 1);
                    onClickPosition = FakeClick;
                    LeftClickRelease(FakeClick);
                    break;
                case 2:
                    FakeClick = new Vector2(loadSavedLoc.X + 1, loadSavedLoc.Y + 1);
                    onClickPosition = FakeClick;
                    LeftClickRelease(FakeClick);
                    break;
                case 3:
                    FakeClick = new Vector2(settingLoc.X + 1, settingLoc.Y + 1);
                    onClickPosition = FakeClick;
                    LeftClickRelease(FakeClick);
                    break;
                default:
                    break;
            }

        }

        public void LeftClickEvent(Vector2 CursorPos)
        {
            if (KBS)
            {
                // left click will cancel keyboard seesion 
                KBS = false;
            }

            if (!LMBSessionOn) // So that it only records this at the start of a session 
            {
                onClickPosition = CursorPos;
                LMBSessionOn = true;
            }
            
        }

        public void LeftClickRelease(Vector2 CursorPos)
        {
            if ((!LMBSessionOn && !KBS) || transitProtection) return; // Don't do anything during transition animation 

            if (rectCampaign.Contains(CursorPos) && rectCampaign.Contains(onClickPosition) && campaignAvailable)
            {
                SoundFX.Instance.PlayTitleClick();
                LMBSessionOn = false;
            }
            else if (rectAdventure.Contains(CursorPos) && rectAdventure.Contains(onClickPosition))
            {
                SoundFX.Instance.PlayTitleClick();
                game.screenFX.SigTransitionStart(Globals.GameStates.Running);

                transitProtection = true;
                transitProtSW.Restart();
                LMBSessionOn = false;
            }
            else if (rectLoadSaved.Contains(CursorPos) && rectLoadSaved.Contains(onClickPosition))
            {
                SoundFX.Instance.PlayTitleClick();
                game.screenFX.SigTransitionStart(Globals.GameStates.LoadAndSave);

                transitProtection = true;
                transitProtSW.Restart();
                LMBSessionOn = false;
            }
            else if (rectSetting.Contains(CursorPos) && rectSetting.Contains(onClickPosition))
            {
                SoundFX.Instance.PlayTitleClick();
                game.screenFX.SetRecordState(Globals.GameStates.TitleScreen);
                game.screenFX.SigTransitionStart(Globals.GameStates.Setting);

                transitProtection = true;
                transitProtSW.Restart();
                LMBSessionOn = false;
            }
            else // If released in a potion that nothing matters 
            {
                LMBSessionOn = false;
            }


        }

        public void UpdateOnhover(Vector2 CursorPos)
        {
            bool HasOnHover = false; 

            if (rectCampaign.Contains(CursorPos) && campaignAvailable)
            {
                if (!onHoverCampaign)
                    SoundFX.Instance.PlayTitleOnHover();

                HasOnHover = true;
                onHoverCampaign = true;
                ResetOnHover(1);
                ReverseKBS(1);
            }
            else if (rectAdventure.Contains(CursorPos))
            {
                if (!onHoverAdventure)
                    SoundFX.Instance.PlayTitleOnHover();

                HasOnHover = true;
                onHoverAdventure = true;
                ResetOnHover(2);
                ReverseKBS(2);
            }
            else if (rectLoadSaved.Contains(CursorPos))
            {
                if (!onHoverLoadSaved)
                    SoundFX.Instance.PlayTitleOnHover();

                HasOnHover = true;
                onHoverLoadSaved = true;
                ResetOnHover(3);
                ReverseKBS(3);
            }
            else if (rectSetting.Contains(CursorPos))
            {
                if (!onHoeverSetting)
                    SoundFX.Instance.PlayTitleOnHover();

                HasOnHover = true;
                onHoeverSetting = true;
                ResetOnHover(4);
                ReverseKBS(4);
            }

            if (!HasOnHover && !KBS)
            {
                ResetOnHover(0);
            }
            else if(HasOnHover)
            {
                KBS = false;
            }
        }

        public void Update()
        {
            if (transitProtSW.ElapsedMilliseconds > Globals.TRANSITION_SINGLE)
            {
                transitProtection = false;
            }

            if (animateSW.ElapsedMilliseconds > animteInterval)
            {
                cloudPosFG.X += cloudSpeedFG;
                cloudPosMG.X += cloudSpeedMG;
                cloudPosBG.X += cloudSpeedBG;
                mountPosFG.X += mountainSpeedFG;
                mountPosBG.X += mountainSpeedBG;

                if (cloudPosFG.X > Globals.OUT_FWIDTH) cloudPosFG.X = 0;
                if (cloudPosMG.X > Globals.OUT_FWIDTH) cloudPosMG.X = 0;
                if (cloudPosBG.X > Globals.OUT_FWIDTH) cloudPosBG.X = 0;
                if (mountPosFG.X > Globals.OUT_FWIDTH) mountPosFG.X = 0;
                if (mountPosBG.X > Globals.OUT_FWIDTH) mountPosBG.X = 0;

                animateSW.Restart();
            }

        }


        public void Draw()
        {
            if (game.virgin)
                DrawPeace();
            else
                DrawReal();

            title.Draw(spriteBatch, drawPosition, defaultTint);

            if (onHoverCampaign)
                campaignTextOnHover.Draw(spriteBatch, campaignLoc, defaultTint);
            else
                campaignText.Draw(spriteBatch, campaignLoc, defaultTint * (campaignAvailable? 1f : .5f));

            if (onHoverAdventure)
                adventureTextOnHover.Draw(spriteBatch, adventureLoc, defaultTint);
            else
                adventureText.Draw(spriteBatch, adventureLoc, defaultTint);

            if (onHoverLoadSaved)
                loadSavedTextOnHover.Draw(spriteBatch, loadSavedLoc, defaultTint);
            else
                loadSavedText.Draw(spriteBatch, loadSavedLoc, defaultTint);


            if (onHoeverSetting)
                settingTextOnHover.Draw(spriteBatch, settingLoc, defaultTint);
            else
                settingText.Draw(spriteBatch, settingLoc, defaultTint);

        }

    }


}
