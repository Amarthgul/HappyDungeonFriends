using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace HappyDungeon.General.Modifiers
{
    /// <summary>
    /// Nullify completely negate an incoming damage instance, 
    /// removing all damage and damage effects from it. 
    /// </summary>
    class ModifierNullify : IModifier
    {

        public ModifierNullify(int TotalDMG)
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
            return 0; 
        }


        public bool Expired()
        {

            return false;
        }

    }
}
