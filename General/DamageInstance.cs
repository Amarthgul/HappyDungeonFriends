﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;


namespace HappyDungeon
{
    public class DamageInstance
    {

        public int DamageCount;

        public Globals.DamageEffect[] effects; // Can have more than 1 damage effect

        public Globals.DamageType damageType;  // Cannot have more than 1 damage type

        public double knowckbackDist = 0;

        public Vector2 damageOrigin; 

        public DamageInstance(int DC, Globals.DamageEffect[] FX)
        {
            DamageCount = DC; 
            effects = FX;
        }

    }
}
