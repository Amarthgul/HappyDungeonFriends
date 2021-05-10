using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace HappyDungeon
{
    public interface IItem
    {
        /// <summary>
        /// If an item is created right on top of player, it could be picked up instantly,
        /// which is not very intuitive for player. So a timer is set up and items freshly 
        /// created would need to pass a certain time to be able to pick up.
        /// </summary>
        /// <returns>True if that item can now be picked up.</returns>
        bool Collectible();

        IItem Collect();

        /// <summary>
        /// Use is one way an item fulfill it's duty. Use is called for items in the spell slots. 
        /// Primary items can not only be used but can also affect how the character attacks. 
        /// </summary>
        void UseItem();

        void Update();

        void Draw();

        Rectangle GetRectangle();

        /// <summary>
        /// Get the headsup/bag view sprite.
        /// All item sprites for headsup and bag view are 4x4. 
        /// </summary>
        /// <returns>GeneralSprite of the item</returns>
        GeneralSprite GetSprite();

        int SelfIndex();

        Globals.ItemType SelfType();
    }
}
