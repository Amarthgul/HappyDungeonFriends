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
    class NoteSetOne :IItem
    {
        private GeneralSprite itemSprite;
        private Vector2 position; 
        private Rectangle collisionRect;


        private Game1 game;
        private SpriteBatch spriteBatch;

        // Item hold 
        private Stopwatch stopwatch = new Stopwatch();
        private long timer;
        private bool cooldownFinished;

        private Color defaultTint = Color.White; 

        public NoteSetOne(Game1 G, Vector2 P)
        {
            game = G;
            spriteBatch = game.spriteBatch;
            position = P;

            collisionRect = new Rectangle((int)P.X, (int)P.Y, Globals.OUT_UNIT, Globals.OUT_UNIT);

            ImageFile NSO = TextureFactory.Instance.itemNoteSetOne;
            itemSprite = new GeneralSprite(NSO.texture, NSO.C, NSO.R, 
                0, Globals.FRAME_CYCLE, Globals.ITEM_LAYER);
            itemSprite.colLimitation = Globals.RND.Next() % Globals.FRAME_CYCLE;

            stopwatch.Restart();
            cooldownFinished = false;
        }

        public bool Collectible()
        {
            return cooldownFinished; 
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
            itemSprite.Draw(spriteBatch, position, defaultTint);
        }

        public void DrawEffects()
        {

        }


        public double CooldownRate()
        {
            return 0;
        }

        public Rectangle GetRectangle()
        {
            return collisionRect;
        }

        public GeneralSprite GetSprite()
        {
            return itemSprite; 
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
        public string GetItemDescription()
        {
            return TextBridge.Instance.GetIndexedDescrption(SelfIndex()); ;
        }
    }
}
