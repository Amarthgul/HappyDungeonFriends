using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace HappyDungeon
{
    class Generator
    {
        private Game1 game; 

        public Generator(Game1 G) 
        {
            game = G;
        }

        /// <summary>
        /// Construct 4 walls around the room,
        /// functions as boundary check. 
        /// </summary>
        /// <returns>A list of static blocks that are invisible. </returns>
        public List<IBlock> GetStaticBlockList()
        {
            List<IBlock> StaticBlockList = new List<IBlock>();
            RoomInfo MapInfo = game.currentRoom.roomInfo;

            const float DOOR_VERTICAL_MARK = 5.25f;
            const float DOOR_HORIZONTAL_MARK = 7.5f;
            const int MID_DIVISION = 6;
            const double LEFT_OFFSET = 1.25;
            const double RIGHT_OFFSET = 2.75;
            const int VERTICAL_DIVISION = 3;
            const double UP_OFFSET = 1.95;
            const double DOWN_OFFSET = 2.05;

            int TopPosition = Globals.OUT_HEADSUP + Globals.OUT_UNIT;
            int ButtPosition = Globals.OUT_FHEIGHT - 2 * Globals.OUT_UNIT;
            int LeftPosition = Globals.OUT_UNIT;
            int RightPosition = Globals.OUT_FWIDTH - 2 * Globals.OUT_UNIT;
            int HorizontalPos, VerticalPos = 0;

            Vector2 LeftDoorBlocking = new Vector2(1 * Globals.OUT_UNIT,
                Globals.OUT_HEADSUP + DOOR_VERTICAL_MARK * Globals.OUT_UNIT);
            Vector2 RightDoorBlocking = new Vector2(Globals.OUT_FWIDTH - 2 * Globals.OUT_UNIT,
                Globals.OUT_HEADSUP + DOOR_VERTICAL_MARK * Globals.OUT_UNIT);
            Vector2 TopDoorBlocking = new Vector2(DOOR_HORIZONTAL_MARK * Globals.OUT_UNIT,
                2 * Globals.OUT_UNIT);
            Vector2 BottomDoorBlocking = new Vector2(DOOR_HORIZONTAL_MARK * Globals.OUT_UNIT,
                Globals.OUT_FHEIGHT - 2 * Globals.OUT_UNIT);

            // Stuck a block if that direction has no entry  
            for (int i = 0; i < MapInfo.OpenDoors.Length; i++)
            {
                bool CouldPass = (MapInfo.LockedDoors[i] || MapInfo.Holes[i] 
                    ||  MapInfo.OpenDoors[i] || MapInfo.MysteryDoors[i]);

                if (!CouldPass)
                {
                    switch (i)
                    {
                        case (int)Globals.Direction.Up:
                            StaticBlockList.Add(new BlockInvis(TopDoorBlocking));
                            break;
                        case (int)Globals.Direction.Down:
                            StaticBlockList.Add(new BlockInvis(BottomDoorBlocking));
                            break;
                        case (int)Globals.Direction.Right:
                            StaticBlockList.Add(new BlockInvis(RightDoorBlocking));
                            break;
                        case (int)Globals.Direction.Left:
                            StaticBlockList.Add(new BlockInvis(LeftDoorBlocking));
                            break;
                        default:
                            break;
                    }
                }
            }


            // The following 2 for loops are for the creation of walls (in lieu of boundary check)
            for (int i = 0; i < Globals.RTILE_COLUMN; i++)
            {
                // All resulting magic numbers are to avoid player
                // from stuck in between and cannot plass through doors 
                if (i < MID_DIVISION)
                    HorizontalPos = (int)((i + LEFT_OFFSET) * Globals.OUT_UNIT);
                else
                    HorizontalPos = (int)((i + RIGHT_OFFSET) * Globals.OUT_UNIT);
                
                StaticBlockList.Add(new BlockInvis(new Vector2(HorizontalPos, TopPosition)));
                StaticBlockList.Add(new BlockInvis(new Vector2(HorizontalPos, ButtPosition)));
            }
            for (int i = 0; i < Globals.RTILE_ROW; i++)
            {
                if (i < VERTICAL_DIVISION)
                    VerticalPos = Globals.OUT_HEADSUP + (int)((i + UP_OFFSET) * Globals.OUT_UNIT);
                if (i > VERTICAL_DIVISION)
                    VerticalPos = Globals.OUT_HEADSUP + (int)((i + DOWN_OFFSET) * Globals.OUT_UNIT);

                // There's actually an overlapping block created here, but doesn't really matter 
                StaticBlockList.Add(new BlockInvis(new Vector2(LeftPosition, VerticalPos)));
                StaticBlockList.Add(new BlockInvis(new Vector2(RightPosition, VerticalPos)));
            }

            return StaticBlockList;
        }

        public List<IItem> GetCollectibleItemList(Game1 game)
        {
            List<IItem> ItemList = new List<IItem>();
            Vector2 Position; 

            int[,] RoomMat = game.roomCycler.GetCurrentRoomInfo().Arrangement;

            for (int i = 0; i < RoomMat.GetLength(0); i++)
            {
                for (int j = 0; j < RoomMat.GetLength(1); j++)
                {
                    Position = Misc.Instance.PositionTranslate(i, j);

                    switch (RoomMat[i, j])
                    {
                        case Globals.ITEM_TORCH:
                            ItemList.Add(new Torch(game, Position)); ;
                            break;
                        default:
                            break;
                    }
                }
            }

            return ItemList; 
        }

    }
}
