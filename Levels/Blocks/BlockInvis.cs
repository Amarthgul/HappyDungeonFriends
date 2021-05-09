using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace HappyDungeon
{
    /// <summary>
    /// Invisible block of one unit's size. 
    /// </summary>
    class BlockInvis : IBlock
    {
        private Vector2 location;
        private Rectangle rectangle;

        public BlockInvis(Vector2 L)
        {
            location = L;
            rectangle = new Rectangle((int)location.X, (int)location.Y, Globals.OUT_UNIT, Globals.OUT_UNIT);
        }

        public void Update()
        {
            //Do nothing
        }
        public void Draw()
        {
            // Do nothing 
        }

        public Rectangle GetRectangle()
        {
            return rectangle;
        }
    }
}
