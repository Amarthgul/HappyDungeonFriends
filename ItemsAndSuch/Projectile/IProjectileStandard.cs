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
        protected Object source; // Whoever produced this projectile 

        protected DamageInstance damageInstance;

        public GeneralSprite selfSprite { get; set; }
        public Color defaultTint { get; set; }

        protected Vector2 position;
        protected Globals.Direction facingDir;
        protected Stopwatch moveSW = new Stopwatch();
        protected Vector2 initialPosition; 
        public int moveSpeed { get; set; }
        public int moveInterval { get; set; }
        public int currentTravelDistance { get; set; }
        public int totalravelDistance { get; set; }
        public int meleeOffset { get; set; }
        public int meleeLastingTime { get; set; }

        protected int collisionWidth = 8;
        protected int collisionHeight = 8; 
        protected Rectangle collisionRect;
        protected ProjectileCollision collisionDect;

        protected Stopwatch selfSW = new Stopwatch();
        protected int lifeEntension = 50;   // Leave a bit more time for melee attack 
        protected bool expireOnHit = true;
        protected bool expired = false;

        public bool isMelee { set; get; }
        public bool isTargetProjectile { set; get; }
        public bool isCurved { set; get; }

        public IProjectileStandard(Game1 G, GeneralSprite GS, DamageInstance DI, Vector2 P, 
            Globals.Direction D, Object S)
        {
            game = G;
            selfSprite = GS;
            damageInstance = DI;
            position = P;
            facingDir = D;
            source = S;

            spriteBatch = game.spriteBatch;

            isMelee = false;
            isTargetProjectile = false;
            isCurved = false;
            collisionDect = new ProjectileCollision(game);

            totalravelDistance = (int)(2.0 * Globals.OUT_UNIT);
            currentTravelDistance = 0; 
            moveSpeed = (int)(3.0 * Globals.SCALAR);
            moveInterval = 100;
            defaultTint = Color.White;
            meleeOffset = (int)(1.0 * Globals.OUT_UNIT);

            UpdateRectangle();
            moveSW.Restart();
            selfSW.Restart();
        }

        public virtual void MarkAsMelee(Object Source)
        {
            isMelee = true; 
            if (Source is IEnemy)
            {
                source = Source;
                initialPosition = ((IEnemy)Source).GetPosition();
            }
        }

        public  virtual void Update()
        {
            // This part deals with its movement 
            if (isMelee) {
                UpdateMelee();
            } 
            else  {
                if (isCurved) {

                }
                else {
                    UpdateProjectileDefault();
                }
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

        public virtual bool IsMelee()
        {
            return isMelee;
        }

        public virtual Rectangle GetRectangle()
        {
            return collisionRect;
        }

        /// <summary>
        /// Return the rectangle of the source of this projectile. 
        /// In most cases it's an enemy. 
        /// </summary>
        /// <returns>Rectangle of the one produced this rectangle</returns>
        public virtual Rectangle GetSrcRectangle()
        {
            if (source is IEnemy)
            {
                return ((IEnemy)source).GetRectangle();
            }
            else
            {
                return new Rectangle() ; 
            }
        }

        public virtual DamageInstance GetDamageInstance()
        {
            return damageInstance; 
        }

        // ================================================================================
        // ================================ Private methods ===============================
        // ================================================================================

        protected virtual void CheckHittingTarget()
        {
            if (source is IEnemy)
            {
                if (collisionDect.CollidedWithPlayer(collisionRect))
                {
                    game.mainChara.TakeDamage(damageInstance, facingDir);
                    if (expireOnHit)
                        expired = true;
                }
            }
        }

        protected virtual void UpdateRectangle()
        {
            collisionRect = new Rectangle(
                (int)(position.X + (Globals.OUT_UNIT - collisionWidth) / 2),
                (int)(position.Y + (Globals.OUT_UNIT - collisionHeight) / 2),
                collisionWidth, collisionHeight
                );
        }

        /// <summary>
        /// The normal update for projectile created by ranged attack 
        /// Expired is calculated by distance travelled. 
        /// </summary>
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

            UpdateRectangle();
            CheckHittingTarget();

            if (currentTravelDistance > totalravelDistance || collisionDect.CollidedWithEnv(collisionRect))
                expired = true;

            selfSprite.Update();
            moveSW.Restart();
        }

        /// <summary>
        /// Update method for melee attack projectile.
        /// Expire is calculated by time. 
        /// </summary>
        protected virtual void UpdateMelee()
        {
            if (source is IEnemy)
            {
                Vector2 Srcposition = ((IEnemy)source).GetPosition();
                switch (facingDir)
                {
                    case Globals.Direction.Left:
                        currentTravelDistance = (int)Math.Abs(initialPosition.X - Srcposition.X);
                        position.X = initialPosition.X - meleeOffset - currentTravelDistance;
                        break;
                    case Globals.Direction.Right:
                        currentTravelDistance = (int)Math.Abs(initialPosition.X - Srcposition.X);
                        position.X = initialPosition.X + meleeOffset + currentTravelDistance;
                        break;
                    case Globals.Direction.Up:
                        currentTravelDistance = (int)Math.Abs(initialPosition.Y - Srcposition.Y);
                        position.Y = initialPosition.Y - meleeOffset - currentTravelDistance;
                        break;
                    case Globals.Direction.Down:
                        currentTravelDistance = (int)Math.Abs(initialPosition.Y - Srcposition.Y);
                        position.Y = initialPosition.Y + meleeOffset + currentTravelDistance;
                        break;
                    default:
                        break;
                }

                CheckHittingTarget();

                if (selfSW.ElapsedMilliseconds > (meleeLastingTime + lifeEntension))
                {
                    expired = true;
                }

                selfSprite.Update();
            }
        }

    }
}
