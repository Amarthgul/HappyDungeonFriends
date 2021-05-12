using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/// <summary>
/// Generate or modify a single room 
/// </summary>
namespace HappyDungeon
{
    class GenerateRoom
    {
        private bool _DEVMODE = true;
        

        private const int ENEMY_MAX = 8;
        private const int ENEMY_SPAWN_BIAS = 15;
        private const int PATTERNED_BIAS = -10;
        private const int ITEM_TOLERANCE = 6;
        private const int ITEM_MAX = 4;
        private const double WALKABLE_SPREAD_MAX = 0.6;
        private const double SOLID_SPREAD_MAX = 0.2;
        private const int MESSY_THRESHOLD = 4;
        private const double MESSY_DIVIDER = 2;
        private const int TREASURE_POSSIBILITY = 20;
        private const int TREASURE_RANGE = 3;

        private double walkableSpread;
        private double solidSpread;
        private int distFromStartup;
        private double rowProgression;
        private double colProgression;

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

        public RoomInfo room { get; set; }

        public GenerateRoom(Globals.GameLevel LevelSetting)
        {
            // Setup the template 

            enemyList = new int[]{
             
            };
            itemList = new int[] {
             
            };
            merchantItems = new int[] {
   
            };
            merchantCharaList = new int[] {

            };

            defaultBlock = 0;
            walkableBlockList = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 
                17, 18, 19, 20, 21, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32};
            solidBlockLIst = new int[] { 48, 64, 80, 96, 112};
            blackRoomInedx = 1;
            //bossIndex = Globals.BOSS_ENEMY;

        }

        /// <summary>
        /// Fill the map info with placeholders, all disabled by default; 
        /// </summary>
        /// <returns></returns>
        public RoomInfo InitRoom()
        {
            RoomInfo template = new RoomInfo();

            template.DefaultBlock = defaultBlock;
            template.Type = Globals.RoomTypes.Normal;

            template.Arrangement = new int[Globals.RTILE_ROW, Globals.RTILE_COLUMN];
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

        public void SetPara(int dist, double rowProg, double colProg)
        {
            distFromStartup = dist;
            rowProgression = rowProg;
            colProgression = colProg;
        }

        public void PopulateEnemy()
        {
            if (_DEVMODE)
            {
            }


            // Amount of enemy in this room 
            int enemyCount = (int)(ENEMY_MAX * (
                (double)distFromStartup / Math.Pow(room.Arrangement.GetLength(0), 1)));

            int Count = 0;
            int Threshold = (int)(enemyCount / Globals.RTILE_ROW * Globals.RTILE_COLUMN) + ENEMY_SPAWN_BIAS;

            for (int i = 0; i < room.Arrangement.GetLength(0); i++)
            {
                for (int j = 0; j < room.Arrangement.GetLength(1); j++)
                {
                    //if (Globals.RND.Next(100) < Threshold && !IsBlock(i, j) && Count < enemyCount && canPlace[i, j])
                    if (Globals.RND.Next(100) < Threshold
                        && Count < enemyCount && Levels.RoomDB.canPlace[i, j])
                    {
                        //room.Arrangement[i, j] = enemyList[Globals.RND.Next(enemyList.Length)];
                        Count++;
                    }
                }
            }

        }

        public void PopulateBlock()
        {
            int walkablePatternedRate;
            int solidPatternedRate;
            bool[,] Rand;

            List<bool[,]> WalkableList = new List<bool[,]>();
            List<bool[,]> SolidList = new List<bool[,]>();
            WalkableList.Add(Levels.RoomDB.cross);
            WalkableList.Add(Levels.RoomDB.corners);
            WalkableList.Add(Levels.RoomDB.midOval);
            SolidList.Add(Levels.RoomDB.grid);
            SolidList.Add(Levels.RoomDB.corners);
            SolidList.Add(Levels.RoomDB.doubleRectangles);
            SolidList.Add(Levels.RoomDB.square);
            SolidList.Add(Levels.RoomDB.cornerDust);
            SolidList.Add(Levels.RoomDB.pipe);
            SolidList.Add(Levels.RoomDB.maze);

            walkableSpread = Math.Min(
                (double)distFromStartup / (double)Math.Pow(room.Arrangement.GetLength(0), 2),
                WALKABLE_SPREAD_MAX);
            solidSpread = Math.Min(
                (double)distFromStartup / (double)Math.Pow(room.Arrangement.GetLength(0), 2),
                SOLID_SPREAD_MAX);

            walkablePatternedRate = (int)(100 * (1 - walkableSpread / WALKABLE_SPREAD_MAX)) - PATTERNED_BIAS;
            solidPatternedRate = (int)(100 * (1 - solidSpread / SOLID_SPREAD_MAX)) - PATTERNED_BIAS;


            if (Globals.RND.Next(100) < walkablePatternedRate)
            {
                Rand = WalkableList[Globals.RND.Next(WalkableList.Count)];
            }
            else
            {
                Rand = RandScatter(walkableSpread);
                MaskOffDoorways(Rand);
            }
            if (distFromStartup > MESSY_THRESHOLD)
                PopulatePatternRand(Rand, walkableBlockList, (int)(distFromStartup / MESSY_DIVIDER));
            else
                PopulatePattern(Rand, walkableBlockList[Globals.RND.Next(walkableBlockList.Length)]);


            if (Globals.RND.Next(100) < solidPatternedRate)
            {
                Rand = SolidList[Globals.RND.Next(SolidList.Count)];
            }
            else
            {
                Rand = RandScatter(solidSpread);
                MaskOffDoorways(Rand);
            }

            if (distFromStartup > MESSY_THRESHOLD)
                PopulatePatternRand(Rand, solidBlockLIst, (int)(distFromStartup / MESSY_DIVIDER));
            else
                PopulatePattern(Rand, solidBlockLIst[Globals.RND.Next(solidBlockLIst.Length)]);
        }

        public void PopulateItem()
        {
            int TotalitemNow = Math.Max(ITEM_MAX - distFromStartup, 0);
            int Threshold = (int)(100 * ((double)TotalitemNow / room.Arrangement.Length));
            int ItemCount = 0;

            // Startup investment, give the player a small fortune 
            if (distFromStartup < TREASURE_RANGE && Globals.RND.Next(100) < TREASURE_POSSIBILITY)
            {
                bool[,] ptn = Subtract(FindIndexInRange(room.Arrangement, solidBlockLIst), Levels.RoomDB.treasure);
                //PopulatePattern(ptn, Globals.RUPY_ITEM);
            }


            for (int i = 0; i < room.Arrangement.GetLength(0); i++)
            {
                for (int j = 0; j < room.Arrangement.GetLength(1); j++)
                {
                    if (distFromStartup == 0)
                    {
                        //PopulatePattern(cornerBig, Globals.BOMB_ITEM);
                    }
                    else if (Globals.RND.Next(100) < Threshold
                        && ItemCount < ITEM_MAX && !IsBlock(i, j))
                    {
                        //room.Arrangement[i, j] = itemList[Globals.RND.Next(itemList.Length)];
                        ItemCount += 1;
                    }
                }
            }
        }

        public void SetAsMerchantRoom()
        {
            room.DefaultBlock = blackRoomInedx;
            room.Type = Globals.RoomTypes.Merchant;

            int RowMid = 2;
            int ColMid = 6;

            FloodMap(blackRoomInedx);

            for (int i = 0; i < room.Arrangement.GetLength(0); i++)
            {
                for (int j = 0; j < room.Arrangement.GetLength(1); j++)
                {
                    if (Levels.RoomDB.merchantRoom[i, j])
                    {
                        //room.Arrangement[i, j] = merchantItems[Globals.RND.Next(merchantItems.Length)];
                    }
                }
            }

            //room.Arrangement[RowMid, ColMid] = merchantCharaList[Globals.RND.Next(merchantCharaList.Length)];
        }

        public RoomInfo SetAsStartupRoom(RoomInfo RL)
        {
            room = RL;
            room.Type = Globals.RoomTypes.Start;

            if (_DEVMODE)
            {
                room.Arrangement = new int[,] {
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, Globals.ITEM_GOLD, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, Globals.ITEM_TORCH, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}};
            }

            return room;
        }

        public void SetAsBossRoom()
        {

            int RowMid = 3;
            int ColMid = 7;

            room.Type = Globals.RoomTypes.Boss;
            FloodMap(defaultBlock);

            //room.Arrangement[RowMid, ColMid] = Globals.BOSS_ENEMY;
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
        private bool IsBlock(int row, int col)
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
        private bool IsSolidBlock(int row, int col)
        {
            return (room.Arrangement[row, col] > Globals.SOLID_BLOCK_BOUND);
        }

        /// <summary>
        /// Check if the given location shall spawn an item. 
        /// </summary>
        /// <param name="row">Meh</param>
        /// <param name="col">Meh</param>
        /// <returns>True if that index is an item</returns>
        private bool IsItem(int row, int col)
        {
            return (room.Arrangement[row, col] < 0 && room.Arrangement[row, col] > Globals.ITEM_BOUND);
        }

        /// <summary>
        /// Given an index, populate it into the map in a given pattern.
        /// </summary>
        /// <param name="Pattern">Pattern to follow</param>
        /// <param name="Index">Index to put</param>
        private void PopulatePattern(bool[,] Pattern, int Index)
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
        /// Given a list of indexes, choose some of them and populate then into a pattern.
        /// If the list has 5 indexes and MaxType is 3, then only 3 of these 5 could appear 
        /// in the populated pattern. 
        /// </summary>
        /// <param name="Pattern">Pattern to follow</param>
        /// <param name="ListOfIndex">All possible indexes that could be added</param>
        /// <param name="MaxType">Max amount of different indexes that can appear</param>
        private void PopulatePatternRand(bool[,] Pattern, int[] ListOfIndex, int MaxType)
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
        /// Check and clear anything that could clog the doors.
        /// </summary>
        /// <param name="Target">The matrix to clear</param>
        private void MaskOffDoorways(bool[,] Target)
        {
            for (int i = 0; i < room.Arrangement.GetLength(0); i++)
            {
                for (int j = 0; j < room.Arrangement.GetLength(1); j++)
                {
                    Target[i, j] = Levels.RoomDB.canPlace[i, j] && Target[i, j];
                }
            }
        }

        /// <summary>
        /// Generate a random bool matrix with given density of true.
        /// Density is in 0-1.
        /// </summary>
        /// <param name="Density">Ratio of total truth values</param>
        /// <returns>Bool matrix with random truth</returns>
        private bool[,] RandScatter(double Density)
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
        /// Subtract one matrix from another. Not XOR.
        /// Does not change any of the input matrices.
        /// </summary>
        /// <param name="m1">Substractor</param>
        /// <param name="m2">Base</param>
        /// <returns>Bool matrix of m2 - m1</returns>
        private bool[,] Subtract(bool[,] m1, bool[,] m2)
        {
            bool[,] result = new bool[room.Arrangement.GetLength(0), room.Arrangement.GetLength(1)];
            for (int i = 0; i < room.Arrangement.GetLength(0); i++)
            {
                for (int j = 0; j < room.Arrangement.GetLength(1); j++)
                {
                    result[i, j] = m1[i, j] ? false : m2[i, j];
                }
            }
            return result;
        }

        /// <summary>
        /// Find the positions of the index that is within given list.  
        /// </summary>
        /// <param name="Matrix">Target matrix</param>
        /// <param name="RangeList">Target list</param>
        /// <returns>Matrix of bool, true if that position is in the list.</returns>
        private bool[,] FindIndexInRange(int[,] Matrix, int[] RangeList)
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
        private void FloodMap(int Index)
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