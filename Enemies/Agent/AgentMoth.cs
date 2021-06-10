using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace HappyDungeon.Enemies
{
    /// <summary>
    /// Moth wonders randomly in the dark, but will try to follow the player character
    /// when illuminati is on. 
    /// </summary>
    class AgentMoth : IAgent
    {
        private const int DELAY_MAX = 8000;
        private const int DELAY_MIN = 4000;

        IEnemy self;

        private Stopwatch stopwatch;
        private long timer;
        private long nextTurn; 

        public AgentMoth(IEnemy FindMyself)
        {
            self = FindMyself;

            stopwatch = new Stopwatch();
            stopwatch.Restart();
            timer = 0;

            RecalNextDelay();
        }

        public Globals.Direction HandleBlockCollision(Globals.Direction FacingDir)
        {
            return Misc.Instance.Opposite(FacingDir);
        }

        public void Update(MC MainChara) 
        {
            timer = stopwatch.ElapsedMilliseconds;
            if (timer > nextTurn)
            {
                self.Turn((Globals.Direction)(Globals.RND.Next()%4)) ;
                RecalNextDelay();

                timer = 0;
                stopwatch.Restart();
            }
        }


        private void RecalNextDelay()
        {
            nextTurn = Globals.RND.Next(4000, 8000);

        }
    }
}
