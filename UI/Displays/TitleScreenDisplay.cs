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

        private Game1 game;
        private SpriteBatch spriteBatch;


        // ================================================================================
        // =========================== Sprites and their stats ============================
        // ================================================================================
        private GeneralSprite background;

        private GeneralSprite campaignText;
        private GeneralSprite campaignTextOnHover;
        private GeneralSprite adventureText;
        private GeneralSprite adventureTextOnHover;
        private GeneralSprite settingText;
        private GeneralSprite settingTextOnHover; 

        private Vector2 campaignLoc = new Vector2(Globals.ORIG_FWIDTH / 2, 112) * Globals.SCALAR;
        private Vector2 adventureLoc = new Vector2(Globals.ORIG_FWIDTH / 2, 128) * Globals.SCALAR;
        private Vector2 settingLoc = new Vector2(Globals.ORIG_FWIDTH / 2, 160) * Globals.SCALAR;

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
        private bool onHoeverSetting = false; 

        private Rectangle rectCampaign;
        private Rectangle rectAdventure;
        private Rectangle rectSetting;

        private bool LMBSessionOn = false;
        private Vector2 onClickPosition = new Vector2();


        private Color defualtTint = Color.White;

        public TitleScreenDisplay(Game1 G)
        {
            game = G;

            spriteBatch = game.spriteBatch;

            LoadAllSprites();
            SetupPositions();
        }

        private void LoadAllSprites()
        {
            ImageFile TBG = TextureFactory.Instance.titleBackground;

            Texture2D CT = textGen.GetText(TextBridge.Instance.GetTitleScreenOption(0), game.GraphicsDevice);
            Texture2D CTO = textOnHoverGen.GetText(TextBridge.Instance.GetTitleScreenOption(0), game.GraphicsDevice);
            Texture2D AT = textGen.GetText(TextBridge.Instance.GetTitleScreenOption(1), game.GraphicsDevice);
            Texture2D ATO = textOnHoverGen.GetText(TextBridge.Instance.GetTitleScreenOption(1), game.GraphicsDevice);
            Texture2D ST = textGen.GetText(TextBridge.Instance.GetTitleScreenOption(2), game.GraphicsDevice);
            Texture2D STO = textOnHoverGen.GetText(TextBridge.Instance.GetTitleScreenOption(2), game.GraphicsDevice);

            background = new GeneralSprite(TBG.texture, TBG.C, TBG.R, 
                Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.UI_MID);

            campaignText = new GeneralSprite(CT, 1, 1, Globals.WHOLE_SHEET, 1, Globals.UI_LAYER);
            campaignTextOnHover = new GeneralSprite(CTO, 1, 1, Globals.WHOLE_SHEET, 1, Globals.UI_LAYER);
            adventureText = new GeneralSprite(AT, 1, 1, Globals.WHOLE_SHEET, 1, Globals.UI_LAYER);
            adventureTextOnHover = new GeneralSprite(ATO, 1, 1, Globals.WHOLE_SHEET, 1, Globals.UI_LAYER);
            settingText = new GeneralSprite(ST, 1, 1, Globals.WHOLE_SHEET, 1, Globals.UI_LAYER);
            settingTextOnHover = new GeneralSprite(STO, 1, 1, Globals.WHOLE_SHEET, 1, Globals.UI_LAYER);


        }

        private void SetupPositions()
        {
            // Align them to the middle, used this way in case other languages are used 
            campaignLoc.X -= (campaignText.selfTexture.Width * Globals.SCALAR) / 2;
            adventureLoc.X -= (adventureText.selfTexture.Width * Globals.SCALAR) / 2;
            settingLoc.X -= (settingText.selfTexture.Width * Globals.SCALAR) / 2;

            rectCampaign = new Rectangle((int)campaignLoc.X, (int)campaignLoc.Y,
                campaignText.selfTexture.Width * Globals.SCALAR, 
                campaignText.selfTexture.Height * Globals.SCALAR);
            rectAdventure = new Rectangle((int)adventureLoc.X, (int)adventureLoc.Y,
                adventureText.selfTexture.Width * Globals.SCALAR, 
                adventureText.selfTexture.Height * Globals.SCALAR);
            rectSetting = new Rectangle((int)settingLoc.X, (int)settingLoc.Y,
                settingText.selfTexture.Width* Globals.SCALAR,
                settingText.selfTexture.Height* Globals.SCALAR);
        }
        

        private void ResetOnHover(int Excemption)
        {
            switch (Excemption)
            {
                case 0:
                    onHoverCampaign = false;
                    onHoverAdventure = false;
                    onHoeverSetting = false;
                    break;
                case 1:
                    onHoverAdventure = false;
                    onHoeverSetting = false;
                    break;
                case 2:
                    onHoverCampaign = false;
                    onHoeverSetting = false;
                    break;
                case 3:
                    onHoverCampaign = false;
                    onHoverAdventure = false;
                    break; 
                default:
                    break;
            }
        }

        // ================================================================================
        // ============================== Public methods ==================================
        // ================================================================================

        public void LeftClickEvent(Vector2 CursorPos)
        {
            if (!LMBSessionOn) // So that it only records this at the start of a session 
            {
                onClickPosition = CursorPos;
                LMBSessionOn = true;
            }
            
        }

        public void LeftClickRelease(Vector2 CursorPos)
        {
            if (!LMBSessionOn) return;

            if (rectCampaign.Contains(CursorPos) && rectCampaign.Contains(onClickPosition))
            {
                SoundFX.Instance.PlayTitleClick();
                LMBSessionOn = false;
            }
            else if (rectAdventure.Contains(CursorPos) && rectAdventure.Contains(onClickPosition))
            {
                SoundFX.Instance.PlayTitleClick();
                game.gameState = Globals.GameStates.Running;
                LMBSessionOn = false;
            }
            else if (rectSetting.Contains(CursorPos) && rectSetting.Contains(onClickPosition))
            {
                SoundFX.Instance.PlayTitleClick();
                LMBSessionOn = false;
            }
            else // If released in a potion that nothing matters 
            {
                LMBSessionOn = false;
            }


        }

        public void UpdateOnhover(Vector2 CursorPos)
        {
            if (rectCampaign.Contains(CursorPos))
            {
                if (!onHoverCampaign)
                    SoundFX.Instance.PlayTitleOnHover();

                onHoverCampaign = true;
                ResetOnHover(1);
            }
            else if (rectAdventure.Contains(CursorPos))
            {
                if (!onHoverAdventure)
                    SoundFX.Instance.PlayTitleOnHover();

                onHoverAdventure = true;
                ResetOnHover(2);
            }
            else if (rectSetting.Contains(CursorPos))
            {
                if (!onHoeverSetting)
                    SoundFX.Instance.PlayTitleOnHover();

                onHoeverSetting = true;
                ResetOnHover(3);
            }
            else
            {
                ResetOnHover(0);
            }
        }

        public void Draw()
        {

            background.Draw(spriteBatch, new Vector2(0, 0), defualtTint);


            if (onHoverCampaign)
                campaignTextOnHover.Draw(spriteBatch, campaignLoc, defualtTint);
            else
                campaignText.Draw(spriteBatch, campaignLoc, defualtTint);


            if (onHoverAdventure)
                adventureTextOnHover.Draw(spriteBatch, adventureLoc, defualtTint);
            else
                adventureText.Draw(spriteBatch, adventureLoc, defualtTint);


            if (onHoeverSetting)
                settingTextOnHover.Draw(spriteBatch, settingLoc, defualtTint);
            else
                settingText.Draw(spriteBatch, settingLoc, defualtTint);

        }

    }


}
