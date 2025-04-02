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
            BevelPathTile(); 



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
        private void BevelPathTile(int smoothStep = 3)
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
        /// This method is supposed to smooth the path but it makes things extremely slow 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="sigma"></param>
        /// <param name="edgeBias"></param>
        /// <returns></returns>
        private  List<Point> GaussianSmoothPoints(List<Point> points, double sigma=2, double edgeBias = 3)
        {
            List<Point> smoothedPoints = new List<Point>();
            double twoSigmaSq = 2 * sigma * sigma;

            // Determine the boundaries of the point set.
            int minX = int.MaxValue, maxX = int.MinValue;
            int minY = int.MaxValue, maxY = int.MinValue;
            foreach (var pt in points)
            {
                if (pt.X < minX) minX = pt.X;
                if (pt.X > maxX) maxX = pt.X;
                if (pt.Y < minY) minY = pt.Y;
                if (pt.Y > maxY) maxY = pt.Y;
            }

            // For each point, compute the Gaussian-smoothed new position.
            foreach (var p in points)
            {
                // Check if the point is on the edge of the set.
                bool isEdge = (p.X == minX || p.X == maxX || p.Y == minY || p.Y == maxY);

                double weightSum = 0.0;
                double xSum = 0.0;
                double ySum = 0.0;

                foreach (var q in points)
                {
                    double dx = p.X - q.X;
                    double dy = p.Y - q.Y;
                    double distanceSq = dx * dx + dy * dy;

                    // Gaussian weight: exp(-distance^2 / (2*sigma^2))
                    double weight = Math.Exp(-distanceSq / twoSigmaSq);

                    // If q is the same as p and p is an edge point, apply extra bias.
                    if (p.Equals(q) && isEdge)
                    {
                        weight *= edgeBias;
                    }

                    weightSum += weight;
                    xSum += q.X * weight;
                    ySum += q.Y * weight;
                }

                int newX = (int)Math.Round(xSum / weightSum);
                int newY = (int)Math.Round(ySum / weightSum);
                smoothedPoints.Add(new Point(newX, newY));
            }

            return smoothedPoints;
        }


        private static List<Point> GetPointsFromMatrix(bool[,] matrix)
        {
            List<Point> points = new List<Point>();
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (matrix[i, j])
                    {
                        // Note: X is the column index and Y is the row index.
                        points.Add(new Point(j, i));
                    }
                }
            }
            return points;
        }


        private static bool[,] PointsToMatrix(List<Point> points, int rows, int cols)
        {
            bool[,] matrix = new bool[rows, cols];
            foreach (var p in points)
            {
                if (p.Y >= 0 && p.Y < rows && p.X >= 0 && p.X < cols)
                {
                    matrix[p.Y, p.X] = true;
                }
            }
            return matrix;
        }



    }

}
