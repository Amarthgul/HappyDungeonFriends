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
    /// This is a meta class for basic enemies 
    /// </summary>
    class IEnemySTD : IEnemy
    {
        public Game1 game { get; set; }
        public SpriteBatch spriteBatch { get; set; }

        // ================================================================================
        // =================================== Basic stats=================================
        // ================================================================================
        public Globals.Direction facingDir;

        protected IAgent brainAgent;
        protected Vector2 position;
        protected Vector2 stagedMovement;

        protected Globals.EnemyTypes selfType = Globals.EnemyTypes.Minion;

        // ================================================================================
        // ================================== Movements ===================================
        // ================================================================================
        protected int currentMoveIndex;
        protected int baseMovementSpeed = (int)(0.4 * Globals.SCALAR); 
        protected bool useSegmentedSpeed = false; 
        protected int[] segmentedSpeed = new int[] {
            (int)(0.2 * Globals.SCALAR),
            (int)(0.2 * Globals.SCALAR),
            (int)(0.2 * Globals.SCALAR),
            (int)(0.2 * Globals.SCALAR) };
        protected int collisionDMG = -8;

        // ================================================================================
        // ================================= Collisions ===================================
        // ================================================================================
        protected Rectangle CollisionRect;
        // Offset collision rect by the following amount
        protected int horizontalTop = 2 * Globals.SCALAR; // When moving horizontally  
        protected int horizontalBot = 2 * Globals.SCALAR;
        protected int sideShrink = 2 * Globals.SCALAR;
        protected Rectangle movingHorizontal;
        protected Rectangle movingVertical;
        protected EnemyBlockCollision enemyBlockCollison;

        // ================================================================================
        // ============================= Drawing of the enemy =============================
        // ================================================================================
        protected GeneralSprite mainSprite; 
        protected GeneralSprite movingSprite;
        protected GeneralSprite deathSprite;
        protected GeneralSprite wakeupSprite;

        protected bool isVisible = false; 

        protected Color defaultTint = Color.White;

        // Drawing layer, used to create depth effect in comparasion to the player character 
        protected float layerOnTop = Globals.ENEMY_LAYER;
        protected float layerAtBack = Globals.ENEMY_LAYER - 0.02f;

        // ================================================================================
        // ========================= Being alive and healthy ==============================
        // ================================================================================ 
        public Globals.GeneralStates selfState; 

        protected bool startWithHibernate = true;  // If the enemy starts in hibernation
        protected bool wakeupByIlluminati = true; // Only waken by illuminati state
        protected bool wakingup = false; 
        protected int wakeUpDistance = (int)(2.0 * Globals.OUT_UNIT); // When player is within this distance 
        protected int wakeupTime = 1600;
        protected Stopwatch wakeupSW = new Stopwatch(); 

        protected Enemies.EnemyHealthBar HPBar;
        protected int totalHealth = 20;
        protected int currentHealth = 20;
        protected Stopwatch damageProtectionSW = new Stopwatch();
        protected int recoverTime = 1000;

        // ================================================================================
        // ==================================== Death =====================================
        // ================================================================================ 
        protected bool startOfEnd = false;
        protected Stopwatch deathSW = new Stopwatch();
        protected int fadeStartTime = 1000; // Opacity starts to decrease after this time 
        protected int lingeringTime = 1500; // Completely disappears after this time 

        /// <summary>
        /// Init
        /// </summary>
        /// <param name="G">Game1 object</param>
        /// <param name="P">Position of where it spawns</param>
        public IEnemySTD(Game1 G, Vector2 P)
        {
            game = G;
            position = P;

            spriteBatch = game.spriteBatch;

            LoadSprites();
            UpdateRect();

            HPBar = new Enemies.EnemyHealthBar(game, selfType);

            brainAgent = new Enemies.AgentStupid(this);
            enemyBlockCollison = new EnemyBlockCollision(game);

            selfState = startWithHibernate ? Globals.GeneralStates.Hold : Globals.GeneralStates.Moving; 
            
            currentMoveIndex = 0; 
            facingDir = (Globals.Direction)(Globals.RND.Next() % 4);
            movingSprite.rowLimitation = (int)facingDir;

            damageProtectionSW.Restart();
        }

        public void Turn(Globals.Direction NewDir)
        {
            facingDir = NewDir;
        }

        public virtual void Move()
        {
            stagedMovement = new Vector2(0, 0);

            // either moving in the same speed, 
            // or depend on current move index. 
            switch (facingDir)
            {
                case Globals.Direction.Left:
                    stagedMovement.X -= useSegmentedSpeed ? segmentedSpeed[currentMoveIndex] : baseMovementSpeed;
                    break;
                case Globals.Direction.Right:
                    stagedMovement.X += useSegmentedSpeed ? segmentedSpeed[currentMoveIndex] : baseMovementSpeed;
                    break;
                case Globals.Direction.Up:
                    stagedMovement.Y -= useSegmentedSpeed ? segmentedSpeed[currentMoveIndex] : baseMovementSpeed;
                    break;
                case Globals.Direction.Down:
                    stagedMovement.Y += useSegmentedSpeed ? segmentedSpeed[currentMoveIndex] : baseMovementSpeed;
                    break;
                default:
                    break;
            }

            movingSprite.rowLimitation = (int)facingDir;
            movingSprite.Update();
        }

        /// <summary>
        /// Try to inflict damge onto this enemy. 
        /// </summary>
        /// <param name="Damage">A damage instance</param>
        public virtual void TakeDamage(DamageInstance Damage)
        {
            if (damageProtectionSW.ElapsedMilliseconds > recoverTime)
            {
                currentHealth += Damage.DamageCount;
                damageProtectionSW.Restart();
            }

        }

        /// <summary>
        /// Updates the movement and behavior of the enemy. 
        /// May use the player's position and other info to make different decision. 
        /// </summary>
        /// <param name="MainChara">Player's character</param>
        public virtual void Update(MC MainChara)
        {
            // Change draw layers if player is lower in screen 
            float DrawLayer;
            if (MainChara.position.Y > position.Y) {
                DrawLayer = layerAtBack;
            }
            else {
                DrawLayer = layerOnTop;
            }
            movingSprite.layer = DrawLayer;

            // Mark death if health drops below 0 
            if (currentHealth <= 0) {
                selfState = Globals.GeneralStates.Dead;
            }

            switch (selfState)
            {
                case Globals.GeneralStates.Hold:
                    UpdateBurrow(MainChara);
                    break;
                case Globals.GeneralStates.Moving:
                    UpdateMoving(MainChara);
                    break;
                case Globals.GeneralStates.Dead:
                    UpdateDeath();
                    break;
                default:
                    break; 
            }


            // Update collision to follow the movement 
            UpdateRect();
            // Update HP if necessary 
            HPBar.Update(totalHealth, currentHealth);
        }

        public virtual void Draw()
        {
            if (isVisible)
                mainSprite.Draw(spriteBatch, position, defaultTint);

            if (currentHealth != totalHealth)
            {
                HPBar.Draw(position);
            }
        }

        public virtual Rectangle GetRectangle()
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

        public virtual bool IsDead()
        {
            return (startOfEnd && deathSW.ElapsedMilliseconds > lingeringTime);
        }

        /// <summary>
        /// Return a damage instance of on collision. 
        /// </summary>
        /// <returns>DamageInstance object</returns>
        public virtual DamageInstance DealCollisionDamage()
        {
            if (currentHealth == 0 || selfState == Globals.GeneralStates.Hold)
                return null;

            DamageInstance DMG = new DamageInstance(collisionDMG, new Globals.DamageEffect[] { Globals.DamageEffect.Knockback });
            DMG.knowckbackDist = collisionDMG;

            return DMG;
        }


        // ================================================================================
        // ================================ Private methods ===============================
        // ================================================================================

        /// <summary>
        /// Used to see if a movement will result in collision or not 
        /// </summary>
        /// <returns>Rectangle of where the enemy is going to be</returns>
        protected virtual Rectangle GetStagedRectangle()
        {
            return new Rectangle(
                CollisionRect.X + (int)stagedMovement.X,
                CollisionRect.Y + (int)stagedMovement.Y,
                CollisionRect.Width,
                CollisionRect.Height
                );
        }

        /// <summary>
        /// Load the sprites, other inherited classes are more than likly to change this method. 
        /// </summary>
        protected virtual void LoadSprites()
        {
            ImageFile STD = TextureFactory.Instance.enemySTD;
            ImageFile STDD = TextureFactory.Instance.enemySTDDie;
            ImageFile STDB = TextureFactory.Instance.enemySTDBurrow; 

            movingSprite = new GeneralSprite(STD.texture, STD.C, STD.R,
                Globals.WHOLE_SHEET, Globals.FRAME_CYCLE, Globals.ENEMY_LAYER);
            movingSprite.frameDelay = 250;

            deathSprite = new GeneralSprite(STDD.texture, STDD.C, STDD.R,
                    Globals.WHOLE_SHEET, Globals.FRAME_CYCLE, Globals.ENEMY_LAYER);
            deathSprite.positionOffset = new Vector2(-2, -2) * Globals.SCALAR;

            wakeupSprite = new GeneralSprite(STDB.texture, STDB.C, STDB.R,
                    Globals.WHOLE_SHEET, STDB.C * STDB.R, Globals.ENEMY_LAYER);
            wakeupSprite.positionOffset = new Vector2(-2, -2) * Globals.SCALAR;
            wakeupSprite.frameDelay = 100;

            mainSprite = movingSprite; // Avoid null init 
        }

        /// <summary>
        /// Update the rectangle depending on current position
        /// and set the collision depending on direciton. 
        /// </summary>
        protected virtual void UpdateRect()
        {
            movingHorizontal = new Rectangle(
                (int)position.X,
                (int)position.Y + horizontalTop,
                Globals.OUT_UNIT,
                Globals.OUT_UNIT - horizontalTop - horizontalBot
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

        /// <summary>
        /// Update when this enemy is burrowed underground.
        /// Mostly about how this enemy is woke up from hobernation. 
        /// </summary>
        /// <param name="MainChara"></param>
        protected virtual void UpdateBurrow(MC MainChara)
        {
            if (!wakingup && Misc.Instance.L2Distance(MainChara.position, position) < wakeUpDistance)
            {
                if(!wakeupByIlluminati || (wakeupByIlluminati && MainChara.Illuminati()))
                {
                    wakingup = true;
                    isVisible = true;
                    wakeupSW.Restart();
                    mainSprite = wakeupSprite;
                    wakeupSprite.currentFrame = 0; 
                }
            }

            wakeupSprite.Update();

            if (wakeupSW.ElapsedMilliseconds > wakeupTime)
            {
                selfState = Globals.GeneralStates.Moving;
                mainSprite = movingSprite; 
            }
        }


        /// <summary>
        /// The ordinary update method when the enemy is out and moving
        /// </summary>
        /// <param name="MainChara">Info about the player character</param>
        protected virtual void UpdateMoving(MC MainChara)
        {
            // Change movement depending on player's stats 
            brainAgent.Update(MainChara);


            // Try to move 
            Move();

            // If it's valid, then move 
            if (enemyBlockCollison.ValidMove(GetStagedRectangle()))
            {
                movingSprite.Update();
                currentMoveIndex = movingSprite.currentFrame % Globals.FRAME_CYCLE;

                position += stagedMovement;
                stagedMovement = new Vector2(0, 0);
            }
            else // Turn if hit a block
            {
                Turn(brainAgent.HandleBlockCollision(facingDir));
            }
        }

        /// <summary>
        /// This method is uesd after the health of the enemy drops under 0. 
        /// Can either be death animation, or something like haunting.  
        /// </summary>
        protected virtual void UpdateDeath()
        {
            if (!startOfEnd) // Mark the start of the death process 
            {
                startOfEnd = true;
                deathSW.Restart();

                // Swap the sprite, by default the death sprite is 2 pixel wider than normal 
                movingSprite = deathSprite; 

                movingSprite.rowLimitation = (int)facingDir;
                movingSprite.currentFrame = 0;
            }
            else
            {
                if (movingSprite.currentFrame != 3)
                    movingSprite.Update();
                else if (movingSprite.opacity > 0f && deathSW.ElapsedMilliseconds > fadeStartTime)
                {
                    movingSprite.opacity -= 0.05f;
                }

                // Gradually fade out 
                if (deathSW.ElapsedMilliseconds > lingeringTime)
                {
                    position = new Vector2(-Globals.OUT_UNIT, -Globals.OUT_UNIT);
                }
            }
        }

    }
}
