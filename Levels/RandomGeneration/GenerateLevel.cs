using System;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;



/// <summary>
/// Generate or modify a level consists of many rooms 
/// </summary>
namespace HappyDungeon
{
    class GenerateLevel
    {
        private bool _DEVMODE = false;

        // ================================================================================
        // ======================== Consts and frequently used ============================
        // ================================================================================
        private const int HIDDEN_DOOR_THRESHOLD = 0;  // Possibility for doors to become hidden 
        private const int MERCHANT_ROOM_COUNT = 2;    
        private const int BOSS_ROOM_COUNT = 1;        
        private const double BOSS_DIST_MOD = 2.6;     // Power of the spawn distance-possibility
        private const int BOSS_SPAWN_OFFSET = 3;      // Range within which boss shall not spawn 


        public RoomInfo[,] levelSet;

        public int levelSetRow { set; get; }
        public int levelSetCol { set; get; }

        public int startUpCol { set; get; }
        public int startUpRow { set; get; }

        private int levelRowCount;
        private int levelColCount;

        private Globals.GameLevel gameLevel; 

        public GenerateLevel()
        {

        }

        public RoomInfo[,] GenerateLevelSet(int Row, int Col, Globals.GameLevel LevelSetting)
        {
            levelSetRow = Row;
            levelSetCol = Col;
            gameLevel = LevelSetting; 

            switch (LevelSetting)
            {
                case Globals.GameLevel.Delight:
                    init();
                    PickStartUpRoom();
                    RegulateDoors();
                    PopulateRoomsDelight();
                    SetBossRooms();
                    SetMerchantRooms();
                    ResumeStartupRoom();
                    break;
                case Globals.GameLevel.Joy:
                    break;
                case Globals.GameLevel.Bliss:
                    break;
                default:
                    break; 
            }

            

            return levelSet;
        }

        /// <summary>
        /// Generate a set of placement, fill these rooms with placeholders.
        /// </summary>
        private void init()
        {
            levelSet = new RoomInfo[levelSetRow, levelSetCol];

            bool[,] Placement = new bool[levelSetRow, levelSetCol];

            Placement = new GeneratePlacement(levelSetRow, levelSetCol).GetPlacement();

            // Fill the rooms with placeholders first 
            for (int i = 0; i < levelSet.GetLength(0); i++)
                for (int j = 0; j < levelSet.GetLength(1); j++)
                    if (Placement[i, j])
                        levelSet[i, j] = new GenerateDelightRoom().InitRoom();

            levelRowCount = levelSet.GetLength(0);
            levelColCount = levelSet.GetLength(1);

        }

        /// <summary>
        /// Change doors depending on the inter-room relationship 
        /// </summary>
        private void RegulateDoors()
        {

            for (int i = 0; i < levelSet.GetLength(0); i++)
            {
                for (int j = 0; j < levelSet.GetLength(1); j++)
                {
                    if (levelSet[i, j] == null) continue; 

                    foreach (Globals.Direction Dir in Globals.FourDirIter)
                    {
                        if (HasNextRoom(new int[] { i, j }, Dir) )
                        {
                            if (Globals.RND.Next(100) < HIDDEN_DOOR_THRESHOLD)
                            {
                                AddHiddenDoors(new int[] { i, j }, Dir);
                                //AddItemInRoom(new int[] { i, j }, Globals.BOMB_ITEM, 1);
                            }
                            else
                                AddOpenDoors(new int[] { i, j }, Dir);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Open a set of doors in this room and the room at the given direction. 
        /// The contract is there must have been a room on that direction.  
        /// </summary>
        /// <param name="CurrentPos">Room index to open door</param>
        /// <param name="Direction">Direction of the neighbor room to open door</param>
        private void AddOpenDoors(int[] CurrentPos, Globals.Direction Direction)
        {
            Vector2 Offset = new Vector2(0, 0);
            int nextRoomDoorDir = 0;

            levelSet[CurrentPos[0], CurrentPos[1]].OpenDoors[(int)Direction] = true;

            switch (Direction)
            {
                case Globals.Direction.Up:
                    Offset.Y = -1;
                    nextRoomDoorDir = (int)Globals.Direction.Down;
                    break;

                case Globals.Direction.Down:
                    Offset.Y = 1;
                    nextRoomDoorDir = (int)Globals.Direction.Up;
                    break;

                case Globals.Direction.Left:
                    Offset.X = -1;
                    nextRoomDoorDir = (int)Globals.Direction.Right;
                    break;

                case Globals.Direction.Right:
                    Offset.X = 1;
                    nextRoomDoorDir = (int)Globals.Direction.Left;
                    break;

                default:
                    break;
            }

            levelSet[CurrentPos[0] + (int)Offset.Y, CurrentPos[1] + (int)Offset.X].OpenDoors[nextRoomDoorDir] = true;
        }

        /// <summary>
        /// Actually just eliminate all visible doors.
        /// </summary>
        /// <param name="CurrentPos">The location of the room</param>
        /// <param name="Direction">The direction relative to the room to add doors</param>
        private void AddHiddenDoors(int[] CurrentPos, Globals.Direction Direction)
        {
            Vector2 Offset = new Vector2(0, 0);
            int nextRoomDoorDir = 0;

            levelSet[CurrentPos[0], CurrentPos[1]].OpenDoors[(int)Direction] = false;

            switch (Direction)
            {
                case Globals.Direction.Up:
                    Offset.Y = -1;
                    nextRoomDoorDir = (int)Globals.Direction.Down;
                    break;

                case Globals.Direction.Down:
                    Offset.Y = 1;
                    nextRoomDoorDir = (int)Globals.Direction.Up;
                    break;

                case Globals.Direction.Left:
                    Offset.X = -1;
                    nextRoomDoorDir = (int)Globals.Direction.Right;
                    break;

                case Globals.Direction.Right:
                    Offset.X = 1;
                    nextRoomDoorDir = (int)Globals.Direction.Left;
                    break;

                default:
                    break;
            }

            levelSet[CurrentPos[0] + (int)Offset.Y, CurrentPos[1] + (int)Offset.X].MysteryDoors[nextRoomDoorDir] = false;
            levelSet[CurrentPos[0] + (int)Offset.Y, CurrentPos[1] + (int)Offset.X].LockedDoors[nextRoomDoorDir] = false;
            levelSet[CurrentPos[0] + (int)Offset.Y, CurrentPos[1] + (int)Offset.X].OpenDoors[nextRoomDoorDir] = false;
        }

        /// <summary>
        /// Add mystery doors, in this game it's money doors. 
        /// </summary>
        /// <param name="CurrentPos">The location of the room</param>
        /// <param name="Direction">The direction relative to the room to add doors</param>
        private void AddMysDoors(int[] CurrentPos, Globals.Direction Direction)
        {
            Vector2 Offset = new Vector2(0, 0);
            int nextRoomDoorDir = 0;

            levelSet[CurrentPos[0], CurrentPos[1]].OpenDoors[(int)Direction] = false;
            levelSet[CurrentPos[0], CurrentPos[1]].MysteryDoors[(int)Direction] = true;

            switch (Direction)
            {
                case Globals.Direction.Up:
                    Offset.Y = -1;
                    nextRoomDoorDir = (int)Globals.Direction.Down;
                    break;

                case Globals.Direction.Down:
                    Offset.Y = 1;
                    nextRoomDoorDir = (int)Globals.Direction.Up;
                    break;

                case Globals.Direction.Left:
                    Offset.X = -1;
                    nextRoomDoorDir = (int)Globals.Direction.Right;
                    break;

                case Globals.Direction.Right:
                    Offset.X = 1;
                    nextRoomDoorDir = (int)Globals.Direction.Left;
                    break;

                default:
                    break;
            }

            levelSet[CurrentPos[0] + (int)Offset.Y, CurrentPos[1] + (int)Offset.X].OpenDoors[nextRoomDoorDir] = false;
            levelSet[CurrentPos[0] + (int)Offset.Y, CurrentPos[1] + (int)Offset.X].MysteryDoors[nextRoomDoorDir] = true;
        }

        private void AddItemInRoom(int[] CurrentPos, int ItemIndex, int Number)
        {
            GenerateDelightRoom RoomGen = new GenerateDelightRoom();
            RoomGen.room = levelSet[CurrentPos[0], CurrentPos[1]];
            int Count = 0;

            while (Count > Number)
            {
                int pos1 = Globals.RND.Next(Globals.RTILE_ROW);
                int pos2 = Globals.RND.Next(Globals.RTILE_COLUMN);

                if (RoomGen.AddSoftIndex(CurrentPos, ItemIndex))
                {
                    Count++;
                }
            }
        }

        /// <summary>
        /// Randomly select a position, if there's a room, then make it as the start up room.
        /// </summary>
        private void PickStartUpRoom()
        {

            while (true)
            {
                startUpRow = Globals.RND.Next(levelSet.GetLength(0) - 1);
                startUpCol = Globals.RND.Next(levelSet.GetLength(1) - 1);

                if (levelSet[startUpRow, startUpCol] != null)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Re-tag the type of the room on startup location as start. 
        /// </summary>
        private void ResumeStartupRoom()
        {
            GenerateDelightRoom RoomGenTemp = new GenerateDelightRoom();
            levelSet[startUpRow, startUpCol] = RoomGenTemp.SetAsStartupRoom(levelSet[startUpRow, startUpCol]);
        }

        /// <summary>
        /// Given a pair of index, calculate the L1 distance from the startup room.
        /// </summary>
        /// <param name="row">Row of the given room</param>
        /// <param name="col">Col of the given room</param>
        /// <returns>Distance in int</returns>
        private int L1DistanceFromStart(int row, int col)
        {
            return Math.Abs(row - startUpRow) + Math.Abs(col - startUpCol);
        }

        /// <summary>
        /// Assume a room is neither boss nor startup room, 
        /// populate it with items, blocks, and enemies.
        /// </summary>
        private void PopulateRoomsDelight()
        {
            int L1Dist;
            double RowProgression = 0, ColProgression = 0;


            for (int i = 0; i < levelSet.GetLength(0); i++)
            {
                for (int j = 0; j < levelSet.GetLength(1); j++)
                {
                    L1Dist = L1DistanceFromStart(i, j);
                    ColProgression = j / levelColCount;
                    RowProgression = i / levelRowCount;

                    if (levelSet[i, j] != null)
                    {
                        GenerateDelightRoom RoomGen = new GenerateDelightRoom();
                        RoomGen.InitRoom();
                        RoomGen.room = levelSet[i, j];
                        RoomGen.SetPara(L1Dist, RowProgression, ColProgression);

                        if (!IsStartUpRoom(i, j))
                        {
                            RoomGen.PopulateBlock();
                            RoomGen.PopulateEnemy();
                        }
                        RoomGen.PopulateItem();

                        levelSet[i, j] = RoomGen.room;
                    }
                }
            }
        }

        /// <summary>
        /// Randomly picking rooms and set them as merchant rooms.
        /// </summary>
        private void SetMerchantRooms()
        {

            int count = 0;
            int[] iter = new int[] { 0, 1, 2, 3 };

            while (count < MERCHANT_ROOM_COUNT)
            {
                int row = Globals.RND.Next(levelSet.GetLength(0));
                int col = Globals.RND.Next(levelSet.GetLength(1));

                if (!IsStartUpRoom(row, col))
                {
                    GenerateDelightRoom RoomGen = new GenerateDelightRoom();
                    RoomGen.InitRoom();
                    RoomGen.SetAsMerchantRoom();

                    levelSet[row, col] = RoomGen.room;

                    foreach (Globals.Direction Dir in Globals.FourDirIter)
                    {
                        if (HasNextRoom(new int[] { row, col }, Dir))
                        {
                            AddMysDoors(new int[] { row, col }, Dir);
                        }
                    }

                    count++;
                }

            }
        }

        /// <summary>
        /// Randomly pick rooms. 
        /// If it's not null and not the start, then 
        /// there's a chance to make it the boss room. 
        /// The further it is from the startup the more likly
        /// if is to become the boss room. 
        /// </summary>
        /// <returns>0 if exit successfully</returns>
        private int SetBossRooms()
        {

            int count = 0;
            int[] iter = new int[] { 0, 1, 2, 3 };
            GenerateDelightRoom RoomGen = new GenerateDelightRoom();
            RoomGen.InitRoom();
            

            if (_DEVMODE)
            {
                if (HasNextRoom(new int[] { startUpRow, startUpCol }, Globals.Direction.Up))
                {
                    RoomGen.SetAsBossRoom();
                    levelSet[startUpRow - 1, startUpCol] = RoomGen.room;
                }
                else if (HasNextRoom(new int[] { startUpRow, startUpCol }, Globals.Direction.Down))
                {
                    RoomGen.SetAsBossRoom();
                    levelSet[startUpRow + 1, startUpCol] = RoomGen.room;
                }
                return 0;
            }

            while (count < BOSS_ROOM_COUNT)
            {
                int row = Globals.RND.Next(levelSet.GetLength(0));
                int col = Globals.RND.Next(levelSet.GetLength(1));

                int Threshold = (int)Math.Pow(L1DistanceFromStart(row, col) - BOSS_SPAWN_OFFSET, BOSS_DIST_MOD); 

                if (!IsStartUpRoom(row, col) && levelSet[row, col] != null &&
                    Globals.RND.Next(100) < Threshold)
                {
                    RoomGen.room = levelSet[row, col]; 
                    RoomGen.SetAsBossRoom();
                    levelSet[row, col] = RoomGen.room;
                    count++;
                }
            }

            return 0;
        }

        /// <summary>
        /// Quick check if a given room is assigned as the startup.
        /// </summary>
        /// <param name="row">Row of the given room</param>
        /// <param name="col">Col of the given room</param>
        /// <returns>True if it is the startup room</returns>
        private bool IsStartUpRoom(int row, int col)
        {
            return (row == startUpRow && col == startUpCol);
        }

        /// <summary>
        /// Check if the direction at the given room has a neighbor. 
        /// </summary>
        /// <param name="CurrentPos">The location of the room</param>
        /// <param name="Direction">The direction relative to the room to loop up</param>
        /// <returns></returns>
        public bool HasNextRoom(int[] CurrentPos, Globals.Direction Direction)
        {
            switch (Direction)
            {
                case Globals.Direction.Up:
                    if (CurrentPos[0] <= 0) return false;
                    return (levelSet[CurrentPos[0] - 1, CurrentPos[1]] != null);

                case Globals.Direction.Down:
                    if (CurrentPos[0] >= levelSet.GetLength(0) - 1) return false;
                    return (levelSet[CurrentPos[0] + 1, CurrentPos[1]] != null);

                case Globals.Direction.Left:
                    if (CurrentPos[1] <= 0) return false;
                    return (levelSet[CurrentPos[0], CurrentPos[1] - 1] != null);

                case Globals.Direction.Right:
                    if (CurrentPos[1] >= levelSet.GetLength(1) - 1) return false;
                    return (levelSet[CurrentPos[0], CurrentPos[1] + 1] != null);

                default:
                    return false;
            }
        }

        public RoomInfo StartUpRoom()
        {
            return  levelSet[ startUpRow, startUpCol ];
        }

        public int[] StartUpRoomIndex()
        {
            return new int[] {startUpRow, startUpCol };
        }

    }
}