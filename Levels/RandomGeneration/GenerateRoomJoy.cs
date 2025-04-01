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

            defaultBlock = 48;
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
                        room.Arrangement[row, col] = walkableBlockList[Globals.RND.Next() % walkableBlockList.Count()];
                    }
                }
            }



        }

        // ================================================================================
        // ================================ Private methods ===============================
        // ================================================================================


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


        private void PathGeneration(RoomInfo[,] levelSet)
        {

            DirectPathCreation();
            IterativeSmoothPathTile(); 

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
            if (room.OpenDoors[0])
            {
                for (int col = 0; col <= centerCol; col++)
                {
                    room.PathTile[centerRow, col] = true;
                }
            }

            // Right: Fill from the center to the right edge.
            if (room.OpenDoors[1])
            {
                for (int col = centerCol; col < Globals.RTILE_COLUMN_EXT; col++)
                {
                    room.PathTile[centerRow, col] = true;
                }
            }

            // Up: Fill from the center to the top edge.
            if (room.OpenDoors[2])
            {
                for (int row = 0; row <= centerRow; row++)
                {
                    room.PathTile[row, centerCol] = true;
                }
            }

            // Down: Fill from the center to the bottom edge.
            if (room.OpenDoors[3])
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
        private void GaussianSmoothPathTile(double threshold = 0.1)
        {
            // Check for exactly one horizontal and one vertical opening.
            // (Using exclusive or to ensure only one door per axis is open.)
            bool horizontalOpen = room.OpenDoors[0] ^ room.OpenDoors[1];
            bool verticalOpen = room.OpenDoors[2] ^ room.OpenDoors[3];

            if (!(horizontalOpen && verticalOpen))
            {
                // Do not apply smoothing if the openings are not exactly one horizontal and one vertical.
                return;
            }

            int rows = Globals.RTILE_ROW_EXT;
            int cols = Globals.RTILE_COLUMN_EXT; 

            // Define a 3x3 Gaussian kernel with sigma approximated to 1.
            // The kernel weights sum to 1.
            double[,] kernel = new double[,]
            {
            { 1.0/16, 2.0/16, 1.0/16 },
            { 2.0/16, 4.0/16, 2.0/16 },
            { 1.0/16, 2.0/16, 1.0/16 }
            };

            // Temporary array to store the smoothed values.
            double[,] blurred = new double[rows, cols];

            // Perform convolution on the matrix.
            // Convert true to 1.0 and false to 0.0.
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    double sum = 0.0;
                    // Apply the kernel.
                    for (int ki = -1; ki <= 1; ki++)
                    {
                        for (int kj = -1; kj <= 1; kj++)
                        {
                            int ni = i + ki;
                            int nj = j + kj;
                            if (ni >= 0 && ni < rows && nj >= 0 && nj < cols)
                            {
                                double value = room.PathTile[ni, nj] ? 1.0 : 0.0;
                                sum += value * kernel[ki + 1, kj + 1];
                            }
                        }
                    }
                    blurred[i, j] = sum;
                }
            }

            // Threshold the blurred values to update the room.PathTile.
            // Cells with a value greater than or equal to the threshold are set to true.
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    room.PathTile[i, j] = blurred[i, j] >= threshold;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="iterations"></param>
        /// <param name="threshold"></param>
        private void IterativeSmoothPathTile(int iterations = 3, double threshold = 0.3)
        {
            // Ensure exactly one horizontal and one vertical door are open.
            bool horizontalOpen = room.OpenDoors[0] ^ room.OpenDoors[1];
            bool verticalOpen = room.OpenDoors[2] ^ room.OpenDoors[3];
            if (!(horizontalOpen && verticalOpen))
            {
                // Only apply smoothing if there is a single horizontal and a single vertical opening.
                return;
            }

            int rows = Globals.RTILE_ROW_EXT;
            int cols = Globals.RTILE_COLUMN_EXT;

            // Convert the boolean matrix to a double matrix (true -> 1.0, false -> 0.0)
            double[,] values = new double[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    values[i, j] = room.PathTile[i, j] ? 1.0 : 0.0;
                }
            }

            // Perform iterative smoothing with a uniform 3x3 averaging filter.
            for (int iter = 0; iter < iterations; iter++)
            {
                double[,] newValues = new double[rows, cols];
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        double sum = 0.0;
                        int count = 0;
                        // Average the cell and its 8 neighbors.
                        for (int di = -1; di <= 1; di++)
                        {
                            for (int dj = -1; dj <= 1; dj++)
                            {
                                int ni = i + di;
                                int nj = j + dj;
                                if (ni >= 0 && ni < rows && nj >= 0 && nj < cols)
                                {
                                    sum += values[ni, nj];
                                    count++;
                                }
                            }
                        }
                        newValues[i, j] = sum / count;
                    }
                }
                values = newValues;
            }

            // Convert the smoothed values back to booleans using the provided threshold.
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    room.PathTile[i, j] = values[i, j] >= threshold;
                }
            }
        }



    }

}

