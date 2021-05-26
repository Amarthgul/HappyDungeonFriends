using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace HappyDungeon.Items
{
    /// <summary>
    /// Shows item cooldown 
    /// </summary>
    class ItemCoolDown
    {
        private Game1 game; 
        private SpriteBatch spriteBatch;

        private Texture2D cooldownBlock;
        private Vector2 drawPosition;

        private Color defaultFill = Color.Black * 0.5f;
        private Color defaultTint = Color.White;

        public ItemCoolDown(Game1 G, Vector2 DP)
        {
            game = G;
            spriteBatch = game.spriteBatch; 
            drawPosition = DP;

            cooldownBlock = TextureFactory.Instance.GenerateTexture(game.GraphicsDevice, 1, 1, pixel => defaultFill);
        }


        public void Draw(double Percent)
        {
            // Check if it's a good time to draw, if not, exit  
            if (Percent <= 0 || Percent >= 1)
                return;

            // This rectangle needs to be offset by 1 on both Y position and height 
            // So that it displays correctly within the slot.
            Rectangle DestRect = new Rectangle(
                (int)(drawPosition.X * Globals.SCALAR), 
                (int)((drawPosition.Y + 1) * Globals.SCALAR), 
                Globals.OUT_UNIT,
                (int)((Globals.OUT_UNIT - 1) * (1 - Percent))
                );

            spriteBatch.Draw(cooldownBlock, DestRect, null, defaultTint, 
                0f, Vector2.Zero, SpriteEffects.None, Globals.UI_SLOTS_CD);

        }

    }
}
