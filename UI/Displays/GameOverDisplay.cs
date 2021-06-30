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
    class GameOverDisplay
    {
        private const int OPTION_COUNT = 2; 

        private Game1 game;
        private SpriteBatch spriteBatch;

        private GeneralSprite deadBG;
        private GeneralSprite[] options;
        private GeneralSprite[] optionOnHover;

        private Vector2 drawPosition = new Vector2(0, 0);
        private Vector2[] optionPos;

        // ================================================================================
        // ================================ On-hover and clicks ==========================
        // ================================================================================
        private Rectangle[] optionRanges;
        private bool[] optionIsOnHover;

        private Vector2 leftClickSessionStartPos; 
        private bool LMBSession = false;

        // Text generator 
        private LargeBR textGen = new LargeBR();
        private LargeWR textOnHoverGen = new LargeWR();

        
        private Color defaultTint = Color.White;

        public GameOverDisplay(Game1 G)
        {
            game = G;

            spriteBatch = game.spriteBatch;

            LoadSprites();
            SetupPositions();

            optionIsOnHover = new bool[OPTION_COUNT];
            ResetOptionOnHover(-1);
        }

        private void LoadSprites()
        {
            ImageFile DBG = TextureFactory.Instance.gameOverDeadBG;

            deadBG = new GeneralSprite(DBG.texture, DBG.C, DBG.R,
                Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.UI_UNDER);

            options = new GeneralSprite[OPTION_COUNT];
            optionOnHover = new GeneralSprite[OPTION_COUNT];

            for (int i = 0; i < OPTION_COUNT; i++)
            {
                Texture2D TO = textGen.GetText(TextBridge.Instance.GetgameOverOptions()[i], game.GraphicsDevice);
                Texture2D TOO = textOnHoverGen.GetText(TextBridge.Instance.GetgameOverOptions()[i], game.GraphicsDevice);

                options[i] = new GeneralSprite(TO, 1, 1, Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.UI_LAYER);
                optionOnHover[i] = new GeneralSprite(TOO, 1, 1, Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.UI_LAYER);
            }
        }

        private void SetupPositions()
        {
            optionPos = new Vector2[OPTION_COUNT] {
                new Vector2( 128 - options[0].selfTexture.Width / 2, 110  ) * Globals.SCALAR,
                new Vector2( 128 - options[1].selfTexture.Width / 2, 128 ) * Globals.SCALAR
            };

            optionRanges = new Rectangle[] {
                new Rectangle( (int)(optionPos[0].X), (int)(optionPos[0].Y),
                options[0].selfTexture.Width * Globals.SCALAR,
                options[0].selfTexture.Height * Globals.SCALAR ),
                new Rectangle( (int)(optionPos[1].X), (int)(optionPos[1].Y),
                options[1].selfTexture.Width * Globals.SCALAR,
                options[1].selfTexture.Height * Globals.SCALAR )
            };
        }

        private void ResetOptionOnHover(int Excemption)
        {
            for (int i = 0; i < OPTION_COUNT; i++)
            {
                if (i != Excemption)
                    optionIsOnHover[i] = false;
            }
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
            if (LMBSession && optionRanges[0].Contains(CursorPos) && optionRanges[0].Contains(CursorPos))
            {
                game.reset(5);
                game.screenFX.SigTransitionStart(Globals.GameStates.TitleScreen);
            }
            else if (LMBSession && optionRanges[0].Contains(CursorPos) && optionRanges[0].Contains(CursorPos))
            {

            }
        }

        public void UpdateOnhover(Vector2 CursorPos)
        {
            bool HasOnHover = false;

            for (int i = 0; i < OPTION_COUNT; i++)
            {
                if (optionRanges[i].Contains(CursorPos))
                {
                    if (!optionIsOnHover[i])
                        SoundFX.Instance.PlayTitleOnHover();

                    optionIsOnHover[i] = true;
                    ResetOptionOnHover(i);

                    HasOnHover = true;
                }
            }

            if (!HasOnHover)
                ResetOptionOnHover(-1);

        }

        public void Update()
        {

        }

        public void Draw()
        {
            deadBG.Draw(spriteBatch, drawPosition, defaultTint);

            for (int i = 0; i < OPTION_COUNT; i++)
            {
                if (optionIsOnHover[i])
                    optionOnHover[i].Draw(spriteBatch, optionPos[i], defaultTint);
                else
                    options[i].Draw(spriteBatch, optionPos[i], defaultTint);
            }
        }

    }
}
