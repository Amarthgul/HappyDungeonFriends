using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace HappyDungeon
{
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
        private Rectangle collisionRect;
        private Vector2 stagedMovement; 
        private Vector2 collisionOffset; 
        private Vector2 startUpPosition = new Vector2(Globals.OUT_FWIDTH / 2 - Globals.OUT_UNIT / 2, 
            Globals.OUT_FHEIGHT / 2 + Globals.OUT_UNIT);

        private PlayerBlockCollision blockCollisionCheck; 

        // ================================================================================
        // ============================= Drawing and textures =============================
        // ================================================================================
        private GeneralSprite walking;
        private GeneralSprite currentSprite;
        private Color defaultTint = Color.White;

        private Game1 game;
        private SpriteBatch spriteBatch;

        // ================================================================================
        // ============================= States and parameters ============================
        // ================================================================================
        private Globals.GeneralStates mcState;
        public int currentHealth;
        public int currentMaxHP;
        public int moveSpeed; 


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

            // Initlize all IMs 
            ImageFile WalkingIM = TextureFactory.Instance.mcWalk;

            // Creating all sprites 
            walking = new GeneralSprite(WalkingIM.texture, WalkingIM.C, WalkingIM.R, 
                Globals.WHOLE_SHEET, Globals.FRAME_CYCLE, Globals.MC_LAYER);

            blockCollisionCheck = new PlayerBlockCollision(game);

            mcState = Globals.GeneralStates.Hold; // Start as on hold 
            isMoving = false;

            currentHealth = (int)(MAX_HEALTH * INIT_HP_RATIO);
            currentMaxHP = MAX_HEALTH;
            moveSpeed = 1 * Globals.SCALAR;
        }

        // ================================================================================
        // ================================= Public methods ===============================
        // ================================================================================
        public void ChangeDirection(Globals.Direction newDir)
        {
            facingDir = newDir;
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

            currentSprite.rowLimitation = (int)facingDir;
            currentSprite.Update();
            
        }

        public void Attack()
        {

        }

        public Rectangle GetRectangle()
        {
            return collisionRect; 
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
                    currentSprite = walking;
                    break;
                case Globals.GeneralStates.Moving:
                    currentSprite = walking;
                    break;
                case Globals.GeneralStates.Attack:
                    break;
                case Globals.GeneralStates.Damaged:
                    break;
                default:
                    break; 
            }


            if (blockCollisionCheck.ValidMove(GetStagedRectangle()) && CanPassDoors())
                position += stagedMovement;

            stagedMovement = new Vector2(0, 0);

            collisionRect.X = (int)(position.X + collisionOffset.X);
            collisionRect.Y = (int)(position.Y + collisionOffset.Y);

            isMoving = false; // Set back to false for next movement  

            Regulate();

            CheckBoundary();
        }

        public void Draw()
        {
            currentSprite.Draw(spriteBatch, position, defaultTint);
        }

        // ================================================================================
        // ================================ Private methods ===============================
        // ================================================================================

        /// <summary>
        /// Regulate parameters to avoid impossible values.
        /// </summary>
        private void Regulate()
        {
            if (currentHealth < 0)
                currentHealth = 0; 
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

    }
}
