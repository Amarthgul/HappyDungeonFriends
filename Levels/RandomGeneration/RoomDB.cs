using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyDungeon.Levels
{
    /// <summary>
    /// All the arrangements presets 
    /// </summary>
    public class RoomDB
    {
        public bool[,] canPlaceDirect = new bool[,] {
            { true , true , true , true , true , false, false, true , true , true , true , true},
            { true , true , true , true , true , true , true , true , true , true , true , true},
            { true , true , true , true , true , true , true , true , true , true , true , true},
            { false, true , true , true , true , true , true , true , true , true , true , false},
            { true , true , true , true , true , true , true , true , true , true , true , true},
            { true , true , true , true , true , true , true , true , true , true , true , true},
            { true , true , true , true , true , false, false, true , true , true , true , true}
        };

        public bool[,] corners = new bool[,] {
            { true , false, false, false, false, false, false, false, false, false, false, true},
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { true , false, false, false, false, false, false, false, false, false, false, true}
        };

        public bool[,] cornerBig = new bool[,] {
            { true , true , false, false, false, false, false, false, false, false, true , true },
            { true , false, false, false, false, false, false, false, false, false, false, true },
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { true , false, false, false, false, false, false, false, false, false, false, true },
            { true , true , false, false, false, false, false, false, false, false, true , true }
        };


        public bool[,] midOval = new bool[,] {
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, false, false, false, false, true , true , false, false, false, false, false},
            { false, false, false, false, true , true , true , true , false, false, false, false},
            { false, false, false, false, true , true , true , true , false, false, false, false},
            { false, false, false, false, true , true , true , true , false, false, false, false},
            { false, false, false, false, false, true , true , false, false, false, false, false},
            { false, false, false, false, false, false, false, false, false, false, false, false}
        };

        public bool[,] cross = new bool[,] {
            { false, false, false, false, false, true , true , false, false, false, false, false},
            { false, false, false, false, false, true , true , false, false, false, false, false},
            { false, false, false, false, false, true , true , false, false, false, false, false},
            { true , true , true , true , true , true , true , true , true , true , true , true},
            { false, false, false, false, false, true , true , false, false, false, false, false},
            { false, false, false, false, false, true , true , false, false, false, false, false},
            { false, false, false, false, false, true , true , false, false, false, false, false}
        };

        public bool[,] maskUp = new bool[,] {
            { false, false, false, false, false, true , true , false, false, false, false, false},
            { false, false, false, false, false, true , true , false, false, false, false, false},
            { false, false, false, false, false, true , true , false, false, false, false, false},
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, false, false, false, false, false, false, false, false, false, false, false}
        };

        public bool[,] maskDown = new bool[,] {
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, false, false, false, false, true , true , false, false, false, false, false},
            { false, false, false, false, false, true , true , false, false, false, false, false},
            { false, false, false, false, false, true , true , false, false, false, false, false}
        };

        public bool[,] maskLeft = new bool[,] {
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { true , true , true , true , true , false, false, false, false, false, false, false},
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, false, false, false, false, false, false, false, false, false, false, false}
        };

        public bool[,] maskRight = new bool[,] {
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, false, false, false, false, false, false, true , true , true , true , true },
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, false, false, false, false, false, false, false, false, false, false, false}
        };

        public bool[,] cornerDust = new bool[,] {
            { true , true , true , false, false, false, false, false, false, true , true , true},
            { true , true , false, false, false, false, false, false, false, false, true , true},
            { true , false, false, false, false, false, false, false, false, false, false, true},
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { true , false, false, false, false, false, false, false, false, false, false, true},
            { true , true , false, false, false, false, false, false, false, false, true , true},
            { true , true , true , false, false, false, false, false, false, true , true , true}
        };

        public bool[,] grid = new bool[,] {
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, true , false, true , false, true , true , false, true , false, true , false},
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, true , false, true , false, true , true , false, true , false, true , false},
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, true , false, true , false, true , true , false, true , false, true , false},
            { false, false, false, false, false, false, false, false, false, false, false, false}
        };

        public bool[,] square = new bool[,] {
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, false, false, false, false, true , true , false, false, false, false, false},
            { false, false, false, false, false, true , true , false, false, false, false, false},
            { false, false, false, false, false, true , true , false, false, false, false, false},
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, false, false, false, false, false, false, false, false, false, false, false}
        };

        public bool[,] doubleRectangles = new bool[,] {
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, true , true , false, false, false, false, false, false, true , true , false},
            { false, true , true , false, false, false, false, false, false, true , true , false},
            { false, true , true , false, false, false, false, false, false, true , true , false},
            { false, true , true , false, false, false, false, false, false, true , true , false},
            { false, true , true , false, false, false, false, false, false, true , true , false},
            { false, false, false, false, false, false, false, false, false, false, false, false}
        };

        public bool[,] maze = new bool[,] {
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, true , false, true , true , true , false, true , true , true , true , false},
            { false, true , false, false, false, true , false, true , false, false, false, false},
            { false, true , false, true , false, true , true , true , false, true , true , false},
            { false, true , false, true , false, true , false, true , false, false, false, false},
            { false, true , true , true , false, true , false, true , true , true , true , false},
            { false, false, false, false, false, false, false, false, false, false, false, false}
        };

        public bool[,] pipe = new bool[,] {
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, true , true , true , true , true , true , true , true , true , true , false},
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, true , true , true , true , true , true , true , true , true , true , false},
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, true , true , true , true , true , true , true , true , true , true , false},
            { false, false, false, false, false, false, false, false, false, false, false, false}
        };

        public bool[,] merchantRoom = new bool[,] {
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, false, false, true , false, false, true , false, false, true , false, false},
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, false, false, false, false, false, false, false, false, false, false, false}
        };

        public bool[,] treasure = new bool[,] {
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, false, false, false, false, true , true , false, false, false, false, false},
            { false, false, false, false, true , true , true , true , false, false, false, false},
            { false, false, false, true , true , true , true , true , true , false, false, false},
            { false, false, false, false, true , true , true , true , false, false, false, false},
            { false, false, false, false, false, true , true , false, false, false, false, false},
            { false, false, false, false, false, false, false, false, false, false, false, false}
        };

        public bool[,] shihonzuki = new bool[,] {
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, false, false, false, true , false, false, true , false, false, false, false},
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, false, false, false, true , false, false, true , false, false, false, false},
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, false, false, false, false, false, false, false, false, false, false, false}
        };

        public bool[,] allFalse = new bool[,] {
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, false, false, false, false, false, false, false, false, false, false, false}
        };

        public RoomDB()
        {

        }


        /// <summary>
        /// Generate a bool map of the 4 corners, the size is decided by the weight. 
        /// </summary>
        /// <param name="Weight">1-5 of the size of the corner</param>
        /// <returns>Bool map of the 4 corners</returns>
        public bool[,] CornersWeighted(int Weight)
        {
            bool[,] result = (bool[,])allFalse.Clone();

            for (int i = 0; i < Globals.RTILE_ROW; i++)
            {
                for (int j = 0; j < Globals.RTILE_COLUMN; j++)
                {
                    int RowWeightIndex = Math.Min(i, (Globals.RTILE_ROW - 1) - i);
                    int ColWeightIndex = Math.Min(j, (Globals.RTILE_COLUMN - 1) - j);
                    if ((RowWeightIndex + ColWeightIndex) < Weight)
                    {
                        result[i, j] = true; 
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Given the door position, cut part from the cross and make it a proper path pattern. 
        /// </summary>
        /// <param name="Doors">Indication of whether a door exists in that direction</param>
        /// <returns>Cross pattern subtracted</returns>
        public bool[,] MaskedPath(bool[] Doors)
        { 
            bool[,] result = (bool[,])cross.Clone();

            if (!Doors[0]) result = Subtract(maskLeft, result);
            if (!Doors[1]) result = Subtract(maskRight, result);
            if (!Doors[2]) result = Subtract(maskUp, result);
            if (!Doors[3]) result = Subtract(maskDown, result);

            return result; 
        }

        /// <summary>
        /// Subtract one matrix from another. This is not XOR.
        /// Does not change any of the input matrices.
        /// </summary>
        /// <param name="m1">Substractor</param>
        /// <param name="m2">Base</param>
        /// <returns>Bool matrix of m2 - m1</returns>
        public bool[,] Subtract(bool[,] m1, bool[,] m2)
        {
            bool[,] result = new bool[Globals.RTILE_ROW, Globals.RTILE_COLUMN];
            for (int i = 0; i < Globals.RTILE_ROW; i++)
            {
                for (int j = 0; j < Globals.RTILE_COLUMN; j++)
                {
                    result[i, j] = m1[i, j] ? false : m2[i, j];
                }
            }
            return result;
        }

        /// <summary>
        /// Generate a bool map with random true and false.
        /// </summary>
        /// <param name="Ratio">Ratio of truth, 0.0-1.0</param>
        /// <returns>Map of bool</returns>
        public bool[,] RandomFill(double Ratio)
        {
            int Possibility = (int)(Ratio * 100);
            bool[,] result = (bool[,])allFalse.Clone();

            for (int i = 0; i < Globals.RTILE_ROW; i++)
            {
                for (int j = 0; j < Globals.RTILE_COLUMN; j++)
                {
                    if (Globals.RND.Next(100) < Possibility)
                        result[i, j] = true; 
                }
            }
            return result;
        }

        public bool[,] AND(bool[,] m1, bool[,] m2)
        {
            bool[,] result = new bool[Globals.RTILE_ROW, Globals.RTILE_COLUMN];
            for (int i = 0; i < Globals.RTILE_ROW; i++)
            {
                for (int j = 0; j < Globals.RTILE_COLUMN; j++)
                {
                    result[i, j] = m1[i, j] && m2[i, j];
                }
            }
            return result;
        }

        public bool[,] OR(bool[,] m1, bool[,] m2)
        {
            bool[,] result = new bool[Globals.RTILE_ROW, Globals.RTILE_COLUMN];
            for (int i = 0; i < Globals.RTILE_ROW; i++)
            {
                for (int j = 0; j < Globals.RTILE_COLUMN; j++)
                {
                    result[i, j] = m1[i, j] || m2[i, j];
                }
            }
            return result;
        }

        public bool[,] NOT(bool[,] m1)
        {
            bool[,] result = new bool[Globals.RTILE_ROW, Globals.RTILE_COLUMN];
            for (int i = 0; i < Globals.RTILE_ROW; i++)
            {
                for (int j = 0; j < Globals.RTILE_COLUMN; j++)
                {
                    result[i, j] = ! m1[i, j];
                }
            }
            return result;
        }
    }
}
