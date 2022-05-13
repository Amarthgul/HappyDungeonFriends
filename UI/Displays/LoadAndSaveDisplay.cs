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

        private const bool _DEBUGGING = false; 

        private const int INSTANCE_WIDTH = 64 * Globals.SCALAR;     // How wide a progression instance displays
        private const int INSTANCE_HEIGHT = 128 * Globals.SCALAR;   // How high a progression instance displays 
        private const int INSTANCE_DISTANCE = 10 * Globals.SCALAR;  // Distance (horizontal) between 2 progressions 

        private const int INSTANCE_TO_LEFT = 16 * Globals.SCALAR;   // White space boundary at left and right 
        private const int INSTANCE_TO_TOP = 17 * Globals.SCALAR;

        private const int TX_LOAD_TO_TOP = 78 * Globals.SCALAR;     // Distance of the "load" text to the top of instance 
        private const int TX_OVERLD_TO_TOP = 94 * Globals.SCALAR;

        private const int TX_BACK_TO_TOP = 159 * Globals.SCALAR;    // Distance of the "save" text to the top of the window

        private const int DRAG_BUFFER_MAX = 32 * Globals.SCALAR;    // When dragging left or right, the furthest distance

        private const int MAX_VACANT_SLOTS = 2;    // Max number of empty slots to save  

        private Game1 game;
        private SpriteBatch spriteBatch;

        // ================================================================================
        // ================================ Save and load =================================
        // ================================================================================
        private List<General.GameProgression.ProgressionInstance> savedInstances;

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
        private GeneralSprite saveText;
        private GeneralSprite saveTextOnHover; 
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

        // Different from some other displays, load and save has a pair of separated KeyBoardIndex, 
        // horizontal and vertical, this is due to the options may change in number as the player
        // creates or removes saved files 
        private int horizontalIndex = 0;
        private int verticalIndex = 0;
        private bool KBS = false; // Keyboard session flag 
        private int KBI_Horizontal = 0;      // Horizontal option index 
        private int KBI_Vertical = 0;        // Vertical option index 
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
        private Rectangle saveTextRect; 
        private Rectangle overrideTextRect;
        private Rectangle backTextRect; 

        private bool[] loadTextIsOnHover;
        private bool[] overrideTextIsOnHover;
        private bool backTextIsOnHover;

        private Vector2 backTextDrawPos; 

        // ================================================================================
        // ================================= Methods ======================================
        // ================================================================================
        public LoadAndSaveDisplay(Game1 G)
        {
            game = G;
            spriteBatch = game.spriteBatch;

            savedInstances = new List<General.GameProgression.ProgressionInstance>();

            LoadAllSprites();
            SetupPositions();

            UpdateSavedInstances();

            backTextIsOnHover = false; 

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
            Texture2D ST = textGen.GetText(TextBridge.Instance.GetLoadAndSaveOptions(1), game.GraphicsDevice);
            Texture2D STO = textOnHoverGen.GetText(TextBridge.Instance.GetLoadAndSaveOptions(1), game.GraphicsDevice);
            Texture2D OT = textGen.GetText(TextBridge.Instance.GetLoadAndSaveOptions(2), game.GraphicsDevice);
            Texture2D OTO = textOnHoverGen.GetText(TextBridge.Instance.GetLoadAndSaveOptions(2), game.GraphicsDevice);
            Texture2D BT = textGen.GetText(TextBridge.Instance.GetLoadAndSaveOptions(3), game.GraphicsDevice);
            Texture2D BTO = textOnHoverGen.GetText(TextBridge.Instance.GetLoadAndSaveOptions(3), game.GraphicsDevice);

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
            saveText = new GeneralSprite(ST, 1, 1, Globals.WHOLE_SHEET, 1, Globals.UI_ICONS);
            saveTextOnHover = new GeneralSprite(STO, 1, 1, Globals.WHOLE_SHEET, 1, Globals.UI_ICONS);
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
            saveTextRect = new Rectangle(
                (INSTANCE_WIDTH / 2 - saveText.selfTexture.Width / 2 * Globals.SCALAR),
                TX_OVERLD_TO_TOP,
                saveText.selfTexture.Width,
                saveText.selfTexture.Height
            );
            overrideTextRect = new Rectangle(
                (INSTANCE_WIDTH / 2 - overrrideText.selfTexture.Width / 2 * Globals.SCALAR),
                TX_OVERLD_TO_TOP,
                overrrideText.selfTexture.Width,
                overrrideText.selfTexture.Height
            );
            // The width and height of the load, save, override is calculated during runtime 
            // Due to the fact that they move in realtime
            // The scalar multiplication is also done in runtime rather than here 

            backTextRect = new Rectangle(Globals.OUT_FWIDTH / 2 - backText.selfTexture.Width / 2 * Globals.SCALAR, 
                TX_BACK_TO_TOP, 
                backText.selfTexture.Width * Globals.SCALAR, 
                backText.selfTexture.Height * Globals.SCALAR);
            backTextDrawPos = new Vector2(backTextRect.X, backTextRect.Y);
        }

        /// <summary>
        /// At the start of the game and every time this display is invoked, this method 
        /// should be called to check for saved game progressions. 
        /// </summary>
        private void UpdateSavedInstances()
        {
            savedInstances = game.loadAndSave.UpdateSavedInstances();

            // Update the amount of saved instances 
            if (savedInstances == null)
                savedInstanceCount = 0;
            else
                savedInstanceCount = savedInstances.Count;

            // Update the total amount of instances that shall be displayed 
            totalInstanceCount = savedInstanceCount + MAX_VACANT_SLOTS;


            // Update the on hover boolean indicators 
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
                    overrideTextToTop = new Vector2(saveTextRect.X, saveTextRect.Y);
                    loadSaveInstanceOverlay.Draw(spriteBatch, currentInstancePivot, defualtTint);
                }


                // -------------------------- Texts and on hover --------------------------

                // Load, if it's an empty slot then draw transprent 
                float loadTextOpacity = (i < savedInstanceCount) ? 1f : .5f; 
                if (loadTextIsOnHover[i] && i < savedInstanceCount)
                    loadTextOnHover.Draw(spriteBatch, currentInstancePivot + loadTextOffset, defualtTint);
                else
                    loadText.Draw(spriteBatch, currentInstancePivot + loadTextOffset, defualtTint * loadTextOpacity);

                float saveTextOpacity = game.virgin ? .5f : 1f;
                if (i >= savedInstanceCount)
                {   // For empty instances, it is "save" not "override"
                    if (overrideTextIsOnHover[i] && !game.virgin)
                        saveTextOnHover.Draw(spriteBatch, currentInstancePivot + overrideTextToTop, defualtTint);
                    else
                        saveText.Draw(spriteBatch, currentInstancePivot + overrideTextToTop, defualtTint * saveTextOpacity);
                }
                else
                {
                    // Override, note that if there is nothing to save then this is greyed out 
                    if (overrideTextIsOnHover[i] && !game.virgin)
                        overrideTextOnHover.Draw(spriteBatch, currentInstancePivot + overrideTextToTop, defualtTint);
                    else
                        overrrideText.Draw(spriteBatch, currentInstancePivot + overrideTextToTop, defualtTint * saveTextOpacity);
                }

                //loadSaveInstanceSelect.Draw(spriteBatch, currentInstancePivot, defualtTint);
            }
        }

        // ================================================================================
        // ============================== Public methods ==================================
        // ================================================================================
         
        public void OptionMoveUp()
        {
            if (!KBS) KBS = true; 

            KBI_Vertical -= 1;
        }

        public void OptionMoveDown() 
        {
            if (!KBS) KBS = true;

            KBI_Vertical += 1;
        }

        public void OptionMoveLeft()
        {
            if (!KBS) KBS = true;

            KBI_Horizontal -= 1;
        }

        public void OptionMoveRight()
        {
            if (!KBS) KBS = true;

            KBI_Horizontal += 1; 
        }

        public void OptionConfirm()
        {
            // Don't do anything if it's not in a keyboard session 
            if (!KBS) return; 

        }

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

            for (int i = 0; i < totalInstanceCount; i++)
            {
                Rectangle currentLoadText = new Rectangle(
                    (int)(i * (INSTANCE_WIDTH + INSTANCE_DISTANCE) +
                        leftClickDelta.X + loadTextRect.X + INSTANCE_TO_LEFT),
                    loadTextRect.Y + INSTANCE_TO_TOP,
                    loadTextRect.Width * Globals.SCALAR,
                    loadTextRect.Height * Globals.SCALAR
                    );

                Rectangle currentOverrideText = new Rectangle(
                    (int)(i * (INSTANCE_WIDTH + INSTANCE_DISTANCE) +
                        leftClickDelta.X + overrideTextRect.X + INSTANCE_TO_LEFT),
                    overrideTextRect.Y + INSTANCE_TO_TOP,
                    overrideTextRect.Width * Globals.SCALAR,
                    overrideTextRect.Height * Globals.SCALAR
                    );

                if (currentLoadText.Contains(CursorPos))
                {

                }

                if (currentOverrideText.Contains(CursorPos) && !game.virgin)
                {
                    game.loadAndSave.SaveNow();

                    savedInstances = game.loadAndSave.UpdateSavedInstances(); 
                }

            }

            // Go back 
            if (backTextRect.Contains(CursorPos))
            {
                game.screenFX.BackToLastState();
            }

            LMBSession = false; 
        }

        /// <summary>
        /// Update on hover effect depending on where the cursor is at. 
        /// Could be on any one of the "Load" or "Override" or the go back. 
        /// </summary>
        /// <param name="CursorPos"></param>
        public void UpdateOnhover(Vector2 CursorPos)
        {
            // Iterate through instances 
            for (int i = 0; i < totalInstanceCount; i++)
            {
                Rectangle currentLoadText = new Rectangle(
                    (int)(i * (INSTANCE_WIDTH + INSTANCE_DISTANCE) + 
                        leftClickDelta.X + loadTextRect.X + INSTANCE_TO_LEFT),
                    loadTextRect.Y + INSTANCE_TO_TOP, 
                    loadTextRect.Width * Globals.SCALAR,
                    loadTextRect.Height * Globals.SCALAR
                    );

                Rectangle currentOverrideText = new Rectangle(
                    (int)(i * (INSTANCE_WIDTH + INSTANCE_DISTANCE) +
                        leftClickDelta.X + overrideTextRect.X + INSTANCE_TO_LEFT),
                    overrideTextRect.Y + INSTANCE_TO_TOP,
                    overrideTextRect.Width * Globals.SCALAR,
                    overrideTextRect.Height * Globals.SCALAR
                    );
                
                if (currentLoadText.Contains(CursorPos))
                {
                    // Not every instance can be loaded, empty slots cannot 
                    if (!loadTextIsOnHover[i] && i < savedInstanceCount)
                        SoundFX.Instance.PlayTitleOnHover();
                    loadTextIsOnHover[i] = true;
                }   
                else 
                    loadTextIsOnHover[i] = false;

                if (currentOverrideText.Contains(CursorPos))
                {
                    if (!overrideTextIsOnHover[i] && !game.virgin)
                        SoundFX.Instance.PlayTitleOnHover();
                    overrideTextIsOnHover[i] = true;
                }
                else
                    overrideTextIsOnHover[i] = false; 
            }


            // Back button on hover 
            if (backTextRect.Contains(CursorPos))
            {
                backTextIsOnHover = true;
            }
            else
            {
                backTextIsOnHover = false; 
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

            if (backTextIsOnHover)
                backTextOhHover.Draw(spriteBatch, backTextDrawPos, defualtTint); 
            else 
                backText.Draw(spriteBatch, backTextDrawPos, defualtTint);

            if (_DEBUGGING)
            {
                DrawRectangle DragZone = new DrawRectangle(game.GraphicsDevice, spriteBatch, backTextRect, Color.Red);
                DragZone.Draw();
            }

        }


    }
}
