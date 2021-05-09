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
        public int[] currentLocationIndex;

        public LevelCycling(int MapSize)
        {
            levelRNG = new GenerateLevel();

            currentMapSet = levelRNG.GenerateLevelSet(MapSize, MapSize);
            currentLocationIndex = new int[] { levelRNG.StartUpRoomIndex()[0], levelRNG.StartUpRoomIndex()[1] };
        }

        public RoomInfo GetStart()
        {

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

        // Return the room in that direction, assume there is one 
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

        
    }
}
