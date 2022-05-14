using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace HappyDungeon
{
    /// <summary>
    /// Fulfilling every method defined in IItem can sometimes be tiedious. 
    /// So this template is created to quickly copy paste into a new item class. 
    /// </summary>
    class IItemSTD : IItem
    {
        protected Game1 game;
        protected GeneralSprite itemSprite;

        protected int selfIndex = Globals.ITEM_STD; 

        // Item hold 
        protected Stopwatch stopwatch;
        protected long timer;
        protected bool cooldownFinished;

        public IItemSTD(Game1 G, Vector2 P)
        {
            game = G; 
            stopwatch = new Stopwatch(G);

            stopwatch.Restart();
        }

        public virtual bool Collectible()
        {
            return true; 
        }

        public virtual IItem Collect()
        {
            return this; 
        }

        public virtual void UseItem()
        {

        }
        public virtual void CountFlux(int Count)
        {

        }

        public virtual int GetCount()
        {
            return 1;
        }

        public void SetCount(int Count)
        {

        }

        public virtual void Update()
        {
            timer = stopwatch.ElapsedMilliseconds;
            if (timer > Globals.ITEM_HOLD)
            {
                cooldownFinished = true;
            }


        }

        public virtual void Draw()
        {

        }

        public virtual void DrawEffects()
        {

        }


        public virtual double CooldownRate()
        {
            return 0;
        }

        public virtual Rectangle GetRectangle()
        {
            return new Rectangle();
        }

        public virtual GeneralSprite GetSprite()
        {
            return itemSprite; 
        }

        public virtual int SelfIndex()
        {
            return Globals.ITEM_STD; 
        }
        public virtual General.Modifiers.IModifier GetPickupModifier()
        {
            return null;
        }

        public virtual General.Modifiers.IModifier GetOutputModifier()
        {
            return null;
        }

        public virtual Globals.ItemType SelfType()
        {
            return Globals.ItemType.Junk;
        }

        public virtual string GetItemDescription() {
            return TextBridge.Instance.GetIndexedDescrption(SelfIndex()); ;
        }

        public int GetPickUpScore()
        {
            return General.ScoreTable.Instance.getScore(
                selfIndex, game.difficulty
                );
        }
    }
}
