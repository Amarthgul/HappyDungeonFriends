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

        public bool ItemGofBreaker(IItem Item, Rectangle PlayerRect, float VisibleRange)
        {
            bool result = false;
            int Threshold = (int)(VisibleRange * Globals.OUT_UNIT); 
            Rectangle ItemRect = Item.GetRectangle();
            Vector2 PlayerCenter = new Vector2(
                PlayerRect.X + PlayerRect.Width / 2, 
                PlayerRect.Y + PlayerRect.Height / 2);

            foreach(float LocX in new float[] {ItemRect.X, ItemRect.X + ItemRect.Width })
            {
                foreach(float LocY in new float[] { ItemRect.Y, ItemRect.Y + ItemRect.Height })
                {
                    if (L2Distance(new Vector2(LocX, LocY), PlayerCenter) < Threshold)
                    {
                        return true;
                    }
                }
            }


            return result; 
        }

        public Vector2 PositionTranslate(int R, int C)
        {
            Vector2 FinalPos = new Vector2(0, 0);

            FinalPos.X = Globals.OUT_BORDER + C * Globals.OUT_UNIT;
            FinalPos.Y = Globals.OUT_HEADSUP + Globals.OUT_BORDER + R * Globals.OUT_UNIT;

            return FinalPos;
        }

        private int L2Distance(Vector2 P1, Vector2 P2)
        {
            return (int)Math.Sqrt(Math.Pow((P1.X - P2.X), 2) + Math.Pow((P1.Y - P2.Y), 2));
        }
    }
}
