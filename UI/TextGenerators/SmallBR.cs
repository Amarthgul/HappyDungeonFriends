using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace HappyDungeon.UI
{
    
    public class SmallBR : Small
    {

        
        public SmallBR()
        {
            sourceTexts = TextureFactory.Instance.fontSmallBR.texture;

            interval = 0;
            spaceSize = 3;
            widthOffset = 1;
            heightOffset = 1;

        }

       
    }

}

