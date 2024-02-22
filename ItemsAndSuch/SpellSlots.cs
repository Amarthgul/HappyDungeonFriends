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

        public List<IItem> bag { set; get; }

        // Slots on the headsup UI
        public IItem primary = null;
        public List<IItem> itemSlots = new List<IItem>(Globals.SLOT_SIZE) { null, null, null};

        private bool primaryReady = true;
        private bool[] itemsReady = new bool[] { true, true, true };

        private ModifierCollection modifiers;

        private Stopwatch stopwatch;
        private Stopwatch[] itemsSW;
        private long timer;

        public SpellSlots(Game1 G)
        {
            game = G;

            stopwatch = new Stopwatch(game);
            itemsSW = new Stopwatch[] { new Stopwatch(game), new Stopwatch(game), new Stopwatch(game) };

            modifiers = new ModifierCollection();

            bag = new List<IItem>(Globals.BAG_SIZE);
            for (int i = 0; i < Globals.BAG_SIZE; i++) bag.Add(null); 
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

        // ================================================================================
        // ============================== Public methods ==================================
        // ================================================================================

        /// <summary>
        /// Only primary items can be put into primary slot.
        /// </summary>
        /// <param name="Item">item to check</param>
        /// <returns>True if it can be squeezed into primary</returns>
        public bool CanPutInPrimary(IItem Item)
        {
            return Item.SelfType() == Globals.ItemType.Primary;
        }

        /// <summary>
        /// Both primary and usable type can be put into normal spell/item slots.
        /// </summary>
        /// <param name="Item">Item to check</param>
        /// <returns>True if that item can be put into item/spell slot</returns>
        public bool CanPutInUsable(IItem Item)
        {
            return (Item.SelfType() == Globals.ItemType.Primary || Item.SelfType() == Globals.ItemType.Usable);
        }

        /// <summary>
        /// Put an item into primary. 
        /// If this is called, it's pre-assumed that the item can be put into primary. 
        /// </summary>
        /// <param name="Item">Item to be placed</param>
        public void PutIntoPrimary(IItem Item)
        {
            primary = Item;

            if (Item != null)
            {
                game.headsupDisplay.SetPrimary(CopySprite(Item));

                if (Item.GetPickupModifier() != null)
                    modifiers.Add(Item.GetPickupModifier());
            }
            else
            {
                game.headsupDisplay.SetPrimary(null);
            }
        }

        /// <summary>
        /// Put an item into one of the three useablt slots.
        /// If this is called, it's pre-assumed that the item can be put into useable. 
        /// </summary>
        /// <param name="Item">Item to be placed</param>
        /// <param name="Index">Which index to be put into</param>
        public void PutIntoUsable(IItem Item, int Index)
        {
            itemSlots[Index] = Item;

            if(Item != null)
            {
                game.headsupDisplay.SetItemSlot(Item.GetSprite(), Index);

                if (Item.GetPickupModifier() != null)
                    modifiers.Add(Item.GetPickupModifier());
            }
            else
            {
                game.headsupDisplay.SetItemSlot(null, Index);
            }
        }
        
        /// <summary>
        /// Put an item into the bag.
        /// </summary>
        /// <param name="Item">Item to be placed</param>
        /// <param name="Index">Which index to be put into</param>
        public void PutIntoBag(IItem Item, int Index)
        {
            bag[Index] = Item;
        }

        /// <summary>
        /// Remove any item in the slot that got depleted 
        /// </summary>
        public void Update()
        {
            if(primary == null)
            {
                game.headsupDisplay.SetPrimary(null);
            }

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
                    if (itemSlots[i].GetPickupModifier() != null)
                        modifiers.Remove(itemSlots[i].GetPickupModifier());

                    itemSlots[i] = null;
                    game.headsupDisplay.SetItemSlot(null, i);
                    game.bagItemList.Remove(itemSlots[i]);

                    itemSlots[i].Update();
                }
            }

        }

        /// <summary>
        /// When picking up and item, try add it into the slots. 
        /// Shall be added if there are empty slots and the item 
        /// is capable of being put into that slot. 
        /// </summary>
        /// <param name="Item">Item to try</param>
        public bool TryAddingItem(IItem Item)
        {
            if(CanPutInPrimary(Item) && primary == null)
            {
                PutIntoPrimary(Item);
                return true;
            }
            else if(CanPutInUsable(Item))
            {
                for (int i = 0; i < Globals.SLOT_SIZE; i++)
                {
                    if (itemSlots[i] == null)
                    {
                        PutIntoUsable(Item, i);
                        return true;
                    }
                }
            }
            else // Put into bag 
            {
                for ( int i = 0; i < Globals.BAG_SIZE; i++)
                {
                    if (bag[i] == null && !(Item is DroppedGold))
                    {
                        PutIntoBag(Item, i);
                        game.bagItemList.Add(Item);
                        return true;
                    }
                }
            }

            return false; 
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

        /// <summary>
        /// Try use the items in the item slots 
        /// </summary>
        /// <param name="Slot">Slot marker</param>
        public void UseItems(int Slot)
        {
            if (itemSlots[Slot] != null && itemsReady[Slot])
            {
                itemSlots[Slot].UseItem();
                itemsReady[Slot] = false;
                itemsSW[Slot].Restart();

                itemSlots[Slot].CountFlux(-1);
            }

            if (itemsSW[Slot].ElapsedMilliseconds > Globals.KEYBOARD_HOLD)
            {
                itemsReady[Slot] = true;
            }
        }

        /// <summary>
        /// Provides MC method to inquiry current primary 
        /// </summary>
        /// <returns>Type of current primary</returns>
        public Globals.primaryTypes GetPrimaryType()
        {
            if (primary is Torch)
                return Globals.primaryTypes.Torch;
            else
                return Globals.primaryTypes.None;
        }

        /// <summary>
        /// For other classes to make inquries about the items currently on heads up display. 
        /// </summary>
        /// <param name="Index">Index of item in slots, negative for the primary</param>
        /// <returns>The item in the slot</returns>
        public IItem GetItem(int Index)
        {
            if (Index < 0)
                return primary;
            else
                return itemSlots[Index]; 
        }

        /// <summary>
        /// Find the item in bag at the given index. 
        /// Null if there's nothing. 
        /// </summary>
        /// <param name="Index">index of the item</param>
        /// <returns>Item if there are any</returns>
        public IItem GetBagItem(int Index)
        {
            if (Index < Globals.BAG_SIZE && bag[Index] != null)
                return bag[Index];
            else
                return null; 
        }

        /// <summary>
        /// The first pass of damage instance processing. 
        /// This methods examines all the modifiers and see if this whole damage instance need
        /// to be changed, if so ,perform the change and return it. 
        /// </summary>
        /// <param name="DMGI">Original incoming damage instance</param>
        /// <returns>Damage instance after modified</returns>
        public DamageInstance IncomingDamageGernealModifier(DamageInstance DMGI)
        {
            // Creates a blank damage instance with all things nullified 
            DamageInstance Result = new DamageInstance(0, new Globals.DamageEffect[] { Globals.DamageEffect.None });

            // If the incoming damge can be nullified, then return this nullified instance 
            foreach (IItem Item in itemSlots)
            {
                if (Item == null) continue;

                if (Item.GetOutputModifier() != null &&
                    Item.GetOutputModifier() is General.Modifiers.ModifierNullify)
                {
                    Item.CountFlux(1); // 1 marks a successful absorption 
                    return Result;
                }  
            }

            // TODO: add more conditions 

            return DMGI; 
        }

        /// <summary>
        /// Modifies incoming damage. 
        /// </summary>
        /// <param name="IncomingDamage"></param>
        /// <returns></returns>
        public int DamageReceivingModifier(int IncomingDamage)
        {
            return IncomingDamage; 
        }

        /// <summary>
        /// Find the attack type of the character. 
        /// If no modifier is on then by default returns none. 
        /// </summary>
        /// <returns>Type of attack, mainly melee or ranged</returns>
        public Globals.AttackType GetAttackType()
        {
            if (modifiers.IsEmpty()) return Globals.AttackType.Melee;

            return modifiers.AttackTypeModifier();
        }

        /// <summary>
        /// Find out the melee attack range of the character.
        /// </summary>
        /// <returns>Melee range bounus by all modifiers</returns>
        public int GetMeleeAttackRange()
        {
            return modifiers.MeleeRangeModifier();
        }

        /// <summary>
        /// Create a damage instance for the chsracter's melee attack. 
        /// </summary>
        /// <param name="MeleeDamge">Baseline damage</param>
        /// <returns>A damage instance with modifier effects</returns>
        public DamageInstance PlayerMeleeDamageInstance(int MeleeDamge)
        {
            int TotalInstantDamage = MeleeDamge + modifiers.DamageDealtModifer();

            DamageInstance DMG = new DamageInstance(-TotalInstantDamage, modifiers.DamageDealtEffectModifer());

            return DMG; 
        }


    }
}
