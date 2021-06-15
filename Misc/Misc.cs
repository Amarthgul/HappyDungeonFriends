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
    /// <summary>
    /// Utility methods of many purposes. 
    /// </summary>
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
        /// <param name="Input">Direction to seek</param>
        /// <returns>The opposit direction</returns>
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

        
        // Splitting items, blocks, and enemies because some of the special cases
        // might require special treatment 

        /// <summary>
        /// Check if the block can be seen. 
        /// </summary>
        /// <param name="Block">Block to be checked</param>
        /// <param name="PlayerRect">Rectangle of the player</param>
        /// <param name="VisibleRange">Visible range of the fog</param>
        /// <returns>True if it can be seen</returns>
        public bool BlockFogBreaker(IBlock Block, Rectangle PlayerRect, float VisibleRange)
        {
            bool result = false;
            int Threshold = (int)(VisibleRange * Globals.OUT_UNIT);
            Rectangle ItemRect = Block.GetRectangle();
            Vector2 PlayerCenter = new Vector2(
                PlayerRect.X + PlayerRect.Width / 2,
                PlayerRect.Y + PlayerRect.Height / 2);

            foreach (float LocX in new float[] { ItemRect.X, ItemRect.X + ItemRect.Width })
            {
                foreach (float LocY in new float[] { ItemRect.Y, ItemRect.Y + ItemRect.Height })
                {
                    if (L2Distance(new Vector2(LocX, LocY), PlayerCenter) < Threshold)
                    {
                        return true;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Check if the item can be seen. 
        /// </summary>
        /// <param name="Item">Item to be checked</param>
        /// <param name="PlayerRect">Rectangle of the player</param>
        /// <param name="VisibleRange">Visible range of the fog</param>
        /// <returns>True if it can be seen</returns>
        public bool ItemFogBreaker(IItem Item, Rectangle PlayerRect, float VisibleRange)
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

        /// <summary>
        /// Check if an enemy can be seen. 
        /// </summary>
        /// <param name="Enemy">Enemy to be checked</param>
        /// <param name="PlayerRect">Rectangle of the player</param>
        /// <param name="VisibleRange">Visible range of the fog</param>
        /// <returns>True if it can be seen</returns>
        public bool EnemyFogBreaker(IEnemy Enemy, Rectangle PlayerRect, float VisibleRange)
        {
            bool result = false;
            int Threshold = (int)(VisibleRange * Globals.OUT_UNIT);
            Rectangle EnemyRect = Enemy.GetRectangle();
            Vector2 PlayerCenter = new Vector2(
                PlayerRect.X + PlayerRect.Width / 2,
                PlayerRect.Y + PlayerRect.Height / 2);

            foreach (float LocX in new float[] { EnemyRect.X, EnemyRect.X + EnemyRect.Width })
            {
                foreach (float LocY in new float[] { EnemyRect.Y, EnemyRect.Y + EnemyRect.Height })
                {
                    if (L2Distance(new Vector2(LocX, LocY), PlayerCenter) < Threshold)
                    {
                        return true;
                    }
                }
            }

            return result;
        }

        public bool ProjectileFogBreaker(IProjectile Proj, Rectangle PlayerRect, float VisibleRange)
        {
            bool result = false;
            int Threshold = (int)(VisibleRange * Globals.OUT_UNIT);
            Rectangle ProjRect;

            // melee attack can be seen as along as the enemy can be seen 
            if (Proj.IsMelee())
                ProjRect = Proj.GetSrcRectangle();
            else 
                ProjRect = Proj.GetRectangle();

            Vector2 PlayerCenter = new Vector2(
                PlayerRect.X + PlayerRect.Width / 2,
                PlayerRect.Y + PlayerRect.Height / 2);

            foreach (float LocX in new float[] { ProjRect.X, ProjRect.X + ProjRect.Width })
            {
                foreach (float LocY in new float[] { ProjRect.Y, ProjRect.Y + ProjRect.Height })
                {
                    if (L2Distance(new Vector2(LocX, LocY), PlayerCenter) < Threshold)
                    {
                        return true;
                    }
                }
            }

            return result;
        }

        // ================================================================================
        // ==================================== Misc ======================================
        // ================================================================================

        /// <summary>
        /// Translate the tile position of row and column to the absolute position
        /// in the game window, as in pixel locations. 
        /// </summary>
        /// <param name="R">Row</param>
        /// <param name="C">Column</param>
        /// <returns>Vector 2 of the position in screen</returns>
        public Vector2 PositionTranslate(int R, int C)
        {
            Vector2 FinalPos = new Vector2(0, 0);

            FinalPos.X = Globals.OUT_BORDER + C * Globals.OUT_UNIT;
            FinalPos.Y = Globals.OUT_HEADSUP + Globals.OUT_BORDER + R * Globals.OUT_UNIT;

            return FinalPos;
        }

        /// <summary>
        /// Given a position, reverse translate it into the column and row tile index.
        /// In relative only to the room area, i.e. 2 offsets on all sides. 
        /// </summary>
        /// <param name="AbsPos">Absolute position in screen</param>
        /// <returns>Tile index of row and col</returns>
        public int[] PositionReverse(Vector2 AbsPos)
        {
            int[] Rev = new int[] { 0, 0 };

            Rev[0] = (int)((AbsPos.X - Globals.OUT_BORDER) / Globals.OUT_UNIT);
            Rev[1] = (int)((AbsPos.Y - Globals.OUT_BORDER - Globals.OUT_HEADSUP) / Globals.OUT_UNIT);

            return Rev;
        }

        /// <summary>
        /// Euclidean distance between 2 points.
        /// </summary>
        /// <param name="P1">Point 1</param>
        /// <param name="P2">Point 2</param>
        /// <returns>Euclidean distance between point 1 and 2</returns>
        public int L2Distance(Vector2 P1, Vector2 P2)
        {
            return (int)Math.Sqrt(Math.Pow((P1.X - P2.X), 2) + Math.Pow((P1.Y - P2.Y), 2));
        }

        public int L1Distance(Vector2 P1, Vector2 P2)
        {
            Vector2 Total = P1 - P2; 
            return (int)(Math.Abs(Total.X) + Math.Abs(Total.Y)) ;
        }
    }
}
