using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace HappyDungeon
{
    public class MouseCursor
    {

        Game1 game;
        private SpriteBatch spriteBatch;

        private GeneralSprite cursor;
        private Vector2 position;

        private Color defaultTint = Color.White;

        public MouseCursor(Game1 G)
        {
            game = G;
            spriteBatch = game.spriteBatch;

            position = new Vector2(0, 0);

            ImageFile MC = TextureFactory.Instance.cursor;
            cursor = new GeneralSprite(MC.texture, MC.C, MC.R, 
                Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.CURSOR_LAYER);

        }

        public void SetPosition(Vector2 NewPos)
        {
            position = NewPos;
        }

        public void Draw()
        {
            cursor.Draw(spriteBatch, position, defaultTint);
        }

    }
}
