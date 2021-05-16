using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace HappyDungeon.Enemies
{
    class EnemyHealthBar
    {
        private const int TOTAL_LENGTH = 10;

        private Game1 game;
        private SpriteBatch spriteBatch;

        private Globals.EnemyTypes targetType; 

        public int totalHealth { set; get; }
        public int currentHealth { set; get; }
        public Color bloodColor { set; get; }

        private Vector2 barBorderOffset;
        private Vector2 barContentOffset;
        private Rectangle destRect; 

        private Texture2D bloodBar; // The inner fill 
        private GeneralSprite bloodBarBorder;

        private Color defaultTint = Color.White;

        public EnemyHealthBar(Game1 G, Globals.EnemyTypes ET)
        {
            game = G;
            targetType = ET;

            spriteBatch = game.spriteBatch;

            bloodColor = Color.Red;

            Setup();
        }

        private void Setup()
        {
            ImageFile BBB = TextureFactory.Instance.healthBarMinions;
            barBorderOffset = new Vector2(2, -1) * Globals.SCALAR;
            barContentOffset = new Vector2(1, 1) * Globals.SCALAR;

            bloodBarBorder = new GeneralSprite(BBB.texture, BBB.C, BBB.R, 
                Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.ENEMY_LAYER);
            bloodBarBorder.positionOffset = barBorderOffset;

            bloodBar = TextureFactory.Instance.GenerateTexture(game.GraphicsDevice, 1, 1, pixel => bloodColor);
        }

        public void Update(int THP, int CHP)
        {
            // Exit if there are changes 
            if (Math.Abs(totalHealth - THP) + Math.Abs(currentHealth - CHP) == 0)
                return; 

            totalHealth = THP;
            currentHealth = CHP;

            double percent = currentHealth / totalHealth;
        }

        public void Draw(Vector2 P)
        {
            destRect = new Rectangle(
                (int)(P.X + barBorderOffset.X + barContentOffset.X), 
                (int)(P.Y + barBorderOffset.Y + barContentOffset.Y),
                (int)((currentHealth / (double)totalHealth) * TOTAL_LENGTH * Globals.SCALAR), 
                1 * Globals.SCALAR);
            
            spriteBatch.Draw(bloodBar, destRect, null, defaultTint, 
                0f, Vector2.Zero, SpriteEffects.None, Globals.ENEMY_LAYER);
            bloodBarBorder.Draw(spriteBatch, P, defaultTint);

        }

    }
}
