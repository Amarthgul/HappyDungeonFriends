using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace HappyDungeon.UI
{
    
    public class SmallWR : Small
    {

        
        public SmallWR()
        {
            sourceTexts = TextureFactory.Instance.fontSmallWR.texture;

            interval = 0;
            spaceSize = 3;
            widthOffset = 1;
            heightOffset = 1;

        }

       
    }

}

