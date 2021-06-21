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
    class PauseDisplay
    {
        private const int TEXT_COUNT = 5;
        private const int VERTICAL_START_POS = 64;
        private const int VERTICAL_INTERVAL = 18; 

        private Game1 game;
        private SpriteBatch spriteBatch;

        private GeneralSprite background;
        private GeneralSprite backgroundSky;
        private GeneralSprite[] texts;
        private GeneralSprite[] textsOnHover; 

        private Vector2[] textLocations; 
        private Vector2 drawPosition = new Vector2(0, 0);

        // Text generator 
        private LargeBR textGen = new LargeBR();
        private LargeWR textOnHoverGen = new LargeWR();

        // ================================================================================
        // ========================= Click and on hover logics  ===========================
        // ================================================================================
        private bool LMBSession = false;
        private Vector2 leftClickSessionStartPos;

        private Rectangle[] textRectangles;
        private bool[] textOnHoverFlag;

        private Color defaultTint = Color.White;

        public PauseDisplay(Game1 G)
        {
            game = G;

            spriteBatch = game.spriteBatch;

            textOnHoverFlag = new bool[TEXT_COUNT];
            ResetTextOnHoverFlag(-1);

            LoadAllSprite();
            SetupPositions();
        }


        private void LoadAllSprite()
        {
            ImageFile TBG = TextureFactory.Instance.titleBackground;
            ImageFile SKY = TextureFactory.Instance.skyBackground;

            background = new GeneralSprite(TBG.texture, TBG.C, TBG.R,
                Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.UI_MID);
            backgroundSky = new GeneralSprite(SKY.texture, SKY.C, SKY.R,
                Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.UI_MID);

            texts = new GeneralSprite[TEXT_COUNT];
            textsOnHover = new GeneralSprite[TEXT_COUNT];
            for (int i = 0; i < TEXT_COUNT; i++)
            {
                Texture2D TX = textGen.GetText(TextBridge.Instance.GetPauseOptions()[i], game.GraphicsDevice);
                Texture2D TXO = textOnHoverGen.GetText(TextBridge.Instance.GetPauseOptions()[i], game.GraphicsDevice);

                texts[i] = new GeneralSprite(TX, 1, 1, Globals.WHOLE_SHEET, 1, Globals.UI_LAYER);
                textsOnHover[i] = new GeneralSprite(TXO, 1, 1, Globals.WHOLE_SHEET, 1, Globals.UI_LAYER);
            }

        }

        private void SetupPositions()
        {
            textLocations = new Vector2[TEXT_COUNT];
            textRectangles = new Rectangle[TEXT_COUNT];
            for (int i = 0; i < TEXT_COUNT; i++)
            {
                textLocations[i] = new Vector2(Globals.ORIG_FWIDTH / 2 - texts[i].selfTexture.Width / 2,
                    VERTICAL_START_POS + i * VERTICAL_INTERVAL) * Globals.SCALAR;
                textRectangles[i] = new Rectangle(
                    (int)(textLocations[i].X), (int)(textLocations[i].Y),
                    texts[i].selfTexture.Width * Globals.SCALAR,
                    texts[i].selfTexture.Height * Globals.SCALAR
                    );
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

        private void ExecuteCommand(int Index)
        {
            switch (Index)
            {
                case 0:
                    game.screenFX.SigTransitionStart(Globals.GameStates.Running);
                    break;
                case 1:
                    game.screenFX.SigTransitionStart(Globals.GameStates.Setting);
                    break;
                case 2:
                    break;
                case 3:
                    game.screenFX.SigTransitionStart(Globals.GameStates.TitleScreen);
                    break;
                case 4:
                    game.Exit();
                    break;
                default:
                    break;
            }
        }

        private void DrawPeace()
        {
            backgroundSky.Draw(spriteBatch, drawPosition, defaultTint);
        }

        private void DrawReal()
        {
            background.Draw(spriteBatch, drawPosition, defaultTint);
        }

        // ================================================================================
        // ============================== Public methods ==================================
        // ================================================================================

        public void LeftClickEvent(Vector2 CursorPos)
        {
            if (!LMBSession)
            {
                LMBSession = true;
                leftClickSessionStartPos = CursorPos;

            }
        }

        public void LeftClickRelease(Vector2 CursorPos)
        {

            for (int i = 0; i < TEXT_COUNT; i++)
            {
                if (LMBSession && textRectangles[i].Contains(CursorPos) && textRectangles[i].Contains(leftClickSessionStartPos))
                {
                    SoundFX.Instance.PlayTitleClick();
                    ExecuteCommand(i);
                }
            }

            LMBSession = false;
        }

        public void UpdateOnhover(Vector2 CursorPos)
        {
            bool hasOnHover = false;

            for (int i = 0; i < texts.Length; i++)
            {
                if (textRectangles[i].Contains(CursorPos))
                {
                    if (!textOnHoverFlag[i])
                        SoundFX.Instance.PlayTitleOnHover();

                    textOnHoverFlag[i] = true;
                    hasOnHover = true;
                    ResetTextOnHoverFlag(i);
                }
            }

            if (!hasOnHover)
            {
                ResetTextOnHoverFlag(-1);
            }
        }


        public void Update()
        {
            game.screenFX.SetRecordState(Globals.GameStates.Pause);
        }

        public void Draw()
        {
            if (game.virgin)
                DrawPeace();
            else
                DrawReal();

            for (int i = 0; i < TEXT_COUNT; i++)
            {
                if (textOnHoverFlag[i])
                    textsOnHover[i].Draw(spriteBatch, textLocations[i], defaultTint);
                else
                    texts[i].Draw(spriteBatch, textLocations[i], defaultTint);
            }

        }


    }
}
