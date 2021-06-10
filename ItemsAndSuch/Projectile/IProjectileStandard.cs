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
        protected int moveSpeed = (int)(3.0 * Globals.SCALAR);
        protected int moveInterval = 100;
        protected int currentTravelDistance = 0; 
        protected int TotalravelDistance = (int)(2.0 * Globals.OUT_UNIT); 
        protected Stopwatch moveSW = new Stopwatch();

        protected int collisionWidth = 8;
        protected int collisionHeight = 8; 
        protected Rectangle collisionRect;

        protected bool expired = false;

        public bool isMelee { set; get; }
        public bool isTargetProjectile { set; get; }
        public bool isCurved { set; get; }

        public IProjectileStandard(Game1 G, GeneralSprite GS, DamageInstance DI, Vector2 P, Globals.Direction D)
        {
            game = G;
            selfSprite = GS;
            damageInstance = DI;
            position = P;
            facingDir = D; 

            spriteBatch = game.spriteBatch;

            isMelee = false;
            isTargetProjectile = false;
            isCurved = false;

            UpdateRectangle();
            moveSW.Restart();
        }

        public  virtual void Update()
        {
            if (isCurved)
            {

            }
            else
            {
                UpdateProjectileDefault();

            }
        }

        public virtual void Draw()
        {
            selfSprite.Draw(spriteBatch, position, defaultTint);
        }

        public virtual bool Expired()
        {
            return expired;
        }

        public virtual Rectangle GetRectangle()
        {
            return new Rectangle();
        }

        public virtual DamageInstance GetDamageInstance()
        {
            return damageInstance; 
        }

        // ================================================================================
        // ================================ Private methods ===============================
        // ================================================================================

        protected virtual void UpdateRectangle()
        {
            collisionRect = new Rectangle(
                (int)(position.X + (Globals.OUT_UNIT - collisionWidth) / 2),
                (int)(position.Y + (Globals.OUT_UNIT - collisionHeight) / 2),
                collisionWidth, collisionHeight
                );
        }

        protected virtual void UpdateProjectileDefault()
        {
            if (moveSW.ElapsedMilliseconds < moveInterval) return; 

            switch (facingDir)
            {
                case Globals.Direction.Left:
                    position.X -= moveSpeed;
                    break;
                case Globals.Direction.Right:
                    position.X += moveSpeed;
                    break;
                case Globals.Direction.Up:
                    position.Y -= moveSpeed; 
                    break;
                case Globals.Direction.Down:
                    position.Y += moveSpeed;
                    break;
                default:
                    break;
            }
            currentTravelDistance += moveSpeed;

            if (currentTravelDistance > TotalravelDistance)
                expired = true;

            UpdateRectangle();
            selfSprite.Update();
            moveSW.Restart();
        }

    }
}
