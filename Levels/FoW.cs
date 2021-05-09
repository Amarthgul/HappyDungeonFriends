using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace HappyDungeon
{
    class FoW
    {
        private int scaler;
        private int width;
        private int height;

        private Vector2 position;
        private Game1 game; 
        private SpriteBatch spriteBatch; // the spritebatch used to draw the enemy
        private GraphicsDevice graphics; // the graphics device used by the spritebatch

        private Texture2D maskArea;
        private Texture2D blackBlock;

        private Color fillColor = Color.Black;  // Draw() method tint 
        private Color defaultTint = Color.White;  // Draw() method tint 
        private double opacity = 0.9;

        private float layer = Globals.FOW_LAYER;

        public FoW(Game1 G)
        {
            game = G;
            graphics = game.GraphicsDevice;
            spriteBatch = game.spriteBatch; 

            maskArea = TextureFactory.Instance.fogOfWar.texture;

            width = maskArea.Width;
            height = maskArea.Height;

            SetRange(1); // Set default fog range 

            Generate();
        }

        public void SetRange(int Range)
        {
            scaler = Range + 2;
            position = new Vector2(-((scaler * width / 2) - Globals.OUT_UNIT / 2),
                -((scaler * height / 2) - Globals.OUT_UNIT / 2));
        }

        public Texture2D GenerateTexture(int width, int height, Func<int, Color> paint)
        {
            Texture2D texture = new Texture2D(graphics, width, height);

            Color[] data = new Color[width * height];
            for (int pixel = 0; pixel < data.Count(); pixel++)
                data[pixel] = paint(pixel);

            texture.SetData(data);
            return texture;
        }

        private void Generate()
        {
            blackBlock = GenerateTexture(Globals.ORIG_FWIDTH, Globals.ORIG_FHEIGHT, pixel => fillColor);
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

