using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;


namespace HappyDungeon
{
    public class ScreenFX
    {
        private const bool _DEBUGGING = false; 

        private Game1 game;
        private SpriteBatch spriteBatch;

        private int respawnPossibility = 5; //5 percent 
        private int maxFlyCount = 3;
        private List<UI.Effects.ScreenFlyFX> flyList;

        private UI.Effects.TransitionFX transitionFX;

        private Globals.GameStates lastState;

        public Texture2D screenCapbackup { set; get; }

        public ScreenFX(Game1 G)
        {
            game = G;
            spriteBatch = game.spriteBatch;
            flyList = new List<UI.Effects.ScreenFlyFX>();

            transitionFX = new UI.Effects.TransitionFX(game);
        }

        /// <summary>
        /// Signal a start of new state transitioning. 
        /// The true transit is in mid stage, when game.transitionProgress[1] becomes true. 
        /// Note that room transitioning does not use this method of transitioning. 
        /// </summary>
        /// <param name="NextState">Which state to change to next</param>
        public void SigTransitionStart(Globals.GameStates NextState)
        {
            // Whenever switch from running state, there is a chance that the player is gonna save.
            // Thus, take a screenshot that can be used as thumbnail. 
            if(game.gameState == Globals.GameStates.Running)
            {
                Texture2D screencap = Screenshot();

                screenCapbackup = screencap;

                if (_DEBUGGING)
                {
                    GeneralSprite cap = new GeneralSprite(screencap, 1, 1, 0, 1, 1f);
                    game.spriteBatch.Begin();
                    cap.Draw(game.spriteBatch, new Vector2(0, 0), Color.White);
                    game.spriteBatch.End();
                }
            }

            transitionFX.SigStart(NextState);
            game.transitionProgress[0] = true;
        }

        public void SetRecordState(Globals.GameStates State)
        {
            lastState = State;
        }

        public void BackToLastState()
        {
            SigTransitionStart(lastState);
        }

        public void Update()
        {
            if (game.transitionProgress.Any(x => x == true))
            {
                transitionFX.Update();
                return;
            }

            if (flyList.Count < maxFlyCount && Globals.RND.Next(100) < respawnPossibility)
            {
                flyList.Add(new UI.Effects.ScreenFlyFX(spriteBatch));
            }

            // Remove all expired flies 
            flyList.RemoveAll(c => c.Finished());

            foreach (UI.Effects.ScreenFlyFX fly in flyList)
            {
                fly.Update();
            }
        }

        public void Draw()
        {
            if (game.transitionProgress.Any(x => x==true))
            {
                transitionFX.Draw();
                return;
            }

        }

        public void DrawFlies(Vector2 FocusPos)
        {
            foreach (UI.Effects.ScreenFlyFX fly in flyList)
            {
                if (FlyVisible(FocusPos, fly.GetPosition()))
                    fly.Draw();
            }
        }

        // ================================================================================
        // ================================ Private methods ===============================
        // ================================================================================

        private bool FlyVisible(Vector2 PlayerPos, Vector2 FlyPos)
        {
            float Range = (game.fogOfWar.GetRange() + 0.5f) * Globals.OUT_UNIT;

            return (Math.Abs(PlayerPos.X - FlyPos.X) + Math.Abs(PlayerPos.Y - FlyPos.Y)) < Range;
        }

        private Texture2D Screenshot()
        {
            byte[] screenData;

            screenData = new byte[game.GraphicsDevice.PresentationParameters.BackBufferWidth *
                game.GraphicsDevice.PresentationParameters.BackBufferHeight * 4];

            game.GraphicsDevice.GetBackBufferData<byte>(screenData);

            Texture2D Screenshot = new Texture2D(game.GraphicsDevice, 
                game.GraphicsDevice.PresentationParameters.BackBufferWidth,
                game.GraphicsDevice.PresentationParameters.BackBufferHeight,
                false, 
                game.GraphicsDevice.PresentationParameters.BackBufferFormat);

            Screenshot.SetData<byte>(screenData);

            return Screenshot; 
        }
    }

}
