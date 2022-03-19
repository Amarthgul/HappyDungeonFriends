using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace HappyDungeon.General.Modifiers
{

    class ModifierBurn : IModifier
    {
        private Game1 game; 

        private int burnDamageTotal;
        private int[] damageSequence;
        private int damageIndex = 0;

        public int totalTiks { set; get; }
        public long totalTime { set; get; } 
        public long tikTime { set; get; }

        private long intervalTimer = 0;
        private Stopwatch intervalSW;  

        private long totalTimer = 0;
        private Stopwatch totalSW;

        public ModifierBurn(Game1 G, int TotalDMG)
        {
            game = G;

            burnDamageTotal = TotalDMG;

            totalTiks = 4;
            tikTime = 500; // 0.5 seconds 
            totalTime = 2250;

            int SingleTikDamage = burnDamageTotal / totalTiks; 

            damageSequence = new int[totalTiks];
            for(int i = 0; i < totalTiks; i++)
            {
                damageSequence[i] = SingleTikDamage; 
            }
            damageSequence[damageSequence.Length - 1] = burnDamageTotal - SingleTikDamage * totalTiks;

            intervalSW = new Stopwatch(game);
            totalSW = new Stopwatch(game);

            intervalSW.Restart();
            totalSW.Restart();
        }

        public int Regen()
        {
            return 0;
        }

        public int MovementSpeedModifier()
        {
            return 0;
        }

        public int DamageTakenModifier()
        {
            return 0;
        }

        public int DamageDealtModifer()
        {
            return 0;
        }

        public Globals.DamageEffect DamageDealtEffects()
        {
            return Globals.DamageEffect.None;
        }

        public int MeleeRangeModifier()
        {
            return 0;
        }

        public Globals.AttackType AttackModifier()
        {
            return Globals.AttackType.None;
        }

        public int DamageOverTime()
        {
            int DMG = 0;

            intervalTimer = intervalSW.ElapsedMilliseconds; 
            if (intervalTimer > tikTime)
            {
                intervalSW.Restart();
                intervalTimer = 0;

                DMG = damageSequence[damageIndex];
                damageIndex++;
            }

            return DMG;
        }


        public bool Expired()
        {

            return (totalSW.ElapsedMilliseconds > totalTime);
        }

    }
}
