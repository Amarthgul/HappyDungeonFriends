using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace HappyDungeon.UI.Effects
{
    class TransitionFX
    {

        private Game1 game;
        private SpriteBatch spriteBatch;

        private GeneralSprite facingDown;
        private GeneralSprite facingUp;
        private GeneralSprite allBlack;

        private Vector2 drawPosition = new Vector2();
        private Stopwatch updateSW = new Stopwatch();
        private int totlaTravelDistance = 256 * Globals.SCALAR; 

        private Color defaultTint = Color.White;
        private Color defaultFill = Color.Black;

        private bool inProgress = false;

        private Globals.GameStates nextState; 

        public TransitionFX(Game1 G)
        {
            game = G;

            spriteBatch = game.spriteBatch;

            LoadAllSprite();
        }


        private void LoadAllSprite()
        {
            ImageFile TU = TextureFactory.Instance.transitionUp;
            ImageFile TD = TextureFactory.Instance.transitionDown;

            facingDown = new GeneralSprite(TD.texture, TD.C, TD.R, 
                Globals.WHOLE_SHEET, 1, Globals.TRANSIT_LAYER);
            facingUp = new GeneralSprite(TU.texture, TU.C, TU.R,
                Globals.WHOLE_SHEET, 1, Globals.TRANSIT_LAYER);
            allBlack = new GeneralSprite(
                TextureFactory.Instance.GenerateTexture(game.GraphicsDevice, Globals.OUT_FWIDTH, Globals.OUT_FHEIGHT, pixel => defaultFill),
                1, 1, Globals.WHOLE_SHEET, 1, Globals.TRANSIT_LAYER);
        }

        public void SigStart(Globals.GameStates NextState)
        {
            if (!inProgress)
            {
                inProgress = true;
                updateSW.Restart();
                drawPosition = new Vector2(0, -256) * Globals.SCALAR;
                game.transitionProgress[0] = true;
                nextState = NextState;
            }
            
        }

        /// <summary>
        /// Update iterate through 3 stages of transitions. 
        /// 1st stage enclose the current screen, marked as game.transitionProgress[0];
        /// 2nd stage lasts shorter and is completely black to allow game state to change without glitch;
        /// 3nd stage reveils the new state, using game.transitionProgress[2]; 
        /// </summary>
        public void Update()
        {
            if (!inProgress) return; // Fail-safe 

            if (game.transitionProgress[0] && !game.transitionProgress[1])
            {
                drawPosition.Y = -totlaTravelDistance + 
                    (updateSW.ElapsedMilliseconds / (float)Globals.TRANSITION_SINGLE) * totlaTravelDistance;
                if (updateSW.ElapsedMilliseconds > Globals.TRANSITION_SINGLE)
                {
                    game.transitionProgress[1] = true;
                    updateSW.Restart();
                }
            }
            else if (game.transitionProgress[1] && !game.transitionProgress[2])
            {
                drawPosition.Y = 0;
                if (updateSW.ElapsedMilliseconds > Globals.TRANSITION_HOLD)
                {
                    game.transitionProgress[2] = true;
                    updateSW.Restart();
                    game.gameState = nextState;
                }
            }
            else if (game.transitionProgress[2])
            {
                drawPosition.Y = (updateSW.ElapsedMilliseconds / (float)Globals.TRANSITION_SINGLE) * totlaTravelDistance;

                if (updateSW.ElapsedMilliseconds > Globals.TRANSITION_SINGLE)
                {
                    game.transitionProgress = new bool[] { false, false, false};
                    inProgress = false;
                }
            }
        }

        public void Draw()
        {
            if (game.transitionProgress[0] && !game.transitionProgress[1])
            {
                facingDown.Draw(spriteBatch, drawPosition, defaultTint);
            }
            else if (game.transitionProgress[1] && !game.transitionProgress[2])
            {
                allBlack.Draw(spriteBatch, drawPosition, defaultTint);
            } 
            else if (game.transitionProgress[2])
            {
                facingUp.Draw(spriteBatch, drawPosition, defaultTint);
            }
            
        }



    }
}
