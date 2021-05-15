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

        public bool isMoving;                   // This field is for limiting movement to only 1 direction 
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

        // ================================================================================
        // ============================= Drawing and textures =============================
        // ================================================================================
        private GeneralSprite walking;
        private GeneralSprite walkingWithTorch; 
        // The sprite to use 
        private GeneralSprite currentWalkingSprite;

        private GeneralSprite torchFlame;
        private GeneralSprite torchShadow; 
        private List<GeneralSprite> additionalSprites; 

        private Color defaultTint = Color.White;


        // ================================================================================
        // ============================= States and parameters ============================
        // ================================================================================
        private enum primaryTypes { None, Torch };
        private primaryTypes primaryState;
        public Globals.GeneralStates mcState { set; get; }
        public int currentHealth;
        public int currentMaxHP;
        public int moveSpeed;
        public int pickupDistance = 2;  // Not having to completely reach an item to pick it up. In original unit

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
            isMoving = false;

            damageProtectionSW = new Stopwatch();
            damageProtectionSW.Restart();
            damageRecoverTimer = 0; 

            currentHealth = (int)(MAX_HEALTH * INIT_HP_RATIO);
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

        public void Move()
        {
            isMoving = true; // Set to tru so only 1 direction is moving at a time 
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

            currentWalkingSprite.rowLimitation = (int)facingDir;
            currentWalkingSprite.Update();

            foreach (GeneralSprite sprite in additionalSprites)
            {
                sprite.rowLimitation = (int)facingDir;
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

        public void Attack()
        {

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
                currentWalkingSprite = walkingWithTorch;

                currentWalkingSprite.rowLimitation = walking.rowLimitation;
                torchFlame.rowLimitation = walking.rowLimitation;
                torchShadow.rowLimitation = walking.rowLimitation;

                additionalSprites.Add(torchFlame);
                additionalSprites.Add(torchShadow);
            }
            else
            {
                primaryState = primaryTypes.None;
                currentWalkingSprite = walking;
                currentWalkingSprite.rowLimitation = walkingWithTorch.rowLimitation;

                additionalSprites.Clear();
            }
        }

        /// <summary>
        /// While holding torch (or in some other conditions),
        /// the character might drive enemies away.
        /// </summary>
        /// <returns></returns>
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
            return collisionRect; 
        }

        /// <summary>
        /// Let player deal with collide with enemies. 
        /// </summary>
        /// <param name="DMG">Damage instance object</param>
        /// <param name="DirFrom">Which direction id the collision from</param>
        public void TakeCollisionDamage(DamageInstance DMG, Globals.Direction DirFrom)
        {
            if (recoverImmunity)
                return;

            currentHealth += DMG.DamageCount;

            foreach(Globals.DamageEffect FX in DMG.effects)
            {
                switch (FX)
                {
                    case Globals.DamageEffect.Break:
                        break;
                    case Globals.DamageEffect.Knockback:
                        stagedMovement += MaxKnockbackDist(DirFrom, DMG.knowckbackDist);
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
                case Globals.GeneralStates.Hold:
                    break;
                case Globals.GeneralStates.Moving:
                    break;
                case Globals.GeneralStates.Attack:
                    break;
                case Globals.GeneralStates.Damaged:
                    break;
                default:
                    break; 
            }

            enemyCollisionCheck.CheckEnemyCollision(GetRectangle());
            
            // Handle collision with blocks 
            if (blockCollisionCheck.ValidMove(GetStagedRectangle()) && CanPassDoors())
            {
                position += stagedMovement;
            }
               
            // Check and handle item pick up
            itemCollisionCheck.CheckItemCollision(GetPickupRectangle());

            stagedMovement = new Vector2(0, 0);

            collisionRect.X = (int)(position.X + collisionOffset.X);
            collisionRect.Y = (int)(position.Y + collisionOffset.Y);

            isMoving = false; // Set back to false for next movement  

            Regulate();
            UpdateTurnCD();
            UpdateDamgeRecoverProtection();

            CheckBoundary();

            UpdateAllSprites();
        }

        public void Draw()
        {
            currentWalkingSprite.Draw(spriteBatch, position, defaultTint);

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
            ImageFile WWT = TextureFactory.Instance.mcTorchWalk; // Walking With Torch 
            ImageFile iTF = TextureFactory.Instance.itemTorchFlame;
            ImageFile iTS = TextureFactory.Instance.itemTorchShadow;

            additionalSprites = new List<GeneralSprite>();

            // Creating all sprites 
            walking = new GeneralSprite(WalkingIM.texture, WalkingIM.C, WalkingIM.R,
                Globals.WHOLE_SHEET, Globals.FRAME_CYCLE, Globals.MC_LAYER);
            walkingWithTorch = new GeneralSprite(WWT.texture, WWT.C, WWT.R,
                Globals.WHOLE_SHEET, Globals.FRAME_CYCLE, Globals.MC_LAYER);

            torchFlame = new GeneralSprite(iTF.texture, iTF.C, iTF.R,
                Globals.WHOLE_SHEET, Globals.FRAME_CYCLE, Globals.MC_LAYER + 0.01f);
            torchFlame.positionOffset = Globals.SPRITE_OFFSET_2;

            torchShadow = new GeneralSprite(iTS.texture, iTS.C, iTS.R,
                Globals.WHOLE_SHEET, Globals.FRAME_CYCLE, Globals.MC_LAYER - 0.01f);
            torchFlame.positionOffset = Globals.SPRITE_OFFSET_2;

            currentWalkingSprite = walking;

        }

        /// <summary>
        /// Since player is always visible, additional sprites can be updated all the time. 
        /// </summary>
        private void UpdateAllSprites()
        {
            foreach (GeneralSprite sprite in additionalSprites)
            {
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

        private Vector2 MaxKnockbackDist(Globals.Direction DirFrom, double Dist)
        {
            Vector2 MaxDist = new Vector2(64, 0);
            int RealDist = (int)Dist * Globals.SCALAR; 

            switch (DirFrom)
            {
                case Globals.Direction.Left:
                    MaxDist = new Vector2(- RealDist, 0);
                    break;
                case Globals.Direction.Right:
                    MaxDist = new Vector2(RealDist, 0);
                    break;
                case Globals.Direction.Up:
                    MaxDist = new Vector2(0, RealDist);
                    break;
                case Globals.Direction.Down:
                    MaxDist = new Vector2(0, -RealDist);
                    break;
                default:
                    break;
            }

            return MaxDist;
        }


    }
}
