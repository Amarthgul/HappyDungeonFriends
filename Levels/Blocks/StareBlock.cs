using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace HappyDungeon
{
    class StareBlock : IBlock
    {
        private const int STARE_BLOCK_ROW = 9; 

        private GeneralSprite blockSprite;
        private Vector2 position;
        private Rectangle collisionRect;

        private Game1 game;
        private SpriteBatch spriteBatch;

        private bool expressionOn = false;
        private bool experssionProtection = false; 

        private Color defaultTint = Color.White;

        public StareBlock(Game1 G, Vector2 P)
        {
            game = G;
            position = P;

            spriteBatch = game.spriteBatch;
            collisionRect = new Rectangle((int)P.X, (int)P.Y, Globals.OUT_UNIT, Globals.OUT_UNIT);

            SetUpSprite();
        }

        private void SetUpSprite()
        {
            ImageFile AM = TextureFactory.Instance.blockAllMight;

            blockSprite = new GeneralSprite(AM.texture, AM.C, AM.R, STARE_BLOCK_ROW, Globals.ONE_FRAME, Globals.BLOCKS_LAYER);
            blockSprite.colLimitation = 0; 
        }

        public void Update()
        {
            if (expressionOn)
            {

            }
            else
            {
                Vector2 CharaPositon = game.mainChara.position; 


            }
        }


        public void Draw()
        {
            blockSprite.Draw(spriteBatch, position, defaultTint);
        }

        public Rectangle GetRectangle()
        {
            return collisionRect; 
        }
    }
}
