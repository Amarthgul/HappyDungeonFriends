﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyDungeon
{
    public class DamageInstance
    {

        public int DamageCount;

        public Globals.DamageEffect[] effects;

        public double knowckbackDist = 0; 

        public DamageInstance(int DC, Globals.DamageEffect[] FX)
        {
            DamageCount = DC; 
            effects = FX;
        }

    }
}