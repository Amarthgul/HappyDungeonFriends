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
        protected int minDelay = 2000;
        protected double delayMultiplier = 1.5; 

        IEnemy self;

        protected int baseSpeed = (int)(0.4 * Globals.SCALAR);
        protected int frenzySpeed = (int)(0.8 * Globals.SCALAR); 

        protected bool seekPlayer = true;        // If it has the ability to identify player as its enemy
        protected bool rangedSensing = false;     // Constrain on seekPlayer only when they're close enough 
        protected bool rangedFrenzy = false;      // Whether if it enters frenzy after sensing the player 
        protected bool photophobia = true;       // If it runs away when the player has illuminati on 
        protected bool photonFrenzy = true;      // If it enters frenzy as it eascapes from light 
        protected bool smartPathFinding = false;  // If it knows to try to go around the wall 
        protected bool smartAttack = true;       // Only attacks when alighned 
        protected bool canLockOnPlayer = true;   // If it can loack on player once encountered 
        protected bool lockPhotonTrigger = true; // If automatically trigger lock on when illuminati is on 
        protected bool lockOnPlayer = false;     // If the player is being locked 
        protected int senseRange = (int)(2.25 * Globals.OUT_UNIT);
        protected int photonRange = (int)(2.5 * Globals.OUT_UNIT);
        protected int attackAlignTolerance = (int)(0.5 * Globals.OUT_UNIT);
        protected int lockOnPlayerTurnRateBase = (int)(1.5 * Globals.OUT_UNIT); 

        protected int wallSeekingTime = 250;
        protected int frenzyCD = 500;
        protected Stopwatch wallSeekingSW = new Stopwatch();
        protected Stopwatch frenzyCDSW = new Stopwatch();
        protected bool turnLock = false;

        protected Stopwatch turnSW;
        protected int nextTurn;
        protected int nextTurnLockModified;
        protected int nextTurnPhotoModified; 

        protected Globals.Direction facingDir; 
        protected Globals.Direction playerDirection;
        protected int playerDistance;

        

        public AgentSTD(IEnemy FindMyself, Globals.Direction D)
        {
            self = FindMyself;
            facingDir = D;

            wallSeekingSW = new Stopwatch();
            turnSW = new Stopwatch();
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
                    (ToPlayerD1Distance(MainChara, false) / (double)lockOnPlayerTurnRateBase));
            }
            if (photophobia)
            {
                nextTurnPhotoModified = (int)(nextTurn * Math.Pow((playerDistance / (double)photonRange), 2));
            }

            if (canLockOnPlayer && lockPhotonTrigger)
            {
                if (MainChara.Illuminati())
                    lockOnPlayer = true;
                else
                    lockOnPlayer = false;
            }

            if (seekPlayer)
            {
                if (photophobia && photonFrenzy && playerDistance < photonRange)
                {
                    MarkFrenzy(true);
                }
                else if (rangedSensing && rangedFrenzy && playerDistance < senseRange)
                {
                    MarkFrenzy(false);
                }
                else
                {
                    self.SpeedChange(baseSpeed);
                }
            }
        }

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
                        Turn(playerDirection);
                    else
                        Turn(PossibleDirections()[Globals.RND.Next() % 3]);
                }
                else
                    Turn((Globals.Direction)(Globals.RND.Next() % 4));
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

        protected virtual void UpdateAttack(MC MainChara)
        {
            if (self.CanAttack())
            {
                if (smartAttack && seekPlayer && facingDir == playerDirection &&
                    ToPlayerD1Distance(MainChara, false) < attackAlignTolerance)
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

        protected virtual void Attack()
        {
            self.Attack();
            self.SetAttackInterval(0);
        }

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

        protected virtual int ToPlayerD1Distance(MC MainChara, bool LongSide)
        {
            int D1Distance;
            Vector2 Distance = self.GetPosition() - MainChara.position;

            Distance.X = Math.Abs(Distance.X);
            Distance.Y = Math.Abs(Distance.Y);

            D1Distance = LongSide? (int)Math.Max(Distance.X, Distance.Y) : (int)Math.Min(Distance.X, Distance.Y);

            return D1Distance;

        }

        protected virtual int ToPlayerL1Distance(MC MainChara)
        {
            return Misc.Instance.L1Distance(self.GetPosition(), MainChara.position);
        }

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
