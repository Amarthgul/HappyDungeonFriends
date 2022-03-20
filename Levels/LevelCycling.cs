using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyDungeon
{
    public class LevelCycling
    {
        private bool _DEVMODE = false; 

        private const int LEVEL_EAGLE_SIZE = 6;

        private GenerateLevel levelRNG;

        public RoomInfo[,] currentMapSet;
        public int[] currentLocationIndex { get; set; }

        public LevelCycling(int MapSize, Globals.GameLevel Level)
        {
            levelRNG = new GenerateLevel();

            currentMapSet = levelRNG.GenerateLevelSet(MapSize, MapSize, Level);
            currentLocationIndex = new int[] { levelRNG.StartUpRoomIndex()[0], levelRNG.StartUpRoomIndex()[1] };
        }

        public RoomInfo GetStart()
        {
            // Note that changing the initial room in GenerateRoom.cs can do the same 
            // But more comprehensive and less buggy 
            if (_DEVMODE)
                return TestRoom();
            else
                return levelRNG.StartUpRoom();
        }

        public RoomInfo TestRoom()
        {
            RoomInfo Template = new RoomInfo();

            Template.Arrangement = new int[,] {
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}};
            Template.Type = Globals.RoomTypes.Normal;
            Template.LockedDoors = new bool[] { false, false, false, false };
            Template.Holes = new bool[] { false, false, false, false }; 
            Template.MysteryDoors = new bool[] { true, true, true, true };
            Template.OpenDoors = new bool[] { false, false, false, false };

            Template.DefaultBlock = 0; 

            return Template; 
        }

        public RoomInfo GetCurrentRoomInfo()
        {
            return currentMapSet[currentLocationIndex[0], currentLocationIndex[1]];
        }
        public int[] GetCurrentLocationIndex()
        {
            return currentLocationIndex;
        }

        /// <summary>
        /// Return the room in that direction.
        /// If there is one. 
        /// </summary>
        /// <param name="Direction">The direction to loop up</param>
        /// <returns>Info about that room. Null if no room in that direction</returns>
        public RoomInfo GetNextRoom(Globals.Direction Direction)
        {
            switch (Direction)
            {
                case Globals.Direction.Up:
                    if (currentLocationIndex[0] <= 0) return null;
                    return (currentMapSet[currentLocationIndex[0] - 1, currentLocationIndex[1]]);

                case Globals.Direction.Down:
                    if (currentLocationIndex[0] >= currentMapSet.GetLength(0) - 1) return null;
                    return (currentMapSet[currentLocationIndex[0] + 1, currentLocationIndex[1]]);

                case Globals.Direction.Left:
                    if (currentLocationIndex[1] <= 0) return null;
                    return (currentMapSet[currentLocationIndex[0], currentLocationIndex[1] - 1]);

                case Globals.Direction.Right:
                    if (currentLocationIndex[1] >= currentMapSet.GetLength(1) - 1) return null;
                    return (currentMapSet[currentLocationIndex[0], currentLocationIndex[1] + 1]);

                default:
                    return null;
            }
        }

        /// <summary>
        /// Set current location as next room.
        /// </summary>
        /// <param name="Direction">Direction ot move</param>
        public void MoveIntoRoom(Globals.Direction Direction)
        {
            switch (Direction)
            {
                case Globals.Direction.Up:
                    currentLocationIndex = new int[] { currentLocationIndex[0] - 1, currentLocationIndex[1] }; 
                    break; 

                case Globals.Direction.Down:
                    currentLocationIndex = new int[] { currentLocationIndex[0] + 1, currentLocationIndex[1] };
                    break;

                case Globals.Direction.Left:
                    currentLocationIndex = new int[] { currentLocationIndex[0], currentLocationIndex[1] - 1};
                    break;

                case Globals.Direction.Right:
                    currentLocationIndex = new int[] { currentLocationIndex[0], currentLocationIndex[1] + 1 };
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Open a set of mystery doors in this room at the direction. 
        /// </summary>
        /// <param name="Direction">Direction of the mys door</param>
        public void OpenMysDoor(Globals.Direction Direction)
        {
            currentMapSet[currentLocationIndex[0], currentLocationIndex[1]].MysteryDoors[(int)Direction] = false;
            currentMapSet[currentLocationIndex[0], currentLocationIndex[1]].OpenDoors[(int)Direction] = true;

            OpenAllDoorsInNextRoom(Direction);
        }

        /// <summary>
        /// Remove an index from the room, either an enemy or an item.
        /// If the given location is not correct, it'll try to find one in the map. 
        /// </summary>
        /// <param name="Index">Index to remove</param>
        /// <param name="RefPosition">Best guess of its position</param>
        public void RemoveIndex(int Index, int[] RefPosition)
        {
            if (currentMapSet[currentLocationIndex[0], currentLocationIndex[1]]
                .Arrangement[RefPosition[1], RefPosition[0]] == Index)
            {
                currentMapSet[currentLocationIndex[0], currentLocationIndex[1]]
                .Arrangement[RefPosition[1], RefPosition[0]] =
                    currentMapSet[currentLocationIndex[0], currentLocationIndex[1]]
                    .DefaultBlock;
            }
            else
            {
                for (int i = 0; i < Globals.RTILE_ROW; i++)
                {
                    for (int j = 0; j < Globals.RTILE_COLUMN; j++)
                    {
                        int TargetIndex = currentMapSet[currentLocationIndex[0], currentLocationIndex[1]].Arrangement[i, j];
                        int AuxIndex = General.IndexCoder.GetAuxIndex(TargetIndex);
                        //int BlockIndex = General.IndexCoder.GetBlockIndex(TargetIndex);
                        
                        if (AuxIndex == Index)
                        {
                            currentMapSet[currentLocationIndex[0], currentLocationIndex[1]].Arrangement[i, j]
                                = General.IndexCoder.OverrideAuxIndex(TargetIndex, 0);
                            break;
                        } 
                    }
                }
            }
                
        }

        // ================================================================================
        // ================================ Private methods ===============================
        // ================================================================================

        /// <summary>
        /// Mark all the doors in the next room as open. 
        /// If that direction can be opened. 
        /// </summary>
        /// <param name="Direction">Target room direction</param>
        private void OpenAllDoorsInNextRoom(Globals.Direction Direction)
        {
            int[] Index = NextRoomIndex(currentLocationIndex, Direction);

            for (int i = 0; i < 4; i++)
            {
                if(currentMapSet[Index[0], Index[1]].MysteryDoors[i])
                {
                    OpenDoorInRoom(Index, (Globals.Direction)i);
                    if (HasNextRoom(Index, (Globals.Direction)i))
                    {
                        OpenDoorInRoom(NextRoomIndex(Index,
                            (Globals.Direction)i), Misc.Instance.Opposite((Globals.Direction)i));
                    } 
                }
            }

        }

        private bool HasNextRoom(int[] Pivot, Globals.Direction Dir)
        {
            int[] NextIndex = NextRoomIndex(Pivot, Dir);
            return currentMapSet[NextIndex[0], NextIndex[1]] != null;
        }

        /// <summary>
        /// For inqurying the index of room at that direction. 
        /// Does not consider whether or not if there's a room at all. 
        /// </summary>
        /// <param name="Direction">The target direction</param>
        /// <returns>Index of the room at given direction</returns>
        private int[] NextRoomIndex(int[] Pivot, Globals.Direction Direction)
        {
            switch (Direction)
            {
                case Globals.Direction.Up:
                    return new int[] { Pivot[0] - 1, Pivot[1] };

                case Globals.Direction.Down:
                    return new int[] { Pivot[0] + 1, Pivot[1] };

                case Globals.Direction.Left:
                    return new int[] { Pivot[0], Pivot[1] - 1 };

                case Globals.Direction.Right:
                    return new int[] { Pivot[0], Pivot[1] + 1 };

                default:
                    return new int[] { Pivot[0], Pivot[1] };
            }
        }

        /// <summary>
        /// Open door at a direction in a room.
        /// </summary>
        /// <param name="Pivot">Index of the room</param>
        /// <param name="Direction">Direction to open</param>
        private void OpenDoorInRoom(int[] Pivot, Globals.Direction Direction)
        {
            currentMapSet[Pivot[0], Pivot[1]].MysteryDoors[(int)Direction] = false;
            currentMapSet[Pivot[0], Pivot[1]].Holes[(int)Direction] = false;

            currentMapSet[Pivot[0], Pivot[1]].OpenDoors[(int)Direction] = true;

        }



    }
}
