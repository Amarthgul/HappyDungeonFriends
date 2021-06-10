﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


namespace HappyDungeon
{
    public interface IProjectile
    {

        void Update();

        void Draw();

        Rectangle GetRectangle();

        DamageInstance GetDamageInstance();
    }
}
