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
        protected Vector2 spawnPosition;    // Initial position of spawning
        protected Vector2 position;         // Current position 
        protected Vector2 stagedMovement;   // Intended position, used to check collision

        protected Globals.EnemyTypes selfType = Globals.EnemyTypes.Minion;

        protected int selfIndex = Globals.ENEMY_STD;

        // ================================================================================
        // ============================= Movements and attack =============================
        // ================================================================================
        protected int currentMoveIndex;
        protected float baseMovementSpeed; // Changed in agent 
        protected bool useSegmentedSpeed = false; 
        protected float[] segmentedSpeed = new float[] {
            0.2f * Globals.SCALAR,
            0.2f * Globals.SCALAR,
            0.2f * Globals.SCALAR,
            0.2f * Globals.SCALAR };
        protected int collisionDMG = -8;        // Well collision damage is sort of an attack 

        protected bool canAttack = true;        // Overall bool to enable or disable attack  
        protected bool canRangedAttack = true;  // By default perfrom ranged, if cannot, perform melee
        protected bool holdOnAttack = false;     // Wether attack will make it stop moving 
        protected bool meleeShowProjectile = true; // If melee projectile draw the sprite 
        protected int rangedAttackDistance = 4 * Globals.OUT_UNIT;
        protected int meleeAttackRange = 1 * Globals.OUT_UNIT;
        protected int attackDamage = -10;
        protected int attackLastingTime = 400;  // If hold on attack, then that's the time it stops 
        protected int attackIntervalMin = 2000; // 2 seconds' interval at least 
        protected int attackIntervalMax = 8000; // Some perform attack randomly, that's the longest interval time
        protected int attackInterval; 
        protected System.Diagnostics.Stopwatch attackSW = new System.Diagnostics.Stopwatch();

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
        // =================== Drawing and the sound of the enemy =========================
        // ================================================================================
        protected GeneralSprite mainSprite; 
        protected GeneralSprite movingSprite;
        protected GeneralSprite deathSprite;
        protected GeneralSprite wakeupSprite;
        protected GeneralSprite attackSprite;
        protected GeneralSprite projectileSprite; 

        protected bool isVisible; // Burrow visibility 

        protected Color defaultTint = Color.White;

        // Drawing layer, used to create depth effect in comparasion to the player character 
        protected float layerOnTop = Globals.ENEMY_LAYER;
        protected float layerAtBack = Globals.ENEMY_LAYER - 0.02f;

        protected int moveSoundIntervalBaseline = 1000;
        protected int moveSoundIntervalFlau = 200;
        protected int moveSoundInterval = 1000;
        protected Stopwatch soundSW;
        private float maxVolume = .2f;
        private int audibleRange = 3 * Globals.OUT_UNIT;

        // ================================================================================
        // ========================= Being alive and healthy ==============================
        // ================================================================================ 
        public Globals.GeneralStates selfState; 

        protected bool startWithHibernate = false;  // If the enemy starts in hibernation
        protected bool wakeupByIlluminati = false; // Only waken by illuminati state
        protected bool wakingup = false; 
        protected int wakeUpDistance = (int)(2.0 * Globals.OUT_UNIT); // When player is within this distance 
        protected int wakeupTime = 1600;
        protected Stopwatch wakeupSW; 

        protected Enemies.EnemyHealthBar HPBar;
        protected int totalHealth = 20;
        protected int currentHealth = 20;
        protected int regenAmount = 1;
        protected int regenInterval = 200;  // Pretty much the min number that can be killed  
        protected Stopwatch regenSW;
        protected Stopwatch damageProtectionSW;  // Avoid being 1 shot
        protected int recoverTime = 1000;

        // ================================================================================
        // ==================================== Death =====================================
        // ================================================================================ 
        protected bool startOfEnd = false;
        protected Stopwatch deathSW;
        protected int fadeStartTime = 1000; // Opacity starts to decrease after this time 
        protected int lingeringTime = 1500; // Completely disappears after this time 

        // ======================== Gold and item drop after death ========================
        protected bool enableDeathDrop = true;  // General drop flag 
        protected bool enableDropGold = true;   // Gold drop flag 

        protected int goldDropBaseline = 10;
        protected int goldDropFluctuate = 5;
        protected float goldDropChance = .1f;  // Possibility of dropping gold 

        // Item drop does not have a flag, if the list is empty, then no item will be dropped
        // If the list is not empty, then go by chance. 
        /*
         * If gold drop is also enabled, then item drop is calculated after gold. 
         * For example, if gold drop chance is 0.8, then the game will first calculate if the 80%
         * chance of dropping gold is triggered. If not, calculate the chance of dropping item. 
         */
        protected float itemDropChance = .8f; 
        // List of items that can be dropped upon death, represented in item index 
        protected List<int> droppableItem = new List<int>();


        /// <summary>
        /// Init
        /// </summary>
        /// <param name="G">Game1 object</param>
        /// <param name="P">Position of where it spawns</param>
        public IEnemySTD(Game1 G, Vector2 P)
        {
            game = G;
            position = P;
            spawnPosition = P;

            baseMovementSpeed = 0.4f * Globals.SCALAR;

            soundSW = new Stopwatch(game);
            wakeupSW = new Stopwatch(game);
            regenSW = new Stopwatch(game);
            damageProtectionSW = new Stopwatch(game);
            deathSW = new Stopwatch(game);

            spriteBatch = game.spriteBatch;

            LoadSprites();
            UpdateRect();

            HPBar = new Enemies.EnemyHealthBar(game, selfType);

            selfState = startWithHibernate ? Globals.GeneralStates.Hold : Globals.GeneralStates.Moving;

            attackInterval = Globals.RND.Next(attackIntervalMin, attackIntervalMax);
            currentMoveIndex = 0; 
            facingDir = (Globals.Direction)(Globals.RND.Next() % 4);
            movingSprite.rowLimitation = (int)facingDir;

            brainAgent = new Enemies.AgentSTD(this, game, facingDir);
            enemyBlockCollison = new EnemyBlockCollision(game);

            damageProtectionSW.Restart();
            regenSW.Restart();
            attackSW.Restart();

            if (!startWithHibernate)
                SetNonHibernate();
        }

        public void Turn(Globals.Direction NewDir)
        {
            facingDir = NewDir;
        }

        public virtual void Move(MC MainChara)
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

            PlaySounds(MainChara, 0);

            movingSprite.rowLimitation = (int)facingDir;
            movingSprite.Update();
        }

        public virtual void SpeedChange(int NewSpeed)
        {
            baseMovementSpeed = NewSpeed;
        }

        /// <summary>
        /// Try to inflict damge onto this enemy. 
        /// Other effects, such as DoT, may also be applied.
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
            if (game.gameState != Globals.GameStates.Running && !Globals.REAL_TIME_ACTION)
                return;

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
                case Globals.GeneralStates.Attack:
                    UpdateAttack(MainChara);
                    if (!holdOnAttack)
                        UpdateMoving(MainChara);
                    break; 
                case Globals.GeneralStates.Dead:
                    UpdateDeath(MainChara);
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

        /// <summary>
        /// Different from the `canAttack` field. This method provides info for outside
        /// inquiry, while the filed `canAttack` is the inside view indicating whether or
        /// not this enemy has the ability to attack at all. 
        /// </summary>
        /// <returns>True of this enemy is capable of attack now</returns>
        public virtual bool CanAttack()
        {
            return attackSW.ElapsedMilliseconds > attackInterval && canAttack;
        }

        public virtual void Attack()
        {
            if (!canAttack) return;  // Fail-safe 

            DamageInstance DmgIns = new DamageInstance(attackDamage, null);
            GeneralSprite ProjSpeite = null; 

            if(canRangedAttack || meleeShowProjectile)
            {
                projectileSprite.rowLimitation = (int)facingDir;
                ProjSpeite = projectileSprite; 
            }
            
            IProjectileStandard proj = new IProjectileStandard(game, ProjSpeite, DmgIns, 
                AttackOffset(), facingDir, this);
            proj.totalravelDistance = canRangedAttack ? rangedAttackDistance : meleeAttackRange;
            if (!canRangedAttack)
            {
                proj.MarkAsMelee(this);
                proj.meleeLastingTime = attackLastingTime;
            }

            game.projList.Add(proj);

            selfState = Globals.GeneralStates.Attack; 

            attackSprite.currentFrame = 0;
            attackSprite.rowLimitation = (int)facingDir;
            mainSprite = attackSprite;
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

        public virtual Vector2 GetPosition()
        {
            return position;
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

        /// <summary>
        /// Reset the interval for next attack attempt. 
        /// </summary>
        /// <param name="Pattern">0 for random range, positive number for fixed min time</param>
        public virtual void SetAttackInterval(int Pattern)
        {
            if (Pattern == 0)
            {
                attackInterval = Globals.RND.Next(attackIntervalMin, attackIntervalMax);
            }
            else if (Pattern > 0)
            {
                attackInterval = attackIntervalMin; 
            }
            attackSW.Restart();
        }

        public virtual int GetIndex()
        {
            return selfIndex; 
        }

        public virtual int GetKillScore()
        {
            return General.ScoreTable.Instance.getScore(
                selfIndex, game.difficulty
                ); 
        }

        // ================================================================================
        // ================================ Private methods ===============================
        // ================================================================================

        protected virtual void SetNonHibernate()
        {
            isVisible = true;
            mainSprite = movingSprite;
        }

        /// <summary>
        /// Used to see if a movement will result in collision or not 
        /// </summary>
        /// <returns>Rectangle of where the enemy is going to be</returns>
        protected virtual Rectangle GetStagedRectangle()
        {
            return new Rectangle(
                (int)(CollisionRect.X + stagedMovement.X),
                (int)(CollisionRect.Y + stagedMovement.Y),
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
            ImageFile STDA = TextureFactory.Instance.enemySTDAttack;
            ImageFile AP = TextureFactory.Instance.enemySTDProjectile;

            // Normal moving 
            movingSprite = new GeneralSprite(STD.texture, STD.C, STD.R,
                Globals.WHOLE_SHEET, Globals.FRAME_CYCLE, Globals.ENEMY_LAYER);
            movingSprite.frameDelay = 250;

            // Death or effect 
            deathSprite = new GeneralSprite(STDD.texture, STDD.C, STDD.R,
                    Globals.WHOLE_SHEET, Globals.FRAME_CYCLE, Globals.ENEMY_LAYER);
            deathSprite.positionOffset = new Vector2(-2, -2) * Globals.SCALAR;

            // Wake up from hibernation animation 
            wakeupSprite = new GeneralSprite(STDB.texture, STDB.C, STDB.R,
                    Globals.WHOLE_SHEET, STDB.C * STDB.R, Globals.ENEMY_LAYER);
            wakeupSprite.positionOffset = new Vector2(-2, -2) * Globals.SCALAR;
            wakeupSprite.frameDelay = 100;

            // Attack animation 
            attackSprite = new GeneralSprite(STDA.texture, STDA.C, STDA.R,
                    Globals.WHOLE_SHEET, Globals.FRAME_CYCLE, Globals.ENEMY_LAYER);
            attackSprite.positionOffset = new Vector2(-2, -2) * Globals.SCALAR;
            attackSprite.frameDelay = (attackLastingTime / Globals.FRAME_CYCLE);

            projectileSprite = new GeneralSprite(AP.texture, AP.C, AP.R,
                    Globals.WHOLE_SHEET, Globals.FRAME_CYCLE, Globals.ENEMY_LAYER);

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
        /// Update health and regenerate HP.
        /// </summary>
        protected virtual void UpdateRegen()
        {
            if (regenSW.ElapsedMilliseconds > regenInterval)
            {
                currentHealth += regenAmount;
                if (currentHealth > totalHealth)
                    currentHealth = totalHealth;

                regenSW.Restart();
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
            UpdateRegen();

            if (wakeupSW.ElapsedMilliseconds > wakeupTime)
            {
                selfState = Globals.GeneralStates.Moving;
                mainSprite = movingSprite; 
            }
        }

        /// <summary>
        /// The ordinary update method when the enemy is out and moving.
        /// It could changed into attack from here. 
        /// </summary>
        /// <param name="MainChara">Info about the player character</param>
        protected virtual void UpdateMoving(MC MainChara)
        {
            // Change movement depending on player's stats 
            brainAgent.Update(MainChara);

            // Try to move 
            Move(MainChara);

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
                brainAgent.HandleBlockCollision(facingDir);
            }

            UpdateRegen();
        }

        /// <summary>
        /// Update method during attack and switch back to moving state when appropriate. 
        /// </summary>
        protected virtual void UpdateAttack(MC MainChara)
        {
            attackSprite.Update();

            if (attackSW.ElapsedMilliseconds > attackLastingTime)
            {
                selfState = Globals.GeneralStates.Moving;
                mainSprite = movingSprite;
            }
        }

        /// <summary>
        /// This method is uesd after the health of the enemy drops under 0. 
        /// Can either be death animation, or something like haunting.  
        /// </summary>
        protected virtual void UpdateDeath(MC MainChara)
        {
            if (!startOfEnd) // Mark the start of the death process 
            {
                startOfEnd = true;
                deathSW.Restart();

                // Check for item drop, if dropped, then add it immediately 
                CheckDeathDrop();

                // Swap the sprite, by default the death sprite is 2 pixel wider than normal 
                mainSprite = deathSprite;

                mainSprite.rowLimitation = (int)facingDir;
                mainSprite.currentFrame = 0;

                PlaySounds(MainChara, 1);
            }
            else
            {
                if (mainSprite.currentFrame != 3)
                    mainSprite.Update();
                else if (mainSprite.opacity > 0f && deathSW.ElapsedMilliseconds > fadeStartTime)
                {
                    mainSprite.opacity -= 0.05f;
                }

                // Gradually fade out and remove itself from level data 
                if (deathSW.ElapsedMilliseconds > lingeringTime)
                {
                    position = new Vector2(-Globals.OUT_UNIT, -Globals.OUT_UNIT);
                    game.roomCycler.RemoveIndex(selfIndex, Misc.Instance.PositionReverse(spawnPosition));
                }
            }
        }

        /// <summary>
        /// Perform a check upon death on whether or not to drop something
        /// </summary>
        protected virtual void CheckDeathDrop()
        {
            if (!enableDeathDrop) return; // Quit if drop is not enabled 

            if (enableDropGold && Globals.RND.NextDouble() < goldDropChance)
            {
                // When drop gold is enabled and RND gave the jackpot 

                int goldCount = goldDropBaseline + Globals.RND.Next(-goldDropFluctuate, goldDropFluctuate);
                DroppedGold goldItem = new DroppedGold(game, position);
                goldItem.SetGoldAmount(goldCount);

                game.collectibleItemList.Add(goldItem);
            }
            else
            {
                if (droppableItem.Count == 0) return; // Quit if nothing to drop 

                if (Globals.RND.NextDouble() < itemDropChance)
                {
                    // When an item should be dropped 
                    int itemIndex = droppableItem[Globals.RND.Next() % droppableItem.Count];
                    IItem itemToBeAdded = game.mapGenerator.CreateItem(game, position, itemIndex);

                    game.collectibleItemList.Add(itemToBeAdded);
                    
                }

            }
        }

        /// <summary>
        /// Instead of creating a projectile (melee or not) at the it's own position,
        /// creating it in front of the current position.
        /// </summary>
        /// <returns>Position of projectile with offset</returns>
        protected virtual Vector2 AttackOffset()
        {
            Vector2 AO = new Vector2(position.X, position.Y);

            switch (facingDir)
            {
                case Globals.Direction.Left:
                    AO.X -= Globals.OUT_UNIT;
                    break;
                case Globals.Direction.Right:
                    AO.X += Globals.OUT_UNIT;
                    break;
                case Globals.Direction.Up:
                    AO.Y -= Globals.OUT_UNIT;
                    break;
                case Globals.Direction.Down:
                    AO.Y += Globals.OUT_UNIT;
                    break;
                default:
                    break;
            }

            return AO;
        }

        /// <summary>
        /// Since many sound effects requires position parameters to make them stereo
        /// play sound is a separated method here to calculate its positon. 
        /// </summary>
        /// <param name="MainChara">Info about player</param>
        /// <param name="Mode">0 for moving sound, 1 for death</param>
        protected virtual void PlaySounds(MC MainChara, int Mode)
        {
            int Dist = Misc.Instance.L2Distance(position, MainChara.position);
            if (Dist > audibleRange) return;

            if (soundSW.ElapsedMilliseconds > moveSoundInterval || Mode != 0)
            {
                float Vol = (float)(maxVolume * ((double)Dist / audibleRange));
                float Pan = (float)((position.X - MainChara.position.X) / (double)(audibleRange));

                switch (Mode)
                {
                    case 0:
                        SoundFX.Instance.PlayEnemyMoveSFX(selfIndex, Vol, Pan);
                        break;
                    case 1:
                        Vol = (Vol + .2f < 1f) ? Vol + .2f: Vol;
                        SoundFX.Instance.PlayEnemyDieSFX(selfIndex, Vol, Pan);
                        break;
                    default:
                        break; 
                }
                
                soundSW.Restart();

                moveSoundInterval = Globals.RND.Next(moveSoundIntervalBaseline - moveSoundIntervalFlau,
                    moveSoundIntervalBaseline + moveSoundIntervalFlau); 
            }
        }
    }
}
