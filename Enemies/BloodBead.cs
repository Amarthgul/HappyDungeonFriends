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
    class BloodBead : IEnemy
    {

        private Game1 game;
        private SpriteBatch spriteBatch;

        private IAgent brainAgent; 
        private Vector2 position;
        private Vector2 stagedMovement; 
        public Globals.Direction facingDir;
        private int currentMoveIndex; 
        private int[] moveSpeed = new int[] {
            (int)(0.1 * Globals.SCALAR),
            (int)(0.3 * Globals.SCALAR),
            (int)(0.6 * Globals.SCALAR),
            (int)(0.3 * Globals.SCALAR) };
        private int collisionDMG = -12; 

        Rectangle CollisionRect;
        private int horzontalTop = 4 * Globals.SCALAR;
        private int horzontalBot = 3 * Globals.SCALAR;
        private int sideShrink = 2 * Globals.SCALAR;
        private Rectangle movingHorizontal;
        private Rectangle movingVertical;
        private EnemyBlockCollision enemyBlockCollison; 

        private GeneralSprite beadSprite;
        private Color defaultTint = Color.White;
        private float layerOnTop = Globals.ENEMY_LAYER;
        private float layerAtBack = Globals.ENEMY_LAYER - 0.02f;

        private Globals.EnemyTypes selfType = Globals.EnemyTypes.Minion;

        private Enemies.EnemyHealthBar HPBar; 
        private int totalHealth = 20;
        private int currentHealth = 20;
        private Stopwatch damageProtectionSW = new Stopwatch();
        private int recoverTime = 1000;

        public BloodBead(Game1 G, Vector2 P)
        {
            game = G;
            position = P;

            spriteBatch = game.spriteBatch;

            LoadSprites();
            UpdateRect();

            HPBar = new Enemies.EnemyHealthBar(game, selfType);

            brainAgent = new Enemies.AgentStupid(this);
            enemyBlockCollison = new EnemyBlockCollision(game);

            currentMoveIndex = 0;
            facingDir = (Globals.Direction)(Globals.RND.Next() % 4);
            beadSprite.rowLimitation = (int)facingDir;

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
                    stagedMovement.X -= moveSpeed[currentMoveIndex];
                    break;
                case Globals.Direction.Right:
                    stagedMovement.X += moveSpeed[currentMoveIndex];
                    break;
                case Globals.Direction.Up:
                    stagedMovement.Y -= moveSpeed[currentMoveIndex];
                    break;
                case Globals.Direction.Down:
                    stagedMovement.Y += moveSpeed[currentMoveIndex];
                    break;
                default:
                    break;
            }

            beadSprite.rowLimitation = (int)facingDir;
            beadSprite.Update();
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
            beadSprite.layer = DrawLayer;

            brainAgent.Update(MainChara);
            HPBar.Update(totalHealth, currentHealth) ;

            Move();
            if (enemyBlockCollison.ValidMove(GetStagedRectangle()))
            {
                beadSprite.Update();
                currentMoveIndex = beadSprite.currentFrame % Globals.FRAME_CYCLE;

                position += stagedMovement;
                stagedMovement = new Vector2(0, 0);
            }
            else
            {
                Turn(brainAgent.HandleBlockCollision(facingDir)) ;
            }

            UpdateRect();
        }

        public void Draw()
        {
            beadSprite.Draw(spriteBatch, position, defaultTint);

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

        /// <summary>
        /// Return a damage instance of on collision. 
        /// </summary>
        /// <returns>DamageInstance object</returns>
        public DamageInstance DealCollisionDamage()
        {

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

            beadSprite = new GeneralSprite(BB.texture, BB.C, BB.R,
                Globals.WHOLE_SHEET, Globals.FRAME_CYCLE, Globals.ENEMY_LAYER);
            beadSprite.frameDelay = 250; 
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

    }
}
