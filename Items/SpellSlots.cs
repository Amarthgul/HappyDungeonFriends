using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace HappyDungeon
{
    /// <summary>
    /// This class stores what the character is currently equiping.
    /// It decides what type of attack the character can do, what ability she has,
    /// but does not record thing like how many items she has or how to draw. 
    /// This class also checks the bag and remove items when depleted. 
    /// </summary>
    public class SpellSlots
    {
        private Game1 game;


        // Slots
        private IItem primary = null;
        private IItem[] itemSlots = new IItem[Globals.SLOT_SIZE] { null, null, null};

        private bool primaryReady = true;
        private bool[] itemsReady = new bool[] { true, true, true };

        private Stopwatch stopwatch = new Stopwatch();
        private long timer;

        public SpellSlots(Game1 G)
        {
            game = G;
        }

        /// <summary>
        /// Only primary items can be put into primary slot.
        /// </summary>
        /// <param name="Item">item to check</param>
        /// <returns>True if it can be squeezed into primary</returns>
        private bool CanPutInPrimary(IItem Item)
        {
            return Item.SelfType() == Globals.ItemType.Primary;
        }

        /// <summary>
        /// Both primary and usable type can be put into normal spell/item slots.
        /// </summary>
        /// <param name="Item">Item to check</param>
        /// <returns>True if that item can be put into item/spell slot</returns>
        private bool CanPutInUsable(IItem Item)
        {
            return (Item.SelfType() == Globals.ItemType.Primary || Item.SelfType() == Globals.ItemType.Usable);
        }

        /// <summary>
        /// Copy the sprite of an item. 
        /// Create a new onr instead of a reference.
        /// Note that some item might return a different sprite than what's on the ground. 
        /// </summary>
        /// <param name="item">Item to copy</param>
        /// <returns>A GeneralSprite of the item type</returns>
        private GeneralSprite CopySprite(IItem item)
        {
            GeneralSprite NewSprite;
            GeneralSprite IS = item.GetSprite();

            NewSprite = new GeneralSprite(IS.selfTexture, IS.columnsCount, IS.rowCount,
                IS.rowLimitation, IS.totalFrames, Globals.UI_LAYER);

            return NewSprite;
        }

        public void Update()
        {

            if (primary != null && primary.GetCount() <= 0)
            {
                primary = null; 
                game.headsupDisplay.SetPrimary(null);
                game.bagItemList.Remove(primary);
            }

            for(int i = 0; i < Globals.SLOT_SIZE; i++)
            {
                if (itemSlots[i] != null && itemSlots[i].GetCount() <= 0)
                {
                    itemSlots[i] = null;
                    game.headsupDisplay.SetItemSlot(null, i);
                    game.bagItemList.Remove(itemSlots[i]);
                }
            }

        }

        /// <summary>
        /// When picking up and item, try add it into the slots. 
        /// Shall be added if there are empty slots and the item 
        /// is capable of being put into that slot. 
        /// </summary>
        /// <param name="Item">Item to try</param>
        public void TryAddingItem(IItem Item)
        {
            if(CanPutInPrimary(Item) && primary == null)
            {
                primary = Item;
                game.headsupDisplay.SetPrimary(CopySprite(Item));
            }
            else
            {
                for (int i = 0; i < Globals.SLOT_SIZE; i++)
                {
                    if (CanPutInUsable(Item) && itemSlots[i] == null)
                    {
                        itemSlots[i] = Item; 
                    }
                }
            }
        }

        /// <summary>
        /// Try use the spell/item in primary slot. 
        /// </summary>
        public void UsePrimary()
        {
            if (primary != null && primaryReady)
            {
                primary.UseItem();
                primaryReady = false;
                stopwatch.Restart();

                primary.CountFlux(-1);
            }

            timer = stopwatch.ElapsedMilliseconds;
            if(timer > Globals.KEYBOARD_HOLD)
            {
                primaryReady = true;
                timer = 0; 
            }
        }
    }
}
