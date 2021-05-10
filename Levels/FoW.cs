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
    public class FoW
    {
        // ================================================================================
        // ======================== Consts and frequently used ============================
        // ================================================================================
        private const int SHAKY_MIN = 100;
        private const int SHAKY_MAX = 200;


        // ================================================================================
        // ================= Abstract resources, parameters and states ====================
        // ================================================================================
        private Game1 game; 
        private SpriteBatch spriteBatch; // the spritebatch used to draw the enemy

        public float rangeBaseline { set; get; }
        private float scaler;
        private int width;
        private int height;
        private bool shakyMode = false;
        private double shakyRange = 0.2;
        private int nextUpdateTime;
        private bool clairvoyantMode = false;
        private float clairvoyantRange = 0;

        // ================================================================================
        // ===================== Regarding the drawing of the fog =========================
        // ================================================================================
        private Texture2D maskArea;
        private Texture2D blackBlock;

        private Vector2 position;

        private Color fillColor = Color.Black;  // Draw() method tint 
        private Color defaultTint = Color.White;  // Draw() method tint 
        private double opacity = 0.9;

        private float layer = Globals.FOW_LAYER;

        private Stopwatch stopwatch = new Stopwatch();
        private long timer;

        public FoW(Game1 G)
        {
            game = G;
            spriteBatch = game.spriteBatch; 

            maskArea = TextureFactory.Instance.fogOfWar.texture;

            width = maskArea.Width;
            height = maskArea.Height;

            nextUpdateTime = Globals.RND.Next(SHAKY_MIN, SHAKY_MAX);
            stopwatch.Restart();

            rangeBaseline = 1; 
            SetRange(rangeBaseline); // Set default fog range 

            Generate();
        }

        /// <summary>
        /// Set the visibility within the fog. 
        /// </summary>
        /// <param name="Range">Roughly count as the number of tiles on one direction
        /// that can be seen</param>
        public void SetRange(float Range)
        {
            scaler = Range + 2;
            position = new Vector2(-((scaler * width / 2) - Globals.OUT_UNIT / 2),
                -((scaler * height / 2) - Globals.OUT_UNIT / 2));
        }

        /// <summary>
        /// Generate textures necessary for the functioning of the fog. 
        /// </summary>
        private void Generate()
        {
            blackBlock = TextureFactory.Instance.GenerateTexture(game.GraphicsDevice, 
                Globals.ORIG_FWIDTH, Globals.ORIG_FHEIGHT, pixel => fillColor);
        }

        /// <summary>
        /// Turn on or off shaky mode. 
        /// In shaky mode the fog range change a bit.  
        /// </summary>
        public void ToggleShakyMode()
        {
            shakyMode = !shakyMode;
        }

        /// <summary>
        /// Turn on or off clairvoyant, whihc grants extra vision while on.
        /// </summary>
        /// <param name="Expansion">The range bonus</param>
        public void ToggleClairvoyant(float Expansion)
        {
            clairvoyantRange = Expansion;
            clairvoyantMode = !clairvoyantMode; 

            if (!clairvoyantMode)
            {
                SetRange(rangeBaseline);
            }
        }

        public void Update()
        {
            if (shakyMode)
            {
                timer = stopwatch.ElapsedMilliseconds;
                if (timer > nextUpdateTime)
                {
                    float NewRange = rangeBaseline + (float)(Globals.RND.NextDouble() * (2 * shakyRange) - shakyRange);
                    if (clairvoyantMode)
                    {
                        NewRange += clairvoyantRange;
                    }

                    SetRange(NewRange);

                    stopwatch.Restart();
                    nextUpdateTime = Globals.RND.Next(SHAKY_MIN, SHAKY_MAX);
                    timer = 0; 
                }
            }
        }

        public void Draw()
        {

            if (game.gameState == Globals.GameStates.RoomTransitioning)
            {
                spriteBatch.Draw(blackBlock, new Vector2(0, 0), null,
                    defaultTint * (float)opacity, 0f, Vector2.Zero, Globals.SCALAR, SpriteEffects.None, layer);
            }
            else
            {
                spriteBatch.Draw(maskArea, position + game.mainChara.position, null,
                    defaultTint * (float)opacity, 0f, Vector2.Zero, scaler, SpriteEffects.None, layer);
            }
        }
    }
}

