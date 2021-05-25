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

        private Color defaultTint = Color.White;

        public LinkenSphere(Game1 G, Vector2 P)
        {
            game = G;
            position = P;

            spriteBatch = game.spriteBatch;

            collisionRect = new Rectangle((int)P.X, (int)P.Y, Globals.OUT_UNIT, Globals.OUT_UNIT);

            ImageFile LS = TextureFactory.Instance.itemLikenSphere;
            ImageFile LSS = TextureFactory.Instance.itemLikenSphereShield;

            likenSphereSprite = new GeneralSprite(LS.texture, 1, 1, 
                Globals.WHOLE_SHEET, 1, Globals.ITEM_LAYER);

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
            likenSphereSprite.Draw(spriteBatch, position, defaultTint);
        }


        public Rectangle GetRectangle()
        {
            return collisionRect;
        }

        public GeneralSprite GetSprite()
        {
            ImageFile LSOB = TextureFactory.Instance.itemLikenSphereOnBar; 
            
            return new GeneralSprite(LSOB.texture, LSOB.C, LSOB.R, 
                Globals.WHOLE_SHEET, Globals.FRAME_CYCLE, Globals.ITEM_LAYER); 
        }

        public int SelfIndex()
        {
            return Globals.ITEM_LINKEN; 
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
            return Globals.ItemType.Usable;
        }

    }
}
