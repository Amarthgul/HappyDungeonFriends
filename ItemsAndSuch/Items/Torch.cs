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
    class Torch : IItem
    {

        private GeneralSprite torchSprite;
        private Rectangle collisionRect;
        private Vector2 position;

        private Game1 game; 
        private SpriteBatch spriteBatch;

        // Item hold 
        private Stopwatch stopwatch;
        private long timer;
        private bool cooldownFinished;

        private int torchCount; 

        private Color defaultTint = Color.White;
        private Globals.ItemType selfType = Globals.ItemType.Primary;
        private int selfIndex = Globals.ITEM_TORCH;

        public bool torchOn { set; get; }

        public Torch(Game1 G, Vector2 P)
        {
            game = G;
            spriteBatch = game.spriteBatch;
            position = P;

            stopwatch = new Stopwatch(game);

            collisionRect = new Rectangle((int)P.X, (int)P.Y, Globals.OUT_UNIT, Globals.OUT_UNIT);

            ImageFile TS = TextureFactory.Instance.itemTorch;
            torchSprite = new GeneralSprite(TS.texture, TS.C, TS.R, 0, Globals.ONE_FRAME, Globals.ITEM_LAYER);

            torchCount = 1; // Theoretically never change 
            torchOn = false;

            stopwatch.Restart();
            cooldownFinished = false;
        }


        public IItem Collect()
        {

            return this;
        }

        /// <summary>
        /// If this is called, then torch must be in primary slots. 
        /// </summary>
        public void UseItem()
        {
            GeneralSprite NewSprite;
            GeneralSprite IS = GetSprite(); 

            torchOn = !torchOn;

            if (torchOn)
            {
                NewSprite = new GeneralSprite(IS.selfTexture, IS.columnsCount, IS.rowCount,
                    2, Globals.FRAME_CYCLE, Globals.UI_LAYER);
            }
            else
            {
                NewSprite = new GeneralSprite(IS.selfTexture, IS.columnsCount, IS.rowCount,
                    0, Globals.FRAME_CYCLE, Globals.UI_LAYER);
            }

            game.mainChara.ToggleTorch();
            game.fogOfWar.ToggleClairvoyant(1.5f);
            game.fogOfWar.ToggleShakyMode();
            game.headsupDisplay.SetPrimary(NewSprite);
        }

        /// <summary>
        /// Torch does not change count. 
        /// </summary>
        /// <param name="Count">Whatever</param>
        public void CountFlux(int Count)
        {
            // Do nothing
        }

        public int GetCount()
        {
            return torchCount;
        }

        public void SetCount(int Count)
        {
            // Do nothing 
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
            torchSprite.Draw(spriteBatch, position, defaultTint);
        }

        public void DrawEffects()
        {

        }

        public bool Collectible()
        {
            return cooldownFinished;
        }

        public Rectangle GetRectangle()
        {
            return collisionRect;
        }

        public GeneralSprite GetSprite()
        {
            return torchSprite;
        }
        public double CooldownRate()
        {
            return 1; // Always out of CD 
        }
        public int SelfIndex()
        {
            return selfIndex;
        }

        public General.Modifiers.IModifier GetPickupModifier()
        {
            return new General.Modifiers.ModifierTorch();
        }

        public General.Modifiers.IModifier GetOutputModifier()
        {
            if (game.mainChara.primaryState != Globals.primaryTypes.Torch)
                return null; 

            return new General.Modifiers.ModifierBurn(game, 5);
        }

        public Globals.ItemType SelfType()
        {
            return selfType;
        }


        public string GetItemDescription()
        {
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
