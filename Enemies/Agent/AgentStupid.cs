using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace HappyDungeon.Enemies
{
    /// <summary>
    /// Stupid does not seek or evade the player, it just wonders randomly in the map, 
    /// turning to random directions at random time.
    /// </summary>
    class AgentStupid : IAgent
    {
        private const int DELAY_MAX = 8000;
        private const int DELAY_MIN = 4000;

        IEnemy self;

        private Stopwatch stopwatch;
        private long timer;
        private long nextTurn; 

        public AgentStupid(IEnemy FindMyself, Game1 G)
        {
            self = FindMyself;

            stopwatch = new Stopwatch(G);
            stopwatch.Restart();
            timer = 0;

            RecalNextDelay();
        }

        public void HandleBlockCollision(Globals.Direction FacingDir)
        {
            self.Turn(Misc.Instance.Opposite(FacingDir));
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
