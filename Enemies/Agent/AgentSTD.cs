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
        protected const int DELAY_MAX = 2000;
        protected const int DELAY_MIN = 1000;

        IEnemy self;


        protected int baseSpeed = (int)(0.4 * Globals.SCALAR);
        protected int frenzySpeed = (int)(0.8 * Globals.SCALAR); 

        protected bool seekPlayer = true;        // If it has the ability to identify player as its enemy
        protected bool rangedSensing = true;     // Constrain on seekPlayer only when they're close enough 
        protected bool rangedFrenzy = true;      // Whether if it enters frenzy after sensing the player 
        protected bool photophobia = true;       // If it runs away when the player has illuminati on 
        protected bool smartPathFinding = true;  // If it knows to try to go around the wall 
        protected int senseRange = 3 * Globals.OUT_UNIT;
        protected int photonRange = 2 * Globals.OUT_UNIT;

        protected int wallSeekingTime = 250;
        protected int frenzyCD = 500;
        protected Stopwatch wallSeekingSW = new Stopwatch();
        protected Stopwatch frenzyCDSW = new Stopwatch();
        protected bool turnLock = false;

        protected Stopwatch turnSW;
        protected long nextTurn;

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

            RecalNextDelay();
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

            if (photophobia && playerDistance < photonRange && MainChara.Illuminati())
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
                    
                if (rangedFrenzy)
                {
                    self.SpeedChange(frenzySpeed);
                    frenzyCDSW.Restart();
                }
            }

            if (seekPlayer)
            {
                if (rangedSensing && rangedFrenzy && playerDistance < senseRange)
                {
                    if (frenzyCDSW.ElapsedMilliseconds > frenzyCD)
                    {
                        self.SpeedChange(frenzySpeed);
                        Turn(playerDirection);
                        frenzyCDSW.Restart();
                    }
                }
                else
                {
                    self.SpeedChange(baseSpeed);
                }
            }
        }

        protected virtual void UpdateTurn(MC MainChara)
        {
            if (turnSW.ElapsedMilliseconds > nextTurn && !turnLock)
            {
                if (seekPlayer)
                {
                    if (rangedSensing && playerDistance < senseRange)
                        Turn(playerDirection);
                    else
                        Turn(PossibleDirections()[Globals.RND.Next() % 3]);
                }
                else
                    Turn((Globals.Direction)(Globals.RND.Next() % 4));
            }

            if (wallSeekingSW.ElapsedMilliseconds > wallSeekingTime)
            {
                turnLock = false;
            }
        }

        protected virtual void UpdateAttack(MC MainChara)
        {
            if (self.CanAttack())
            {
                self.Attack();
                self.SetAttackInterval(0);
            }
        }

        protected virtual void Turn(Globals.Direction NewDir)
        {
            self.Turn(NewDir);
            RecalNextDelay();
            facingDir = NewDir;

            turnSW.Restart();
        }

        protected virtual void RecalNextDelay()
        {
            nextTurn = Globals.RND.Next(DELAY_MIN, DELAY_MAX);

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

        protected virtual int ToPlayerD1Distance(MC MainChara)
        {
            int D1Distance;
            Vector2 Distance = self.GetPosition() - MainChara.position;

            Distance.X = Math.Abs(Distance.X);
            Distance.Y = Math.Abs(Distance.Y);

            D1Distance = Distance.X > Distance.Y ? (int)Distance.X : (int)Distance.Y;

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
