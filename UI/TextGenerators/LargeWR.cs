using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


namespace HappyDungeon.UI
{
    /// <summary>
    /// Larger font with white rims
    /// </summary>
    public class LargeWR : Large
    {

        public LargeWR()
        {
            sourceTexts = TextureFactory.Instance.fontLargeWR.texture;

        }

    }
}
