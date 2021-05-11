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
    /// Draw rectangles around the collision box. 
    /// Only for debugging purpose. 
    /// </summary>
    class DrawRectangle
    {
        private GraphicsDevice graphics;
        private SpriteBatch spriteBatch;

        public int thickness { get; set; }
        private Texture2D RectangleLine;
        private GeneralSprite rectSprite; 
        public Rectangle position;
        public float opacity = 0.5f;

        public Color fillColor { get; set; }
        private Color transp = Color.Transparent; // Texture generation placeholder colod 
        private Color defaultTint = Color.White;  // Draw() method tint 


        public DrawRectangle(GraphicsDevice Graphics, SpriteBatch SB, Rectangle P, Color fill)
        {
            graphics = Graphics;
            spriteBatch = SB;
            position = P;

            thickness = 4;
            fillColor = fill;

            Generate();

            rectSprite = new GeneralSprite(RectangleLine, Globals.ONE_FRAME, Globals.ONE_FRAME, 
                Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.DEBUG_LAYER);

            rectSprite.scaleCof = 1;
            rectSprite.opacity = opacity; 
        }

        private void Generate()
        {
            RectangleLine = TextureFactory.Instance.GenerateTexture(graphics, position.Width, position.Height, pixel => fillColor);

            Texture2D BorderInnerTransp = TextureFactory.Instance.GenerateTexture(graphics, 
                RectangleLine.Width - 2 * thickness, RectangleLine.Height - 2 * thickness, pixel => transp);

            Color[] InnerFillData = new Color[BorderInnerTransp.Width * BorderInnerTransp.Height];

            BorderInnerTransp.GetData(InnerFillData);

            RectangleLine.SetData(0, new Rectangle(thickness, thickness,
                BorderInnerTransp.Width, BorderInnerTransp.Height),
                InnerFillData, 0, InnerFillData.Length);
        }

        public void Draw()
        {
            rectSprite.Draw(spriteBatch, new Vector2(position.X, position.Y),  defaultTint);
        }
    }
}
