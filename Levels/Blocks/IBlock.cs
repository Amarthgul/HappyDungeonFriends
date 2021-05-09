using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace HappyDungeon
{
    public interface IBlock
    {
        void Update();
        void Draw();
        Rectangle GetRectangle();
    }
}
