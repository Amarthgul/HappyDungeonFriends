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
    /// Main character. 
    /// Most of the code here are merely behaviral, the sprites are dealt in CharacterSprite class. 
    /// </summary>
    public class MC
    {

        // ================================================================================
        // ======================== Consts and landmark positions =========================
        // ================================================================================
        private const int MAX_HEALTH = 100;
        private const double INIT_HP_RATIO = 0.75;
        private const float RECT_WIDTH_OFFSET = 0.1f;
        private const float RECT_HEIGHT_OFFSET = 0.3f;
        private const int NEW_ROOM_OFFSET = 2 * Globals.SCALAR;
        private const int DIRECTION_CD = 200;    // Avoid gltching direction when using mouse 
        private const int INFLICT_MAX_COUNTER = 8; 

        // The limits for entering doors 
        private int leftLim = 2 * Globals.OUT_UNIT;
        private int rightLim = Globals.OUT_FWIDTH - 2 * Globals.OUT_UNIT - (int)((1 - RECT_WIDTH_OFFSET * 2) * Globals.OUT_UNIT);
        private int topLim = Globals.OUT_HEADSUP + 2 * Globals.OUT_UNIT;
        private int botLim = Globals.OUT_FHEIGHT - 2 * Globals.OUT_UNIT - (int)((1 - RECT_HEIGHT_OFFSET) * Globals.OUT_UNIT);

        // The limits for entering next room and trigger room transition 
        private int leftTrigger = (int)((RECT_WIDTH_OFFSET * 2) * Globals.OUT_UNIT);
        private int rightTrigger = Globals.OUT_FWIDTH - Globals.OUT_UNIT;
        private int topTrigger = (int)(RECT_HEIGHT_OFFSET * Globals.OUT_UNIT) + Globals.OUT_HEADSUP;
        private int botTrigger = Globals.OUT_FHEIGHT - Globals.OUT_UNIT; 

        // ================================================================================
        // ====================== Position, direction, and movement =======================
        // ================================================================================
        public Vector2 position { set; get; }
        public Globals.Direction facingDir;

        public bool moveRestricted;                   // This field is for limiting movement to only 1 direction 
        public bool[] canTurn = new bool[] { true, true, true, true };
        private Stopwatch[] dirStopwatches;
        private long[] dirTimers = new long[] { 0, 0, 0, 0 }; 

        private Rectangle collisionRect;
        private Vector2 stagedMovement; 
        private Vector2 collisionOffset; 
        private Vector2 startUpPosition = new Vector2(Globals.OUT_FWIDTH / 2 - Globals.OUT_UNIT / 2, 
            Globals.OUT_FHEIGHT / 2 + Globals.OUT_UNIT);

        // New room protection is used to give time for the MC stats to warm-up 
        private bool newRoomProtectionOn;  
        private Stopwatch newRoomProtectSW;
        private int newRoomProtectTime = 100; 

        // ================================================================================
        // ================================= Interactions =================================
        // ================================================================================

        private PlayerBlockCollision blockCollisionCheck;
        private PlayerItemCollision itemCollisionCheck;
        private PlayerEnemyCollision enemyCollisionCheck;

        private bool recoverImmunity = false; 
        private Stopwatch damageProtectionSW;
        private long damageRecoverTimer;
        private int recoverTime = 1500;

        private bool canAttack = true;
        private bool canRangedAttack = false;
        private Stopwatch attackInternalSW;
        private long attackInternalCDTimer;
        private int attackInternalCD = 250;

        private Stopwatch attackExternalSW;
        private int attackExternalCD = 500;

        // ================================================================================
        // ============================= Drawing and textures =============================
        // ================================================================================
        private CharacterSprite selfSprite; // Shit's a own class

        // ================================================================================
        // ============================= States and parameters ============================
        // ================================================================================
        private bool Untouchable = false; // Controls damge taking, as in ContinuedHealthReduction
        
        public Globals.primaryTypes primaryState;
        public Globals.GeneralStates mcState { set; get; }
        public int currentHealth;
        public int pastHealth;          // For the headsup display to signify the damage 
        public int currentMaxHP;

        public int moveSpeedBaseline = 1 * Globals.SCALAR;
        public int attackDamageBaseline = 5;
        public int meleeRangeBaseline = 8; // Half of a unit 

        public int pickupDistance = 2;  // Not having to completely reach an item to pick it up. In original unit

        // For decresing the HP with animation instead of suddenly drops 
        private int damageInflictedTotal = 0;
        private int[] DamageInflictSequence; 
        private int damageInflictCounter = 0; // In contrast of INFLICT_MAX_COUNTER
        private bool damageInflictionOn = false; 

        private Game1 game;
        private SpriteBatch spriteBatch;

        // Init 
        public MC(Game1 G)
        {
            game = G;
            spriteBatch = game.spriteBatch;

            dirStopwatches = new Stopwatch[] { new Stopwatch(game), new Stopwatch(game), new Stopwatch(game), new Stopwatch(game) };
            newRoomProtectSW = new Stopwatch(game);

            position = startUpPosition;
            facingDir = Globals.Direction.Up;

            stagedMovement = new Vector2(0, 0);
            collisionOffset = new Vector2(RECT_WIDTH_OFFSET * Globals.OUT_UNIT, RECT_HEIGHT_OFFSET * Globals.OUT_UNIT);
            collisionRect = new Rectangle((int)collisionOffset.X, (int)collisionOffset.Y, 
                (int)((1 - RECT_WIDTH_OFFSET * 2) * Globals.OUT_UNIT), (int)((1 - RECT_HEIGHT_OFFSET) * Globals.OUT_UNIT));


            selfSprite = new CharacterSprite(game);
            
            // Collisions 
            blockCollisionCheck = new PlayerBlockCollision(game);
            itemCollisionCheck = new PlayerItemCollision(game);
            enemyCollisionCheck = new PlayerEnemyCollision(game);

            primaryState = Globals.primaryTypes.None;
            mcState = Globals.GeneralStates.Hold; // Start as on hold 
            moveRestricted = false;

            damageProtectionSW = new Stopwatch(game);
            damageProtectionSW.Restart();
            damageRecoverTimer = 0;

            attackExternalSW = new Stopwatch(game);
            attackInternalSW = new Stopwatch(game);
            attackExternalSW.Restart();

            newRoomProtectionOn = true;
            newRoomProtectSW.Restart();

            currentHealth = (int)(MAX_HEALTH * INIT_HP_RATIO);
            pastHealth = -1; 
            currentMaxHP = MAX_HEALTH;

            // Initlize the sprite so that the character will be there when first starts 
            selfSprite.Update(facingDir, mcState, primaryState, damageInflictionOn);
        }

        // ================================================================================
        // ================================= Public methods ===============================
        // ================================================================================
        
        /// <summary>
        /// Change the direction of the player. 
        /// Add a direction lock on that direction to avoid glitch turn when
        /// that direction is called in another short period of time. 
        /// </summary>
        /// <param name="newDir">Direction to turn</param>
        public void ChangeDirection(Globals.Direction newDir)
        {
            facingDir = newDir;

            canTurn[(int)newDir] = false;
            dirTimers[(int)newDir] = 0;
            dirStopwatches[(int)newDir].Restart();
        }

        /// <summary>
        /// Invoke the character to try to move.
        /// This is when the character tried to move, not being forced to displace. 
        /// </summary>
        public void Move()
        {
            if (newRoomProtectSW.ElapsedMilliseconds < newRoomProtectTime)
                return;

            moveRestricted = true; // Set to tru so only 1 direction is moving at a time 
            canAttack = false; 
            mcState = Globals.GeneralStates.Moving;
            stagedMovement = new Vector2(0, 0);

            int StagedMove = moveSpeedBaseline;

            switch (facingDir)
            {
                case Globals.Direction.Left:
                    stagedMovement.X -= StagedMove;
                    break;
                case Globals.Direction.Right:
                    stagedMovement.X += StagedMove;
                    break;
                case Globals.Direction.Up:
                    stagedMovement.Y -= StagedMove;
                    break;
                case Globals.Direction.Down:
                    stagedMovement.Y += StagedMove;
                    break;
                default:
                    break; 
            }
            
        }

        /// <summary>
        /// Determine the location of the cursor and makes the character walk towrds that place.
        /// </summary>
        /// <param name="CursorLoc">Position of the cursor in game screen</param>
        public void RightClickMove(Vector2 CursorLoc)
        {
            Vector2 Delta = CursorLoc - position; 

            if(Math.Abs(Delta.X) > Math.Abs(Delta.Y)) // Left or right move 
            {
                if (Delta.X > 0)
                    new MoveRightCommand(game).execute();
                else
                    new MoveLeftCommand(game).execute();
            }
            else
            {
                if (Delta.Y > 0)
                    new MoveDownCommand(game).execute();
                else
                    new MoveUpCommand(game).execute();
            }

        }

        /// <summary>
        /// Try to issue an attack. 
        /// If holding an ranged primary item, then this issues an projectile.
        /// If holding an melee primary item, the collision is being performed in other methods.
        /// </summary>
        public void Attack()
        {
            if(mcState != Globals.GeneralStates.Attack && canAttack)
            {
                mcState = Globals.GeneralStates.Attack;

                selfSprite.RefreshAttack();

                attackInternalSW.Restart();
                attackInternalCDTimer = 0;
                moveRestricted = true; 
                canAttack = false;

                SoundFX.Instance.PlayMCAttack(this);
            }
        }

        /// <summary>
        /// When being called, torch should be the primary item, thus when toggle off, 
        /// it goes back to normal walking. 
        /// </summary>
        public void ToggleTorch()
        {
            if(primaryState != Globals.primaryTypes.Torch)
            {
                primaryState = Globals.primaryTypes.Torch;
            }
            else
            {
                primaryState = Globals.primaryTypes.None;
            }
        }

        /// <summary>
        /// While holding torch (or in some other conditions),
        /// the character emits light and might drive enemies away.
        /// </summary>
        /// <returns>If the character is in Illuminati mode</returns>
        public bool Illuminati()
        {
            // The primaryState is only Torch when torch is eqiped and turned on. 
            // If torch is eqiped but not on, this returns false. 
            return primaryState == Globals.primaryTypes.Torch;
        }

        /// <summary>
        /// Get the current rectangle of the main character. 
        /// </summary>
        /// <returns>Current collision rectangle</returns>
        public Rectangle GetRectangle()
        {
            return new Rectangle(collisionRect.X, collisionRect.Y, 
                collisionRect.Width, collisionRect.Height); ; 
        }

        /// <summary>
        /// Let player deal with incoming damages. 
        /// For collisions, the DirFrom is the direction from which collision occurs. 
        /// None collision may not have knockback effects or distance. 
        /// </summary>
        /// <param name="DMG">Damage instance object</param>
        /// <param name="DirFrom">Which direction id the collision from</param>
        public void TakeDamage(DamageInstance DMG, Globals.Direction DirFrom)
        {
            // In some cases the collision damge might be a null
            if (recoverImmunity || DMG == null)
                return;

            DamageInstance ReceivedInstance = game.spellSlots.IncomingDamageGernealModifier(DMG);

            if (ReceivedInstance.DamageCount != 0)
                MarkDamgeInfliction(ReceivedInstance.DamageCount);

            if (DMG.effects != null)
            {
                foreach (Globals.DamageEffect FX in DMG.effects)
                {
                    switch (FX)
                    {
                        case Globals.DamageEffect.Break:
                            break;
                        case Globals.DamageEffect.Knockback: // instant knockback
                            position += MaxKnockbackDist(DirFrom, ReceivedInstance.knowckbackDist);
                            break;
                        case Globals.DamageEffect.Stun:
                            break;
                        default:
                            break;
                    }
                }
            }

            recoverImmunity = true;
            damageProtectionSW.Restart();
            damageRecoverTimer = 0;

        }

        /// <summary>
        /// On entering a new room, refresh the player to basic status
        /// </summary>
        /// <param name="G">Game 1 refresher</param>
        public void Refresh(Game1 G)
        {
            game = G;
            blockCollisionCheck = new PlayerBlockCollision(game);
            enemyCollisionCheck = new PlayerEnemyCollision(game);
            itemCollisionCheck = new PlayerItemCollision(game);

            newRoomProtectSW.Restart();

            // Possible check for state
        }

        public void Update()
        {

            switch (mcState)
            {
                case Globals.GeneralStates.Attack:
                    UpdateAttack();
                    CommitAttack();
                    break;
                case Globals.GeneralStates.Broken: // Broken can not move and can do no shit
                    return; 
                default:
                    break; 
            }


            UpdateCollisionsAndMovements();

            currentHealth += ContinuedHealthReduction(); 

            // Update sprites 
            selfSprite.Update(facingDir, mcState, primaryState, damageInflictionOn);

            Regulate();
            UpdateCD();
            UpdateDamgeRecoverProtection();

            CheckBoundary();
            CheckHealth();

            if (mcState == Globals.GeneralStates.Moving)
            {
                mcState = Globals.GeneralStates.Hold;
                moveRestricted = false; // Set back to false for next movement  
            }
        }

        public void Draw()
        {
            selfSprite.Draw(spriteBatch, position);
        }

        // ================================================================================
        // ================================ Private methods ===============================
        // ================================================================================

        /// <summary>
        /// Check all collision realted stuff, including movement 
        /// </summary>
        private void UpdateCollisionsAndMovements()
        {
            // Handle collision with blocks 
            if (blockCollisionCheck.ValidMove(GetStagedRectangle()) && CanPassDoors())
            {
                position += stagedMovement;
            }

            collisionRect.X = (int)(position.X + collisionOffset.X);
            collisionRect.Y = (int)(position.Y + collisionOffset.Y);

            // Clear off the staged movement
            stagedMovement = new Vector2(0, 0);

            // Check and handle item pick up
            itemCollisionCheck.CheckItemCollision(GetPickupRectangle());

            // Check and send singal if collides with enemy 
            enemyCollisionCheck.CheckEnemyCollision(GetRectangle());
        }

        /// <summary>
        /// Release the directions frozen if enough time have passed.
        /// </summary>
        private void UpdateCD()
        {
            for (int i = 0; i < dirTimers.Length; i++)
            {
                dirTimers[i] = dirStopwatches[i].ElapsedMilliseconds; 

                if (dirTimers[i] > DIRECTION_CD)
                {
                    canTurn[i] = true;
                }
            }

            if (attackExternalSW.ElapsedMilliseconds > attackExternalCD)
            {
                canAttack = true;
                attackExternalSW.Restart();
            }
        }

        /// <summary>
        /// Update the damage protection, after a certain time the player will not be protected
        /// by the inflicted damage instance and becomes able to take damage again. 
        /// </summary>
        private void UpdateDamgeRecoverProtection()
        {
            damageRecoverTimer = damageProtectionSW.ElapsedMilliseconds; 
            if (damageRecoverTimer > recoverTime)
            {
                recoverImmunity = false;
            }
        }

        /// <summary>
        /// When an attck is issued, the character goes into attack state. 
        /// This methods checks if the attack hits anything and if it is over. 
        /// </summary>
        private void UpdateAttack()
        {
            attackInternalCDTimer = attackInternalSW.ElapsedMilliseconds;
            if(attackInternalCDTimer > attackInternalCD)
            {
                moveRestricted = false;
                mcState = Globals.GeneralStates.Hold;
                if (game.spellSlots.GetAttackType() == Globals.AttackType.Ranged)
                    canRangedAttack = true;
            }

            if (game.spellSlots.GetAttackType() == Globals.AttackType.Melee)
            {
                new PlayerMeleeAttackCollision(game).CheckMeleeAttack(facingDir, meleeRangeBaseline,
                    attackDamageBaseline, position);
            }

        }

        /// <summary>
        /// Start an attack, if the current type is ranged attack. 
        /// </summary>
        private void CommitAttack()
        {
            if (game.spellSlots.GetAttackType() == Globals.AttackType.Ranged)
            {
                // Add projectile here 
                canRangedAttack = false;
            }
        }

        /// <summary>
        /// Regulate parameters to avoid impossible values.
        /// Also updates the stats. 
        /// </summary>
        private void Regulate()
        {
            if (currentHealth < 0)
                currentHealth = 0;

            if (currentHealth > currentMaxHP)
                currentHealth = currentMaxHP;
        }

        /// <summary>
        /// Count in the movement that the MC is about to make. 
        /// </summary>
        /// <returns>Rectangle of the MC in the future</returns>
        private Rectangle GetStagedRectangle()
        {
            return new Rectangle((int)(collisionRect.X + stagedMovement.X),
                (int)(collisionRect.Y + stagedMovement.Y),
                collisionRect.Width, collisionRect.Height);
        }

        /// <summary>
        /// Rectangle for calculating item pick up.
        /// </summary>
        /// <returns></returns>
        private Rectangle GetPickupRectangle()
        {
            return new Rectangle(
                collisionRect.X - pickupDistance * Globals.SCALAR,
                collisionRect.Y - pickupDistance * Globals.SCALAR,
                collisionRect.Width + pickupDistance * Globals.SCALAR,
                collisionRect.Height + pickupDistance * Globals.SCALAR
                );
        }

        /// <summary>
        /// If the MC's position is reaching doors, check if these doors can be passed.
        /// </summary>
        /// <returns>True if the door is open or not colliding with any doors.</returns>
        private bool CanPassDoors()
        {


            if (newRoomProtectSW.ElapsedMilliseconds < newRoomProtectTime && newRoomProtectionOn)
            {
                return true;
            } else
            {
                newRoomProtectionOn = false;
            }

            bool CanPass = true;

            RoomInfo currentInfo = game.currentRoom.roomInfo;
            Globals.Direction Dir = Globals.Direction.None; 

            if (GetStagedRectangle().X < leftLim)
            {
                Dir = Globals.Direction.Left; 
                CanPass = currentInfo.OpenDoors[(int)Dir]
                    || currentInfo.Holes[(int)Dir];
                TryOpenMysDoor(Dir);
            }


            if (GetStagedRectangle().X > rightLim)
            {
                Dir = Globals.Direction.Right;
                CanPass = currentInfo.OpenDoors[(int)Dir]
                      || currentInfo.Holes[(int)Dir];
                TryOpenMysDoor(Dir);
            }

            if (GetStagedRectangle().Y < topLim)
            {
                Dir = Globals.Direction.Up;
                CanPass = currentInfo.OpenDoors[(int)Dir]
                      || currentInfo.Holes[(int)Dir];
                TryOpenMysDoor(Dir);
            }

            if (GetStagedRectangle().Y > botLim)
            {
                Dir = Globals.Direction.Down;
                CanPass = currentInfo.OpenDoors[(int)Dir]
                      || currentInfo.Holes[(int)Dir];
                TryOpenMysDoor(Dir);
            }

            return CanPass;
        }

        /// <summary>
        /// Detects the character's facing direction, if facing the door and has money
        /// then open it. 
        /// </summary>
        /// <param name="Dir"></param>
        private void TryOpenMysDoor(Globals.Direction Dir)
        {
            if (game.mainChara.facingDir == Dir 
                && game.currentRoom.roomInfo.MysteryDoors[(int)Dir]
                && game.goldCount > 0)
            {
                game.currentRoom.OpenMysDoor((int)Dir);
                game.goldCount -= game.goldCount / 5;
                SoundFX.Instance.PlayEnvOpenMysDoor(Dir);
            }
        }

        /// <summary>
        /// Check if the MC goes beyond the walls. 
        /// By contract, displace effect would not knock MC into blocks or aether area.  
        /// Thus, if she reaches boundary, then there must be another room she can go to. 
        /// </summary>
        private void CheckBoundary()
        {

            if (GetStagedRectangle().X < leftTrigger)
            {
                position = new Vector2(rightTrigger - NEW_ROOM_OFFSET * 2, position.Y);

                game.reset((int)Globals.Direction.Left);
            }

            if (GetStagedRectangle().X > rightTrigger)
            {
                position = new Vector2(leftTrigger + NEW_ROOM_OFFSET, position.Y);

                game.reset((int)Globals.Direction.Right);
            }

            if (GetStagedRectangle().Y < topTrigger)
            {
                position = new Vector2(position.X, botTrigger - NEW_ROOM_OFFSET * 3);

                game.reset((int)Globals.Direction.Up);
            }

            if (GetStagedRectangle().Y > botTrigger)
            {
                position = new Vector2(position.X, topTrigger + NEW_ROOM_OFFSET);

                game.reset((int)Globals.Direction.Down);
            }

        }

        /// <summary>
        /// Check the health of the player, mark as dead if no other options are left 
        /// </summary>
        private void CheckHealth()
        {
            if (currentHealth <= 0)
            {
                // Might have other options 
                mcState = Globals.GeneralStates.Dead;
                game.screenFX.SigTransitionStart(Globals.GameStates.GameOver);
            }
        }

        /// <summary>
        /// When the character is being knocked back, find the maxinum amount of distance 
        /// the cahracter can move before goes into a wall or block. 
        /// </summary>
        /// <param name="DirFrom">Direction from which being knocked</param>
        /// <param name="Dist">Designated knock back distance</param>
        /// <returns></returns>
        private Vector2 MaxKnockbackDist(Globals.Direction DirFrom, double Dist)
        {
            Vector2 VecIter = new Vector2(0, 0);
            Vector2 Result = new Vector2(0, 0); 
            int IterateStep = 1 * Globals.SCALAR;

            while(Math.Abs(VecIter.X) < (int)Math.Abs(Dist) * Globals.SCALAR
                && Math.Abs(VecIter.Y) < (int)Math.Abs(Dist) * Globals.SCALAR)
            {
                switch (DirFrom)
                {
                    case Globals.Direction.Left:
                        VecIter.X += IterateStep;
                        break;
                    case Globals.Direction.Right:
                        VecIter.X -= IterateStep;
                        break;
                    case Globals.Direction.Up:
                        VecIter.Y += IterateStep;
                        break;
                    case Globals.Direction.Down:
                        VecIter.Y -= IterateStep;
                        break;
                    default:
                        break;
                }
                Rectangle CheckingRect = new Rectangle(GetRectangle().X + (int)VecIter.X,
                    GetRectangle().Y + (int)VecIter.Y, GetRectangle().Width, GetRectangle().Height);
                if (!blockCollisionCheck.ValidMove(CheckingRect))
                {
                    break;
                }
                Result = new Vector2(VecIter.X, VecIter.Y) ;
            }

            return Result;
        }

        /// <summary>
        /// When a damage is being delt to the character, calculate the sequence of damage 
        /// to be delt in the following 8 updates, displays as animated health reduction. 
        /// The input is for INSTANT DAMGE only, DoT are counted separately. 
        /// </summary>
        /// <param name="TotalDamage">If it's dealing damage, pass a negative number</param>
        private void MarkDamgeInfliction(int TotalDamage)
        {
            int Division = 12; 
            int SinglePart = game.spellSlots.DamageReceivingModifier(TotalDamage) / Division;
            damageInflictedTotal = TotalDamage;
            damageInflictionOn = true; // TODO: overlaping damge 
            damageInflictCounter = 0;
            DamageInflictSequence = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };
            
            if (Math.Abs(damageInflictedTotal) < INFLICT_MAX_COUNTER)
            {
                for (int i = 0; i < damageInflictedTotal; i++)
                    DamageInflictSequence[i] = 1;
            }
            else
            {
                for (int i = 0; i < INFLICT_MAX_COUNTER; i++)
                    DamageInflictSequence[i] = Math.Min(i, INFLICT_MAX_COUNTER - 1 - i) * SinglePart;
                // Allow me to leave 2 magic number here, it's to deal with the peak of damage
                // And ensure the division does not reduce total damage due to remainders. 
                // One damage instance is splited into 0, 1, 2, 3, x, 2, 1, 0 parts 
                // and this entry is the `x`
                DamageInflictSequence[4] = damageInflictedTotal - 9 * SinglePart;
            }

        }

        /// <summary>
        /// Upon taking damage, it'll be distributed in the next 8 update cycles. 
        /// (Depending on the machine that time varies)
        /// This method returns the amount of health reduction in the current cycle. 
        /// </summary>
        /// <returns>Amount of thealth to be removed.</returns>
        private int ContinuedHealthReduction()
        {
            int DamageInflictedNow = 0;

            if (!damageInflictionOn || Untouchable) return 0; // Provide a chance to br damage immune 

            if (damageInflictCounter >= INFLICT_MAX_COUNTER)
            {
                damageInflictionOn = false;
                pastHealth = -1; 
                return 0;
            }

            DamageInflictedNow = DamageInflictSequence[damageInflictCounter];

            if (damageInflictCounter > 0)
                pastHealth = currentHealth + DamageInflictSequence[damageInflictCounter - 1];

            damageInflictCounter++;

            return DamageInflictedNow; 
        }

    }
}
