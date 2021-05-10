using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace HappyDungeon
{
    public class GeneralSprite
    {
        public Texture2D selfTexture { get; set; }
        public int rowCount { get; set; }
        public int columnsCount { get; set; }
        public int rowLimitation { get; set; }
        public int colLimitation { get; set; }
        public int totalFrames { get; set; }
        public float layer { get; set; }        // Layer to draw on 
        public float scaleCof { get; set; }     // Scale coefficient 
        public float opacity { get; set; }      // Draw opacity 
        public Vector2 positionOffset { get; set; }

        private int textureWidth;
        private int textureHeight;


        // Updating 
        private int currentFrame;
        public bool isStatic { get; set; } 
        private Stopwatch stopwatch = new Stopwatch();
        private long timer;

        private Color defaultTine = Color.White;

        /// <summary>
        /// Init the main character. 
        /// </summary>
        /// <param name="Tx">Texture</param>
        /// <param name="C">Column count</param>
        /// <param name="R">Row counr</param>
        /// <param name="RL">Row limiter</param>
        /// <param name="TF">Total frame</param>
        /// <param name="L">Layer</param>
        public GeneralSprite(Texture2D Tx, int C, int R, int RL, int TF, float L)
        {
            selfTexture = Tx;
            rowCount = R;
            columnsCount = C;
            rowLimitation = RL;
            totalFrames = TF;
            layer = L;

            isStatic = totalFrames == 1 ? true : false; // By fedault taking every sprite as dynamic/animated 
            scaleCof = Globals.SCALAR;                  // By default using the global setting 
            opacity = 1;
            colLimitation = -1;
            positionOffset = new Vector2(0, 0);

            textureWidth = selfTexture.Width / columnsCount;
            textureHeight = selfTexture.Height / rowCount;

            stopwatch.Restart();
        }

        public void Update()
        {
            if (!isStatic)
            {
                timer = stopwatch.ElapsedMilliseconds;
                if (timer > Globals.FRAME_DELAY)
                {
                    currentFrame++;
                    stopwatch.Restart();
                    timer = 0;
                }
                if (currentFrame == totalFrames)
                {
                    currentFrame = 0;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 location, Color color)
        {
            
            int row = rowLimitation >= 0 ? rowLimitation : (int)((float)currentFrame / (float)columnsCount);
            int column = colLimitation >= 0? colLimitation : currentFrame % columnsCount;

            Rectangle sourceRectangle = new Rectangle(textureWidth * column, textureHeight * row, textureWidth, textureHeight);

            spriteBatch.Draw(selfTexture, location + positionOffset, sourceRectangle, defaultTine * opacity, 
                0f, Vector2.Zero, scaleCof, SpriteEffects.None, layer);

        }

    }
}
