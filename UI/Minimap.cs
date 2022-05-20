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
    /// <summary>
    /// For the display and update of minimap.
    /// Not that the minimap can be displayed at 2 locations, 
    /// with each having 2 components and updating, 
    /// the location calculation is rather (unnecessarily) complex. 
    /// </summary>
    public class Minimap : IUIElement
    {
        // ================================================================================
        // ======================== Consts and frequently used ============================
        // ================================================================================ 
        private const bool _DEVMODE = false;
        private const int PLAYER_NOTATION_SIZE = 4;
        private const int UNIT_WIDTH = 12;   // Width of a room unit's texture 
        private const int UNIT_HEIGHT = 7;   // Height of a room unit's texture
        private const int WHOLE_WIDTH = 14;  // Width of the sapce given to a room unit 
        private const int WHOLE_HEIGHT = 9;  // Height of the space given to a room unit 
        private const int BRIDGE_LEN_0 = 2;
        private const int BRIDGE_LEN_1 = 1;
        private const int DRAW_POSITION_X = 8 * Globals.SCALAR;
        private const int DRAW_POSITION_Y = 6 * Globals.SCALAR;
        private const int DISPLAY_REGION_X = 3 * WHOLE_WIDTH;
        private const int DISPLAY_REGION_Y = 3 * WHOLE_HEIGHT;
        public const double SIZE_RATIO = (double)WHOLE_HEIGHT / (double)(Globals.OUT_FHEIGHT);
        public const double MC_WIDTH_RATIO = (UNIT_WIDTH ) / (double)(Globals.OUT_GWIDTH);
        public const double MC_HEIGHT_RATIO = (UNIT_HEIGHT-1) / (double)(Globals.OUT_GHEIGHT);
        private const double TRANSITION_STEP_Y = 8 * SIZE_RATIO;  // From class Level
        private const double TRANSITION_STEP_X = 16 * SIZE_RATIO;

        private Game1 game; 
        private SpriteBatch spriteBatch; 
        private LevelCycling roomCycler;

        // ================================================================================
        // ============================ Textures and sprites ==============================
        // ================================================================================
        private Texture2D miniRooms;          // 4x4 rooms texture 
        private Texture2D minimap;            // The entire map 
        private Texture2D singleRoom;         // 1 single room 
        private Texture2D horizontalBridge;   
        private Texture2D verticalBridge;
        private GeneralSprite playerNotation; // I don't have a reason making it animated, probably just b/c I can

        // ================================================================================
        // ============================ Stats and parameters ==============================
        // ================================================================================
        // The size of the current maps 
        private int roomRowCount;
        private int roomColCount;

        // Regarding the drawing of the texture 
        private int extraDisplayMode = 0; 
        public int[] currentFocusIndex { get; set; }
        private double clipX = 0;
        private double clipY = 0;
        private Vector2 tabDisplayPos;
        private Vector2 playerOffset = new Vector2(0, 0);        // Offset for replicating player movement
        private Vector2 transOffset = new Vector2(0, 0);         // Offset for transition animation 
        private Vector2 playerEdgeRelocate = new Vector2(0, 0);  // In case the player is in an edge room 
        private Vector2 tabOffsetRatio = new Vector2(0, 0);      // Offset the unexplored to avoid spoilers (range 0 - 1 )
        private Vector2 tabOffset;
        private Vector2 tabClickDelta;
        private Vector2 tabClickLastRecord; 
        private bool tabClicking = false;
        private bool tabClickSameSession = false; 

        //Misc 
        private float layer = Globals.UI_MINIMAP; 
        private Color defaultTint = Color.White;         // Draw() method tint 
        private Color transp = Color.Transparent;        // Texture generation placeholder colod 
        private Color fillColor = new Color(30, 28, 26); // Bridge fill color 

        //--------------------------------------------------------------------------------------

        // Init
        public Minimap(Game1 G)
        {
            game = G;

            spriteBatch = game.spriteBatch; 
            roomCycler = game.roomCycler;

            roomRowCount = roomCycler.currentMapSet.GetLength(0);
            roomColCount = roomCycler.currentMapSet.GetLength(1);

            tabClickDelta = new Vector2(0, 0);
            tabClickLastRecord = new Vector2(0, 0);

            InitlizeMinimap();
        }

        /// <summary>
        /// Create and initilize all the textures and matrices 
        /// </summary>
        private void InitlizeMinimap()
        {
            ImageFile PLN = TextureFactory.Instance.playerNote;
            int[] CurrentIndex = roomCycler.GetCurrentLocationIndex();

            miniRooms = TextureFactory.Instance.miniRooms.texture; 
            minimap = TextureFactory.Instance.GenerateTexture(game.GraphicsDevice, 
                roomColCount * WHOLE_WIDTH, roomRowCount * WHOLE_HEIGHT, pixel => transp);

            // For copy-paste rooms 
            singleRoom = TextureFactory.Instance.GenerateTexture(game.GraphicsDevice, 
                UNIT_WIDTH, UNIT_HEIGHT, pixel => fillColor);

            // Bridges 
            horizontalBridge = TextureFactory.Instance.GenerateTexture(game.GraphicsDevice, 
                BRIDGE_LEN_0, BRIDGE_LEN_1, pixel => fillColor);
            verticalBridge = TextureFactory.Instance.GenerateTexture(game.GraphicsDevice, 
                BRIDGE_LEN_0, BRIDGE_LEN_0, pixel => fillColor);

            playerNotation = new GeneralSprite(PLN.texture, PLN.C, PLN.R, Globals.WHOLE_SHEET, PLN.C * PLN.R, Globals.UI_MINIMAP_PLAYER);

            tabDisplayPos = new Vector2(Globals.OUT_FWIDTH / 2 - minimap.Width / 2 * Globals.SCALAR,
                Globals.OUT_FHEIGHT / 2 - minimap.Height / 2 * Globals.SCALAR + Globals.OUT_HEADSUP);
            tabOffsetRatio = new Vector2(CurrentIndex[1] / (float)roomRowCount + 1 / (float)roomRowCount -.5f,
                CurrentIndex[0] / (float)roomColCount + 1 / (float)roomColCount -.5f);
            RecalTabOffset();
        }

        /// <summary>
        /// Randomly select a room block from the texture set. 
        /// </summary>
        /// <param name="RoomType">Type of room</param>
        /// <returns>Texture of one of the room in given type.</returns>
        private Texture2D SelectRoomBlock(Globals.RoomTypes RoomType)
        {
            int RandLimit = 4; // 4 rand choices for each type 
            int col, row;
            Color[] RoomData = new Color[singleRoom.Width * singleRoom.Height];
            Rectangle SrcRectangle; 

            // Clear texture first 
            singleRoom = TextureFactory.Instance.GenerateTexture(game.GraphicsDevice, 
                UNIT_WIDTH, UNIT_HEIGHT, pixel => fillColor);

            // Determine source rectangle position 
            row = (Globals.RND.Next() % RandLimit) * UNIT_WIDTH; 
            switch (RoomType)
            {
                case Globals.RoomTypes.Boss:
                    col = 0; 
                    break;
                case Globals.RoomTypes.Merchant:
                    col = 2 * UNIT_HEIGHT; 
                    break;
                case Globals.RoomTypes.Normal:
                    col = UNIT_HEIGHT;
                    break;
                case Globals.RoomTypes.Start:
                    col = 3 * UNIT_HEIGHT;
                    break;
                default:
                    col = UNIT_HEIGHT; // Not really possible to get there 
                    break;
            }
            SrcRectangle = new Rectangle(row, col, UNIT_WIDTH, UNIT_HEIGHT);

            // Copy and paste 
            miniRooms.GetData<Color>(0, SrcRectangle, RoomData, 0, RoomData.Length);
            singleRoom.SetData(RoomData);

            return singleRoom; 
        }

        /// <summary>
        /// Add the connection between 2 minimap rooms. 
        /// </summary>
        /// <param name="Direction">Direction of that bridge</param>
        private void AddBridge(Globals.Direction Direction)
        {
            if (game.gameLevel == Globals.GameLevel.Bliss) return; // Bliss need no bridge on minimap 

            const int HORIZONTAL_OFFST = 6;
            const int VERTICAL_OFFSET = 4;

            Texture2D Bridge = (Direction == Globals.Direction.Up || Direction == Globals.Direction.Down) ?
                verticalBridge : horizontalBridge;
            Color[] BridgeData = new Color[Bridge.Width * Bridge.Height];
            Bridge.GetData(BridgeData);

            int VerticalPosition = currentFocusIndex[0] * WHOLE_HEIGHT;
            int HorizontalPosition = currentFocusIndex[1] * WHOLE_WIDTH;

            if (Direction == Globals.Direction.Left || Direction == Globals.Direction.Right)
            {
                HorizontalPosition += ((Direction == Globals.Direction.Left) ? 0 : WHOLE_WIDTH) - 1;
                VerticalPosition += VERTICAL_OFFSET;
            }
            else if (Direction == Globals.Direction.Up || Direction == Globals.Direction.Down)
            {
                VerticalPosition += ((Direction == Globals.Direction.Up) ? 0 : WHOLE_HEIGHT) - 1;
                HorizontalPosition += HORIZONTAL_OFFST;
            }

            minimap.SetData(0, new Rectangle(HorizontalPosition, VerticalPosition, Bridge.Width, Bridge.Height),
                BridgeData, 0, BridgeData.Length);
        }

        /// <summary>
        /// Re-calculate the real tab display offset
        /// </summary>
        private void RecalTabOffset()
        {
            tabOffset = new Vector2(tabOffsetRatio.X * minimap.Width * Globals.SCALAR ,
                tabOffsetRatio.Y * minimap.Height * Globals.SCALAR );
        }

        // ================================================================================
        // =============================== Public Methods =================================
        // ================================================================================

        /// <summary>
        /// When loading game from previously saved, this method is called to reconstruct 
        /// the entire minimap based on all rooms that has been explored. 
        /// </summary>
        public void RedrawEntireMinimap()
        {
            int[] BackupIndex = currentFocusIndex; 

            for (int i = 0; i < game.mapSize; i++)
            {
                for (int j = 0; j < game.mapSize; j++)
                {
                    if (game.roomCycler.currentMapSet[i, j] != null && game.roomCycler.currentMapSet[i, j].Explored)
                    {
                        int[] currentIndex = new int[] { i, j };
                        currentFocusIndex = currentIndex; 
                        FlagExplored(currentIndex);
                    }
                }
            }

            currentFocusIndex = BackupIndex; 
        }

        /// <summary>
        /// Re-calculate the ratio of the tab display offset.
        /// </summary>
        /// <param name="Direction">Direction of the new room</param>
        public void RecalTabOffsetRatio(Globals.Direction Direction)
        {
            switch (Direction)
            {
                case Globals.Direction.Up:
                    tabOffsetRatio.Y -= 1.0f / roomColCount; 
                    break;
                case Globals.Direction.Down:
                    tabOffsetRatio.Y += 1.0f / roomColCount;
                    break;
                case Globals.Direction.Left:
                    tabOffsetRatio.X -= 1.0f / roomRowCount;
                    break;
                case Globals.Direction.Right:
                    tabOffsetRatio.X += 1.0f / roomRowCount;
                    break;
                default:
                    break;
            }
            RecalTabOffset();
        }

        /// <summary>
        /// Mark a room as explored and make it visible in minimap. 
        /// </summary>
        /// <param name="Index"></param>
        public void FlagExplored(int[] Index)
        {
            RoomInfo CurrentRoomInfo = game.roomCycler.currentMapSet[Index[0], Index[1]];
            Color[] RoomData = new Color[singleRoom.Width * singleRoom.Height];

            int PosX = Index[1] * WHOLE_WIDTH + 1;
            int PosY = Index[0] * WHOLE_HEIGHT + 1;

            // Iterate throught 4 directions, if there's a door, then add a bridge 
            foreach (Globals.Direction Dir in Globals.FourDirIter)
            {
                if (CurrentRoomInfo.LockedDoors[(int)Dir] || CurrentRoomInfo.MysteryDoors[(int)Dir] || CurrentRoomInfo.OpenDoors[(int)Dir])
                    AddBridge(Dir);
            }

            SelectRoomBlock(CurrentRoomInfo.Type); 
            singleRoom.GetData(RoomData);

            minimap.SetData(0, new Rectangle(PosX, PosY,
                singleRoom.Width, singleRoom.Height), RoomData,
                0, RoomData.Length);
            
        }

        /// <summary>
        /// Set clipping area of the map. 
        /// </summary>
        /// <param name="Index">Location of the pivot</param>
        public void SetPivot(int[] Index)
        {
            currentFocusIndex = Index;

            // Dealing with outlier cases when there's no more area to clip or move
            if (currentFocusIndex[0] >= roomCycler.currentMapSet.GetLength(0) - 1)
            {
                clipY = (roomCycler.currentMapSet.GetLength(0) - 2) * WHOLE_HEIGHT;
            }
            else if (currentFocusIndex[0] <= 0)
            {
                playerEdgeRelocate.Y = -UNIT_HEIGHT * Globals.SCALAR - PLAYER_NOTATION_SIZE * 2;
                clipY = 0;
            }
            else
            {
                clipY = (currentFocusIndex[0] - 1) * WHOLE_HEIGHT;
                playerEdgeRelocate.Y = 0;
            }

            if (currentFocusIndex[1] >= roomCycler.currentMapSet.GetLength(1) - 1)
            {
                clipX = (roomCycler.currentMapSet.GetLength(1) - 2) * WHOLE_WIDTH;

            }
            else if (currentFocusIndex[1] <= 0)
            {
                clipX = 0;
                playerEdgeRelocate.X = -UNIT_WIDTH * Globals.SCALAR;
            }
            else
            {
                clipX = (currentFocusIndex[1] - 1) * WHOLE_WIDTH;
                playerEdgeRelocate.X = 0;
            }

        }

        /// <summary>
        /// Update for IUIElement.
        /// </summary>
        public void Update()
        {
            playerNotation.Update();

            switch (game.gameState)
            {
                case Globals.GameStates.RoomTransitioning:
                    Globals.Direction Direction = game.transitionDir;

                    switch (Direction)
                    {
                        case Globals.Direction.Up:
                            if (currentFocusIndex[0] > 1)
                                transOffset.Y -= (float)(TRANSITION_STEP_Y);
                            break;
                        case Globals.Direction.Down:
                            if (currentFocusIndex[0] > 0)
                                transOffset.Y += (float)(TRANSITION_STEP_Y);
                            break;
                        case Globals.Direction.Right:
                            if (currentFocusIndex[1] > 0)
                                transOffset.X += (float)(TRANSITION_STEP_X);
                            break;
                        case Globals.Direction.Left:
                            if (currentFocusIndex[1] > 1)
                                transOffset.X -= (float)(TRANSITION_STEP_X);
                            break;
                        default:
                            break;
                    }
                    break;

                case Globals.GameStates.Running:
                    Vector2 PlayerPos = game.mainChara.position;
                    Vector2 GameAreaStart = new Vector2(Globals.OUT_BORDER,
                        Globals.OUT_HEADSUP + Globals.OUT_BORDER);
                    Vector2 RealOffset = PlayerPos - GameAreaStart;

                    playerOffset.X = (float)(RealOffset.X * MC_WIDTH_RATIO * Globals.SCALAR) ;
                    playerOffset.Y = (float)(RealOffset.Y * MC_HEIGHT_RATIO * Globals.SCALAR) ;

                    transOffset = new Vector2(0, 0);
                    break;
                default:
                    break; 
            }

        }

        /// <summary>
        /// Toggle options on when encountering a LMB click while tab display is on.
        /// </summary>
        /// <param name="NewPosition">Position of the cursor</param>
        public void TabClick(Vector2 NewPosition)
        {
            
            if (tabClicking == true)
            {
                tabClickDelta += (NewPosition - tabClickLastRecord);
                tabClickLastRecord = NewPosition;
            } else
            {
                tabClicking = true;
                tabClickLastRecord = NewPosition;
            }

        }

        /// <summary>
        /// When LMB is not being pressed. 
        /// </summary>
        public void TabClickRelease()
        {
            if (extraDisplayMode == 1)
            {
                tabClicking = false;
            }
            else
            {
                tabClickDelta = new Vector2(0, 0);
            }
            
        }
        
        /// <summary>
        /// Top-left corner minimap. 
        /// This part is always on. 
        /// </summary>
        public void Draw()
        {
            Rectangle MiniMapSrcClip = new Rectangle((int)(clipX + transOffset.X), (int)(clipY + transOffset.Y),
                DISPLAY_REGION_X, DISPLAY_REGION_Y);
            Vector2 PlayerBoxLocation = -3 * transOffset + new Vector2(
                DRAW_POSITION_X + (WHOLE_WIDTH + 1) * Globals.SCALAR,
                DRAW_POSITION_Y + (WHOLE_HEIGHT + 1)  * Globals.SCALAR)
                + playerEdgeRelocate + playerOffset;


            // The minimap 
            spriteBatch.Draw(minimap, new Vector2(DRAW_POSITION_X, DRAW_POSITION_Y),
                MiniMapSrcClip, defaultTint, 0f, Vector2.Zero, Globals.SCALAR, SpriteEffects.None, layer);

            // The tiny box denoting player's position 
            playerNotation.Draw(spriteBatch, PlayerBoxLocation, defaultTint);
        }

        /// <summary>
        /// For special display. 
        /// Tab display should only be avilable in running state. 
        /// </summary>
        /// <param name="DisplayMode">Either tab or a separated map view</param>
        public void DrawWholeMap(int DisplayMode)
        {
            extraDisplayMode = DisplayMode; 
            if (DisplayMode == 1)
                {
                Vector2 PlayerDot = new Vector2(
                (currentFocusIndex[1] * WHOLE_WIDTH + 1) * Globals.SCALAR + tabDisplayPos.X,
                (currentFocusIndex[0] * WHOLE_HEIGHT + 1) * Globals.SCALAR + tabDisplayPos.Y
                ) + playerOffset;
                

                spriteBatch.Draw(minimap, tabDisplayPos - tabOffset + tabClickDelta,
                    null, defaultTint, 0f, Vector2.Zero, Globals.SCALAR, SpriteEffects.None, layer);

                playerNotation.Draw(spriteBatch, PlayerDot - tabOffset + tabClickDelta, defaultTint);

                if (_DEVMODE)
                {
                    DrawRectangle OriginalRect = new DrawRectangle(game.GraphicsDevice, spriteBatch, 
                        new Rectangle((int)tabDisplayPos.X, (int)tabDisplayPos.Y,
                        roomColCount * WHOLE_WIDTH * Globals.SCALAR, roomRowCount * WHOLE_HEIGHT * Globals.SCALAR),
                        Color.Green);
                    OriginalRect.Draw();

                    DrawRectangle NewRect = new DrawRectangle(game.GraphicsDevice, spriteBatch,
                        new Rectangle((int)(tabDisplayPos.X - tabOffset.X), (int)(tabDisplayPos.Y - tabOffset.Y),
                        roomColCount * WHOLE_WIDTH * Globals.SCALAR, roomRowCount * WHOLE_HEIGHT * Globals.SCALAR),
                        Color.Red);
                    NewRect.Draw();
                }
            }
            
        }


    }
}
