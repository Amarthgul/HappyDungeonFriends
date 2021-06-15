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
                bool Success = false;

                if (Intersection.Width > 0)
                {
                    IItem Duplicate = game.bagItemList.Find(c => c.SelfIndex() == item.SelfIndex());

                    if (Duplicate != null) // If this item is already in the bag 
                    {
                        Duplicate.CountFlux(item.GetCount());
                        Success = true;
                    }
                    else
                    {
                        Success = game.spellSlots.TryAddingItem(item);
                    }
                    SoundFX.Instance.PlayitemPickup(item.SelfIndex());

                    if (item is DroppedGold)
                    {   // Dropped gold also marks a success collection 
                        SoundFX.Instance.PlayGoldPickupSFX();
                        game.goldCount += item.GetCount();
                        Success = true; // Disable this for fast gold farming lol 
                    }


                    // If that item is "pciked up", remove it 
                    if (Success)
                    {
                        ToBeRemoved.Add(item);
                        Vector2 Position = new Vector2(item.GetRectangle().X, item.GetRectangle().Y);
                        game.roomCycler.RemoveIndex(item.SelfIndex(), Misc.Instance.PositionReverse(Position)) ;
                    }
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
