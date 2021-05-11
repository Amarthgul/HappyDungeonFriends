using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


namespace HappyDungeon.UI
{
    public interface IText
    {
        Texture2D GetText(string Text, GraphicsDevice G);

    }
}
