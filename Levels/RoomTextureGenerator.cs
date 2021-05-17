using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace HappyDungeon.Levels
{
    class RoomTextureGenerator
    {
        private const int ALL_MIGHT_DIV = 16;
        private const int ALL_MIGH_COUNT = 256;
        private const int DOOR_OPEN_INDEX = 1;
        private const int DOOR_LOCKED_INDEX = 2;
        private const int DOOR_MYS_INDEX = 3;
        private const int DOOR_HOLE_INDEX = 4;
        private const int EDGE_PRESERVE = 17;

        private Dictionary<int, int> doorDirMapping = new Dictionary<int, int>(){
            {0, 1},
            {1, 2},
            {2, 0},
            {3, 3}
            };  // Mapping the direction Enum to the image of the doors 

        private SpriteBatch spriteBatch;
        private Game1 game;

        public RoomInfo roomInfo;

        public Texture2D levelTexture;     // Can be accessed to make map transitioning 
        private Texture2D roomOverlay;
        private Texture2D blockAllMight;   // Containing all blocks 

        private Color defaultColor = Color.Black;

        public RoomTextureGenerator(Game1 G, RoomInfo RI)
        {
            game = G;
            roomInfo = RI;

            spriteBatch = game.spriteBatch;

            GenerateTexture();
        }

        private void GenerateTexture()
        {
            Load();
            AlterTexture();
            UpdateDrawDoors();
            GenerateOverlay();
        }

        private void Load()
        {
            levelTexture = TextureFactory.Instance.GenerateTexture(game.GraphicsDevice,
                Globals.ORIG_GWIDTH, Globals.ORIG_GHEIGHT, pixel => defaultColor);

            blockAllMight = TextureFactory.Instance.blockAllMight.texture;
        }

        /// <summary>
        /// Crop the target block from the all might texture.
        /// Note that this only applies to static images, moving blocks are in other classes. 
        /// </summary>
        /// <param name="index">Block index</param>
        /// <returns>Texture of the block</returns>
        private Texture2D getBlockByIndex(int index)
        {
            Texture2D block = TextureFactory.Instance.GenerateTexture(game.GraphicsDevice,
                Globals.ORIG_UNIT, Globals.ORIG_UNIT, pixel => defaultColor);
            int row = index / ALL_MIGHT_DIV;
            int col = index % ALL_MIGHT_DIV;

            Rectangle SourceRectangle = new Rectangle(
                col * Globals.ORIG_UNIT,
                row * Globals.ORIG_UNIT,
                Globals.ORIG_UNIT,
                Globals.ORIG_UNIT);

            Color[] data = new Color[SourceRectangle.Width * SourceRectangle.Height];
            blockAllMight.GetData<Color>(0, SourceRectangle, data, 0, data.Length);
            block.SetData(data);

            return block;
        }

        /// <summary>
        /// Overwrite the texture, add the border and tiles 
        /// </summary>
        private void AlterTexture()
        {
            Texture2D Border = TextureFactory.Instance.roomBorder
                [Globals.RND.Next() % TextureFactory.Instance.roomBorder.Length].texture;
            int CountBorder = Border.Width * Border.Height;
            Color[] RawDataBorder = new Color[CountBorder];
            int[,] mapMatrix = roomInfo.Arrangement;
            int defaultBlockIndex = roomInfo.DefaultBlock;

            Border.GetData<Color>(RawDataBorder);
            levelTexture.SetData(0, new Rectangle(0, 0, Border.Width, Border.Height), RawDataBorder, 0, CountBorder);

            // Add tiles 
            for (int r = 0; r < Globals.RTILE_ROW; r++)
            {
                for (int c = 0; c < Globals.RTILE_COLUMN; c++)
                {
                    Vector2 StartPoint = new Vector2(Globals.ORIG_BORDER + c * Globals.ORIG_UNIT,
                        Globals.ORIG_BORDER + r * Globals.ORIG_UNIT);

                    int TileTypeNow = (mapMatrix[r, c] >= 0 && mapMatrix[r, c] < ALL_MIGH_COUNT) ?
                        mapMatrix[r, c] : defaultBlockIndex;
                    Texture2D TextureNow = getBlockByIndex(TileTypeNow);

                    int CountNow = TextureNow.Width * TextureNow.Height;
                    Color[] RawDataNow = new Color[CountNow];
                    TextureNow.GetData<Color>(RawDataNow);

                    // Paste the data of the tiles 
                    levelTexture.SetData(0, new Rectangle(
                        (int)StartPoint.X, (int)StartPoint.Y,
                        Globals.ORIG_UNIT, Globals.ORIG_UNIT),
                        RawDataNow, 0, CountNow);
                }
            }

        }

        public Texture2D GenerateOverlay()
        {
            Texture2D TranspBlock = TextureFactory.Instance.GenerateTexture(game.GraphicsDevice,
                Globals.ORIG_GWIDTH - EDGE_PRESERVE * 2, Globals.ORIG_GHEIGHT - EDGE_PRESERVE * 2,
                pixel => Color.Transparent);

            roomOverlay = TextureFactory.Instance.GenerateTexture(game.GraphicsDevice,
                Globals.ORIG_GWIDTH, Globals.ORIG_GHEIGHT, pixel => Color.Transparent);

            // First copy the level texture 
            Color[] SrcData = new Color[levelTexture.Width * levelTexture.Height];
            levelTexture.GetData<Color>(SrcData);
            roomOverlay.SetData(SrcData);

            // Then dig a hole in the middle to make it reansparent 
            Color[] TranspData = new Color[TranspBlock.Width * TranspBlock.Height];
            TranspBlock.GetData<Color>(TranspData);

            roomOverlay.SetData(0, new Rectangle(EDGE_PRESERVE, EDGE_PRESERVE,
                TranspBlock.Width, TranspBlock.Height), TranspData, 0, TranspData.Length);

            return roomOverlay;
        }

        /// <summary>
        /// Update the doors, used both in initialization and 
        /// when new doors are being dynamically added. 
        /// </summary>
        /// <returns>Updated texture</returns>
        public Texture2D UpdateDrawDoors()
        {
            Texture2D doors = TextureFactory.Instance.roomDoors.texture;
            int DoorSizeUnit = Globals.ORIG_UNIT * 2;

            int HorizontalPos = Globals.ORIG_GWIDTH / 2 - Globals.ORIG_UNIT;
            int VerticalPos = Globals.ORIG_GHEIGHT / 2 - Globals.ORIG_UNIT;
            int TopPosition = 0;
            int ButtPosition = Globals.ORIG_GHEIGHT - Globals.ORIG_BORDER;

            int LeftPosition = 0;
            int RightPosition = Globals.ORIG_GWIDTH - Globals.ORIG_BORDER;

            int Col = 0;
            Rectangle SourceRectangle, DestRectangle = new Rectangle(0, 0, DoorSizeUnit, DoorSizeUnit);


            foreach (Globals.Direction Dir in Globals.FourDirIter)
            {
                // Pre-launch check to pick up proper clipping area 
                Col = 0;
                if (roomInfo.LockedDoors[(int)Dir]) Col = DOOR_LOCKED_INDEX;
                if (roomInfo.Holes[(int)Dir]) Col = DOOR_HOLE_INDEX;
                if (roomInfo.OpenDoors[(int)Dir]) Col = DOOR_OPEN_INDEX;
                if (roomInfo.MysteryDoors[(int)Dir]) Col = DOOR_MYS_INDEX;
                if (Col == 0) continue;
                switch (Dir)
                {
                    case Globals.Direction.Left:
                        DestRectangle.X = LeftPosition;
                        DestRectangle.Y = VerticalPos;
                        break;
                    case Globals.Direction.Right:
                        DestRectangle.X = RightPosition;
                        DestRectangle.Y = VerticalPos;
                        break;
                    case Globals.Direction.Up:
                        DestRectangle.X = HorizontalPos;
                        DestRectangle.Y = TopPosition;
                        break;
                    case Globals.Direction.Down:
                        DestRectangle.X = HorizontalPos;
                        DestRectangle.Y = ButtPosition;
                        break;
                    default: break; // Not possible 
                }
                SourceRectangle = new Rectangle(
                    Col * DoorSizeUnit, doorDirMapping[(int)Dir] * DoorSizeUnit,
                    DoorSizeUnit, DoorSizeUnit);

                // Copy 
                Color[] data = new Color[SourceRectangle.Width * SourceRectangle.Height];
                doors.GetData<Color>(0, SourceRectangle, data, 0, data.Length);

                // Paste 
                levelTexture.SetData(0, DestRectangle,
                        data, 0, data.Length);
            }

            return levelTexture;
        }

        public Texture2D GetRoomTexture()
        {
            GenerateTexture();
            return levelTexture;
        }
    }
}
