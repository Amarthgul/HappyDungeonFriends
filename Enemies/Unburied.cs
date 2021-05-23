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
    /// Unburied remains static and at ease until the player comes too close or is illuminated. 
    /// It will then follow the player regardless of the player has Illuminati buff ot not. 
    /// </summary>
    class Unburied : IEnemy
    {

        private Game1 game;
        private SpriteBatch spriteBatch;

        private IAgent brainAgent; 
        private Vector2 position;
        private Vector2 stagedMovement; 
        public Globals.Direction facingDir;
        private int moveSpeed = (int)( 0.75 * Globals.SCALAR ); 
        private int collisionDMG = -12; 

        Rectangle CollisionRect;
        private int horzontalTop = 4 * Globals.SCALAR;
        private int horzontalBot = 3 * Globals.SCALAR;
        private int sideShrink = 2 * Globals.SCALAR;
        private Rectangle movingHorizontal;
        private Rectangle movingVertical;
        private EnemyBlockCollision enemyBlockCollison; 

        private GeneralSprite unburiedSprite;
        private Color defaultTint = Color.White;
        private float layerOnTop = Globals.ENEMY_LAYER;
        private float layerAtBack = Globals.ENEMY_LAYER - 0.02f;

        private Globals.EnemyTypes selfType = Globals.EnemyTypes.Minion;

        private Enemies.EnemyHealthBar HPBar; 
        private int totalHealth = 20;
        private int currentHealth = 20;
        private Stopwatch damageProtectionSW = new Stopwatch();
        private int recoverTime = 1000;

        private bool startOfEnd = false; 
        private Stopwatch deathSW = new Stopwatch();
        private int fadeStartTime = 1000;
        private int lingeringTime = 1500; 

        public Unburied(Game1 G, Vector2 P)
        {
            game = G;
            position = P;

            spriteBatch = game.spriteBatch;

            LoadSprites();
            UpdateRect();

            HPBar = new Enemies.EnemyHealthBar(game, selfType);

            brainAgent = new Enemies.AgentStupid(this);
            enemyBlockCollison = new EnemyBlockCollision(game);

            facingDir = (Globals.Direction)(Globals.RND.Next() % 4);
            unburiedSprite.rowLimitation = (int)facingDir;

            damageProtectionSW.Restart();
        }

        public void Turn(Globals.Direction NewDir)
        {
            facingDir = NewDir; 
        }

        public void Move()
        {
            stagedMovement = new Vector2(0, 0);

            switch (facingDir)
            {
                case Globals.Direction.Left:
                    stagedMovement.X -= moveSpeed;
                    break;
                case Globals.Direction.Right:
                    stagedMovement.X += moveSpeed;
                    break;
                case Globals.Direction.Up:
                    stagedMovement.Y -= moveSpeed;
                    break;
                case Globals.Direction.Down:
                    stagedMovement.Y += moveSpeed;
                    break;
                default:
                    break;
            }

            unburiedSprite.rowLimitation = (int)facingDir;
            unburiedSprite.Update();
        }

        public void TakeDamage(DamageInstance Damage)
        {
            if(damageProtectionSW.ElapsedMilliseconds > recoverTime)
            {
                currentHealth += Damage.DamageCount;
                damageProtectionSW.Restart();
            }
            
        }

        public void Update(MC MainChara)
        {
            // Change draw layers if player is lower in screen 
            float DrawLayer; 
            if(MainChara.position.Y > position.Y)
            {
                DrawLayer = layerAtBack;
            }
            else
            {
                DrawLayer = layerOnTop;
            }
            unburiedSprite.layer = DrawLayer;

            // Deal with live and die 
            if (currentHealth <= 0)
            {
                UpdateDeath();
            }
            else
            {
                // Change movement depending on player's stats 
                brainAgent.Update(MainChara);


                // Try to move 
                Move();
                // Turn if hit a block
                if (enemyBlockCollison.ValidMove(GetStagedRectangle()))
                {
                    unburiedSprite.Update();

                    position += stagedMovement;
                    stagedMovement = new Vector2(0, 0);
                }
                else
                {
                    Turn(brainAgent.HandleBlockCollision(facingDir));
                }

            }

            // Update collision to follow the movement 
            UpdateRect();
            // Update HP if necessary 
            HPBar.Update(totalHealth, currentHealth);
        }

        public void Draw()
        {
            unburiedSprite.Draw(spriteBatch, position, defaultTint);

            if(currentHealth != totalHealth)
            {
                HPBar.Draw(position);
            }
        }

        public Rectangle GetRectangle()
        {
            switch (facingDir)
            {
                case Globals.Direction.Left:
                    return movingHorizontal;
                case Globals.Direction.Right:
                    return movingHorizontal;
                case Globals.Direction.Up:
                    return movingVertical;
                case Globals.Direction.Down:
                    return movingVertical;
                default:
                    return CollisionRect;
            }
        }

        public bool IsDead()
        {
            return (startOfEnd && deathSW.ElapsedMilliseconds > lingeringTime); 
        }

        /// <summary>
        /// Return a damage instance of on collision. 
        /// </summary>
        /// <returns>DamageInstance object</returns>
        public DamageInstance DealCollisionDamage()
        {
            if (currentHealth == 0)
                return null; 

            DamageInstance DMG = new DamageInstance(collisionDMG, new Globals.DamageEffect [] { Globals.DamageEffect.Knockback });
            DMG.knowckbackDist = collisionDMG;

            return DMG;
        }


        // ================================================================================
        // ================================ Private methods ===============================
        // ================================================================================

        private Rectangle GetStagedRectangle()
        {
            return new Rectangle(
                CollisionRect.X + (int)stagedMovement.X,
                CollisionRect.Y + (int)stagedMovement.Y,
                CollisionRect.Width,
                CollisionRect.Height
                );
        }

        private void LoadSprites()
        {
            ImageFile BB = TextureFactory.Instance.enemyBead;

            unburiedSprite = new GeneralSprite(BB.texture, BB.C, BB.R,
                Globals.WHOLE_SHEET, Globals.FRAME_CYCLE, Globals.ENEMY_LAYER);
            unburiedSprite.frameDelay = 250; 
        }

        private void UpdateRect()
        {
            movingHorizontal = new Rectangle(
                (int)position.X,
                (int)position.Y + horzontalTop,
                Globals.OUT_UNIT,
                Globals.OUT_UNIT - horzontalTop - horzontalBot
                );
            movingVertical = new Rectangle(
                (int)position.X + sideShrink,
                (int)position.Y,
                Globals.OUT_UNIT - 2 * sideShrink,
                Globals.OUT_UNIT
                );

            switch (facingDir)
            {
                case Globals.Direction.Left:
                    CollisionRect = movingHorizontal; 
                    break; 
                case Globals.Direction.Right:
                    CollisionRect = movingHorizontal;
                    break;
                case Globals.Direction.Up:
                    CollisionRect = movingVertical;
                    break;
                case Globals.Direction.Down:
                    CollisionRect = movingVertical;
                    break;
                default:
                    CollisionRect = movingHorizontal;
                    break;
            }
        }

        private void UpdateDeath()
        {
            if(!startOfEnd)
            {
                startOfEnd = true;
                deathSW.Restart();
                ImageFile BD = TextureFactory.Instance.enemyBeadDeath;
                unburiedSprite = new GeneralSprite(BD.texture, BD.C, BD.R,
                    Globals.WHOLE_SHEET, Globals.FRAME_CYCLE, Globals.ENEMY_LAYER);
                unburiedSprite.rowLimitation = (int)facingDir;
                unburiedSprite.positionOffset = new Vector2(-2, -2) * Globals.SCALAR;
                unburiedSprite.currentFrame = 0; 
            }
            else
            {
                if(unburiedSprite.currentFrame != 3)
                    unburiedSprite.Update();
                else if(unburiedSprite.opacity > 0f && deathSW.ElapsedMilliseconds > fadeStartTime)
                {
                    unburiedSprite.opacity -= 0.05f;
                }

                // Gradually fade out 
                if(deathSW.ElapsedMilliseconds > lingeringTime)
                {
                    position = new Vector2(- Globals.OUT_UNIT, - Globals.OUT_UNIT); 
                }
            }
        }

    }
}
