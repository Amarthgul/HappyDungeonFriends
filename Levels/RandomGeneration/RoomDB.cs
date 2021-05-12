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

        // Room matrices 
        public static bool[,] canPlace = new bool[,] {
            { true , true , true , true , true , false, false, true , true , true , true , true},
            { true , true , true , true , true , true , true , true , true , true , true , true},
            { true , true , true , true , true , true , true , true , true , true , true , true},
            { false, true , true , true , true , true , true , true , true , true , true , false},
            { true , true , true , true , true , true , true , true , true , true , true , true},
            { true , true , true , true , true , true , true , true , true , true , true , true},
            { true , true , true , true , true , false, false, true , true , true , true , true}
        };

        public static bool[,] corners = new bool[,] {
            { true , false, false, false, false, false, false, false, false, false, false, true},
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { true , false, false, false, false, false, false, false, false, false, false, true}
        };

        public static bool[,] cornerBig = new bool[,] {
            { true , true , false, false, false, false, false, false, false, false, true , true },
            { true , false, false, false, false, false, false, false, false, false, false, true },
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { true , false, false, false, false, false, false, false, false, false, false, true },
            { true , true , false, false, false, false, false, false, false, false, true , true }
        };


        public static bool[,] midOval = new bool[,] {
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, false, false, false, false, true , true , false, false, false, false, false},
            { false, false, false, false, true , true , true , true , false, false, false, false},
            { false, false, false, false, true , true , true , true , false, false, false, false},
            { false, false, false, false, true , true , true , true , false, false, false, false},
            { false, false, false, false, false, true , true , false, false, false, false, false},
            { false, false, false, false, false, false, false, false, false, false, false, false}
        };

        public static bool[,] cross = new bool[,] {
            { false, false, false, false, false, true , true , false, false, false, false, false},
            { false, false, false, false, false, true , true , false, false, false, false, false},
            { false, false, false, false, false, true , true , false, false, false, false, false},
            { true , true , true , true , true , true , true , true , true , true , true , true},
            { false, false, false, false, false, true , true , false, false, false, false, false},
            { false, false, false, false, false, true , true , false, false, false, false, false},
            { false, false, false, false, false, true , true , false, false, false, false, false}
        };

        public static bool[,] cornerDust = new bool[,] {
            { true , true , true , false, false, false, false, false, false, true , true , true},
            { true , true , false, false, false, false, false, false, false, false, true , true},
            { true , false, false, false, false, false, false, false, false, false, false, true},
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { true , false, false, false, false, false, false, false, false, false, false, true},
            { true , true , false, false, false, false, false, false, false, false, true , true},
            { true , true , true , false, false, false, false, false, false, true , true , true}
        };

        public static bool[,] grid = new bool[,] {
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, true , false, true , false, true , true , false, true , false, true , false},
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, true , false, true , false, true , true , false, true , false, true , false},
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, true , false, true , false, true , true , false, true , false, true , false},
            { false, false, false, false, false, false, false, false, false, false, false, false}
        };

        public static bool[,] square = new bool[,] {
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, false, false, false, false, true , true , false, false, false, false, false},
            { false, false, false, false, false, true , true , false, false, false, false, false},
            { false, false, false, false, false, true , true , false, false, false, false, false},
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, false, false, false, false, false, false, false, false, false, false, false}
        };

        public static bool[,] doubleRectangles = new bool[,] {
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, true , true , false, false, false, false, false, false, true , true , false},
            { false, true , true , false, false, false, false, false, false, true , true , false},
            { false, true , true , false, false, false, false, false, false, true , true , false},
            { false, true , true , false, false, false, false, false, false, true , true , false},
            { false, true , true , false, false, false, false, false, false, true , true , false},
            { false, false, false, false, false, false, false, false, false, false, false, false}
        };

        public static bool[,] maze = new bool[,] {
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, true , false, true , true , true , false, true , true , true , true , false},
            { false, true , false, false, false, true , false, true , false, false, false, false},
            { false, true , false, true , false, true , true , true , false, true , true , false},
            { false, true , false, true , false, true , false, true , false, false, false, false},
            { false, true , true , true , false, true , false, true , true , true , true , false},
            { false, false, false, false, false, false, false, false, false, false, false, false}
        };

        public static bool[,] pipe = new bool[,] {
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, true , true , true , true , true , true , true , true , true , true , false},
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, true , true , true , true , true , true , true , true , true , true , false},
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, true , true , true , true , true , true , true , true , true , true , false},
            { false, false, false, false, false, false, false, false, false, false, false, false}
        };

        public static bool[,] merchantRoom = new bool[,] {
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, false, false, true , false, false, true , false, false, true , false, false},
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, false, false, false, false, false, false, false, false, false, false, false}
        };

        public static bool[,] treasure = new bool[,] {
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, false, false, false, false, true , true , false, false, false, false, false},
            { false, false, false, false, true , true , true , true , false, false, false, false},
            { false, false, false, true , true , true , true , true , true , false, false, false},
            { false, false, false, false, true , true , true , true , false, false, false, false},
            { false, false, false, false, false, true , true , false, false, false, false, false},
            { false, false, false, false, false, false, false, false, false, false, false, false}
        };

        public static bool[,] allFalse = new bool[,] {
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, false, false, false, false, false, false, false, false, false, false, false},
            { false, false, false, false, false, false, false, false, false, false, false, false}
        };
    }
}
