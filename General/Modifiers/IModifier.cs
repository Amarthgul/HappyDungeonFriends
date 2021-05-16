using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyDungeon.General.Modifiers
{
    public interface IModifier
    {

        int Regen();

        int MovementSpeedModifier();

        int DamageTakenModifier();

        int DamageDealtModifer();

        int DamageOverTime();

        Globals.AttackType AttackModifier();

        int MeleeRangeModifier();

        bool Expired();



    }
}
