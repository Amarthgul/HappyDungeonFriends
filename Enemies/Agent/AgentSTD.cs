using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace HappyDungeon.Enemies
{
    /// <summary>
    /// Vampire is afraid of illuminati, but will follow and attack the player character
    /// when illuminati is not on. 
    /// </summary>
    class AgentSTD : IAgent
    {

        private Game1 game; 

        protected int minDelay = 2000;
        protected double delayMultiplier = 1.5; 

        IEnemy self;

        protected float baseSpeed = 0.4f * Globals.SCALAR;
        protected float frenzySpeed = 0.8f * Globals.SCALAR; 

        protected bool seekPlayer = true;        // If it has the ability to identify player as its enemy
        protected bool rangedSensing = true;     // Constrain on seekPlayer only when they're close enough 
        protected bool rangedFrenzy = true;      // Whether if it enters frenzy after sensing the player 
        protected bool photophobia = true;       // If it runs away when the player has illuminati on 
        protected bool photonFrenzy = true;      // If it enters into a frenzy after seeing light 
        protected bool smartPathFinding = false;  // If it knows to try to go around the wall 
        protected bool smartRedirection = true;  // If it keeps a memeory of recent directions 
        protected bool smartAttack = true;       // Only attacks when aligned with the player  
        protected bool canLockOnPlayer = true;   // If it can lock onto the player once encountered 
        protected bool lockPhotonTrigger = true; // If automatically trigger lock on when illuminati is on
        protected bool isBlind = false;          // Being blind will make nullify photophobia 

        protected bool lockOnPlayer = false;     // If the player is being locked 
        protected float senseRange = 2.25f * Globals.OUT_UNIT;
        protected float photonRange = 2.5f * Globals.OUT_UNIT;
        protected int attackAlignTolerance = (int)(0.5 * Globals.OUT_UNIT);
        protected int lockOnPlayerTurnRateBase = (int)(1.5 * Globals.OUT_UNIT);
        protected int directionMemoryCount = 10; // For smartRedirection, try to find the direction appear least in the list
        protected Queue<Globals.Direction> directionHistory = new Queue<Globals.Direction>();

        protected int wallSeekingTime = 250;
        protected int frenzyCD = 500;
        protected Stopwatch wallSeekingSW;
        protected Stopwatch frenzyCDSW;
        protected bool turnLock = false;

        protected Stopwatch turnSW;
        protected int nextTurn;
        protected int nextTurnLockModified;
        protected int nextTurnPhotoModified; 

        protected Globals.Direction facingDir; 
        protected Globals.Direction playerDirection;
        protected int playerDistance;

        

        public AgentSTD(IEnemy FindMyself, Game1 G, Globals.Direction D)
        {
            self = FindMyself;
            game = G; 
            facingDir = D;

            wallSeekingSW = new Stopwatch(G);
            turnSW = new Stopwatch(G);
            frenzyCDSW = new Stopwatch(G);

            wallSeekingSW.Restart();
            turnSW.Restart();
            frenzyCDSW.Restart();

            RecalNextDelay(-1);
        }

        public virtual void HandleBlockCollision(Globals.Direction FacingDir)
        {

            if (seekPlayer)
            {
                if (smartPathFinding && wallSeekingSW.ElapsedMilliseconds > wallSeekingTime)
                {
                    Turn(SideDirection()[Globals.RND.Next() % 2]);
                    wallSeekingSW.Restart();
                    turnLock = true;
                }
                else if (!turnLock)
                    Turn(playerDirection);
            }
            else 
            {
                Turn(Misc.Instance.Opposite(FacingDir));
            }

        }

        public virtual void Update(MC MainChara)
        {
            // If the game is not currently running and not set to real time, then do not update
            if (game.gameState != Globals.GameStates.Running && !Globals.REAL_TIME_ACTION)
                return; 

            UpdateSeeker(MainChara);

            UpdateTurn(MainChara);

            UpdateAttack(MainChara);

        }

        // ================================================================================
        // ================================ Private methods ===============================
        // ================================================================================
        
        /// <summary>
        /// Update if seekPlayer is enabled, speed up if frenzy is enabled. 
        /// </summary>
        /// <param name="MainChara">Info about player</param>
        protected virtual void UpdateSeeker(MC MainChara)
        {
            playerDirection = ToPlayerDirection(MainChara);
            playerDistance = Misc.Instance.L2Distance(self.GetPosition(), MainChara.position);

            // Dynamically scale the threshold for some turn situation
            if (seekPlayer && canLockOnPlayer)
            {
                nextTurnLockModified = (int)(nextTurn * 
                    (ToTargetD1Distance(MainChara.position, false) / (double)lockOnPlayerTurnRateBase));
            }
            if (photophobia)
            {
                nextTurnPhotoModified = (int)(nextTurn * Math.Pow((playerDistance / (double)photonRange), 2));
            }

            // Assume that when player emits light, the enemy can see the player regardless 
            if (canLockOnPlayer && lockPhotonTrigger)
            {
                if (MainChara.Illuminati())
                    lockOnPlayer = true;
                else
                    lockOnPlayer = false;
            }

            if (seekPlayer)
            {
                if (!isBlind && // Photophobia requires vision
                    MainChara.Illuminati() && // The light is on 
                    (photophobia && photonFrenzy && (playerDistance < photonRange)))
                {
                    // When player with light comes into range, flee away
                    MarkFrenzy(true);
                }
                else if ((rangedSensing && rangedFrenzy && (playerDistance < senseRange)))
                {
                    // Chase the player
                    MarkFrenzy(false);
                }
                else
                {
                    // Go back to normal 
                    self.SpeedChange(baseSpeed);
                }
            }
        }

        /// <summary>
        /// Update the direction, could be affected by player position, player distance, and self-awareness. 
        /// </summary>
        /// <param name="MainChara">The player character for reference</param>
        protected virtual void UpdateTurn(MC MainChara)
        {
            if (lockOnPlayer && turnSW.ElapsedMilliseconds > nextTurnLockModified)
            {   // Dynamically turn if locked on to player
                if (!photophobia || (photophobia && playerDistance > photonRange))
                    Turn(playerDirection);
            }
            else if (turnSW.ElapsedMilliseconds > nextTurn && !turnLock)
            {   // Normal random turn 
                if (seekPlayer)
                {
                    if ((rangedSensing && playerDistance < senseRange))
                        Turn(playerDirection); // If it can sense the player, turn to the player direction 
                    else
                        Turn(TurnPseudoRand(PossibleDirections())); // Rand turn 
                }
                else
                    Turn(TurnPseudoRand());
            }

            // Deal with the case where player is holding a torch and is coming too close 
            if (photophobia && playerDistance < photonRange && MainChara.Illuminati())
            {
                if (turnSW.ElapsedMilliseconds > nextTurnPhotoModified)
                {
                    if (smartPathFinding)
                    {
                        List<Globals.Direction> Dirs = new List<Globals.Direction>(Globals.FourDirIter);
                        Dirs.Remove(playerDirection);
                        Turn(Dirs[Globals.RND.Next() % Dirs.Count]);
                    }
                    else
                    {
                        Turn(Misc.Instance.Opposite(playerDirection));
                    }
                }
                if (rangedFrenzy)
                {
                    self.SpeedChange(frenzySpeed);
                    frenzyCDSW.Restart();
                }
            }

            if (wallSeekingSW.ElapsedMilliseconds > wallSeekingTime)
            {   // Refresh smart path finding lock 
                turnLock = false;
            }
        }

        /// <summary>
        /// Check if it can make an attack, and if so, what kind of attack it will do.
        /// </summary>
        /// <param name="MainChara">Player and related information</param>
        protected virtual void UpdateAttack(MC MainChara)
        {
            if (self.CanAttack())
            {
                if (smartAttack && seekPlayer && facingDir == playerDirection &&
                    ToTargetD1Distance(MainChara.position, false) < attackAlignTolerance)
                {
                    Attack();
                }
                else if (!smartAttack)
                {
                    Attack();
                }
                
            }
        }

        /// <summary>
        /// Make the agent frenzy. Either eascape from player or actively chase the player. 
        /// </summary>
        /// <param name="InFear">Escape if true</param>
        protected virtual void MarkFrenzy(bool InFear)
        {
            if (frenzyCDSW.ElapsedMilliseconds > frenzyCD)
            {
                self.SpeedChange(frenzySpeed);
                frenzyCDSW.Restart();

                if (InFear)
                {
                    // Flee from the player 
                    List<Globals.Direction> Dirs = new List<Globals.Direction>(Globals.FourDirIter);
                    Dirs.Remove(playerDirection);
                    Turn(Dirs[Globals.RND.Next() % Dirs.Count]);
                }
                else
                {
                    Turn(playerDirection);
                }
                
            }
        }

        protected virtual void Turn(Globals.Direction NewDir)
        {
            self.Turn(NewDir);
            RecalNextDelay(-1);
            facingDir = NewDir;

            turnSW.Restart();
        }

        protected virtual Globals.Direction TurnPseudoRand()
        {
            if (!smartRedirection)
                return (Globals.Direction)(Globals.RND.Next() % 4);


            Globals.Direction Result;
            int[] DirRecords = new int[] { 0, 0, 0, 0};
            int SmallestIndex = 10; 

            foreach (Globals.Direction Dir in directionHistory)
            {
                DirRecords[(int)Dir] += 1; 
            }
            for(int i = 0; i < DirRecords.Length; i++)
            {
                if (DirRecords[i] < SmallestIndex)
                    SmallestIndex = i; 
            }

            Result = Globals.FourDirIter[SmallestIndex];
            directionHistory.Enqueue(Result);

            if (directionHistory.Count > directionMemoryCount)
                directionHistory.Dequeue();

            return Result;
        }

        protected virtual Globals.Direction TurnPseudoRand(Globals.Direction[] Options)
        {
            if (!smartRedirection)
                return Options[Globals.RND.Next() % Options.Length];

            Globals.Direction Result = Globals.FourDirIter[Globals.RND.Next() % 4];
            int[] DirRecords = new int[] { 0, 0, 0, 0 };
            int SmallestIndex = 10;

            foreach (Globals.Direction Dir in directionHistory)
            {
                DirRecords[(int)Dir] += 1;
            }
            for (int i = 0; i < DirRecords.Length; i++)
            {
                if (DirRecords[i] < SmallestIndex && Options.Contains(Globals.FourDirIter[i]))
                {
                    SmallestIndex = DirRecords[i];
                    Result = Globals.FourDirIter[i];
                }
            }

            directionHistory.Enqueue(Result);

            if (directionHistory.Count > directionMemoryCount)
                directionHistory.Dequeue();

            return Result;

        }

        protected virtual void Attack()
        {
            self.Attack();
            self.SetAttackInterval(0);
        }

        /// <summary>
        /// Calculate the next random delay constant for turning. 
        /// </summary>
        /// <param name="Standard">Mode index, negative will use its minDelay</param>
        protected virtual void RecalNextDelay(int Standard)
        {
            if (Standard > 0)
                nextTurn = Globals.RND.Next(Standard, (int)(Standard * delayMultiplier));
            else 
                nextTurn = Globals.RND.Next(minDelay, (int)(minDelay * delayMultiplier));

            nextTurnLockModified = nextTurn;
            nextTurnPhotoModified = nextTurn;
        }

        protected virtual Globals.Direction ToPlayerDirection(MC MainChara)
        {
            Globals.Direction RelDirection;
            Vector2 Distance = self.GetPosition() - MainChara.position;
            Vector2 DistanceAbs = new Vector2(Math.Abs(Distance.X), Math.Abs(Distance.Y));

            if (DistanceAbs.X > DistanceAbs.Y)
            {
                if (Distance.X > 0)
                    RelDirection = Globals.Direction.Left;
                else
                    RelDirection = Globals.Direction.Right;
            }
            else
            {
                if (Distance.Y > 0)
                    RelDirection = Globals.Direction.Up;
                else
                    RelDirection = Globals.Direction.Down;

            }

            return RelDirection;
        }

        protected virtual float ToTargetD1Distance(Vector2 Target, bool LongSide)
        {
            float D1Distance;
            Vector2 Distance = self.GetPosition() - Target;

            Distance.X = Math.Abs(Distance.X);
            Distance.Y = Math.Abs(Distance.Y);

            D1Distance = LongSide? (int)Math.Max(Distance.X, Distance.Y) : (int)Math.Min(Distance.X, Distance.Y);

            return D1Distance;

        }

        protected virtual int ToPlayerL1Distance(MC MainChara)
        {
            return Misc.Instance.L1Distance(self.GetPosition(), MainChara.position);
        }

        /// <summary>
        /// Get all directions despite the current facing direction. 
        /// </summary>
        /// <returns>List of 3 directions</returns>
        protected virtual Globals.Direction[] PossibleDirections()
        {
            List<Globals.Direction> PD = new List<Globals.Direction>() { 
                Globals.Direction.Down,
                Globals.Direction.Up,
                Globals.Direction.Left,
                Globals.Direction.Right
            };
            PD.Remove(facingDir);

            return PD.ToArray();
            
        }

        protected virtual Globals.Direction[] SideDirection()
        {
            if (facingDir == Globals.Direction.Down || facingDir == Globals.Direction.Up)
            {
                return new Globals.Direction[] { Globals.Direction.Left, Globals.Direction.Right };
            }
            else
            {
                return new Globals.Direction[] { Globals.Direction.Up, Globals.Direction.Down };
            }
        }
    }
}
