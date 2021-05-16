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
                DamageBonus += M.Regen();
            }

            return DamageBonus;
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

        public Globals.AttackType AttackModifier()
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
