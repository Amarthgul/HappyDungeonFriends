using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace HappyDungeon
{
    /// <summary>
    /// Since items are quite diverse, there's no one single formula or prototype. 
    /// Each item is implemented on their own. 
    /// </summary>
    public interface IItem
    {
        /// <summary>
        /// If an item is created right on top of player, it could be picked up instantly,
        /// which is not very intuitive for player ("what did I just see?"). 
        /// To solve this,  a timer is created when an item is added, 
        /// and items need to pass a certain time to be able to pick up (Globals.ITEM_HOLD).
        /// For some decorative items, this may return false forever to avoid picking up. 
        /// </summary>
        /// <returns>True if that item can now be picked up.</returns>
        bool Collectible();

        /// <summary>
        /// The name might be a bit misleading, Collect() returns the item itself 
        /// for it to be added into the bag (See in PlayerItemCollision.cs).
        /// </summary>
        /// <returns>The item itself</returns>
        IItem Collect();

        /// <summary>
        /// Use is one way an item fulfill it's duty (another is only for primary items). 
        /// Use is called for items in the spell/item slots. 
        /// Primary items can not only be used but can also affect how the character attacks. 
        /// </summary>
        void UseItem();

        /// <summary>
        /// Alter the count of the item. 
        /// Can either add or subtract. 
        /// Some items also uses this to restart cooldown. 
        /// </summary>
        /// <param name="Count">Change amount</param>
        void CountFlux(int Count);

        void Update();

        void Draw();

        /// <summary>
        /// Some items has a cooldown after ebing used. 
        /// This methods provides a way to find the CD progression. 
        /// Returns 0-1, where 1 means finsihed. 
        /// </summary>
        /// <returns>Progression of the CD</returns>
        double CooldownRate();

        /// <summary>
        /// Mainly to draw some effects for thoes in item slots. 
        /// </summary>
        void DrawEffects();

        /// <summary>
        /// Rectangle for collision detection if the item is on the ground. 
        /// </summary>
        /// <returns>Collision rectangle of the item</returns>
        Rectangle GetRectangle();

        /// <summary>
        /// Get the headsup/bag view sprite.
        /// All item sprites for headsup and bag view are 4x4. 
        /// </summary>
        /// <returns>GeneralSprite of the item</returns>
        GeneralSprite GetSprite();

        /// <summary>
        /// Find the count of this item the character is currently in possess.  
        /// </summary>
        /// <returns>Count of the item</returns>
        int GetCount();

        /// <summary>
        /// Pick up modifier is static. 
        /// This is added once this item is picked up and added into item slots. 
        /// May contains passive effects. 
        /// </summary>
        /// <returns>Modifers on pick up</returns>
        General.Modifiers.IModifier GetPickupModifier();

        /// <summary>
        /// Output modifier is dynamic.
        /// The result of this method depends on the state of the item.
        /// </summary>
        /// <returns>Modifer depends on situation</returns>
        General.Modifiers.IModifier GetOutputModifier();

        /// <summary>
        /// Each item has an item index denoting what it is,
        /// note that this index is not unique per item in game. 
        /// Although most items with same index shalled be merged, 
        /// some remains separated. 
        /// </summary>
        /// <returns>The index of the item</returns>
        int SelfIndex();

        /// <summary>
        /// Unlike index, type designates the usage of the item as defined in Globals.ItemType,.
        /// *Primary* items can be used, and when put into primary slot, 
        /// can also affect the main character's behavior and appearance. 
        /// *Usable* items cannot be put into primary slot, and dspite their only 
        /// usage, does not affect how the player acts or displays. 
        /// *Junk* cannot be put into any spell/item slots and remains in the bag,
        /// either for entertainment or for plot purpose (if there are any). 
        /// </summary>
        /// <returns>The type of the itm</returns>
        Globals.ItemType SelfType();

        /// <summary>
        /// Item description, does not include stats 
        /// </summary>
        /// <returns>Some short sentences</returns>
        string GetItemDescription();

        /// <summary>
        /// Get the score per item per pickup 
        /// </summary>
        /// <returns>Score of picking up 1 of this item</returns>
        int GetPickUpScore();
    }
}

