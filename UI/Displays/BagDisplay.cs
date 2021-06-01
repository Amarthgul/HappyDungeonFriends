using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace HappyDungeon.UI.Displays
{

    class BagDisplay
    {
        private Game1 game;
        private SpriteBatch spriteBatch;

        public BagDisplay(Game1 G)
        {
            game = G;

            spriteBatch = game.spriteBatch;
        }

    }
}
