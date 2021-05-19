using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace HappyDungeon
{
    /// <summary>
    /// Draw a red point on screen. 
    /// Mostly foe debugging purpose; 
    /// </summary>
    class DrawPoint
    {

        private GraphicsDevice graphics;
        private SpriteBatch spriteBatch;

        private Texture2D pointTexture;
        private Vector2 position; 
        private GeneralSprite pointSprite;
        public float opacity = 1f;
        public int pointSize = 2;
        public int pointOffset = -1; 

        public Color fillColor { get; set; }
        private Color defaultTint = Color.White;

        public DrawPoint(GraphicsDevice G, SpriteBatch SB, Vector2 P)
        {
            graphics = G;
            spriteBatch = SB;
            position = P;

            GenerateLevel();

        }

        private void GenerateLevel()
        {
            pointTexture = TextureFactory.Instance.GenerateTexture(graphics, pointSize, pointSize, pixel => Color.Red);

            pointSprite = new GeneralSprite(pointTexture, 1, 1, 
                Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.DEBUG_LAYER);
            pointSprite.positionOffset = new Vector2(pointOffset, pointOffset);
            pointSprite.opacity = opacity;
        }

        public void Draw()
        {
            pointSprite.Draw(spriteBatch, position, defaultTint);
        }
    }
}
