using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;



namespace HappyDungeon
{
    public class Stopwatch
    {
        private Game1 game;

        private long milliseconds;
        private long memoryMS;

        private Globals.GameStates previousState;
        private Globals.GameStates currentState; 

        private System.Diagnostics.Stopwatch innerSW; 

        public long ElapsedMilliseconds
        {
            get {

                if (game.gameState == Globals.GameStates.Running)
                {
                    milliseconds = memoryMS + innerSW.ElapsedMilliseconds;
                } 
                else
                {
                    memoryMS = milliseconds;
                    innerSW.Restart(); 
                }
                

                return milliseconds; 
            }
            set { }
        }

        public Stopwatch(Game1 G)
        {
            game = G;

            innerSW = new System.Diagnostics.Stopwatch();

            memoryMS = 0; 
            milliseconds = 0; 
        }

        public void Start()
        {
            innerSW.Start();
        }

        public void Restart()
        {
            innerSW.Restart();
        }

        public void Reset()
        {
            innerSW.Reset();
        }

        public void Stop()
        {
            innerSW.Stop(); 
        }
    }
}
