using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace HappyDungeon
{
    class PlayerItemCollision
    {
        private Game1 game;

        public PlayerItemCollision(Game1 G)
        {
            game = G;
        }

        public void CheckItemCollision(Rectangle PlayerCollisionRect)
        {
            List<IItem> ToBeRemoved = new List<IItem>();

            // Check collision and put into bag 
            foreach (IItem item in game.collectibleItemList)
            {
                Rectangle Intersection = Rectangle.Intersect(PlayerCollisionRect, item.GetRectangle());

                if (Intersection.Width > 0)
                {
                    game.bagItemList.Add(item.Collect());

                    game.spellSlots.TryAddingItem(item);
                    ToBeRemoved.Add(item);
                }
            }

            // Remove them from map 
            foreach(IItem item in ToBeRemoved)
            {
                game.collectibleItemList.Remove(item);
            }

        }


    }
}
