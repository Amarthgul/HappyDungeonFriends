using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace HappyDungeon
{
    public class Room
    {
        // ================================================================================
        // ======================== Consts and frequently used ============================
        // ================================================================================
        private const int ALL_MIGHT_DIV = 16;
        private const int ALL_MIGH_COUNT = 256;
        private const int DOOR_OPEN_INDEX = 1;
        private const int DOOR_LOCKED_INDEX = 2;
        private const int DOOR_MYS_INDEX = 3;
        private const int DOOR_HOLE_INDEX = 4;
        private const int UPDATE_DELAY = 4;
        private const int TRANSITION_STEP_Y = 8;
        private const int TRANSITION_STEP_X = 16;
        private const int EDGE_PRESERVE = 17;

        // ================================================================================
        // ========================= Infomation about the room ============================
        // ================================================================================
        public RoomInfo roomInfo;
        // Doors related. Left, Right, Up, Down as in global settings 
        private bool[] doorOpen = { false, false, false, false };    // Highest priority 
        private bool[] doorHole = { false, false, false, false };    // Second in command 
        private bool[] doorLocked = { false, false, false, false };    // Lowest priority  
        private bool[] doorMys = { false, false, false, false };
        private Dictionary<int, int> doorDirMapping = new Dictionary<int, int>(){
            {0, 1},
            {1, 2},
            {2, 0},
            {3, 3}
            };  // Mapping the direction Enum to the image of the doors 

        // ================================================================================
        // ============================ Textures and display ==============================
        // ================================================================================
        public Texture2D nextLevelTexture;
        public Texture2D levelTexture;     // Can be accessed to make map transitioning 
        private Texture2D roomOverlay; 
        private Texture2D blockAllMight;   // Containing all blocks 
        private Color defaultColor = Color.Black;
        private Color defaultTint = Color.White;

        // ================================================================================
        // ================= Abstract resources, parameters and states ====================
        // ================================================================================
        private SpriteBatch spriteBatch;
        private Game1 game;

        // Transition 
        private bool transtionFinished;
        private Stopwatch stopwatch = new Stopwatch();
        private long timer;
        private Globals.Direction TransDir;
        public Vector2[] nextLvPos = {
            new Vector2(-Globals.OUT_GWIDTH, Globals.OUT_HEADSUP ),     // Go through left door 
            new Vector2(Globals.OUT_GWIDTH, Globals.OUT_HEADSUP ),      // Go through right door 
            new Vector2(0, -Globals.OUT_GHEIGHT + Globals.OUT_HEADSUP), // Go through top door 
            new Vector2(0, Globals.OUT_FHEIGHT)   // Go through bottom door 
        }; // Position for next level 
        private Vector2 delta = new Vector2(0, 0);


        // Init 
        public Room(Game1 G)
        {
            game = G;

            spriteBatch = game.spriteBatch;
            
        }

        // ================================================================================
        // ================================= Methods ======================================
        // ================================================================================

        public void SetupRoom()
        {
            transtionFinished = true;

            doorHole = roomInfo.Holes;
            doorLocked = roomInfo.LockedDoors;
            doorMys = roomInfo.MysteryDoors;
            doorOpen = roomInfo.OpenDoors;

            generate();
            AlterTexture();
            UpdateDrawDoors();
            GenerateOverlay();
        }

        private void generate()
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

        // Overwrite the texture, add the border and tiles 
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

        private Texture2D GenerateOverlay()
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

        // Update the doors, used both in initialization and when new doors are being dynamically added 
        public void UpdateDrawDoors()
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
                if (doorLocked[(int)Dir]) Col = DOOR_LOCKED_INDEX;
                if (doorHole[(int)Dir]) Col = DOOR_HOLE_INDEX;
                if (doorOpen[(int)Dir]) Col = DOOR_OPEN_INDEX;
                if (doorMys[(int)Dir]) Col = DOOR_MYS_INDEX;
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
        }

        // For explosions to add a hole on the wall 
        public void AddHole(int Dir)
        {
            doorHole[Dir] = true;
            doorOpen[Dir] = false;
            UpdateDrawDoors();
        }

        // Open a mys door 
        public void OpenMysDoor(int Dir)
        {
            doorOpen[Dir] = true;
            doorMys[Dir] = false;
            UpdateDrawDoors();
        }

        public bool HasMysDoor(int Dir)
        {
            return doorMys[Dir];
        }

        public void SignalStart(Globals.Direction Direction)
        {
            stopwatch.Restart();
            transtionFinished = false;
            TransDir = Direction; 
        }

        public bool TransitionListener()
        {
            return transtionFinished;
        }

        public void Update()
        {
            timer += stopwatch.ElapsedMilliseconds;
            if (timer > UPDATE_DELAY)
            {
                if (TransDir == Globals.Direction.Up
                    || TransDir == Globals.Direction.Down) // up and down 
                    delta.Y += (TransDir == Globals.Direction.Up) ? TRANSITION_STEP_Y : -TRANSITION_STEP_Y;
                else
                    delta.X += (TransDir == Globals.Direction.Left) ? TRANSITION_STEP_X : -TRANSITION_STEP_X;
                timer = 0;
            }

            if ((delta.Y > 0 ? delta.Y : -delta.Y) > Globals.OUT_GHEIGHT ||
                (delta.X > 0 ? delta.X : -delta.X) > Globals.OUT_GWIDTH)
            {
                transtionFinished = true;
                game.gameState = Globals.GameStates.Running;
            }
        }

        public void Draw()
        {

            if(game.gameState == Globals.GameStates.RoomTransitioning)
            {
                spriteBatch.Draw(nextLevelTexture, nextLvPos[(int)TransDir] + delta, null, defaultTint,
                    0f, Vector2.Zero, Globals.SCALAR, SpriteEffects.None, Globals.MAP_LAYER);

                spriteBatch.Draw(levelTexture, new Vector2(0, Globals.OUT_HEADSUP) + delta, null, defaultTint, 
                    0f, Vector2.Zero, Globals.SCALAR, SpriteEffects.None, Globals.MAP_LAYER);
            }
            else
            {
                spriteBatch.Draw(levelTexture, new Vector2(0, Globals.OUT_HEADSUP), null, defaultTint, 
                    0, Vector2.Zero, Globals.SCALAR, SpriteEffects.None, Globals.MAP_LAYER);

                spriteBatch.Draw(roomOverlay, new Vector2(0, Globals.OUT_HEADSUP), null, defaultTint,
                    0, Vector2.Zero, Globals.SCALAR, SpriteEffects.None, Globals.MAP_OVERLAY);
            }

        }

    }
}
