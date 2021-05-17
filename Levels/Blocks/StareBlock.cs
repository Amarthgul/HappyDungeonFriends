using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace HappyDungeon
{
    /// <summary>
    /// 2 blocks that look at the player and sometimes does funny face 
    /// </summary>
    class StareBlock : IBlock
    {
        private const int RAND_MIN = 4000;  // Decides the time of next interval 
        private const int RAND_MAX = 8000;
        private const int POSSIBILITY = 10; // Percent 

        private GeneralSprite blockSprite;
        private Vector2 position;
        private Rectangle collisionRect;

        private Game1 game;
        private SpriteBatch spriteBatch;

        private int toleranceDistance = 1 * Globals.SCALAR;
        private int rowChoice;
        private bool expressionOn = false;
        private Stopwatch stopwatch = new Stopwatch();
        private int interval = 5000;
        private int experssionLastingTime = 1000; 
        private long timer;

        private Color defaultTint = Color.White;

        public StareBlock(Game1 G, Vector2 P, int Index)
        {
            game = G;
            position = P;
            rowChoice = Index; 

            spriteBatch = game.spriteBatch;
            collisionRect = new Rectangle((int)P.X, (int)P.Y, Globals.OUT_UNIT, Globals.OUT_UNIT);

            SetUpSprite();
        }

        private void SetUpSprite()
        {
            ImageFile AM = TextureFactory.Instance.blockAllMight;

            blockSprite = new GeneralSprite(AM.texture, AM.C, AM.R, rowChoice, Globals.ONE_FRAME, Globals.BLOCKS_LAYER);
            blockSprite.colLimitation = 0;

            stopwatch.Restart();
        }


        public void Update()
        {

            if (expressionOn)
            {
                // If it's doing the face 
                blockSprite.Update();

                timer = stopwatch.ElapsedMilliseconds; 
                if (timer > experssionLastingTime)
                {
                    blockSprite.totalFrames = 1;
                    blockSprite.colLimitation = -1;
                    blockSprite.currentFrame = 0;
                    expressionOn = false;

                    interval = Globals.RND.Next(RAND_MIN, RAND_MAX);
                    stopwatch.Restart();
                    timer = 0;
                }
            }
            else // Updating stare direction 
            {
                
                Vector2 CharaPositon = game.mainChara.position;
                int LeftTrigger = (int)position.X - toleranceDistance - Globals.OUT_UNIT;
                int RightTrigger = (int)position.X + toleranceDistance + Globals.OUT_UNIT;
                int TopTrigger = (int)position.Y - toleranceDistance - Globals.OUT_UNIT;
                int BottTrigger = (int)position.Y + toleranceDistance + Globals.OUT_UNIT;

                // Find out which direction should it be looing at 
                if (CharaPositon.X < position.X)
                {
                    if (CharaPositon.Y < TopTrigger) blockSprite.colLimitation = 5; // Top left
                    else if (CharaPositon.Y > BottTrigger) blockSprite.colLimitation = 7; // Bottom left 
                    else blockSprite.colLimitation = 1; // At left 
                }
                else if (CharaPositon.X > LeftTrigger && CharaPositon.X < RightTrigger)
                {
                    if (CharaPositon.Y < position.Y) blockSprite.colLimitation = 3; // On top
                    else blockSprite.colLimitation = 4; // At bottom
                }
                else if (CharaPositon.X > position.X)
                {
                    if (CharaPositon.Y < TopTrigger) blockSprite.colLimitation = 6; // Top right 
                    else if (CharaPositon.Y > BottTrigger) blockSprite.colLimitation = 8; // Bottom right 
                    else blockSprite.colLimitation = 2; // At Right 
                }

                // If enought time has passed, do the faces again
                timer = stopwatch.ElapsedMilliseconds;
                if (timer > interval && Globals.RND.Next(100) < POSSIBILITY)
                {   // Start doing the face 
                    expressionOn = true;
                    blockSprite.colLimitation = 9;
                    blockSprite.currentFrame = 0;
                    blockSprite.totalFrames = 7;
                    blockSprite.stopwatch.Restart();

                    stopwatch.Restart();
                    timer = 0;
                }
            }

        }


        public void Draw()
        {
            

            blockSprite.Draw(spriteBatch, position, defaultTint);
        }

        public Rectangle GetRectangle()
        {
            return collisionRect; 
        }
    }
}
