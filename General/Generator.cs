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
    public class Generator
    {
        private Game1 game; 

        public Generator(Game1 G) 
        {
            game = G;
        }

        /// <summary>
        /// Construct 4 walls around the room, functions as boundary check. 
        /// Also blocks the door position if there's no door. 
        /// </summary>
        /// <returns>A list of static blocks that are invisible. </returns>
        public List<IBlock> GetStaticBlockList()
        {
            List<IBlock> StaticBlockList = new List<IBlock>();
            RoomInfo MapInfo = game.currentRoom.roomInfo;

            const float DOOR_VERTICAL_MARK = 5.25f;
            const float DOOR_HORIZONTAL_MARK = 7.5f;
            const int MID_DIVISION = 6;
            const double LEFT_OFFSET = 1.2;
            const double RIGHT_OFFSET = 2.8;
            const int VERTICAL_DIVISION = 3;
            const double UP_OFFSET = 1.85;
            const double DOWN_OFFSET = 2.15;

            int TopPosition = Globals.OUT_HEADSUP + Globals.OUT_UNIT;
            int ButtPosition = Globals.OUT_FHEIGHT - 2 * Globals.OUT_UNIT;
            int LeftPosition = Globals.OUT_UNIT;
            int RightPosition = Globals.OUT_FWIDTH - 2 * Globals.OUT_UNIT;
            int HorizontalPos, VerticalPos = 0;

            Vector2 LeftDoorBlocking = new Vector2(1 * Globals.OUT_UNIT,
                Globals.OUT_HEADSUP + DOOR_VERTICAL_MARK * Globals.OUT_UNIT - 4 * Globals.SCALAR);
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

                if (i < MID_DIVISION)
                    HorizontalPos = (int)((i + LEFT_OFFSET) * Globals.OUT_UNIT);
                else
                    HorizontalPos = (int)((i + RIGHT_OFFSET) * Globals.OUT_UNIT);

                if (i == 5 || i == 6)
                {   // Adding extra blocks to avoid player from going into cracks 
                    StaticBlockList.Add(new BlockInvis(new Vector2(HorizontalPos, TopPosition - Globals.OUT_UNIT)));
                    StaticBlockList.Add(new BlockInvis(new Vector2(HorizontalPos, ButtPosition + Globals.OUT_UNIT)));
                }

                StaticBlockList.Add(new BlockInvis(new Vector2(HorizontalPos, TopPosition)));
                StaticBlockList.Add(new BlockInvis(new Vector2(HorizontalPos, ButtPosition)));
            }
            for (int i = 0; i < Globals.RTILE_ROW; i++)
            {
                if (i < VERTICAL_DIVISION)
                    VerticalPos = Globals.OUT_HEADSUP + (int)((i + UP_OFFSET) * Globals.OUT_UNIT);
                if (i > VERTICAL_DIVISION)
                    VerticalPos = Globals.OUT_HEADSUP + (int)((i + DOWN_OFFSET) * Globals.OUT_UNIT);

                if (i == 2 || i == 4)
                {   // Adding extra blocks to avoid player from going into cracks 
                    StaticBlockList.Add(new BlockInvis(new Vector2(LeftPosition - Globals.OUT_UNIT, VerticalPos)));
                    StaticBlockList.Add(new BlockInvis(new Vector2(RightPosition + Globals.OUT_UNIT, VerticalPos)));
                }

                if(i < VERTICAL_DIVISION || i > VERTICAL_DIVISION)
                {
                    StaticBlockList.Add(new BlockInvis(new Vector2(LeftPosition, VerticalPos)));
                    StaticBlockList.Add(new BlockInvis(new Vector2(RightPosition, VerticalPos)));
                }
                
            }

            return StaticBlockList;
        }

        /// <summary>
        /// Generate a list of items in this room 
        /// that are not yet in the bag of the character.
        /// </summary>
        /// <param name="game">Game1 object</param>
        /// <returns>List of items in this room</returns>
        public List<IItem> GetItemList(Game1 Game)
        {
            List<IItem> ItemList = new List<IItem>();
            Vector2 Position; 

            int[,] RoomMat = game.roomCycler.GetCurrentRoomInfo().Arrangement;
            int ItemIndex; 

            for (int i = 0; i < RoomMat.GetLength(0); i++)
            {
                for (int j = 0; j < RoomMat.GetLength(1); j++)
                {
                    Position = Misc.Instance.PositionTranslate(i, j);
                    ItemIndex = General.IndexCoder.GetAuxIndex(RoomMat[i, j]);

                    
                    IItem itemToBeAdded = CreateItem(Game, Position, ItemIndex);

                    if(itemToBeAdded != null)
                        ItemList.Add(itemToBeAdded);
                    
                }
            }

            return ItemList; 
        }

        /// <summary>
        /// Generate a list of blocks
        /// that are in the main game area and are visible, 
        /// they may also have animations. 
        /// </summary>
        /// <param name="Game">Game1 object</param>
        /// <returns>List of blocks in this room</returns>
        public List<IBlock> GetBlockList(Game1 Game)
        {
            List<IBlock> BlockList = new List<IBlock>();
            int BlockColCount = 16; 
            Vector2 Position;

            int[,] RoomMat = game.roomCycler.GetCurrentRoomInfo().Arrangement;
            int BlockIndex; 

            for (int i = 0; i < RoomMat.GetLength(0); i++)
            {
                for (int j = 0; j < RoomMat.GetLength(1); j++)
                {
                    Position = Misc.Instance.PositionTranslate(i, j);
                    BlockIndex = General.IndexCoder.GetBlockIndex(RoomMat[i, j]);

                    switch (BlockIndex)
                    {
                        case Globals.STARE_BLOCK_1:
                            BlockList.Add(new StareBlock(Game, Position, BlockIndex / BlockColCount)); ;
                            break;
                        case Globals.STARE_BLOCK_2:
                            BlockList.Add(new StareBlock(Game, Position, BlockIndex / BlockColCount));
                            break;
                        default:
                            break;
                    }
                }
            }

            return BlockList; 
        }

        /// <summary>
        /// Generate a list of enemies in this room.
        /// </summary>
        /// <param name="Game">Game1 object</param>
        /// <returns>List of enemy in this room</returns>
        public List<IEnemy> GetEnemyList(Game1 Game)
        {
            List<IEnemy> EnemyList = new List<IEnemy>();
            Vector2 Position;

            int[,] RoomMat = game.roomCycler.GetCurrentRoomInfo().Arrangement;
            int EnemyIndex; 

            for (int i = 0; i < RoomMat.GetLength(0); i++)
            {
                for (int j = 0; j < RoomMat.GetLength(1); j++)
                {
                    Position = Misc.Instance.PositionTranslate(i, j);
                    EnemyIndex = General.IndexCoder.GetAuxIndex(RoomMat[i, j]);

                    switch (EnemyIndex)
                    {
                        case Globals.ENEMY_BEAD:
                            EnemyList.Add(new BloodBead(Game, Position));
                            break;
                        case Globals.ENEMY_STD:
                            EnemyList.Add(new IEnemySTD(Game, Position));
                            break;
                        default:
                            break;
                    }
                }
            }

            return EnemyList;
        }

        /// <summary>
        /// Given an index and other info, create the item
        /// </summary>
        /// <param name="Game">Game1 object</param>
        /// <param name="Position">Position of the item</param>
        /// <param name="ItemIndex">Index of item that indicates its type</param>
        /// <returns>An Iitem object of certain type</returns>
        public IItem CreateItem(Game1 Game, Vector2 Position, int ItemIndex)
        {
            switch (ItemIndex)
            {
                case Globals.ITEM_TORCH:
                    return new Torch(Game, Position); 
                case Globals.ITEM_LINKEN:
                    return new LinkenSphere(Game, Position);
                case Globals.ITEM_GOLD:
                    return new DroppedGold(Game, Position);
                case Globals.ITEM_NOTE_SO:
                    return new NoteSetOne(Game, Position);
                default:
                    return null; 
            }
        }
    }
}
