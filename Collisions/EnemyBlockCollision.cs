using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace HappyDungeon
{
    class EnemyBlockCollision
    {

        private Game1 game;

        List<IBlock> blockList;
        private Rectangle gameArea;

        public EnemyBlockCollision(Game1 G)
        {
            game = G;
            blockList = G.staticBlockList;
            blockList = blockList.Concat(game.dynamicBlockList).ToList();

            gameArea = new Rectangle(
                Globals.OUT_BORDER,
                Globals.OUT_HEADSUP + Globals.OUT_BORDER,
                Globals.OUT_GWIDTH - 2 * Globals.OUT_BORDER,
                Globals.OUT_GHEIGHT - 2 *Globals.OUT_BORDER
                ) ;
        }

        public bool ValidMove(Rectangle EnemyCollisionRect)
        {
            bool result = true;

            foreach (IBlock block in blockList)
            {
                Rectangle Intersection = Rectangle.Intersect(EnemyCollisionRect, block.GetRectangle());

                if (Intersection.Width > 0)
                {
                    result = false;
                }
            }

            // Don't go gentle  into the next room 
            if (!EnemyCollisionRect.Equals(Rectangle.Intersect(gameArea, EnemyCollisionRect)))
            {
                result = false;
            }


            return result;
        }
    }
}
