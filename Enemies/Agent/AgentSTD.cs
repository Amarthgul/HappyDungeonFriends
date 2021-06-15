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

        protected bool seekPlayer = true;
        protected bool rangedSensing = true;     // Constrain on seekPlayer
        protected bool rangedFrenzy = true;
        protected bool smartPathFinding = false;
        protected int senseRange = 3 * Globals.OUT_UNIT;

        protected Globals.Direction facingDir; 
        protected Globals.Direction playerDirection;
        protected int playerDistance;

        protected Stopwatch turnSW;
        protected long timer;
        protected long nextTurn;

        public AgentSTD(IEnemy FindMyself, Globals.Direction D)
        {
            self = FindMyself;
            facingDir = D; 

            turnSW = new Stopwatch();
            turnSW.Restart();
            timer = 0;

            RecalNextDelay();
        }

        public virtual void HandleBlockCollision(Globals.Direction FacingDir)
        {

            if (seekPlayer)
            {

                Turn(playerDirection);
            }
            else
            {
                Turn(Misc.Instance.Opposite(FacingDir));
            }

        }

        public virtual void Update(MC MainChara)
        {
            if (seekPlayer)
            {
                playerDirection = ToPlayerDirection(MainChara);
                playerDistance = Misc.Instance.L2Distance(self.GetPosition(), MainChara.position);

                if (rangedSensing && rangedFrenzy && playerDistance < senseRange)
                {
                    self.SpeedChange(frenzySpeed);
                }
                else
                {
                    self.SpeedChange(baseSpeed);
                }
            }

            if (turnSW.ElapsedMilliseconds > nextTurn)
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

            if (self.CanAttack())
            {
                self.Attack();
                self.SetAttackInterval(0);
            }


        }

        // ================================================================================
        // ================================ Private methods ===============================
        // ================================================================================

        protected virtual void Turn(Globals.Direction NewDir)
        {
            self.Turn(NewDir);
            RecalNextDelay();

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
    }
}
