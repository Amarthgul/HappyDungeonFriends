using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace HappyDungeon
{
    public class Misc
    {

        private static Misc instance = new Misc();
        public static Misc Instance
        {
            get
            {
                return instance;
            }
        }
        private Misc()
        {
        }


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

        public Vector2 PositionTranslate(int R, int C)
        {
            Vector2 FinalPos = new Vector2(0, 0);

            FinalPos.X = Globals.OUT_BORDER + C * Globals.OUT_UNIT;
            FinalPos.Y = Globals.OUT_HEADSUP + Globals.OUT_BORDER + R * Globals.OUT_UNIT;

            return FinalPos;
        }
    }
}
