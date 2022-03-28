using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace HappyDungeon.UI.Displays
{
    class LoadAndSaveDisplay
    {

        private const bool _DEBUGGING = true; 

        private const int INSTANCE_WIDTH = 64 * Globals.SCALAR;     // How wide a progression instance displays
        private const int INSTANCE_HEIGHT = 128 * Globals.SCALAR;   // How high a progression instance displays 
        private const int INSTANCE_DISTANCE = 10 * Globals.SCALAR;  // Distance (horizontal) between 2 progressions 

        private const int INSTANCE_TO_LEFT = 16 * Globals.SCALAR;   // White space boundary at left and right 
        private const int INSTANCE_TO_TOP = 17 * Globals.SCALAR;

        private const int TX_LOAD_TO_TOP = 78 * Globals.SCALAR;     // Distance of the "load" text to the top of inst 
        private const int TX_OVERLD_TO_TOP = 94 * Globals.SCALAR;

        private const int DRAG_BUFFER_MAX = 32 * Globals.SCALAR;    // When dragging left or right, the furthest distance

        private const int MAX_VACANT_SLOTS = 2;    // Max number of empty slots to save  

        private Game1 game;
        private SpriteBatch spriteBatch;

        // ================================================================================
        // ================================ Save and load =================================
        // ================================================================================
        private General.GameProgression.ProgressionInstance[] savedInstances;

        private int savedInstanceCount; 
        private int totalInstanceCount;

        // ================================================================================
        // =========================== Sprites and their stats ============================
        // ================================================================================

        // Text generator 
        private LargeBR textGen = new LargeBR();
        private LargeWR textOnHoverGen = new LargeWR();

        private GeneralSprite backgroundWhole;
        private GeneralSprite backgroundRed; 

        private GeneralSprite loadSaveInstanceSelect;    // Highlight selection 
        private GeneralSprite[] loadSaveInstance;        // There are several sprites for instances 
        private GeneralSprite loadSaveInstanceOverlay;   // Empty slot use this to cover the hole 

        private GeneralSprite loadText;
        private GeneralSprite loadTextOnHover;
        private GeneralSprite overrrideText;
        private GeneralSprite overrideTextOnHover;
        private GeneralSprite backText;
        private GeneralSprite backTextOhHover; 

        private Vector2 instanceStartPosition;
        private Vector2 zeroVector = new Vector2(0, 0);

        
        // Instances together form a block (<div>, if you would), and this is the top-left
        // pivot of the entire instance block. 
        private Vector2 instanceBlockPivot = new Vector2(INSTANCE_TO_LEFT, INSTANCE_TO_TOP);

        private Color defualtTint = Color.White;

        // ================================================================================
        // ========================= Click and on hover logics  ===========================
        // ================================================================================
        
        private bool LMBSession = false;             // If LMB is pressing
        private bool instanceDragSession = false;    // If LMB is dragging the instances  
        private bool instanceDragRecovery = false;   // If LMB is released and is recovering 

        // Target X coordinate of the top-left pivot when a recovery is needed 
        private float instanceDragRecTarget;

        private int instanceDragRecTime = 500; // 500 ms recovery time 
        private System.Diagnostics.Stopwatch recoverySW;

        private int horizontalIndex = 0;
        private int verticalIndex = 0;
        private bool KBS = false; // Keyboard session flag 
        private int KBI = 0;      // Option index 
        private System.Diagnostics.Stopwatch KBSW = new System.Diagnostics.Stopwatch();
        private int KBInterval = 100;

        private Vector2 leftClickSessionStartPos;
        private Vector2 leftClickDelta = new Vector2(0, 0);   // Delta distance caused by direct click dragging 
        private Vector2 leftClickRelease = new Vector2(0, 0); // Temp variable for when the click is released 

        private Rectangle instanceBlockZone = new Rectangle(
            INSTANCE_TO_LEFT, INSTANCE_TO_TOP, 
            Globals.OUT_FWIDTH - 2 * INSTANCE_TO_LEFT,
            INSTANCE_HEIGHT
            );

        private Rectangle loadTextRect;
        private Rectangle overrideTextRect;

        private bool[] loadTextIsOnHover;
        private bool[] overrideTextIsOnHover; 


        // ================================================================================
        // ================================= Methods ======================================
        // ================================================================================
        public LoadAndSaveDisplay(Game1 G)
        {
            game = G;
            spriteBatch = game.spriteBatch;

            LoadAllSprites();
            SetupPositions();

            UpdateSavedInstances();

            recoverySW = new System.Diagnostics.Stopwatch();
        }

        /// <summary>
        /// Load all background sprites and creates all text sprites
        /// </summary>
        private void LoadAllSprites()
        {
            ImageFile LSIE = TextureFactory.Instance.loadSaveInsSelect;
            ImageFile LSIO = TextureFactory.Instance.loadSaveInsOverlay; 
            ImageFile[] LSI = TextureFactory.Instance.loadSaveIns;
            ImageFile BGW = TextureFactory.Instance.titleBackgroundWhole;
            ImageFile BGR = TextureFactory.Instance.titleBackground; 

            Texture2D LT = textGen.GetText(TextBridge.Instance.GetLoadAndSaveOptions(0), game.GraphicsDevice);
            Texture2D LTO = textOnHoverGen.GetText(TextBridge.Instance.GetLoadAndSaveOptions(0), game.GraphicsDevice);
            Texture2D OT = textGen.GetText(TextBridge.Instance.GetLoadAndSaveOptions(1), game.GraphicsDevice);
            Texture2D OTO = textOnHoverGen.GetText(TextBridge.Instance.GetLoadAndSaveOptions(1), game.GraphicsDevice);
            Texture2D BT = textGen.GetText(TextBridge.Instance.GetLoadAndSaveOptions(2), game.GraphicsDevice);
            Texture2D BTO = textOnHoverGen.GetText(TextBridge.Instance.GetLoadAndSaveOptions(2), game.GraphicsDevice);

            loadSaveInstance = new GeneralSprite[LSI.Length];

            backgroundWhole = new GeneralSprite(BGW.texture, 1, 1, Globals.WHOLE_SHEET, 1, Globals.UI_UNDER);
            backgroundRed = new GeneralSprite(BGR.texture, 1, 1, Globals.WHOLE_SHEET, 1, Globals.UI_UNDER);
            loadSaveInstanceOverlay = new GeneralSprite(LSIO.texture, 1, 1, Globals.WHOLE_SHEET, 1, Globals.UI_ICONS);
            loadSaveInstanceSelect = new GeneralSprite(LSIE.texture, 1, 1, Globals.WHOLE_SHEET, 1, Globals.UI_LAYER);
            for (int i = 0; i < LSI.Length; i++)
            {
                loadSaveInstance[i] = new GeneralSprite(LSI[i].texture, 1, 1, Globals.WHOLE_SHEET, 1, Globals.UI_LAYER);
            }

            loadText = new GeneralSprite(LT, 1, 1, Globals.WHOLE_SHEET, 1, Globals.UI_ICONS);
            loadTextOnHover = new GeneralSprite(LTO, 1, 1, Globals.WHOLE_SHEET, 1, Globals.UI_ICONS);
            overrrideText = new GeneralSprite(OT, 1, 1, Globals.WHOLE_SHEET, 1, Globals.UI_ICONS);
            overrideTextOnHover = new GeneralSprite(OTO, 1, 1, Globals.WHOLE_SHEET, 1, Globals.UI_ICONS);
            backText = new GeneralSprite(BT, 1, 1, Globals.WHOLE_SHEET, 1, Globals.UI_ICONS);
            backTextOhHover = new GeneralSprite(BTO, 1, 1, Globals.WHOLE_SHEET, 1, Globals.UI_ICONS);
        }

        private void SetupPositions()
        {
            instanceStartPosition = new Vector2(INSTANCE_TO_LEFT * Globals.SCALAR, INSTANCE_TO_TOP * Globals.SCALAR);

            loadTextRect = new Rectangle(
                (INSTANCE_WIDTH / 2 - loadText.selfTexture.Width / 2 * Globals.SCALAR),
                TX_LOAD_TO_TOP,
                loadText.selfTexture.Width,
                loadText.selfTexture.Height
            );
            overrideTextRect = new Rectangle(
                (INSTANCE_WIDTH / 2 - overrrideText.selfTexture.Width / 2 * Globals.SCALAR),
                TX_OVERLD_TO_TOP,
                overrrideText.selfTexture.Width,
                overrrideText.selfTexture.Height
            );
        }

        private void UpdateSavedInstances()
        {
            savedInstances = game.loadAndSave.GetSavedInstances();

            if (savedInstances == null)
                savedInstanceCount = 0;
            else
                savedInstanceCount = savedInstances.Length;

            totalInstanceCount = savedInstanceCount + MAX_VACANT_SLOTS;

            loadTextIsOnHover = new bool[totalInstanceCount];
            overrideTextIsOnHover = new bool[totalInstanceCount];

            for (int i = 0; i< totalInstanceCount; i++)
            {
                loadTextIsOnHover[i] = false;
                overrideTextIsOnHover[i] = false;
            }
        }


        /// <summary>
        /// Calculate whether or not the darg need to be reset, and if so, signal the start. 
        /// </summary>
        private void ResetInstanceZone()
        {
            float LeftDragThreshold;

            // Determine if it has been dragged to the left for too far, 
            // and if so, where should it be recovered to. 
            // This is set up this way becasue depending on the number of instances, 
            // It may be pivoted to the left-most or the right-most part. 
            if((totalInstanceCount * (INSTANCE_DISTANCE + INSTANCE_WIDTH) - INSTANCE_DISTANCE) < instanceBlockZone.Width)
            {
                // When the total length of instances cannot fill the entire zone
                // Any drag to the left should be reset. 
                LeftDragThreshold = 0;
            }
            else
            {
                LeftDragThreshold = instanceBlockZone.Width - (totalInstanceCount * (INSTANCE_DISTANCE + INSTANCE_WIDTH) - INSTANCE_DISTANCE);
            }

            
            // Setup left or right recovery, signal the start. 
            if (leftClickDelta.X > 0) // Positive number says it's been dragged to the right side 
            {
                // Dragged to the right 
                instanceDragRecovery = true;
                instanceDragRecTarget = 0;
                recoverySW.Start();
            }
            else if (leftClickDelta.X < LeftDragThreshold)
            {
                // Dragged to the left 
                instanceDragRecovery = true;
                instanceDragRecTarget = LeftDragThreshold;
                recoverySW.Start();
            }

            
        }

        /// <summary>
        /// Drag recovery animation. 
        /// </summary>
        private void InstanceDragRecovery()
        {
            //leftClickDelta.X = instanceDragRecTarget; 
            leftClickDelta.X = (leftClickRelease.X * (1 - (float)recoverySW.ElapsedMilliseconds / instanceDragRecTime))
                + (instanceDragRecTarget * ((float)recoverySW.ElapsedMilliseconds / instanceDragRecTime));

            if (recoverySW.ElapsedMilliseconds > instanceDragRecTime)
            {
                leftClickDelta.X = instanceDragRecTarget; 
                instanceDragRecovery = false;
                recoverySW.Reset();
            }
                
        }

        /// <summary>
        /// Instances has so many things to draw, it seems only fitting to give them 
        /// a separated method for all the texts and sprites. 
        /// </summary>
        private void DrawInstances()
        {
            for (int i = 0; i < totalInstanceCount; i++)
            {
                Vector2 currentInstancePivot = new Vector2(
                    i * (INSTANCE_WIDTH + INSTANCE_DISTANCE) + leftClickDelta.X,
                    0)
                    + instanceBlockPivot;
                Vector2 loadTextOffset = new Vector2(loadTextRect.X, loadTextRect.Y);
                Vector2 overrideTextToTop = new Vector2(overrideTextRect.X, overrideTextRect.Y);

                loadSaveInstance[i % loadSaveInstance.Length].Draw(spriteBatch, currentInstancePivot, defualtTint);

                if (i >= savedInstanceCount)
                {
                    loadSaveInstanceOverlay.Draw(spriteBatch, currentInstancePivot, defualtTint);
                }

                if(loadTextIsOnHover[i])
                    loadTextOnHover.Draw(spriteBatch, currentInstancePivot + loadTextOffset, defualtTint);
                else
                    loadText.Draw(spriteBatch, currentInstancePivot + loadTextOffset, defualtTint);
                overrrideText.Draw(spriteBatch, currentInstancePivot + overrideTextToTop, defualtTint);

                //loadSaveInstanceSelect.Draw(spriteBatch, currentInstancePivot, defualtTint);
            }
        }

        // ================================================================================
        // ============================== Public methods ==================================
        // ================================================================================

        /// <summary>
        /// Dealing with left click. Either marks the start of a left click session,
        /// or updating the left click seesion. 
        /// </summary>
        /// <param name="CursorPos">Position of the cursor</param>
        public void LeftClickEvent(Vector2 CursorPos)
        {

            if (!LMBSession) // If not in a session, start a session 
            {
                
                LMBSession = true;

                if (instanceBlockZone.Contains(CursorPos))
                {
                    instanceDragSession = true;
                    leftClickSessionStartPos = CursorPos - leftClickDelta;
                }

            }
            else // When already in a session 
            {
                if (instanceBlockZone.Contains(CursorPos) && instanceDragSession) 
                {
                    // If player is clicking inside the instance zone
                    leftClickDelta = CursorPos - leftClickSessionStartPos;
                }
            }
        }

        /// <summary>
        /// Releasing the left click, maybe doing nothing or selected something. 
        /// </summary>
        /// <param name="CursorPos">Position of the cursor release</param>
        public void LeftClickRelease(Vector2 CursorPos)
        {
            if (!LMBSession) return; // If not in a click session, then skip

            if (instanceDragSession)
            {
                instanceDragSession = false;
                leftClickRelease = leftClickDelta;
                recoverySW.Start();
                ResetInstanceZone(); 
            }

            LMBSession = false; 
        }

        public void UpdateOnhover(Vector2 CursorPos)
        {
            for (int i = 0; i < totalInstanceCount; i++)
            {
                Rectangle currentLoadText = new Rectangle(
                    (int)(i * (INSTANCE_WIDTH + INSTANCE_DISTANCE) + 
                        leftClickDelta.X + loadTextRect.X + INSTANCE_TO_LEFT),
                    loadTextRect.Y + INSTANCE_TO_TOP, 
                    loadTextRect.Width * Globals.SCALAR,
                    loadTextRect.Height * Globals.SCALAR
                    );
                
                if (currentLoadText.Contains(CursorPos))
                {
                    if (!loadTextIsOnHover[i])
                        SoundFX.Instance.PlayTitleOnHover();
                    loadTextIsOnHover[i] = true;
                }   
                else 
                    loadTextIsOnHover[i] = false;

            }
        }

        public void Update()
        {

            if (instanceDragRecovery)
                InstanceDragRecovery();
        }

        public void Draw()
        {
            if (game.virgin)
                backgroundWhole.Draw(spriteBatch, zeroVector, defualtTint);
            else
                backgroundRed.Draw(spriteBatch, zeroVector, defualtTint);

            DrawInstances();


            if (_DEBUGGING)
            {
                DrawRectangle DragZone = new DrawRectangle(game.GraphicsDevice, spriteBatch, instanceBlockZone, Color.Red);
                DragZone.Draw();
            }

        }


    }
}
