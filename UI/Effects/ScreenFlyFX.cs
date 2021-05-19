using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;



namespace HappyDungeon.UI.Effects
{
    /// <summary>
    /// Tiny dots flying over the screen 
    /// </summary>
    class ScreenFlyFX
    {

        SpriteBatch spriteBatch;

        private float progression;
        private float iterStep = 0.01f;

        private Vector2 pointStart;
        private Vector2 pointEnd;
        private Vector2 tanStart;
        private Vector2 tanEnd;

        private Vector2 positionNow;

        private GeneralSprite flySprite;
        private Vector2 spriteOffset;

        private Color defaultTint = Color.White;

        public ScreenFlyFX(SpriteBatch SB)
        {
            spriteBatch = SB; 

            progression = 0f;
            iterStep = (float)(Globals.RND.NextDouble() * 0.03);

            PickUpPoints();
            SetupSprites();
        }

        /// <summary>
        /// Pick random points and tangents for the curve path
        /// </summary>
        private void PickUpPoints()
        {
            // Random pick vertical or horizontal 
            if(Globals.RND.Next() % 2 == 0)
            {
                pointStart = new Vector2(
                    0,
                    Globals.RND.Next(0, Globals.OUT_FHEIGHT)
                    );
                pointEnd = new Vector2(
                    Globals.OUT_FWIDTH,
                    Globals.RND.Next(0, Globals.OUT_FHEIGHT)
                    );
            }
            else
            {
                pointStart = new Vector2(
                    Globals.RND.Next(0, Globals.OUT_FWIDTH),
                    0
                    );
                pointEnd = new Vector2(
                    Globals.RND.Next(0, Globals.OUT_FWIDTH),
                    Globals.OUT_FHEIGHT
                    );
            }

            // Half chance of swapping start and end 
            if (Globals.RND.Next(100) < 50)
            {
                Vector2 temp = pointEnd;
                pointEnd = pointStart;
                pointStart = temp;
            }

            tanStart = new Vector2(
                Globals.RND.Next(-Globals.OUT_FWIDTH, Globals.OUT_FWIDTH),
                Globals.RND.Next(-Globals.OUT_FHEIGHT, Globals.OUT_FHEIGHT));
            tanEnd = new Vector2(
                Globals.RND.Next(-Globals.OUT_FWIDTH, Globals.OUT_FWIDTH),
                Globals.RND.Next(-Globals.OUT_FHEIGHT, Globals.OUT_FHEIGHT));
        }

        /// <summary>
        /// Create the sprite and add some random effects 
        /// </summary>
        private void SetupSprites()
        {
            ImageFile SFXF = TextureFactory.Instance.SFX_fly;
            float SizeShrink = Globals.SCALAR - 2 * (float)Globals.RND.NextDouble();
            spriteOffset = new Vector2(-8, -8) * SizeShrink;

            flySprite = new GeneralSprite(SFXF.texture, SFXF.C, SFXF.R,
                Globals.WHOLE_SHEET, SFXF.C * SFXF.R, Globals.DEBUG_LAYER);
            flySprite.scaleCof = SizeShrink;
            flySprite.opacity = 1 - (float)(Globals.RND.NextDouble() * 0.5);
            flySprite.positionOffset = spriteOffset;
        }

        public void Update()
        {
            progression += iterStep;
            positionNow = Vector2.Hermite(pointStart, tanStart, pointEnd, tanEnd, progression);
            flySprite.Update();
        }

        public void Draw()
        {

            flySprite.Draw(spriteBatch, positionNow, defaultTint);
        }

        public bool Finished()
        {
            return progression > 0.99;
        }

        public Vector2 GetPosition()
        {
            return positionNow;
        }

    }
}
