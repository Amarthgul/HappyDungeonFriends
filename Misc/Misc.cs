using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyDungeon
{
    public class Misc
    {



        /// <summary>
        /// Get the opposite direction. 
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public Globals.Direction Opposite(Globals.Direction Input)
        {
            switch (Input)
            {
                case Globals.Direction.Left:
                    return Globals.Direction.Right;
                case Globals.Direction.Right:
                    return Globals.Direction.Left;
                case Globals.Direction.Up:
                    return Globals.Direction.Down;
                case Globals.Direction.Down:
                    return Globals.Direction.Up;
                default:
                    return Globals.Direction.None;
            }
        }
    }
}
