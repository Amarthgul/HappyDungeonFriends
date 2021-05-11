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
    class DroppedGold : IItem
    {
        private const int GOLD_TIER_ROWS = 3; 

        private GeneralSprite goldOnGround;
        private GeneralSprite goldFX;
        private Rectangle collisionRect;
        private Vector2 position;

        private Game1 game;
        private SpriteBatch spriteBatch;

        // Item hold 
        private Stopwatch stopwatch = new Stopwatch();
        private long timer;
        private bool cooldownFinished;

        private int goldCount;
        

        private Color defaultTint = Color.White;
        private Globals.ItemType selfType = Globals.ItemType.Junk;  // Money is useless essentially 
        private int selfIndex = Globals.ITEM_GOLD; 

        public DroppedGold(Game1 G, Vector2 P)
        {
            game = G;
            position = P;

            spriteBatch = game.spriteBatch;

            ImageFile GOG = TextureFactory.Instance.goldOnGround;
            ImageFile GFX = TextureFactory.Instance.goldOnGroundFX;

            goldOnGround = new GeneralSprite(GOG.texture, GOG.C, GOG.R, 
                Globals.WHOLE_SHEET, Globals.FRAME_CYCLE, Globals.ITEM_LAYER);
            goldFX = new GeneralSprite(GFX.texture, GFX.C, GFX.R,
                Globals.WHOLE_SHEET, Globals.FRAME_CYCLE, Globals.ITEM_LAYER);
            

            goldCount = 45;
            SetGoldSize();

            cooldownFinished = false;
        }

        private void SetGoldSize()
        {
            int Tier = (goldCount / 10 > GOLD_TIER_ROWS) ? GOLD_TIER_ROWS : (int)Math.Floor(goldCount / 10.0);
            int Offset = GOLD_TIER_ROWS - Tier;

            goldOnGround.rowLimitation = Tier;
            goldFX.rowLimitation = Tier;

            collisionRect = new Rectangle(
                (int)position.X + Offset,
                (int)position.Y + Offset,
                Globals.OUT_UNIT - 2 * Offset,
                Globals.OUT_UNIT - 2 * Offset
                );
        }

        public void SetGoldAmount(int Amount)
        {
            goldCount = Amount;

            SetGoldSize(); 
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
            goldCount += Count; 
        }

        public int GetCount()
        {
            return goldCount;
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
            

            if (game.mainChara.Illuminati())
            {
                // Add gold glowing FX 
                goldFX.Draw(spriteBatch, position, defaultTint);

                // Update the sprite animation only if being lit by torch
                goldOnGround.Update();
                goldFX.Update();
            }
            else
            {
                goldOnGround.colLimitation = 0; 
            }

            goldOnGround.Draw(spriteBatch, position, defaultTint);

        }

        public Rectangle GetRectangle()
        {
            return collisionRect;
        }

        public GeneralSprite GetSprite()
        {
            return goldOnGround;
        }

        public int SelfIndex()
        {
            return selfIndex;
        }

        public Globals.ItemType SelfType()
        {
            return selfType;
        }

    }
}
