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
    class IProjectileStandard : IProjectile
    {

        protected Game1 game;
        protected SpriteBatch spriteBatch;

        protected DamageInstance damageInstance;

        protected GeneralSprite selfSprite;
        protected Color defaultTint = Color.White;

        protected Vector2 position;
        protected Globals.Direction facingDir;
        protected int moveSpeed = (int)(0.6 * Globals.SCALAR);
        protected int moveInterval = 100;
        protected int travelDistance = (int)(2.0 * Globals.OUT_UNIT); 
        protected Stopwatch moveSW = new Stopwatch();

        public bool isMelee { set; get; }
        public bool isTargetProjectile { set; get; }
        public bool isCurved { set; get; }

        public IProjectileStandard(Game1 G, GeneralSprite GS, DamageInstance DI, Vector2 P)
        {
            game = G;
            selfSprite = GS;
            damageInstance = DI;
            position = P; 

            spriteBatch = game.spriteBatch;

            isMelee = false;
            isTargetProjectile = false;
            isCurved = false;
        }

        public  virtual void Update()
        {

        }

        public virtual void Draw()
        {
            selfSprite.Draw(spriteBatch, position, defaultTint);
        }

        public virtual Rectangle GetRectangle()
        {
            return new Rectangle();
        }

        public virtual DamageInstance GetDamageInstance()
        {
            return damageInstance; 
        }
    }
}
