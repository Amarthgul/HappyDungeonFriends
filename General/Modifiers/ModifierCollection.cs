using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyDungeon
{
    /// <summary>
    /// Modifier may affect the movement speed, damge output, damge taken, DoT
    /// </summary>
    class ModifierCollection
    {

        private List<General.Modifiers.IModifier> modifierList; 

        public ModifierCollection()
        {
            modifierList = new List<General.Modifiers.IModifier>();
        }

        public bool IsEmpty()
        {
            return modifierList.Count == 0; 
        }

        public int Regen()
        {
            int TotalRegen = 0;

            foreach(General.Modifiers.IModifier M in modifierList)
            {
                TotalRegen += M.Regen();
            }

            return TotalRegen;
        }

        public int MovementSpeedModifier()
        {
            int TotalRegen = 0;

            foreach (General.Modifiers.IModifier M in modifierList)
            {
                TotalRegen += M.Regen();
            }

            return TotalRegen;
        }

        public int DamageTakenModifier()
        {
            int ReductionBonus = 0;

            foreach (General.Modifiers.IModifier M in modifierList)
            {
                ReductionBonus += M.Regen();
            }

            return ReductionBonus;
        }

        public int DamageDealtModifer()
        {
            int DamageBonus = 0;

            foreach (General.Modifiers.IModifier M in modifierList)
            {
                DamageBonus += M.DamageDealtModifer();
            }

            return DamageBonus;
        }

        public int MeleeRangeModifier()
        {
            int RangeBonus = 0;

            foreach (General.Modifiers.IModifier M in modifierList)
            {
                RangeBonus += M.MeleeRangeModifier();
            }

            return RangeBonus;
        }

        public Globals.DamageEffect[] DamageDealtEffectModifer()
        {
            List<Globals.DamageEffect> EffectList = new List<Globals.DamageEffect>(); ;

            foreach (General.Modifiers.IModifier M in modifierList)
            {
                if(M.DamageDealtEffects() != Globals.DamageEffect.None)
                {
                    EffectList.Add(M.DamageDealtEffects());
                }
            }

            return EffectList.ToArray();
        }

        public int DamageOverTime()
        {
            int DoT = 0;

            foreach (General.Modifiers.IModifier M in modifierList)
            {
                DoT += M.Regen();
            }

            return DoT;
        }

        public Globals.AttackType AttackTypeModifier()
        {
            Globals.AttackType defaultType = Globals.AttackType.None;

            foreach (General.Modifiers.IModifier M in modifierList)
            {
                if (M.AttackModifier() != Globals.AttackType.None)
                    return M.AttackModifier();
            }

            return defaultType;
        }

        public void Update()
        {

        }

        public void Add(General.Modifiers.IModifier M)
        {
            modifierList.Add(M);
        }

        public void Remove(General.Modifiers.IModifier M)
        {
            modifierList.Remove(M);
        }

    }
}
