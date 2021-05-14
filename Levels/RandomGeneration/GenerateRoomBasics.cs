using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyDungeon.Levels
{
    public class GenerateRoomBasics
    {
        public const int ENEMY_MAX = 8;
        public const int ENEMY_SPAWN_BIAS = 15;
        public const int PATTERNED_BIAS = -10;
        public const int ITEM_TOLERANCE = 6;
        public const int ITEM_MAX = 4;
        public const double WALKABLE_SPREAD_MAX = 0.6;
        public const double SOLID_SPREAD_MAX = 0.2;
        public const int MESSY_THRESHOLD = 4;
        public const double MESSY_DIVIDER = 2;
        public const int TREASURE_POSSIBILITY = 20;
        public const int TREASURE_RANGE = 3;

        public double walkableSpread;
        public double solidSpread;
        public int distFromStartup;
        public double rowProgression;
        public double colProgression;

        // In regards to the Map class
        public int defaultBlock { get; set; }

        // Related to the style and possible things that appears in the rooms
        public int blackRoomInedx { get; set; }
        public int[] enemyList { get; set; }
        public int[] walkableBlockList { get; set; }
        public int[] solidBlockLIst { get; set; }
        public int[] itemList { get; set; }
        public int[] merchantItems { get; set; }
        public int[] merchantCharaList { get; set; }
        public int bossIndex { get; set; }

        public RoomDB roomDB;
        public bool[,] canSpawnEnemy;

        public RoomInfo room { get; set; }


        /// <summary>
        /// Set parameters for the current room.
        /// </summary>
        /// <param name="dist">Distance from the startup room</param>
        /// <param name="rowProg">Row progression between 0-1</param>
        /// <param name="colProg">Column progression between 0-1</param>
        public void SetPara(int dist, double rowProg, double colProg)
        {
            distFromStartup = dist;
            rowProgression = rowProg;
            colProgression = colProg;
        }


        /// <summary>
        /// Given an index, populate it into the map in a given pattern.
        /// </summary>
        /// <param name="Pattern">Pattern to follow</param>
        /// <param name="Index">Index to put</param>
        public void PopulatePattern(bool[,] Pattern, int Index)
        {
            for (int i = 0; i < room.Arrangement.GetLength(0); i++)
            {
                for (int j = 0; j < room.Arrangement.GetLength(1); j++)
                {
                    if (Pattern[i, j])
                    {
                        room.Arrangement[i, j] = Index;
                    }
                }
            }
        }

        /// <summary>
        /// Given a pattern and a list of indexes, fill part of the pattern with the index. 
        /// </summary>
        /// <param name="Pattern">Pattern to fill</param>
        /// <param name="ListOfIndex">Possible indexes to fill with</param>
        /// <param name="FillPercent">Precent of the pattern to be filled</param>
        public void PopulatePatternPartial(bool[,] Pattern, int[] ListOfIndex, double FillPercent)
        {
            int Possibility = (int)(FillPercent * 100);

            for (int i = 0; i < room.Arrangement.GetLength(0); i++)
            {
                for (int j = 0; j < room.Arrangement.GetLength(1); j++)
                {
                    if (Globals.RND.Next(100) < Possibility && Pattern[i, j])
                    {
                        room.Arrangement[i, j] = ListOfIndex[Globals.RND.Next(ListOfIndex.Length)];
                    }
                }
            }

        }

        /// <summary>
        /// Given a list of indexes, choose some of them and populate then into a pattern.
        /// If the list has 5 indexes and MaxType is 3, then only 3 of these 5 could appear 
        /// in the populated pattern. 
        /// </summary>
        /// <param name="Pattern">Pattern to follow</param>
        /// <param name="ListOfIndex">All possible indexes that could be added</param>
        /// <param name="MaxType">Max amount of different indexes that can appear</param>
        public void PopulatePatternRand(bool[,] Pattern, int[] ListOfIndex, int MaxType)
        {
            List<int> ActualList = new List<int>(); ;

            if (MaxType > ListOfIndex.Length)
                for (int i = 0; i < ListOfIndex.Length; i++)
                    ActualList.Add(ListOfIndex[i]);
            else
            {
                ActualList = new List<int>(ListOfIndex);
            }

            for (int i = 0; i < room.Arrangement.GetLength(0); i++)
            {
                for (int j = 0; j < room.Arrangement.GetLength(1); j++)
                {
                    if (Pattern[i, j])
                    {
                        room.Arrangement[i, j] = ActualList[Globals.RND.Next(ActualList.Count)];
                    }
                }
            }
        }

        /// <summary>
        /// Check the matrix, replace with random index in the list 
        /// when both the condition and pattern are met.  
        /// </summary>
        /// <param name="Pattern">Pattern to fill</param>
        /// <param name="ListOfIndex">Possible indexes to fill with</param>
        /// <param name="Condition">The condition for current index to meet</param>
        public void PopulatPatternCondition(bool[,] Pattern, int[] ListOfIndex, Func<int, bool> Condition)
        {

            for (int i = 0; i < room.Arrangement.GetLength(0); i++)
            {
                for (int j = 0; j < room.Arrangement.GetLength(1); j++)
                {
                    if (Pattern[i, j] && Condition(room.Arrangement[i, j]))
                    {
                        room.Arrangement[i, j] = ListOfIndex[Globals.RND.Next(ListOfIndex.Length)];
                    }
                }
            }
        }

        /// <summary>
        /// Given a pattern and list, populate the pattern with random list index.
        /// But the indexes are subtracted by a function of row and column. 
        /// </summary>
        /// <param name="Pattern">Pattern to fill</param>
        /// <param name="ListOfIndex">Possible indexes to fill with</param>
        /// <param name="OffsetX">Row function</param>
        /// <param name="OffsetY">Column function</param>
        public void PopulatePatternWeighted(bool[,] Pattern, int[] ListOfIndex, Func<int, int> OffsetX, Func<int, int> OffsetY)
        {
            for (int i = 0; i < room.Arrangement.GetLength(0); i++)
            {
                for (int j = 0; j < room.Arrangement.GetLength(1); j++)
                {
                    if (Pattern[i, j])
                    {
                        int NewlyPopulate = ListOfIndex[Globals.RND.Next(ListOfIndex.Length)];
                        NewlyPopulate -= (OffsetX(j) + OffsetY(i));
                        if (NewlyPopulate < ListOfIndex.Min()) NewlyPopulate = ListOfIndex.Min();

                        room.Arrangement[i, j] = NewlyPopulate;
                    }
                }
            }
        }

        /// <summary>
        /// Soft index is added to places where it's not a solid block. 
        /// </summary>
        /// <param name="Position">Position to try to place the index</param>
        /// <param name="Index">Index to be placed</param>
        /// <returns></returns>
        public bool AddSoftIndex(int[] Position, int Index)
        {
            if (IsBlock(Position[0], Position[1]))
                return false;
            else
            {
                room.Arrangement[Position[0], Position[1]] = Index;
                return true;
            }
        }

        /// <summary>
        /// Check if that position is going to spawn as a block.
        /// </summary>
        /// <param name="row">Meh</param>
        /// <param name="col">Meh</param>
        /// <returns>True if it's a block</returns>
        public bool IsBlock(int row, int col)
        {
            return (room.Arrangement[row, col] > 0);
        }

        /// <summary>
        /// Check if that position is going to spawn as a solid block.
        /// Solid block hiders movement. 
        /// </summary>
        /// <param name="row">Meh</param>
        /// <param name="col">Meh</param>
        /// <returns>True if that position is going to have a solid block</returns>
        public bool IsSolidBlock(int row, int col)
        {
            return (room.Arrangement[row, col] > Globals.SOLID_BLOCK_BOUND);
        }

        /// <summary>
        /// Check if the given location shall spawn an item. 
        /// </summary>
        /// <param name="row">Meh</param>
        /// <param name="col">Meh</param>
        /// <returns>True if that index is an item</returns>
        public bool IsItem(int row, int col)
        {
            return (room.Arrangement[row, col] < 0 && room.Arrangement[row, col] > Globals.ITEM_BOUND);
        }

        /// <summary>
        /// Check and clear anything that could clog the doors.
        /// </summary>
        /// <param name="Target">The matrix to clear</param>
        public void MaskOffDoorways(bool[,] Target)
        {
            for (int i = 0; i < room.Arrangement.GetLength(0); i++)
            {
                for (int j = 0; j < room.Arrangement.GetLength(1); j++)
                {
                    Target[i, j] = roomDB.canPlaceDirect[i, j] && Target[i, j];
                }
            }
        }

        /// <summary>
        /// Generate a random bool matrix with given density of true.
        /// Density is in 0-1.
        /// </summary>
        /// <param name="Density">Ratio of total truth values</param>
        /// <returns>Bool matrix with random truth</returns>
        public bool[,] RandScatter(double Density)
        {
            int Threshold = (int)(Globals.RTILE_COLUMN * Globals.RTILE_ROW * Density) * 100;
            Threshold /= (Globals.RTILE_COLUMN * Globals.RTILE_ROW);
            bool[,] scatter = new bool[room.Arrangement.GetLength(0), room.Arrangement.GetLength(1)];

            for (int i = 0; i < room.Arrangement.GetLength(0); i++)
            {
                for (int j = 0; j < room.Arrangement.GetLength(1); j++)
                {
                    if (Globals.RND.Next(100) < Threshold)
                    {
                        scatter[i, j] = true;
                    }
                    else
                    {
                        scatter[i, j] = false;
                    }
                }
            }

            return scatter;
        }

        /// <summary>
        /// Find the positions of the index that is within given list.  
        /// </summary>
        /// <param name="Matrix">Target matrix</param>
        /// <param name="RangeList">Target list</param>
        /// <returns>Matrix of bool, true if that position is in the list.</returns>
        public bool[,] FindIndexInRange(int[,] Matrix, int[] RangeList)
        {
            bool[,] result = new bool[room.Arrangement.GetLength(0), room.Arrangement.GetLength(1)];
            List<int> lst = new List<int>(RangeList);

            for (int i = 0; i < room.Arrangement.GetLength(0); i++)
            {
                for (int j = 0; j < room.Arrangement.GetLength(1); j++)
                {
                    result[i, j] = lst.Contains(Matrix[i, j]);
                }
            }

            return result;
        }

        /// <summary>
        /// Flood the arrangement matrix with one single index. 
        /// </summary>
        /// <param name="Index">Flooding index.</param>
        public void FloodMap(int Index)
        {
            for (int i = 0; i < room.Arrangement.GetLength(0); i++)
            {
                for (int j = 0; j < room.Arrangement.GetLength(1); j++)
                {
                    room.Arrangement[i, j] = Index;
                }
            }
        }



    }
}
