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


        private bool CanPutInPrimary(IItem Item)
        {
            return Item.SelfType() == Globals.ItemType.Primary;
        }

        private bool CanPutInUsable(IItem Item)
        {
            return (Item.SelfType() == Globals.ItemType.Primary || Item.SelfType() == Globals.ItemType.Usable);
        }

        private GeneralSprite CopySprite(IItem item)
        {
            GeneralSprite NewSprite;
            GeneralSprite IS = item.GetSprite();

            NewSprite = new GeneralSprite(IS.selfTexture, IS.columnsCount, IS.rowCount, 
                Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.UI_LAYER);

            return NewSprite;
        }

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

        public void UsePrimary()
        {
            if (primary != null && primaryReady)
            {
                primary.UseItem();
                primaryReady = false;
                stopwatch.Restart();
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
