using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace HappyDungeon
{
    /// <summary>
    /// This is the game class. 
    /// My lazy ass did not rename it, and when I want to , it's too late. 
    /// </summary>
    public class Game1 : Game
    {
        /// <summary>
        /// Dev options. 
        /// Please note that _DEVMODE is the master trigger. All other triggers are 
        /// regarded as not enabled unless _DEVMODE is on. 
        /// Also note that Dev mode is not the same as BEGUGGING in the Globals. 
        /// Dev mode aims in tweaking features, while DEBUGGING is for coding. 
        /// </summary>
        private bool _DEVMODE = false;       // Root flag for all other dev options 
        private bool _ENABLE_FOW = true;   // Enable fog of war
        private bool _SHOW_BOX = true;     // Draw boundary boxes for objects 

        // ================================================================================
        // ============================= Abstract resources ===============================
        // ================================================================================
        public GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;

        // ================================================================================
        // ========================= Game states and parameters ===========================
        // ================================================================================

        public LoadAndSave loadAndSave { get; set; } 

        public Globals.GameStates gameState { get; set; }
        public int mapSize = 9;
        public bool virgin { get; set; } // Some UI display differently depend on this

        // --------------------------------------------------------------------------------
        // -------------------------------- Player changeable -----------------------------
        public float[] volumes { get; set; }                // Sound volume
        public Globals.GameLevel gameLevel { get; set; }    // 3 levels: Delight, Joy, Bliss

        public Globals.KeyboardControl keyboardControl { get; set; }
        public Globals.Language gameLanguage { get; set; }  
        // Only English, add others if you have the patience of drawing sprites for texts 
        public Globals.GameDifficulty difficulty { get; set; }

        // ================================================================================
        // ====================== Character, items, enemies, etc. =========================
        // ================================================================================
        // Main character 
        public MC mainChara;
        public List<IEnemy> enemyList { get; set; }           // Refreshes with every room
        public List<IItem> collectibleItemList { get; set; }  // Refreshes with every room 
        public List<IItem> bagItemList { get; set; }          // Stays same in each game session 
        public List<IProjectile> projList { get; set; }       // Cleans after entering new room
        public int goldCount { get; set; }
        public int gameScore { get; set; }

        // ================================================================================
        // =================== Maps, rooms, and environmental effects =====================
        // ================================================================================
        public Generator mapGenerator;
        public FoW fogOfWar;
        public LevelCycling roomCycler;
        public Room currentRoom;
        public List<IBlock> staticBlockList;
        public List<IBlock> dynamicBlockList;
        public Globals.Direction transitionDir;

        // ================================================================================
        // ============================= UIs and Screens ==================================
        // ================================================================================
        public HeadsupDisplay headsupDisplay;  // The bar on top
        public GeneralDisplay generalDisplay;  // Such as title screen, bag, settings, etc.
        public Minimap minimap;                // Map on top-left and when pressing tab 
        public MouseCursor cursor;             // Replacement for displayMouse option 
        public ScreenFX screenFX;              // All minor special effects

        public OverlayInput overlay;

        public int displayWholeMinimap { set; get; }  // For tab and other minimap display 

        public bool[] transitionProgress { get; set; }


        // ================================================================================
        // =========================== Controls and logic =================================
        // ================================================================================
        List<Object> controllerList;
        public SpellSlots spellSlots; 

        // ================================================================================
        // ================================= Methods ======================================
        // ================================================================================
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Load all classes
        /// </summary>
        /// <param name="mode"></param>
        /// <returns>0 if all loaded successfually</returns>
        public int LoadClasses(int mode)
        {
            gameLevel = Globals.GameLevel.Delight;
            gameLanguage = Globals.Language.English;
            

            TextBridge.Instance.Init(this);

            loadAndSave = new LoadAndSave(this);   // Proceeds the initialization of displays
            // Load and save uses the text database,
            // and thus need to be refreshed afterwards 
            TextRefresh(); 

            roomCycler = new LevelCycling(mapSize, gameLevel);

            currentRoom = new Room(this);
            currentRoom.roomInfo = roomCycler.GetStart();
            currentRoom.SetupRoom();

            bagItemList = new List<IItem>();

            ReloadLists();

            mainChara = new MC(this);

            spellSlots = new SpellSlots(this);

            fogOfWar = new FoW(this);
            fogOfWar.SetDevMode(_DEVMODE);

            minimap = new Minimap(this);
            minimap.SetPivot(roomCycler.GetCurrentLocationIndex());
            minimap.FlagExplored(roomCycler.GetCurrentLocationIndex());
            displayWholeMinimap = 0;

            overlay = new OverlayInput(this);

            screenFX = new ScreenFX(this);
            headsupDisplay = new HeadsupDisplay(this);
            generalDisplay = new GeneralDisplay(this);
            cursor = new MouseCursor(this);

            transitionProgress = new bool[] { false, false, false };
            goldCount = 0;

            return 0; 
        }

        /// <summary>
        /// Reset current game, either into a new room, refresh current room or hard restart.
        /// </summary>
        /// <param name="Direction">Deciding the mode, 0-3 for room transition</param>
        public void reset(int Direction)
        {
            
            if (Direction <= 3) // A room transition 
            {
                transitionDir = (Globals.Direction)Direction;

                staticBlockList = new List<IBlock>();

                Room NextRoom = new Room(this);
                NextRoom.roomInfo = roomCycler.GetNextRoom(transitionDir);
                NextRoom.SetupRoom();
                
                currentRoom.nextLevelTexture = NextRoom.levelTexture;
                currentRoom.SignalStart(transitionDir);

                minimap.RecalTabOffsetRatio(transitionDir);

                gameState = Globals.GameStates.RoomTransitioning;

            }
            else if (Direction == 4)  // For death respawn in the same room
            {
                
            }
            else if (Direction == 5)  // Total reset 
            {
                LoadClasses(0);
            }
            else if (Direction == Globals.LOAD_SAVED) // Load game from save file 
            {
                currentRoom.roomInfo = roomCycler.GetCurrentRoomInfo(); 
                currentRoom.SetupRoom();

                ReloadLists();

                mainChara.Refresh(this);

                minimap = new Minimap(this);
                minimap.SetPivot(roomCycler.GetCurrentLocationIndex());
                minimap.FlagExplored(roomCycler.GetCurrentLocationIndex());
                minimap.RedrawEntireMinimap();
                displayWholeMinimap = 0;
            }

        }

        /// <summary>
        /// Text database uses bool flags to create some of the descriptions, 
        /// this method is for refreshing the database for each game progressions. 
        /// </summary>
        public void TextRefresh()
        {
            TextBridge.Instance.Refresh();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            keyboardControl = Globals.KeyboardControl.RPG;

            gameState = Globals.GameStates.TitleScreen;
            // Setup window size 
            graphics.PreferredBackBufferWidth = Globals.OUT_FWIDTH;
            graphics.PreferredBackBufferHeight = Globals.OUT_FHEIGHT;
            graphics.ApplyChanges();

            controllerList = new List<Object>();
            controllerList.Add(new KeyboardController(this));
            controllerList.Add(new MouseController(this));

            virgin = true;
            difficulty = Globals.GameDifficulty.Normal;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            volumes = new float[] { 1f, 1f, 1f };

            TextureFactory.Instance.LoadAll(Content);
            SoundFX.Instance.LoadAll(Content);
            SoundFX.Instance.SetVolume(volumes);

            LoadClasses(0);
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Flag the game to its true color if the player has started to play 
            if (virgin) {
                if (gameState == Globals.GameStates.Running)
                    virgin = false;
            }

            // Note that keyboard actually don;t update if overlay is enabled  
            foreach (IController controller in controllerList)
            {
                controller.Update();
            }
            // Mouse and cursor always updates, but is dependent on game state in their Update method
            cursor.Update();
            screenFX.Update();
            SoundFX.Instance.UpdateBGM();

            if (overlay.IsEnabled())
            {
                overlay.Update(); 
            }
            else
            {
                switch (gameState)
                {
                    case Globals.GameStates.Running:
                        UpdateRunning();
                        break;
                    case Globals.GameStates.RoomTransitioning:
                        UpdateRoomTransit();
                        break;
                    case Globals.GameStates.Bag:
                        UpdateBagView();
                        break;
                    case Globals.GameStates.TitleScreen:
                        UpdateTitleScreen();
                        break;
                    case Globals.GameStates.Setting:
                        UpdateSettings();
                        break;
                    case Globals.GameStates.Pause:
                        UpdatePaused();
                        break;
                    case Globals.GameStates.LoadAndSave:
                        UpdateLoadAndSave();
                        break;
                    case Globals.GameStates.GameOver:
                        UpdateGameOver();
                        break;
                    default:
                        break;
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp);

            // Always draw the cursor 
            cursor.Draw();
            screenFX.Draw();

            overlay.Draw();

            switch (gameState)
            {
                case Globals.GameStates.Running:
                    DrawRunning();
                    break;
                case Globals.GameStates.RoomTransitioning:
                    DrawTransitioning();
                    break;
                case Globals.GameStates.Bag:
                    DrawBagView();
                    break;
                case Globals.GameStates.TitleScreen:
                    DrawTitleScreen();
                    break;
                case Globals.GameStates.Setting:
                    DrawSettings();
                    break;
                case Globals.GameStates.Pause:
                    DrawPaused();
                    break;
                case Globals.GameStates.LoadAndSave:
                    DrawLoadAndSave();
                    break; 
                case Globals.GameStates.GameOver:
                    DrawGameOver();
                    break;
                default:
                    break;
            }


            spriteBatch.End();
            base.Draw(gameTime);
        }


        // ================================================================================
        // ================================ Private methods ===============================
        // ================================================================================

        // --------------------------------------------------------------------------------
        // --------------------------------- Updates --------------------------------------

        /// <summary>
        /// For a selection of classes (item, enemy, projectiles), their updates are needed
        /// to deal with some of the change of game state (e.g. stop colldown update while 
        /// the game is not running)
        /// </summary>
        private void UpdateRoutine()
        {
            foreach (IItem item in collectibleItemList)
            {
                item.Update();
            }
            foreach (IItem item in spellSlots.itemSlots)
            {
                if (item != null)
                    item.Update();
            }

            foreach (IEnemy enemy in enemyList)
            {
                enemy.Update(mainChara);
            }
            foreach (IProjectile proj in projList)
            {
                proj.Update();
            }
        }

        /// <summary>
        /// Update method for when the game is running 
        /// </summary>
        private void UpdateRunning()
        {
            if (transitionProgress.Any(x => x == true)) return; 

            foreach (IBlock block in dynamicBlockList)
            {
                if (Misc.Instance.BlockFogBreaker(block, mainChara.GetRectangle(), fogOfWar.GetRange()))
                    block.Update();
            }

            UpdateRoutine();

            spellSlots.Update();

            mainChara.Update();

            fogOfWar.Update();

            minimap.Update();
            headsupDisplay.Update();

            ClearLists();
        }

        /// <summary>
        /// Update method for room transitioning. 
        /// Also includes what to do when trantion finishes. 
        /// </summary>
        private void UpdateRoomTransit()
        {

            currentRoom.Update();

            minimap.Update();
            headsupDisplay.Update();

            if (currentRoom.TransitionListener()) // True if the transition is finished 
            {
                roomCycler.MoveIntoRoom(transitionDir);

                currentRoom = new Room(this);
                currentRoom.roomInfo = roomCycler.GetCurrentRoomInfo();
                currentRoom.SetupRoom();

                ReloadLists();
                

                mainChara.Refresh(this);

                minimap.SetPivot(roomCycler.GetCurrentLocationIndex());
                minimap.FlagExplored(roomCycler.GetCurrentLocationIndex());

                gameState = Globals.GameStates.Running;
            }

        }

        /// <summary>
        /// The rest of the update methods are more generic, consisting of mostly only the 
        /// general display update and routine update. 
        /// </summary>

        private void UpdateBagView()
        {
            generalDisplay.Update();

            UpdateRoutine();
        }

        private void UpdateTitleScreen()
        {
            generalDisplay.Update();

            UpdateRoutine();
        }

        private void UpdateSettings()
        {
            generalDisplay.Update();

            UpdateRoutine();
        }

        private void UpdatePaused()
        {
            generalDisplay.Update();

            UpdateRoutine();
        }

        private void UpdateLoadAndSave()
        {
            generalDisplay.Update();

            UpdateRoutine();
        }

        private void UpdateGameOver()
        {
            generalDisplay.Update();
        }


        // --------------------------------------------------------------------------------
        // ----------------------------------- Draw ---------------------------------------

        /// <summary>
        /// Draw method for when the game is runnning 
        /// </summary>
        private void DrawRunning()
        {
            currentRoom.Draw();

            foreach (IItem item in collectibleItemList)
            {
                if((_DEVMODE && !_ENABLE_FOW) || 
                    Misc.Instance.ItemFogBreaker(item, mainChara.GetRectangle(), fogOfWar.GetRange()))
                    item.Draw();
            }
            foreach (IItem item in spellSlots.itemSlots)
            {
                if (item != null)
                    item.DrawEffects();
            }

            foreach (IBlock block in dynamicBlockList)
            {
                if ((_DEVMODE && !_ENABLE_FOW) || 
                    Misc.Instance.BlockFogBreaker(block, mainChara.GetRectangle(), fogOfWar.GetRange()))
                    block.Draw();
            }

            foreach (IEnemy enemy in enemyList)
            {
                if ((_DEVMODE && !_ENABLE_FOW) || 
                    Misc.Instance.EnemyFogBreaker(enemy, mainChara.GetRectangle(), fogOfWar.GetRange()))
                    enemy.Draw();
            }
            foreach (IProjectile proj in projList)
            {
                if ((_DEVMODE && !_ENABLE_FOW) || 
                    Misc.Instance.ProjectileFogBreaker(proj, mainChara.GetRectangle(), fogOfWar.GetRange()))
                    proj.Draw();
            }

            mainChara.Draw();

            if (_ENABLE_FOW)
                fogOfWar.Draw();

            screenFX.DrawFlies(mainChara.position);
            headsupDisplay.Draw();
            minimap.Draw();

            // Check for tab display 
            minimap.DrawWholeMap(displayWholeMinimap);

            // Set back to not displaying 
            displayWholeMinimap = 0; 

            // Dev mode debuggings 
            if (_DEVMODE && _SHOW_BOX)
            {
                DrawRectangle PlayerRect = new DrawRectangle(GraphicsDevice, spriteBatch, mainChara.GetRectangle(), Color.Green);
                PlayerRect.Draw();

                foreach (IBlock block in staticBlockList)
                {
                    DrawRectangle BlockRect = new DrawRectangle(GraphicsDevice, spriteBatch, block.GetRectangle(), Color.Yellow);
                    BlockRect.Draw();
                }

                foreach(IItem item in collectibleItemList)
                {
                    DrawRectangle ItemRect = new DrawRectangle(GraphicsDevice, spriteBatch, item.GetRectangle(), Color.Yellow);
                    ItemRect.Draw();
                }

                foreach (IEnemy enemy in enemyList)
                {
                    DrawRectangle enemyRect = new DrawRectangle(GraphicsDevice, spriteBatch, enemy.GetRectangle(), Color.Red);
                    enemyRect.Draw();
                }
            }
        }

        /// <summary>
        /// Draw method for room trantioning.  
        /// </summary>
        private void DrawTransitioning()
        {
            currentRoom.Draw();

            fogOfWar.Draw();

            headsupDisplay.Draw();
            minimap.Draw();
        }

        /// <summary>
        /// Draw method when in bag view 
        /// </summary>
        private void DrawBagView()
        {
            generalDisplay.Draw();
        }

        private void DrawTitleScreen()
        {
            generalDisplay.Draw();
        }

        private void DrawSettings()
        {
            generalDisplay.Draw();
        }

        private void DrawPaused()
        {
            generalDisplay.Draw();
        }

        private void DrawLoadAndSave()
        {
            generalDisplay.Draw();
        }

        private void DrawGameOver()
        {
            generalDisplay.Draw();
        }

        // --------------------------------------------------------------------------------
        // --------------------------------- Utility --------------------------------------

        /// <summary>
        /// Start a new generator and populate the room again 
        /// </summary>
        private void ReloadLists()
        {
            mapGenerator = new Generator(this);

            staticBlockList = mapGenerator.GetStaticBlockList();
            dynamicBlockList = mapGenerator.GetBlockList(this);

            collectibleItemList = mapGenerator.GetItemList(this);

            enemyList = mapGenerator.GetEnemyList(this);

            projList = new List<IProjectile>();

        }

        /// <summary>
        /// Clear expired things in lists
        /// </summary>
        private void ClearLists()
        {
            projList.RemoveAll(x => x.Expired() == true);
        }

    }
}
