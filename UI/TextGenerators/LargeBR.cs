﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


namespace HappyDungeon.UI
{
    /// <summary>
    /// Larger font with black rims
    /// </summary>
    public class LargeBR : Large
    {

        public LargeBR()
        {
            sourceTexts = TextureFactory.Instance.fontLargeBR.texture;

        }

    }
}
