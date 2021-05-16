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
    /// Due to some design flaws, there are more sprites than there should be. 
    /// You can see this in LoadAllSprites().
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
        private Stopwatch[] dirStopwatches = new Stopwatch[] { new Stopwatch() , new Stopwatch() , new Stopwatch() , new Stopwatch() };
        private long[] dirTimers = new long[] { 0, 0, 0, 0 }; 

        private Rectangle collisionRect;
        private Vector2 stagedMovement; 
        private Vector2 collisionOffset; 
        private Vector2 startUpPosition = new Vector2(Globals.OUT_FWIDTH / 2 - Globals.OUT_UNIT / 2, 
            Globals.OUT_FHEIGHT / 2 + Globals.OUT_UNIT);

        // ================================================================================
        // =================================== Interactions ===============================
        // ================================================================================

        private PlayerBlockCollision blockCollisionCheck;
        private PlayerItemCollision itemCollisionCheck;
        private PlayerEnemyCollision enemyCollisionCheck;

        private bool recoverImmunity = false; 
        private Stopwatch damageProtectionSW;
        private long damageRecoverTimer;
        private int recoverTime = 1500;

        private bool canAttack = true;
        private Stopwatch attackSW;
        private long attackIntervalTimer;
        private int attackInterval = 500; 

        // ================================================================================
        // ============================= Drawing and textures =============================
        // ================================================================================
        private GeneralSprite walking;
        private GeneralSprite attack;
        private GeneralSprite walkingWithTorch;
        private GeneralSprite attackWithTorch; 
        // The sprite to use 
        private GeneralSprite currentMainSprite;
        private GeneralSprite lastMainSprite; 

        private GeneralSprite torchFlame;
        private GeneralSprite torchShadow;
        private GeneralSprite torchAttackFlame;
        private List<GeneralSprite> additionalSprites; // Select some and add them into this for draw and update 
        private List<GeneralSprite> lastAddSprites; 

        private Color defaultTint = Color.White;
        private Color damagedTint = Color.Red;
        private Color tintNow; 


        // ================================================================================
        // ============================= States and parameters ============================
        // ================================================================================
        private bool Untouchable = false; // Controls damge taking, as in ContinuedHealthReduction

        private enum primaryTypes { None, Torch };
        private primaryTypes primaryState;
        public Globals.GeneralStates mcState { set; get; }
        public int currentHealth;
        public int pastHealth;          // For the headsup display to signify the damage 
        public int currentMaxHP;
        public int moveSpeed;
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

            position = startUpPosition;
            facingDir = Globals.Direction.Up;

            stagedMovement = new Vector2(0, 0);
            collisionOffset = new Vector2(RECT_WIDTH_OFFSET * Globals.OUT_UNIT, RECT_HEIGHT_OFFSET * Globals.OUT_UNIT);
            collisionRect = new Rectangle((int)collisionOffset.X, (int)collisionOffset.Y, 
                (int)((1 - RECT_WIDTH_OFFSET * 2) * Globals.OUT_UNIT), (int)((1 - RECT_HEIGHT_OFFSET) * Globals.OUT_UNIT));

            LoadAllSprites();
            
            // Collisions 
            blockCollisionCheck = new PlayerBlockCollision(game);
            itemCollisionCheck = new PlayerItemCollision(game);
            enemyCollisionCheck = new PlayerEnemyCollision(game);

            primaryState = primaryTypes.None;
            mcState = Globals.GeneralStates.Hold; // Start as on hold 
            moveRestricted = false;

            damageProtectionSW = new Stopwatch();
            damageProtectionSW.Restart();
            damageRecoverTimer = 0;

            attackSW = new Stopwatch();

            currentHealth = (int)(MAX_HEALTH * INIT_HP_RATIO);
            pastHealth = -1; 
            currentMaxHP = MAX_HEALTH;
            moveSpeed = 1 * Globals.SCALAR;
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
            moveRestricted = true; // Set to tru so only 1 direction is moving at a time 
            mcState = Globals.GeneralStates.Moving;
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

            currentMainSprite.rowLimitation = (int)facingDir;
            currentMainSprite.Update(); // so that the animation plays only when a key being pressed
            
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

        public void Attack()
        {
            if(mcState != Globals.GeneralStates.Attack && canAttack)
            {
                mcState = Globals.GeneralStates.Attack;

                lastMainSprite = currentMainSprite;
                attack.rowLimitation = (int)facingDir;
                currentMainSprite = AttackSprite();

                lastAddSprites.Clear();
                foreach (GeneralSprite GS in additionalSprites)
                    lastAddSprites.Add(GS);

                additionalSprites = AttackSpriteExtra();

                attackSW.Restart();
                attackIntervalTimer = 0; 
                canAttack = false; 
            }
        }

        /// <summary>
        /// When being called, torch should be the primary item, thus when toggle off, 
        /// it goes back to normal walking. 
        /// </summary>
        public void ToggleTorch()
        {
            if(primaryState != primaryTypes.Torch)
            {
                primaryState = primaryTypes.Torch;
                currentMainSprite = walkingWithTorch;

                currentMainSprite.rowLimitation = walking.rowLimitation;
                torchFlame.rowLimitation = walking.rowLimitation;
                torchShadow.rowLimitation = walking.rowLimitation;

                additionalSprites.Add(torchFlame);
                additionalSprites.Add(torchShadow);
            }
            else
            {
                primaryState = primaryTypes.None;
                currentMainSprite = walking;
                currentMainSprite.rowLimitation = walkingWithTorch.rowLimitation;

                additionalSprites.Clear();
            }
        }

        /// <summary>
        /// While holding torch (or in some other conditions),
        /// the character might drive enemies away.
        /// </summary>
        /// <returns>If the character is in Illuminati mode</returns>
        public bool Illuminati()
        {
            return primaryState == primaryTypes.Torch;
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
        /// Let player deal with colliding with enemies. 
        /// Collision damage is a separated method because collisions are most likly to
        /// have a directional knockback. 
        /// </summary>
        /// <param name="DMG">Damage instance object</param>
        /// <param name="DirFrom">Which direction id the collision from</param>
        public void TakeCollisionDamage(DamageInstance DMG, Globals.Direction DirFrom)
        {
            if (recoverImmunity)
                return;

            MarkDamgeInfliction(DMG.DamageCount);

            foreach(Globals.DamageEffect FX in DMG.effects)
            {
                switch (FX)
                {
                    case Globals.DamageEffect.Break:
                        break;
                    case Globals.DamageEffect.Knockback:
                        position += MaxKnockbackDist(DirFrom, DMG.knowckbackDist);
                        break;
                    case Globals.DamageEffect.Stun:
                        break;
                    default:
                        break;
                }
            }

            recoverImmunity = true;
            damageProtectionSW.Restart();
            damageRecoverTimer = 0;

        }

        public void Refresh(Game1 G)
        {
            game = G;
            blockCollisionCheck = new PlayerBlockCollision(game);

            mcState = Globals.GeneralStates.Hold;
        }

        public void Update()
        {

            switch (mcState)
            {
                
                case Globals.GeneralStates.Attack:
                    UpdateAttack();
                    break;
                case Globals.GeneralStates.Broken: // Broken can not move and can do no shit
                    return; 
                default:
                    break; 
            }

            currentMainSprite.rowLimitation = (int)facingDir; 

            // Check and send singal if collides with enemy 
            enemyCollisionCheck.CheckEnemyCollision(GetRectangle());
            
            // Handle collision with blocks 
            if (blockCollisionCheck.ValidMove(GetStagedRectangle()) && CanPassDoors())
            {
                position += stagedMovement;
            }
               
            // Check and handle item pick up
            itemCollisionCheck.CheckItemCollision(GetPickupRectangle());

            collisionRect.X = (int)(position.X + collisionOffset.X);
            collisionRect.Y = (int)(position.Y + collisionOffset.Y);

            stagedMovement = new Vector2(0, 0);
            moveRestricted = false; // Set back to false for next movement  

            currentHealth += ContinuedHealthReduction(); 

            Regulate();
            UpdateTurnCD();
            UpdateDamgeRecoverProtection();

            CheckBoundary();

            UpdateAllSprites();
        }

        public void Draw()
        {
            if (damageInflictCounter % 2 == 0 && damageInflictionOn)
                tintNow = damagedTint;
            else
                tintNow = defaultTint;

            currentMainSprite.Draw(spriteBatch, position, tintNow);

            foreach (GeneralSprite sprite in additionalSprites)
            {
                sprite.Draw(spriteBatch, position, defaultTint);
            }
            
        }

        // ================================================================================
        // ================================ Private methods ===============================
        // ================================================================================

        /// <summary>
        /// Since there are a lot of sprites, they are being loaded in this separate method.
        /// </summary>
        private void LoadAllSprites()
        {
            // Initlize all IMs 
            ImageFile WalkingIM = TextureFactory.Instance.mcWalk;
            ImageFile Attack = TextureFactory.Instance.mcAttack;
            ImageFile WWT = TextureFactory.Instance.mcTorchWalk; // Walking With Torch 
            ImageFile AWT = TextureFactory.Instance.mcAttackTorch;

            ImageFile iTF = TextureFactory.Instance.itemTorchFlame;
            ImageFile iTS = TextureFactory.Instance.itemTorchShadow;
            ImageFile iTAF = TextureFactory.Instance.itemTorchAttackFlame;

            lastAddSprites = new List<GeneralSprite>();
            additionalSprites = new List<GeneralSprite>();

            // Creating all sprites 
            walking = new GeneralSprite(WalkingIM.texture, WalkingIM.C, WalkingIM.R,
                Globals.WHOLE_SHEET, Globals.FRAME_CYCLE, Globals.MC_LAYER);
            walkingWithTorch = new GeneralSprite(WWT.texture, WWT.C, WWT.R,
                Globals.WHOLE_SHEET, Globals.FRAME_CYCLE, Globals.MC_LAYER);
            attack = new GeneralSprite(Attack.texture, Attack.C, Attack.R,
                Globals.WHOLE_SHEET, Globals.FRAME_CYCLE, Globals.MC_LAYER);
            attackWithTorch = new GeneralSprite(AWT.texture, AWT.C, AWT.R,
                Globals.WHOLE_SHEET, Globals.FRAME_CYCLE, Globals.MC_LAYER);

            torchFlame = new GeneralSprite(iTF.texture, iTF.C, iTF.R,
                Globals.WHOLE_SHEET, Globals.FRAME_CYCLE, Globals.MC_LAYER + 0.01f);
            torchFlame.positionOffset = Globals.SPRITE_OFFSET_2;

            torchShadow = new GeneralSprite(iTS.texture, iTS.C, iTS.R,
                Globals.WHOLE_SHEET, Globals.FRAME_CYCLE, Globals.MC_LAYER - 0.01f);
            torchFlame.positionOffset = Globals.SPRITE_OFFSET_2;

            torchAttackFlame = new GeneralSprite(iTAF.texture, iTAF.C, iTAF.R,
                Globals.WHOLE_SHEET, Globals.FRAME_CYCLE, Globals.ITEM_EFFECT_LAYER);
            torchAttackFlame.positionOffset = Globals.SPRITE_OFFSET_UNIT;

            currentMainSprite = walking;

        }

        /// <summary>
        /// Since player is always visible, additional sprites can be updated all the time. 
        /// </summary>
        private void UpdateAllSprites()
        {

            foreach (GeneralSprite sprite in additionalSprites)
            {
                sprite.rowLimitation = (int)facingDir;
                sprite.Update();
            }
        }

        /// <summary>
        /// Release the directions frozen if enough time have passed.
        /// </summary>
        private void UpdateTurnCD()
        {
            for (int i = 0; i < dirTimers.Length; i++)
            {
                dirTimers[i] = dirStopwatches[i].ElapsedMilliseconds; 

                if (dirTimers[i] > DIRECTION_CD)
                {
                    canTurn[i] = true;
                }
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

        private void UpdateAttack()
        {
            attackIntervalTimer = attackSW.ElapsedMilliseconds;
            attack.Update();

            if(attackIntervalTimer > attackInterval)
            {
                canAttack = true;

                additionalSprites.Clear();
                foreach (GeneralSprite GS in lastAddSprites)
                    additionalSprites.Add(GS);

                mcState = Globals.GeneralStates.Hold;
                currentMainSprite = lastMainSprite;
            }
        }

        private GeneralSprite AttackSprite()
        {
            switch (primaryState)
            {
                case primaryTypes.Torch:
                    return attackWithTorch;
                default:
                    return attack;
            }
        }
        private List<GeneralSprite> AttackSpriteExtra()
        {
            List<GeneralSprite> AttackExtraList = new List<GeneralSprite>();

            switch (primaryState)
            {
                case primaryTypes.Torch:
                    AttackExtraList.Add(torchAttackFlame);
                    break;
                default:
                    break;
            }

            return AttackExtraList; 
        }

        /// <summary>
        /// Regulate parameters to avoid impossible values.
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
            bool CanPass = true;

            RoomInfo currentInfo = game.currentRoom.roomInfo;
            

            if (GetStagedRectangle().X < leftLim)
            {
                CanPass = currentInfo.OpenDoors[(int)Globals.Direction.Left]
                    || currentInfo.Holes[(int)Globals.Direction.Left];
            }


            if (GetStagedRectangle().X > rightLim)
            {
                CanPass = currentInfo.OpenDoors[(int)Globals.Direction.Right]
                      || currentInfo.Holes[(int)Globals.Direction.Right];
            }

            if (GetStagedRectangle().Y < topLim)
            {
                CanPass = currentInfo.OpenDoors[(int)Globals.Direction.Up]
                      || currentInfo.Holes[(int)Globals.Direction.Up];
            }

            if (GetStagedRectangle().Y > botLim)
            {
                CanPass = currentInfo.OpenDoors[(int)Globals.Direction.Down]
                      || currentInfo.Holes[(int)Globals.Direction.Down];
            }

            return CanPass;
        }

        /// <summary>
        /// Check if the MC goes beyond the walls. 
        /// By contract, if she does, then there must be another room she can go to. 
        /// </summary>
        private void CheckBoundary()
        {

            if (GetStagedRectangle().X < leftTrigger)
            {
                position = new Vector2(rightTrigger - NEW_ROOM_OFFSET, position.Y);

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
        /// When the character is being knocked back, find the maxinum amount of distance 
        /// the cahracter can move before goes into a wall or block. 
        /// </summary>
        /// <param name="DirFrom">Direction from which being knocked</param>
        /// <param name="Dist">Designated knock back distance</param>
        /// <returns></returns>
        private Vector2 MaxKnockbackDist(Globals.Direction DirFrom, double Dist)
        {
            Vector2 VecIter = new Vector2(0, 0);
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
                
            }

            return VecIter;
        }

        /// <summary>
        /// When a damage is being delt to the character, calculate the sequence of damage 
        /// to be delt in the following 8 updates, displays as animated health reduction. 
        /// </summary>
        /// <param name="TotalDamage">If it's dealing damage, pass a negative number</param>
        private void MarkDamgeInfliction(int TotalDamage)
        {
            int Division = 12; 
            int SinglePart = TotalDamage / Division;
            damageInflictedTotal = TotalDamage;
            damageInflictionOn = true;
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
