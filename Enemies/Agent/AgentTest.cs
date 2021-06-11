﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace HappyDungeon.Enemies
{
    /// <summary>
    /// Vampire is afraid of illuminati, but will follow and attack the player character
    /// when illuminati is not on. 
    /// </summary>
    class AgenTest : IAgent
    {
        private const int DELAY_MAX = 8000;
        private const int DELAY_MIN = 4000;

        IEnemy self;

        private Stopwatch stopwatch;
        private long timer;
        private long nextTurn; 

        public AgenTest(IEnemy FindMyself)
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

            if (self.CanAttack())
            {
                self.Attack();
                self.SetAttackInterval(0);
            }
                

        }


        private void RecalNextDelay()
        {
            nextTurn = Globals.RND.Next(4000, 8000);

        }
    }
}