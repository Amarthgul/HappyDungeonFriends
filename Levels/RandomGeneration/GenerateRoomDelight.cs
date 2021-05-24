using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions; 
using System.Text;
using System.Threading.Tasks;



namespace HappyDungeon
{
    /// <summary>
    /// Generate or modify a single room in level delight 
    /// </summary>
    class GenerateRoomDelight : Levels.GenerateRoomBasics
    {
        private bool _DEVMODE = true;

        private const int DELIGHT_ENEMY_MAX = 5; 

        public int[] tileList { get; set; } // For level delight

        public GenerateRoomDelight()
        {
            // Setup the template 
            roomDB = new Levels.RoomDB();

            enemyList = new int[]{
                Globals.ENEMY_BEAD
            };
            itemList = new int[] {
             
            };
            merchantItems = new int[] {
   
            };
            merchantCharaList = new int[] {

            };

            defaultBlock = 0;
            tileList = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
            walkableBlockList = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 
                17, 18, 19, 20, 21, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32};
            solidBlockLIst = new int[] { 128, 144, 160, 161};
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

 

        public void PopulateEnemy()
        {
            if (_DEVMODE)
            {
            }


            // Amount of enemy in this room 
            int enemyCount = (int)(DELIGHT_ENEMY_MAX * ((double)distFromStartup 
                / Math.Pow(room.Arrangement.GetLength(0), 1)));

            int Count = 0;
            int Threshold = (int)(enemyCount / Globals.RTILE_ROW * Globals.RTILE_COLUMN) + ENEMY_SPAWN_BIAS;

            for (int i = 0; i < room.Arrangement.GetLength(0); i++)
            {
                for (int j = 0; j < room.Arrangement.GetLength(1); j++)
                {
                    if (Globals.RND.Next(100) < Threshold 
                        && roomDB.NOT(roomDB.MaskedPath(room.OpenDoors))[i, j] 
                        && Count < enemyCount 
                        && roomDB.canPlaceDirect[i, j])
                    {
                        room.Arrangement[i, j] = enemyList[Globals.RND.Next(enemyList.Length)];
                        Count++;
                    }
                }
            }

        }

        /// <summary>
        /// Generates the map blocks for the rrom. 
        /// This method is packed with too much imformation, virtually impossible for me to 
        /// explain it. If you want to change the appearances of the rooms, it's probably better 
        /// just start another method rather than modifiying on this one.  
        /// </summary>
        public void PopulateBlock()
        {
            int HighBound = 15; // 15 basic path tile indexes for level delight  
            int VarityCount = 5;
            int NonPathVarityCount = 8; 
            int FillDivider = 12;  // Decides how much to fill up 
            int Compensation = 16; // Path compensation blcok index start at 16 
            int CornerTileStart = 48;
            int CornerTileEnd = 64;
            int NonPathTileIndexOffset = 32; // The non path tiles are 16 tiles behind in the texture 

            // The lowest index of the path tile 
            int LowBoundSeed = distFromStartup + VarityCount <= HighBound ? distFromStartup : HighBound - VarityCount;
            // The lowest index of the non-path tile 
            int NonPathLowBoundSeed = distFromStartup + NonPathVarityCount <= HighBound ?
                distFromStartup : HighBound - NonPathVarityCount - Globals.RND.Next(distFromStartup / 2);
            CornerTileStart = CornerTileStart + distFromStartup <= CornerTileEnd ? CornerTileStart : CornerTileEnd - VarityCount; ;
            int DefaultBlock = LowBoundSeed + NonPathTileIndexOffset;
            // Further away makes less path tiles
            double FillPercentLinear = distFromStartup / (double)FillDivider; 
            double PathFillPercent = 1 - FillPercentLinear;

            bool[,] MaskedPath = roomDB.MaskedPath(room.OpenDoors);
            bool[,] NotPath = roomDB.NOT(MaskedPath);
            bool[,] CornerWights = roomDB.AND(roomDB.CornersWeighted(distFromStartup / 2), 
                roomDB.RandomFill(FillPercentLinear)) ;
            int[] SelectedTiles = new int[] { 0, 0, 0, 0, 0 };
            int[] NonPathTiles = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };
            int[] CompensateTiles = new int[] { 0, 0, 0, 0, 0 };
            int[] CornerTiles = new int[] { 0, 0, 0, 0, 0 };

            int[] StareBlocks = new int[] { Globals.STARE_BLOCK_1, Globals.STARE_BLOCK_2 };
            int StareBlockThreshold = 6;
            int StareBlockPossibility = 40; 
            
            // Select all tiles that would be needed for the cross pattern 
            for (int i = 0; i < VarityCount; i++)
            {
                SelectedTiles[i] = LowBoundSeed + i;
                CompensateTiles[i] = Globals.RND.Next(Compensation) + HighBound;
                CornerTiles[i] = CornerTileStart + i; 
            }
            for (int i = 0; i < NonPathVarityCount; i++)
            {
                NonPathTiles[i] = DefaultBlock + i;
            }

            // Set default blocks and populate the basic grounds 
            room.DefaultBlock = DefaultBlock;
            FloodMap(DefaultBlock);
            PopulatePatternWeighted(NotPath, NonPathTiles, 
                x => (Math.Max(x, x - 12) / 3), y => (Math.Max(y, y - 7)) / 3);
            PopulatePatternRand(CornerWights, CornerTiles, CornerTiles.Length);

            // Put some stare blocks around the start up room 
            if (Globals.RND.Next(100) < StareBlockPossibility && distFromStartup < StareBlockThreshold)
            {
                if (Globals.RND.Next(100) < 50)
                    PopulatePattern(roomDB.shihonzuki, StareBlocks[Globals.RND.Next() % StareBlocks.Length]);
                else
                    PopulatePattern(roomDB.corners, StareBlocks[Globals.RND.Next() % StareBlocks.Length]);
            }

            // Add paths in the room 
            PopulatePatternPartial(MaskedPath, SelectedTiles, PathFillPercent);
            PopulatPatternCondition(MaskedPath, CompensateTiles, c => (c > HighBound));

        }


        public void PopulateItem()
        {
            int TotalitemNow = Math.Max(ITEM_MAX - distFromStartup, 0);
            int Threshold = (int)(100 * ((double)TotalitemNow / room.Arrangement.Length));
            int ItemCount = 0;

            // Startup investment, give the player a small fortune 
            if (distFromStartup < TREASURE_RANGE && Globals.RND.Next(100) < TREASURE_POSSIBILITY)
            {
                bool[,] ptn = roomDB.Subtract(FindIndexInRange(room.Arrangement, solidBlockLIst), roomDB.treasure);
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
                    if (roomDB.merchantRoom[i, j])
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
                { 0, 0, Globals.ITEM_LINKEN, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, Globals.ITEM_GOLD, 0, 0, 0, 0, 0, 0, Globals.STARE_BLOCK_2, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, Globals.ITEM_TORCH, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, Globals.STARE_BLOCK_1, 0, 0, 0, 0, 0, Globals.ENEMY_BEAD, 0, 0, 0, 0},
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



    }

}