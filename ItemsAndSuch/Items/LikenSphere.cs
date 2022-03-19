using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
//using System.Diagnostics;

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
        private GeneralSprite likenShieldSprite;
        private Vector2 shieldOffset = new Vector2(-2, -2) * Globals.SCALAR;

        // When item is on the ground 
        private Rectangle collisionRect;
        private Vector2 position;

        private Game1 game;
        private SpriteBatch spriteBatch;

        // Item cooldown and pickup protection  
        private int itemEffectiveTime = 8000;  // 8 seconds 
        private int itemCoolDown = 12000;      // 12 Seconds
        private Stopwatch stopwatch;
        private bool shieldOn = false;
        private bool pickupProtection = false;
        private bool cooldownFinished = true; 

        private Color defaultTint = Color.White;

        private Stopwatch radiatingSFXSW;
        private int radSoundInterval = 400; 

        public LinkenSphere(Game1 G, Vector2 P)
        {
            game = G;
            position = P;

            radiatingSFXSW = new Stopwatch(game);
            stopwatch = new Stopwatch(game);
            
            spriteBatch = game.spriteBatch;

            collisionRect = new Rectangle((int)P.X, (int)P.Y, Globals.OUT_UNIT, Globals.OUT_UNIT);

            ImageFile LS = TextureFactory.Instance.itemLikenSphere;
            ImageFile LSS = TextureFactory.Instance.itemLikenSphereShield;

            likenSphereSprite = new GeneralSprite(LS.texture, 1, 1, 
                Globals.WHOLE_SHEET, 1, Globals.ITEM_LAYER);
            likenShieldSprite = new GeneralSprite(LSS.texture, LSS.C, LSS.R,
                Globals.WHOLE_SHEET, LSS.C * LSS.R, Globals.ITEM_EFFECT_LAYER);

            stopwatch.Restart();
            radiatingSFXSW.Restart();
        }

        public bool Collectible()
        {
            return pickupProtection; 
        }

        public IItem Collect()
        {
            return this; 
        }

        public void UseItem()
        {
            if(cooldownFinished && !shieldOn)
            {
                shieldOn = true;
                cooldownFinished = false;
                SoundFX.Instance.PlayItemLikenOn();

                stopwatch.Restart();
            }
        }

        /// <summary>
        /// Liken sphere's count flex marks a nullification. 
        /// Thus terminates the shield. 
        /// </summary>
        /// <param name="Count"></param>
        public void CountFlux(int Count)
        {
            // 1 means the sheild has been triggered 
            // Triggered in SpellSlots IncomingDamageGernealModifier()
            if (Count == 1)
            {
                shieldOn = false;
                SoundFX.Instance.PlayItemLinkenBreak();
            }
        }

        /// <summary>
        /// Updates pick up protection when it's on the ground.
        /// After being picked up, updates cooldown when being used. 
        /// </summary>
        public void Update()
        {
            if (Globals.DEBUGGING && game.gameState != Globals.GameStates.Running)
            {
                long current = stopwatch.ElapsedMilliseconds;
            }

            if (stopwatch.ElapsedMilliseconds > Globals.ITEM_HOLD && pickupProtection == false)
            {
                pickupProtection = true;
                stopwatch.Restart();
                stopwatch.Stop();
            }

            // Mark the shield as epxired when it exceeds the lasting time 
            if(stopwatch.ElapsedMilliseconds > itemEffectiveTime && shieldOn)
            {
                shieldOn = false;
            }

            // mark the item useablt again after cooldown 
            if( stopwatch.ElapsedMilliseconds > itemCoolDown)
            {
                cooldownFinished = true;
            }
        }

        public void Draw()
        {
            likenSphereSprite.Draw(spriteBatch, position, defaultTint);
        }

        /// <summary>
        /// Draw the shield effect. Also updates the sprite for animation. 
        /// </summary>
        public void DrawEffects()
        {
            if (shieldOn)
            {
                if (radiatingSFXSW.ElapsedMilliseconds > radSoundInterval)
                {
                    radiatingSFXSW.Restart();
                    SoundFX.Instance.PlayitemLinkenRadiating();
                }
                likenShieldSprite.Draw(spriteBatch, game.mainChara.position + shieldOffset, defaultTint);
                likenShieldSprite.Update();
            }
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
        public double CooldownRate()
        { 

            if (cooldownFinished)
                return 1.0;
            else 
                return stopwatch.ElapsedMilliseconds / (double)itemCoolDown;
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
            if (shieldOn)
                return new General.Modifiers.ModifierNullify();
            else
                return null;
        }

        public Globals.ItemType SelfType()
        {
            return Globals.ItemType.Usable;
        }

        public int GetCount()
        {
            return 1;
        }

        public string GetItemDescription()
        {
            return TextBridge.Instance.GetIndexedDescrption(SelfIndex());
        }
    }
}
