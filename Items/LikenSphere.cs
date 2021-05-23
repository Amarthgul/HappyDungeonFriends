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
    /// Liken Sphere (should be called Liken's Sphere) is a reference of DotA 2's item. 
    /// Here it can be used to activate a shield that nullifies next incoming damage instance.  
    /// The shield lasts 8 seconds and has a 12 seconds' colldaown. 
    /// </summary>
    class LinkenSphere :IItem
    {
        private GeneralSprite likenSphereSprite;
        private Rectangle collisionRect;
        private Vector2 position;

        private Game1 game;
        private SpriteBatch spriteBatch;

        // Item hold 
        private Stopwatch stopwatch = new Stopwatch();
        private long timer;
        private bool cooldownFinished;

        public LinkenSphere(Game1 G)
        {
            game = G; 

        }

        public bool Collectible()
        {
            return true; 
        }

        public IItem Collect()
        {
            return this; 
        }

        public void UseItem()
        {

        }
        public void CountFlux(int Count)
        {

        }

        public int GetCount()
        {
            return 1;
        }

        public void Update()
        {
            timer = stopwatch.ElapsedMilliseconds;
            if (timer > Globals.ITEM_HOLD)
            {
                cooldownFinished = true;
            }


        }

        public void Draw()
        {

        }


        public Rectangle GetRectangle()
        {
            return new Rectangle();
        }

        public GeneralSprite GetSprite()
        {
            return likenSphereSprite; 
        }

        public int SelfIndex()
        {
            return 0; 
        }
        public General.Modifiers.IModifier GetPickupModifier()
        {
            return null;
        }

        public General.Modifiers.IModifier GetOutputModifier()
        {
            return null;
        }

        public Globals.ItemType SelfType()
        {
            return Globals.ItemType.Junk;
        }

    }
}
