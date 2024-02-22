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
    // TODO: fix 8 times scale up FoW size issue 
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

        private float rangeBaseline;
        private float rangeNow; 
        private float scaler;
        private int width;
        private int height;
        private double shakyRange = 0.4;
        private int nextUpdateTime;
        private float clairvoyantRange = 0;
        public bool clairvoyantMode { set; get; }
        public bool shakyMode { set; get; }
        // ================================================================================
        // ===================== Regarding the drawing of the fog =========================
        // ================================================================================
        private Texture2D maskArea;
        private Texture2D blackBlock;

        private Vector2 position;

        private Color fillColor = Color.Black;  // Draw() method tint 
        private Color defaultTint = Color.White;  // Draw() method tint 
        private double baseOpacity = 0.9; 
        private double devOpacity = 0.6;
        private double opacity = 0.9;

        private float layer = Globals.FOW_LAYER;

        // Irrelvant to game state so used normal stopwatch  
        private System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
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

            shakyMode = false;
            clairvoyantMode = false;
            rangeBaseline = 1; 
            SetRange(rangeBaseline); // Set default fog range 
            rangeNow = rangeBaseline; 

            Generate();
        }

        /// <summary>
        /// Invoke at the start of game to mark the way FoW is drawn. 
        /// When dev mode is enabled, FoW has lower opacity. 
        /// </summary>
        /// <param name="isDevMode"></param>
        public void SetDevMode(bool isDevMode)
        {
            if (isDevMode)
                opacity = devOpacity;
            else
                opacity = baseOpacity; 
        }

        /// <summary>
        /// Set the visibility within the fog. 
        /// </summary>
        /// <param name="Range">Roughly count as the number of tiles on one direction
        /// that can be seen</param>
        public void SetRange(float Range)
        {
            rangeNow = Range; 
            scaler = (Range + 2) * (Globals.SCALAR / (float)Globals.DEV_SCALE);
            position = new Vector2(-((scaler * width / 2) - Globals.OUT_UNIT / 2),
                -((scaler * height / 2) - Globals.OUT_UNIT / 2));
        }

        /// <summary>
        /// Relocate the texture to fit the glitters. 
        /// This range is not permnent and does not count into the actual range. 
        /// </summary>
        /// <param name="RangeWithShake">Range with shake effect.</param>
        private void SetShakyRange(float RangeWithShake)
        {
            scaler = (RangeWithShake + 2) * (Globals.SCALAR / (float)Globals.DEV_SCALE);
            position = new Vector2(-((scaler * width / 2) - Globals.OUT_UNIT / 2),
                -((scaler * height / 2) - Globals.OUT_UNIT / 2));
        }

        /// <summary>
        /// Return the steady range, regardless of weather it's currently shaky or not. 
        /// </summary>
        /// <returns>Visible range</returns>
        public float GetRange()
        {
            return rangeNow;
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
        /// The reason why shaky didn't also call on clairvoyantMode
        /// is because calirvoyant actually makes player sees further,
        /// while shaky mode only looks shaky. 
        /// </summary>
        public void ToggleShakyMode()
        {
            shakyMode = !shakyMode;
        }

        /// <summary>
        /// Turn on or off clairvoyant, which grants extra vision.
        /// </summary>
        /// <param name="Expansion">The range bonus</param>
        public void ToggleClairvoyant(float Expansion)
        {
            clairvoyantRange = Expansion;
            clairvoyantMode = !clairvoyantMode; 

        }

        public void Update()
        {
            if (clairvoyantMode)
            {
                rangeNow = clairvoyantRange + rangeBaseline;
            }
            else
            {
                SetRange(rangeBaseline);
            }
            if (shakyMode)
            {
                timer = stopwatch.ElapsedMilliseconds;
                if (timer > nextUpdateTime)
                {
                    float ShakyRange = rangeNow + (float)(Globals.RND.NextDouble() * (2 * shakyRange) - shakyRange);

                    SetShakyRange(ShakyRange);

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

