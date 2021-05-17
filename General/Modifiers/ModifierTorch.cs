using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace HappyDungeon.General.Modifiers
{
    class ModifierTorch : IModifier
    {

        public ModifierTorch()
        {

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
            return 5;
        }

        public Globals.DamageEffect DamageDealtEffects()
        {
            return Globals.DamageEffect.None;
        }

        public int MeleeRangeModifier()
        {
            return 8;
        }

        public Globals.AttackType AttackModifier()
        {
            return Globals.AttackType.Melee;
        }

        public int DamageOverTime()
        {

            return 0;
        }


        /// <summary>
        /// Torch never expired on it's own
        /// </summary>
        /// <returns>False always</returns>
        public bool Expired()
        {
            return false;
        }


    }
}
