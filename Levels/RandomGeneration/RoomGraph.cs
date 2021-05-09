using System;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;


/// <summary>
/// For quick access of the relationship between rooms in a level set. 
/// </summary>
namespace HappyDungeon
{

    /// <summary>
    /// A single node representing a room, created for easy access 
    /// of relationship during placement creation. 
    /// </summary>
    class RoomNode
    {
        public Globals.Direction expansionDir { set; get; }
        public int[] index { set; get; }

        public int dirCombo { set; get; }

        public int maxCombo { set; get; }

        public int roomType { get; set; }

        /// <summary>
        /// Init for a Room Node. 
        /// </summary>
        /// <param name="Position">Position in the entire room set.</param>
        /// <param name="Expansion">The direction this room is expended from.</param>
        public RoomNode(int[] Position, Globals.Direction Expansion)
        {
            index = Position;
            expansionDir = Expansion;

            maxCombo = Globals.MAX_DIR_COMBO;
        }

        /// <summary>
        /// Does not consider being in boundary, thus needing extra check. 
        /// </summary>
        /// <returns></returns>
        public Globals.Direction[] PossibleExpandDir()
        {
            List<Globals.Direction> PossibleDir = new List<Globals.Direction>() 
            { Globals.Direction.Left, Globals.Direction.Right, Globals.Direction.Up, Globals.Direction.Down };

            // If it's expanded from one direction, remove that direction 
            switch (expansionDir)
            {
                case Globals.Direction.Up:
                    PossibleDir.Remove(Globals.Direction.Down);
                    break;

                case Globals.Direction.Down:
                    PossibleDir.Remove(Globals.Direction.Up);
                    break;

                case Globals.Direction.Left:
                    PossibleDir.Remove(Globals.Direction.Right);
                    break;

                case Globals.Direction.Right:
                    PossibleDir.Remove(Globals.Direction.Left);
                    break;

                default:
                    break;
            }

            // Disable too many continuous expansion in one direction 
            if (dirCombo >= maxCombo)
                PossibleDir.Remove(expansionDir);

            return PossibleDir.ToArray();
        }

        public int[] NeighborPos(Globals.Direction Direction)
        {
            Vector2 Offset = new Vector2(0, 0);
            switch (Direction)
            {
                case Globals.Direction.Up:
                    Offset.Y = -1;
                    break;

                case Globals.Direction.Down:
                    Offset.Y = 1;
                    break;

                case Globals.Direction.Left:
                    Offset.X = -1;
                    break;

                case Globals.Direction.Right:
                    Offset.X = 1;
                    break;

                default:
                    break;
            }

            return new int[] { index[0] + (int)Offset.Y, index[1] + (int)Offset.X };
        }
    }

    /// <summary>
    /// The graph of the rooms. Created for easy access of 
    /// misc conditions and polynomial calculations during 
    /// placement creation. 
    /// </summary>
    class RoomGraph
    {
        public int[] startUpLocation { get; set; }
        public int[] currentLocationIndex { get; set; }
        public bool[,] arrangement { get; set; }

        public RoomGraph(int levelSetRow, int levelSetCol)
        {
            arrangement = new bool[levelSetRow, levelSetCol];

            for (int i = 0; i < arrangement.GetLength(0); i++)
                for (int j = 0; j < arrangement.GetLength(1); j++)
                    arrangement[i, j] = false;

            startUpLocation = new int[] { levelSetRow - 1, levelSetCol / 2 };
        }

        public void SetStartUp(int Row, int Col)
        {
            startUpLocation = new int[] { Row, Col };
            arrangement[Row, Col] = true;
        }

        public bool IsEmpty(int[] CurrentPos, Globals.Direction Direction)
        {
            switch (Direction)
            {
                case Globals.Direction.Up:
                    if (CurrentPos[0] <= 0) return false;
                    return (arrangement[CurrentPos[0] - 1, CurrentPos[1]] == false);

                case Globals.Direction.Down:
                    if (CurrentPos[0] >= arrangement.GetLength(0) - 1) return false;
                    return (arrangement[CurrentPos[0] + 1, CurrentPos[1]] == false);

                case Globals.Direction.Left:
                    if (CurrentPos[1] <= 0) return false;
                    return (arrangement[CurrentPos[0], CurrentPos[1] - 1] == false);

                case Globals.Direction.Right:
                    if (CurrentPos[1] >= arrangement.GetLength(1) - 1) return false;
                    return (arrangement[CurrentPos[0], CurrentPos[1] + 1] == false);

                default:
                    return false;
            }
        }

        public void AddRoom(int[] CurrentPos, Globals.Direction Direction)
        {
            Vector2 Offset = new Vector2(0, 0);
            switch (Direction)
            {
                case Globals.Direction.Up:
                    Offset.Y = -1;
                    break;

                case Globals.Direction.Down:
                    Offset.Y = 1;
                    break;

                case Globals.Direction.Left:
                    Offset.X = -1;
                    break;

                case Globals.Direction.Right:
                    Offset.X = 1;
                    break;

                default:
                    break;
            }

            arrangement[CurrentPos[0] + (int)Offset.Y, CurrentPos[1] + (int)Offset.X] = true;
        }

        /// <summary>
        /// Find the maxium amount of continuous empty space in that direction on a line. 
        /// </summary>
        /// <param name="CurrentPos"></param>
        /// <param name="Direction"></param>
        /// <returns></returns>
        public int EmptyCount(int[] CurrentPos, Globals.Direction Direction)
        {
            int count = 0;
            int[] counter = new int[] { CurrentPos[0], CurrentPos[1] }; // Avoid reference 


            while (IsEmpty(counter, Direction))
            {
                switch (Direction)
                {
                    case Globals.Direction.Up:
                        counter[0] -= 1;
                        break;

                    case Globals.Direction.Down:
                        counter[0] += 1;
                        break;

                    case Globals.Direction.Left:
                        counter[1] -= 1;
                        break;

                    case Globals.Direction.Right:
                        counter[1] += 1;
                        break;

                    default:
                        break;
                }
                count += 1;
            }


            return count;
        }

        /// <summary>
        /// Rate of emptiness in the square region at that direction.
        /// </summary>
        /// <param name="CurrentPos"></param>
        /// <param name="Direction"></param>
        /// <returns>A double between 0 to 1</returns>
        public double EmptyRegionalRate(int[] CurrentPos, Globals.Direction Direction)
        {
            int allCount = 0, trueCount = 0;

            int iStart = 0, iEnd = 0;
            int jStart = 0, jEnd = 0;

            switch (Direction)
            {
                case Globals.Direction.Up:
                    iEnd = CurrentPos[0];
                    jEnd = arrangement.GetLength(1);

                    break;

                case Globals.Direction.Down:
                    if (CurrentPos[0] == arrangement.GetLength(0) - 1) return 0;
                    iStart = CurrentPos[0];
                    iEnd = arrangement.GetLength(0);
                    jEnd = arrangement.GetLength(1);
                    break;

                case Globals.Direction.Left:
                    iEnd = arrangement.GetLength(0);
                    jEnd = CurrentPos[1];
                    break;

                case Globals.Direction.Right:
                    if (CurrentPos[1] == arrangement.GetLength(1) - 1) return 0;
                    iEnd = arrangement.GetLength(0);
                    jStart = CurrentPos[1];
                    jEnd = arrangement.GetLength(1);
                    break;

                default:
                    break;
            }

            for (int i = iStart; i < iEnd; i++)
            {
                for (int j = jStart; j < jEnd; j++)
                {
                    if (arrangement[i, j])
                        trueCount += 1;
                    allCount += 1;
                }
            }

            return (double)trueCount / allCount;
        }

        public double DiagonalEmptyRate(int[] CurrentPos, Globals.Direction Direction)
        {
            int trueCount = 0;


            switch (Direction)
            {
                case Globals.Direction.Up:
                    if (CurrentPos[0] == 0) break;

                    if (CurrentPos[1] > 0)
                        trueCount += arrangement[CurrentPos[0] - 1, CurrentPos[1] - 1] ? 1 : 0;
                    if (CurrentPos[1] < arrangement.GetLength(1) - 1)
                        trueCount += arrangement[CurrentPos[0] - 1, CurrentPos[1] + 1] ? 1 : 0;
                    break;

                case Globals.Direction.Down:
                    if (CurrentPos[0] == arrangement.GetLength(0) - 1) break;

                    if (CurrentPos[1] > 0)
                        trueCount += arrangement[CurrentPos[0] + 1, CurrentPos[1] - 1] ? 1 : 0;
                    if (CurrentPos[1] < arrangement.GetLength(1) - 1)
                        trueCount += arrangement[CurrentPos[0] + 1, CurrentPos[1] + 1] ? 1 : 0;
                    break;

                case Globals.Direction.Left:
                    if (CurrentPos[1] == 0) break;

                    if (CurrentPos[0] > 0)
                        trueCount += arrangement[CurrentPos[0] - 1, CurrentPos[1] - 1] ? 1 : 0;
                    if (CurrentPos[0] < arrangement.GetLength(0) - 1)
                        trueCount += arrangement[CurrentPos[0] + 1, CurrentPos[1] - 1] ? 1 : 0;
                    break;

                case Globals.Direction.Right:
                    if (CurrentPos[1] == arrangement.GetLength(1) - 1) break;

                    if (CurrentPos[0] > 0)
                        trueCount += arrangement[CurrentPos[0] - 1, CurrentPos[1] + 1] ? 1 : 0;
                    if (CurrentPos[0] < arrangement.GetLength(0) - 1)
                        trueCount += arrangement[CurrentPos[0] + 1, CurrentPos[1] + 1] ? 1 : 0;
                    break;

                default:
                    break;
            }

            return trueCount / 2.0;
        }

        public bool ReachingBorder(int[] CurrentPos)
        {
            return (CurrentPos[0] == 0 ||
                CurrentPos[1] == 0 ||
                CurrentPos[0] == arrangement.GetLength(0) - 1 ||
                CurrentPos[1] == arrangement.GetLength(1) - 1);
        }

        public bool ReachingCorner(int[] CurrentPos)
        {
            int row = arrangement.GetLength(0) - 1;
            int col = arrangement.GetLength(1) - 1;

            return (CurrentPos[0] == 0 && CurrentPos[1] == 0 ||
                CurrentPos[0] == 0 && CurrentPos[1] == col ||
                CurrentPos[0] == row && CurrentPos[1] == 0 ||
                CurrentPos[0] == row && CurrentPos[1] == col);
        }

        public bool ReachingDeadend(int[] CurrentPos)
        {
            foreach (Globals.Direction Dir in Globals.FourDirIter)
                if (IsEmpty(CurrentPos, Dir))
                    return false;

            return true;
        }

        public bool[,] GetArrangement()
        {
            return arrangement;
        }
    }
}