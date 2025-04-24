using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HappyDungeon
{
    class GenerateRoomJoy : Levels.GenerateRoomBasics
    {

        private bool _DEVMODE = true;

        // ================================================================================
        // ======================== Consts and frequently used ============================
        // ================================================================================

        private const int JOY_ENEMY_MAX = 5;

        // Path number variables 
        private int[] full = new int[] {48, 49, 50, 51, 52, 53, 54, 55 };
        private int[] oppositeLR = new int[] {100, 101, 102, 103 };
        private int[] oppositeUD = new int[] {96, 96, 98, 99 };
        private int[] turnLU = new int[] {80, 81, 82, 83 }; // Turn connecting left and up 
        private int[] turnLD = new int[] {84, 85, 86, 87 }; // Turn connecting left and down 
        private int[] turnRU = new int[] {88, 89, 90, 91 }; // Turn connecting right and up 
        private int[] turnRD = new int[] {92, 93, 94, 95 }; // Turn connecting right and down 
        private int[] bankR = new int[] {64, 65, 66, 67 };  // Bank at left, connecting up and down  
        private int[] bankL = new int[] {68, 69, 70, 71 };  // Bank at right, connecting up and down  
        private int[] bankD = new int[] {72, 73, 74, 75 };  // Bank at up, connecting left and right  
        private int[] bankU = new int[] {76, 77, 78, 79 };  // Bank at down, connecting left and right
        private int[] endL = new int[] {104, 105 };
        private int[] endR = new int[] {106, 107 };
        private int[] endU = new int[] {108, 109 };
        private int[] endD = new int[] {110, 111 };

        public int[] tileList { get; set; } 


        /// <summary>
        /// Things put here will stay true for all level Joy rooms. 
        /// </summary>
        public GenerateRoomJoy()
        {
            // Setup the template 

            enemyList = new int[]{
                Globals.ENEMY_BEAD
            };
            itemList = new int[] {

            };
            merchantItems = new int[] {

            };
            merchantCharaList = new int[] {

            };

            defaultBlock = 176;
            tileList = new int[] { 33, 34, 35, 36 };
            walkableBlockList = new int[] { 96 };
            solidBlockLIst = new int[] { 128, 144, 160, 161 };
            blackRoomInedx = 1;
            //bossIndex = Globals.BOSS_ENEMY;
        }


        /// <summary>
        /// Fill the room info with placeholders, all disabled by default; 
        /// </summary>
        /// <returns></returns>
        public RoomInfo InitRoom()
        {

            RoomInfo template = new RoomInfo();

            template.DefaultBlock = defaultBlock;
            template.Type = Globals.RoomTypes.Normal;

            template.Arrangement = new int[Globals.RTILE_ROW_EXT, Globals.RTILE_COLUMN_EXT];
            template.PathTile = new bool[Globals.RTILE_ROW_EXT, Globals.RTILE_COLUMN_EXT];

            for (int i = 0; i < template.Arrangement.GetLength(0); i++)
                for (int j = 0; j < template.Arrangement.GetLength(1); j++)
                    template.Arrangement[i, j] = defaultBlock;


            template.LockedDoors = new bool[] { false, false, false, false };
            template.MysteryDoors = new bool[] { false, false, false, false };
            template.OpenDoors = new bool[] { false, false, false, false };
            template.Holes = new bool[] { false, false, false, false };

            room = template;

            return template;
        }


        public void GenerateTilePlacement(RoomInfo[,] levelSet)
        {
            PathGeneration(levelSet);


            // Iterate through the tiles, assign them with proper tile texture indices 
            for (int row = 0; row < Globals.RTILE_ROW_EXT; row++)
            {
                for (int col = 0; col < Globals.RTILE_COLUMN_EXT; col++)
                {
                    if(room.PathTile[row, col])
                    {
                        List<bool> neighborType = GetNeighborPath(row, col);
                        // Bool in order of Direction { Left, Right, Up, Down };

                        int tileIndex = PathTileNumberSelection(neighborType);


                        room.Arrangement[row, col] = tileIndex; 

                    }
                }
            }



        }

        // ================================================================================
        // ================================ Private methods ===============================
        // ================================================================================


        /// <summary>
        /// neighbourPath is a 4-bool list ordered { Left, Right, Up, Down }.
        /// • 1 connection  → “end”  sprite in the matching direction.
        /// • 2 connections → if opposite  use “opposite”, otherwise use the right corner “turn”.
        /// • 3 connections → use a “bank” (T-junction) whose missing side is the only false entry.
        /// • 4 connections → use a “full” (cross).
        /// The method returns one random frame from the corresponding array.
        /// </summary>
        private int PathTileNumberSelection(List<bool> neighborPath)
        {
            // Fallback if the input is bad.
            if (neighborPath == null || neighborPath.Count != 4)
                return full[Globals.RND.Next(full.Length)];

            int connectionCount = neighborPath.Count(b => b);
            int[] pool;

            switch (connectionCount)
            {
                case 1:   // Dead-end
                    pool = neighborPath[0] ? endL :
                           neighborPath[1] ? endR :
                           neighborPath[2] ? endU : endD;
                    break;

                case 2:   // Straight or corner
                    if (neighborPath[0] && neighborPath[1])           // Left & Right
                        pool = oppositeLR;
                    else if (neighborPath[2] && neighborPath[3])      // Up & Down
                        pool = oppositeUD;
                    else if (neighborPath[0] && neighborPath[2])      // Left & Up
                        pool = turnLU;
                    else if (neighborPath[0] && neighborPath[3])      // Left & Down
                        pool = turnLD;
                    else if (neighborPath[1] && neighborPath[2])      // Right & Up
                        pool = turnRU;
                    else                                              // Right & Down
                        pool = turnRD;
                    break;

                case 3:   // T-junction  (bank)
                    pool = !neighborPath[0] ? bankL :
                           !neighborPath[1] ? bankR :
                           !neighborPath[2] ? bankU : bankD;
                    break;

                case 4:   // Cross
                    pool = full;
                    break;

                default:  // Isolated tile (shouldn’t happen) – fall back to full
                    pool = full;
                    break;
            }

            return pool[Globals.RND.Next(pool.Length)];
        }



        /// <summary>
        /// Randomly select walkable and non-walkable tiles. 
        /// </summary>
        private void RandomPlacement()
        {
            for (int row = 0; row < Globals.RTILE_ROW_EXT; row++)
            {
                for (int col = 0; col < Globals.RTILE_COLUMN_EXT; col++)
                {
                    room.PathTile[row, col] = (Globals.RND.NextDouble() > 0.5f);
                }
            }

        }


        /// <summary>
        /// Main method for generating the path area in the room. 
        /// </summary>
        /// <param name="levelSet"></param>
        private void PathGeneration(RoomInfo[,] levelSet)
        {

            DirectPathCreation();

            // For the L corners, bevel them 
            BevelPathTile();

            JitterPath();

            ConvolveGrow();

            

        }


        /// <summary>
        /// Apply convolution additively to expand the path area. 
        /// </summary>
        /// <param name="iteration">Number of iterations. The more iteration, the bigger the growth.</param>
        private void ConvolveGrow(int iteration = 4)
        {
            double[,] mapArray = MathUtil.BoolToDouble(room.PathTile);

            //double[,] kernel = {
            //    { 1.0 / 16, 3.0 / 16, 1.0 / 16 },
            //    { 3.0 / 16, 0   / 16, 3.0 / 16 },
            //    { 1.0 / 16, 3.0 / 16, 1.0 / 16 }
            //};
            double[,] kernel = {
                { 1.0 / 16, 1.0 / 16, 4.0 / 16 },
                { 1.0 / 16, 0   / 16, 3.0 / 16 },
                { 1.0 / 16, 1.0 / 16, 4.0 / 16 }
            };
            double[,] result = new double[mapArray.GetLength(0), mapArray.GetLength(1)]; 

            for (int i = 0; i < iteration; i++)
            {
                result = MathUtil.Convolve2D(result, kernel);
                result = MathUtil.ArrayAdd(result, mapArray);
            }


            room.PathTile = MathUtil.DoubleToBool(result); 


        }



        /// <summary>
        /// Add jitter onto the rigid path, this also tends to increase the thickness a little. 
        /// </summary>
        /// <param name="iteration">Number of iterations. More iteration creates more jitter.</param>
        private void JitterPath(int iteration = 4)
        {
            bool horizontalOpen = room.OpenDoors[0] ^ room.OpenDoors[1];
            bool verticalOpen = room.OpenDoors[2] ^ room.OpenDoors[3];

            if (horizontalOpen && verticalOpen)
            {
                // Skip the room if it's L corner 
                return;
            }

            int centerRow = Globals.RTILE_ROW_EXT / 2;
            int centerCol = Globals.RTILE_COLUMN_EXT / 2;

            for (int i = 0;  i < iteration; i++)
            {
                ShiftRows(centerRow, centerCol);
                ShiftCols(centerRow, centerCol);
            }

        }


        /// <summary>
        /// This method creates the direct path connecting open rooms. 
        /// For example, if a room has a neighbor up and right, then it will create an L pattern. 
        /// </summary>
        private void DirectPathCreation()
        {
            // Clear the matrix 
            for (int row = 0; row < Globals.RTILE_ROW_EXT; row++)
                for (int col = 0; col < Globals.RTILE_COLUMN_EXT; col++)
                    room.PathTile[row, col] = false;


            // Find the center point that will be used as an anchor 
            int centerRow = Globals.RTILE_ROW_EXT / 2;
            int centerCol = Globals.RTILE_COLUMN_EXT / 2;
            room.PathTile[centerRow, centerCol] = true;

            // Left: Fill from the center to the left edge.
            if (room.OpenDoors[(int)Globals.Direction.Left])
            {
                for (int col = 0; col <= centerCol; col++)
                {
                    room.PathTile[centerRow, col] = true;
                }
            }

            // Right: Fill from the center to the right edge.
            if (room.OpenDoors[(int)Globals.Direction.Right])
            {
                for (int col = centerCol; col < Globals.RTILE_COLUMN_EXT; col++)
                {
                    room.PathTile[centerRow, col] = true;
                }
            }

            // Up: Fill from the center to the top edge.
            if (room.OpenDoors[(int)Globals.Direction.Up])
            {
                for (int row = 0; row <= centerRow; row++)
                {
                    room.PathTile[row, centerCol] = true;
                }
            }

            // Down: Fill from the center to the bottom edge.
            if (room.OpenDoors[(int)Globals.Direction.Down])
            {
                for (int row = centerRow; row < Globals.RTILE_ROW_EXT; row++)
                {
                    room.PathTile[row, centerCol] = true;
                }
            }


        }


        /// <summary>
        /// Perform Gaussian blur on the bool array and re-convert it back to boolean. 
        /// This will most likely expend the path and make it less straight along the L shaped corners. 
        /// </summary>
        /// <param name="threshold">Threshold above which shall be converted to true</param>
        private void BevelPathTile(int smoothStep = 4)
        {
            // Check for exactly one horizontal and one vertical opening.
            // (Using exclusive or to ensure only one door per axis is open.)
            bool horizontalOpen = room.OpenDoors[0] ^ room.OpenDoors[1];
            bool verticalOpen = room.OpenDoors[2] ^ room.OpenDoors[3];

            if (!(horizontalOpen && verticalOpen))
            {
                // Do not apply bevel if the openings are not exactly one horizontal and one vertical.
                return;
            }

            // Clear the matrix 
            for (int row = 0; row < Globals.RTILE_ROW_EXT; row++)
                for (int col = 0; col < Globals.RTILE_COLUMN_EXT; col++)
                    room.PathTile[row, col] = false;

            
            int horizontalSign = room.OpenDoors[(int)Globals.Direction.Left] ? 1 : -1;
            int verticalSign = room.OpenDoors[(int)Globals.Direction.Up] ? -1 : 1;

            int centerRow = Globals.RTILE_ROW_EXT / 2;
            int centerCol = Globals.RTILE_COLUMN_EXT / 2;

            if(smoothStep == 0)
            {   // If there is no smooth, fill the center line 
                room.PathTile[centerRow, centerCol] = true;
            }

            // Left: Fill from the center to the left edge.
            if (room.OpenDoors[(int)Globals.Direction.Left])
            {
                for (int col = 0; col < centerCol; col++)
                {
                    int distFromCtr = Math.Abs(centerCol - col);
                    int offset = (smoothStep >= distFromCtr) ? smoothStep - distFromCtr : 0;

                    if (smoothStep >= distFromCtr)
                    {
                        int compensate = offset > 0 ? -verticalSign : 0; 
                        room.PathTile[centerRow + verticalSign * offset, col] = true;
                        room.PathTile[centerRow + verticalSign * offset + compensate, col] = true;
                    }
                    else
                    {
                        room.PathTile[centerRow, col] = true;
                    }
 
                }
            }

            // Right: Fill from the center to the right edge.
            if (room.OpenDoors[(int)Globals.Direction.Right])
            {
                for (int col = centerCol+1; col < Globals.RTILE_COLUMN_EXT; col++)
                {
                    int distFromCtr = Math.Abs(centerCol - col);
                    int offset = (smoothStep >= distFromCtr) ? smoothStep - distFromCtr : 0;

                    if (smoothStep >= distFromCtr)
                    {
                        int compensate = offset > 0 ? -verticalSign : 0;
                        room.PathTile[centerRow + verticalSign * offset, col] = true;
                        room.PathTile[centerRow + verticalSign * offset + compensate, col] = true;
                    }
                    else
                    {
                        room.PathTile[centerRow, col] = true;
                    }
                }
            }

            // Up: Fill from the center to the top edge.
            if (room.OpenDoors[(int)Globals.Direction.Up])
            {
                for (int row = 0; row <= centerRow + 1 - smoothStep; row++)
                {
                    room.PathTile[row, centerCol] = true;
                }
            }

            // Down: Fill from the center to the bottom edge.
            if (room.OpenDoors[(int)Globals.Direction.Down])
            {
                for (int row = centerRow+smoothStep-1; row < Globals.RTILE_ROW_EXT; row++)
                {
                    room.PathTile[row, centerCol] = true;
                }
            }

        }
        

        /// <summary>
        /// Shifting some random vertical patterns to the left or right. 
        /// </summary>
        /// <param name="ctrRow">Index for the center row</param>
        /// <param name="ctrCol">Index for the center column</param>
        private void ShiftRows(int ctrRow, int ctrCol)
        {
            int rangeRow1 = Globals.RND.Next(0, Globals.RTILE_ROW_EXT);
            int rangeRow2 = Globals.RND.Next(0, Globals.RTILE_ROW_EXT);

            if (rangeRow2 < rangeRow1)
                Misc.Swap(ref rangeRow1, ref rangeRow2);

            if (rangeRow1 == rangeRow2 || 
                Math.Abs(rangeRow2 - rangeRow1)<3 ||
                Math.Abs(rangeRow1 - ctrRow) < 1 ||
                Math.Abs(rangeRow2 - ctrRow) < 1)
                // If the random index are the same or if the difference is too small, skip. 
                return;

            int direction = Globals.RND.NextDouble() > 0.5 ? 1 : -1;

            
            for (int row = rangeRow1; row <= rangeRow2; row++)
            {
                if (direction == -1)
                    for (int c = 1; c < Globals.RTILE_COLUMN_EXT-1; c++)
                    {
                        bool Changed = false; 
                        if (room.PathTile[row, c])
                        {
                            room.PathTile[row, c + direction] = true;
                            Changed = true;
                        }

                        if (Changed && row != rangeRow1 && row != rangeRow2 && row != ctrRow)
                            room.PathTile[row, c] = false;
                    }

                if (direction == 1)
                    for (int c = Globals.RTILE_COLUMN_EXT - 2; c > 0 ; c--)
                    {
                        bool Changed = false;
                        if (room.PathTile[row, c])
                        {
                            room.PathTile[row, c + direction] = true;
                            Changed = true;
                        }

                        if (Changed && row != rangeRow1 && row != rangeRow2 && row != ctrRow)
                            room.PathTile[row, c] = false;
                    }
            }


        }


        /// <summary>
        /// Shifting some random horizontal patterns upward or downward. 
        /// </summary>
        /// <param name="ctrRow">Index for the center row</param>
        /// <param name="ctrCol">Index for the center column</param>
        private void ShiftCols(int ctrRow, int ctrCol)
        {

            int rangeCol1 = Globals.RND.Next(0, Globals.RTILE_COLUMN_EXT);
            int rangeCol2 = Globals.RND.Next(0, Globals.RTILE_COLUMN_EXT);

            if (rangeCol1 > rangeCol2)
                Misc.Swap(ref rangeCol1, ref rangeCol2);


            if (rangeCol1 == rangeCol2 || 
                Math.Abs(rangeCol2 - rangeCol1) < 3 ||
                Math.Abs(rangeCol1 - ctrCol) < 1 ||
                Math.Abs(rangeCol2 - ctrCol) < 1)
                // If the random index are the same or if the difference is too small, skip. 
                return;

            int direction = Globals.RND.NextDouble() > 0.5 ? 1 : -1;
            
            for  (int col = rangeCol1; col <= rangeCol2; col++)
            {
                if (direction == -1)
                    for (int r = 1; r < Globals.RTILE_ROW_EXT - 1; r++)
                    {
                        bool Changed = false;
                        if (room.PathTile[r, col])
                        {
                            room.PathTile[r + direction, col] = true;
                            Changed = true;
                        }

                        if (Changed && col != rangeCol1 && col != rangeCol2 && col != ctrCol)
                            room.PathTile[r, col] = false;
                    }

                if (direction == 1)
                    for (int r = Globals.RTILE_ROW_EXT - 2; r > 0 ; r--)
                    {
                        bool Changed = false;
                        if (room.PathTile[r, col])
                        {
                            room.PathTile[r + direction, col] = true;
                            Changed = true;
                        }

                        if (Changed && col != rangeCol1 && col != rangeCol2 && col != ctrCol)
                            room.PathTile[r, col] = false;
                    }
            }


        }







        /// <summary>
        /// Returns the index of the first cell in the specified row from which there are at least MAX_CONT consecutive true values.
        /// If no such sequence exists, returns -1.
        /// </summary>
        /// <param name="matrix">The 2D boolean array.</param>
        /// <param name="rowIndex">The row index to inspect.</param>
        /// <param name="maxCont">The required number of consecutive true values.</param>
        /// <returns>The starting column index for the consecutive true sequence, or -1 if none exists.</returns>
        private static int GetFirstConsecutiveTrueIndexInRow(bool[,] matrix, int rowIndex, int maxCont)
        {
            int cols = matrix.GetLength(1);
            // We only need to iterate up to cols - maxCont, as starting later cannot have enough consecutive cells.
            for (int start = 0; start <= cols - maxCont; start++)
            {
                bool valid = true;
                for (int offset = 0; offset < maxCont; offset++)
                {
                    if (!matrix[rowIndex, start + offset])
                    {
                        valid = false;
                        break;
                    }
                }
                if (valid)
                {
                    return start;
                }
            }
            return -1;
        }


        /// <summary>
        /// Returns the index of the first cell in the specified column from which there are at least MAX_CONT consecutive true values.
        /// If no such sequence exists, returns -1.
        /// </summary>
        /// <param name="matrix">The 2D boolean array.</param>
        /// <param name="colIndex">The column index to inspect.</param>
        /// <param name="maxCont">The required number of consecutive true values.</param>
        /// <returns>The starting row index for the consecutive true sequence, or -1 if none exists.</returns>
        private static int GetFirstConsecutiveTrueIndexInColumn(bool[,] matrix, int colIndex, int maxCont)
        {
            int rows = matrix.GetLength(0);
            // Iterate only up to rows - maxCont.
            for (int start = 0; start <= rows - maxCont; start++)
            {
                bool valid = true;
                for (int offset = 0; offset < maxCont; offset++)
                {
                    if (!matrix[start + offset, colIndex])
                    {
                        valid = false;
                        break;
                    }
                }
                if (valid)
                {
                    return start;
                }
            }
            return -1;
        }


        /// <summary>
        /// Finds the maximum number of consecutive true values in the specified row.
        /// </summary>
        /// <param name="matrix">The 2D boolean array.</param>
        /// <param name="rowIndex">The row index to inspect.</param>
        /// <returns>The maximum consecutive true count in the row.</returns>
        private static int GetMaxConsecutiveTrueInRow(bool[,] matrix, int rowIndex)
        {
            int maxCount = 0;
            int currentCount = 0;
            int columns = matrix.GetLength(1);

            for (int col = 0; col < columns; col++)
            {
                if (matrix[rowIndex, col])
                {
                    currentCount++;
                    if (currentCount > maxCount)
                    {
                        maxCount = currentCount;
                    }
                }
                else
                {
                    currentCount = 0;
                }
            }

            return maxCount;
        }


        /// <summary>
        /// Finds the maximum number of consecutive true values in the specified column.
        /// </summary>
        /// <param name="matrix">The 2D boolean array.</param>
        /// <param name="colIndex">The column index to inspect.</param>
        /// <returns>The maximum consecutive true count in the column.</returns>
        private static int GetMaxConsecutiveTrueInColumn(bool[,] matrix, int colIndex)
        {
            int maxCount = 0;
            int currentCount = 0;
            int rows = matrix.GetLength(0);

            for (int row = 0; row < rows; row++)
            {
                if (matrix[row, colIndex])
                {
                    currentCount++;
                    if (currentCount > maxCount)
                    {
                        maxCount = currentCount;
                    }
                }
                else
                {
                    currentCount = 0;
                }
            }

            return maxCount;
        }


        

    }

}
