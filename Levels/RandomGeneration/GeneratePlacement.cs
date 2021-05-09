using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/// <summary>
/// Generate the placement of all rooms in a level set 
/// </summary>
namespace HappyDungeon
{
    class GeneratePlacement
    {
        private const double THRESHOLD = 0.5;
        private const double OFFSET = 0.5;
        private const double EMPTY_LINE_WEIGHT = 2;
        private const double DIVERGE_WEIGHT = 2;
        private const double DIAGONAL_WEIGHT = 3;
        private const double REGIONAL_WEIGHT = 0.75;
        private const Globals.Direction UP = Globals.Direction.Up;
        private const Globals.Direction DOWN = Globals.Direction.Down;
        private const Globals.Direction LEFT = Globals.Direction.Left;
        private const Globals.Direction RIGHT = Globals.Direction.Right;

        public int _RandWeight = 10;
        public bool _StartMiddle = false; // Middle or bottom 

        public int[] startUpLocation { get; set; }
        private int row;
        private int col;


        public GeneratePlacement(int Row, int Col)
        {
            row = Row;
            col = Col;
        }

        public bool[,] GetPlacement()
        {
            RoomGraph graph = new RoomGraph(row, col);
            List<RoomNode> exploreLeads = new List<RoomNode>();
            startUpLocation = _StartMiddle ? new int[] { row / 2, col / 2 }
                : new int[] { row - 1, col / 2 };

            graph.SetStartUp(startUpLocation[0], startUpLocation[1]);
            exploreLeads.Add(new RoomNode(startUpLocation, UP));

            while (exploreLeads.Count > 0)
            {
                foreach (RoomNode node in exploreLeads.ToArray())
                {
                    if (graph.ReachingCorner(node.index) || graph.ReachingDeadend(node.index))
                    {
                        exploreLeads.Remove(node);
                        continue;
                    }


                    foreach (Globals.Direction Direction in node.PossibleExpandDir())
                    {
                        int emptyCount = graph.EmptyCount(node.index, Direction);
                        double possbility = Globals.RND.NextDouble() - OFFSET;
                        int directionCount = row;

                        if (Direction == UP || Direction == DOWN)
                            directionCount = col;

                        possbility += EMPTY_LINE_WEIGHT * emptyCount / (directionCount - 1.0);

                        if (Direction != node.expansionDir)
                        {
                            possbility = Math.Abs(possbility);
                            possbility *= DIVERGE_WEIGHT;
                        }

                        possbility += graph.EmptyRegionalRate(node.index, Direction) * REGIONAL_WEIGHT;
                        possbility -= graph.DiagonalEmptyRate(node.index, Direction) * DIAGONAL_WEIGHT;

                        if (graph.IsEmpty(node.index, Direction) && possbility > THRESHOLD)
                        {
                            graph.AddRoom(node.index, Direction);
                            RoomNode newNode = new RoomNode(node.NeighborPos(Direction), Direction);
                            newNode.maxCombo = (Direction == node.expansionDir) ? node.maxCombo + 1 : 0;

                            exploreLeads.Add(newNode);
                        }
                    }
                    exploreLeads.Remove(node);
                }
            }


            return graph.GetArrangement();
        }
    }

}