using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace HappyDungeon
{
    class PlayerBlockCollision
    {
        private Game1 game;
        List<IBlock> blockList; 

        public PlayerBlockCollision(Game1 G)
        {
            game = G;
            blockList = G.staticBlockList;
            blockList = blockList.Concat(game.dynamicBlockList).ToList();
        }

        public bool ValidMove(Rectangle PlayerCollisionRect)
        {
            bool result = true; 

            foreach(IBlock block in blockList)
            {
                Rectangle Intersection = Rectangle.Intersect(PlayerCollisionRect, block.GetRectangle());

                if (Intersection.Width > 0) { 

                    result = false;

                }
            }

            return result;
        }
    }
}
