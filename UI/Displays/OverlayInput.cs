using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using Microsoft.Xna.Framework.Input;
using System.Text.RegularExpressions;

namespace HappyDungeon
{
    /// <summary>
    /// When an window that floats on top is needed for player interaction. 
    /// Name window: get player to input text for thing like name. 
    /// </summary>
    public class OverlayInput
    {
        public enum Mode { Save };

        private enum Options { EnterName, Confirm, Cancel}

        private const int HEIGHT_TO_TOP = 54;

        private const int NAME_PRMPT_TO_TOP = 6;   // Prompt of enter name text to the top of the window
        private const int INPUT_TO_TOP = 24;       // Player input text to the top of the window 
        private const int NAME_OPTION_TO_TOP = 48; // Vertical location of options to the top of the window 
        private const int NAME_OPTION_TO_MID = 32; // Horizontal location of options to the middle of the window

        private Game1 game;
        private SpriteBatch spriteBatch;

        private Mode currentMode = Mode.Save; 

        // ================================================================================
        // =========================== Abstract and parameters ============================
        // ================================================================================
        private bool newWindowInSession = false;

        // If the LMB is currently being pressed 
        private bool LMBSession = false; 

        private List<string> playerInputString = new List<string>();

        // ================================================================================
        // ==================== Sprites, positions, and their stats =======================
        // ================================================================================

        // Text generator 
        private UI.Small textGenSML = new UI.SmallBR(); 
        private UI.Large textGenBasic = new UI.Large(); 
        private UI.LargeBR textGen = new UI.LargeBR();
        private UI.LargeWR textOnHoverGen = new UI.LargeWR();

        private List<GeneralSprite> InteractiveWindowsMidSize;

        // Pick one from the windows above 
        private GeneralSprite windowNow;

        // Dim out the background to signify the overlay window 
        private GeneralSprite backgroundGray;

        // Text of the player input, has left and right 2 parts b/c there's a cursor 
        private List<GeneralSprite> playerInputSprite = new List<GeneralSprite>();
        private GeneralSprite playerInputCursor; // That tiny vertical line 

        private List<GeneralSprite> nameOptionTexts;
        private List<GeneralSprite> nameOptionTextsOnHover;

        private Vector2 zeroVector = new Vector2();
        private Vector2 windowDrawPosition = new Vector2();
        private Vector2 playerInputLoc = new Vector2();
        private Vector2 inputCursorOffset; 
        private List<Vector2> nameWindowTxLoc = new List<Vector2>(); // Locations for naming window 

        private List<Rectangle> nameWindowTxRect = new List<Rectangle>(); 

        private Color backgroundColor = Color.White; 
        private Color defualtTint = Color.White;

        private float backgroundOpacity = 0.5f;

        private List<bool> nameTextIsOnHover = new List<bool>();


        // For player input, a timer is used to avoid one press event to spawn a million characters 
        private System.Diagnostics.Stopwatch inputSW = new System.Diagnostics.Stopwatch();
        private long inputInterval = 200; // Interval between each valid key pressing  

        private System.Diagnostics.Stopwatch cursorBlinkSW = new System.Diagnostics.Stopwatch();
        private long cursorBlinkInterval = 500;
        private bool cursorOnDisplay = false;

        // ================================================================================
        // ==================================== Methods ===================================
        // ================================================================================
        public OverlayInput(Game1 G)
        {
            game = G;
            spriteBatch = G.spriteBatch; 

            LoadAllSprites();

            // Just for safty, initilize it 
            InitilizeNewSession();

            SetupPositions();

            InitilizeInput();

            inputSW.Start();
            cursorBlinkSW.Start();
        }

        /// <summary>
        /// Load and setup all sprites 
        /// </summary>
        private void LoadAllSprites()
        {
            List<ImageFile> WindowMidSize = new List<ImageFile>();
            List<Texture2D> nameOptionTextRaw = new List<Texture2D>();
            List<Texture2D> nameOptionTextRawOH = new List<Texture2D>();
            Texture2D InputCursorTxt = textGenSML.GetSpecialChar(Globals.SpecialCharacters.InputCursor, game.GraphicsDevice);
            foreach (ImageFile img in TextureFactory.Instance.interactiveWindowMidSzie)
                WindowMidSize.Add(img);
            foreach (string txt in TextBridge.Instance.GetNameWindowTexts())
            {
                nameOptionTextRaw.Add(textGen.GetText(txt, game.GraphicsDevice));
                nameOptionTextRawOH.Add(textOnHoverGen.GetText(txt, game.GraphicsDevice));
                nameTextIsOnHover.Add(false);
            }


            playerInputCursor = new GeneralSprite(InputCursorTxt, 1, 1, Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.UI_TEXT_LAYER);
            
            InteractiveWindowsMidSize = new List<GeneralSprite>();
            nameOptionTexts = new List<GeneralSprite>();
            nameOptionTextsOnHover = new List<GeneralSprite>();

            foreach (ImageFile img in WindowMidSize)
                InteractiveWindowsMidSize.Add(new GeneralSprite(img.texture, 
                    1, 1, Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.INTERACTIVE_WIN_LAYER));
            foreach (Texture2D txt in nameOptionTextRaw)
                nameOptionTexts.Add(new GeneralSprite(txt, 1, 1, Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.UI_TEXT_LAYER));
            foreach (Texture2D txt in nameOptionTextRawOH)
                nameOptionTextsOnHover.Add(new GeneralSprite(txt, 1, 1, Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.UI_TEXT_LAYER));

            backgroundGray = new GeneralSprite(
                TextureFactory.Instance.GenerateTexture(game.GraphicsDevice, Globals.OUT_FWIDTH, Globals.OUT_FHEIGHT, pixel=> backgroundColor), 
                1, 1, Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.INTERACTIVE_BG_LAYER
                );

            
        }

        /// <summary>
        /// This is for setting up the positions that would not need to be changed in runtime 
        /// </summary>
        private void SetupPositions()
        {
            inputCursorOffset = new Vector2(playerInputCursor.selfTexture.Width * Globals.SCALAR, 0);
        }

        /// <summary>
        /// Clear old inputs and add 2 slots for new ones. 
        /// </summary>
        private void InitilizeInput()
        {
            playerInputString.Clear(); 
            playerInputString.Add("");
            playerInputString.Add("");

            playerInputSprite.Clear();
            playerInputSprite.Add(null);
            playerInputSprite.Add(null);
        }

        /// <summary>
        /// In case windows will be changed in the future, each session recalculate the positions
        /// </summary>
        private void InitilizeNewSession()
        {
            InitilizeInput();

            windowNow = InteractiveWindowsMidSize[Globals.RND.Next() % InteractiveWindowsMidSize.Count];

            windowDrawPosition = new Vector2(
                (Globals.ORIG_FWIDTH/2 - windowNow.selfTexture.Width/2) * Globals.SCALAR,
                HEIGHT_TO_TOP * Globals.SCALAR);

            nameWindowTxLoc = new List<Vector2>();
            nameWindowTxLoc.Add(new Vector2(Globals.OUT_FWIDTH / 2 - nameOptionTexts[0].selfTexture.Width * Globals.SCALAR / 2,
                windowDrawPosition.Y + NAME_PRMPT_TO_TOP * Globals.SCALAR));
            nameWindowTxLoc.Add(new Vector2(Globals.OUT_FWIDTH / 2 - NAME_OPTION_TO_MID * Globals.SCALAR + 
                nameOptionTexts[1].selfTexture.Width * Globals.SCALAR,
                windowDrawPosition.Y + NAME_OPTION_TO_TOP * Globals.SCALAR));
            nameWindowTxLoc.Add(new Vector2(Globals.OUT_FWIDTH / 2 + NAME_OPTION_TO_MID * Globals.SCALAR -
                nameOptionTexts[2].selfTexture.Width * Globals.SCALAR,
                windowDrawPosition.Y + NAME_OPTION_TO_TOP * Globals.SCALAR));

            nameWindowTxRect = new List<Rectangle>();
            for (int i = 0; i < nameWindowTxLoc.Count; i++)
                nameWindowTxRect.Add(new Rectangle((int)nameWindowTxLoc[i].X, (int)nameWindowTxLoc[i].Y, 
                    nameOptionTexts[i].selfTexture.Width * Globals.SCALAR, 
                    nameOptionTexts[i].selfTexture.Height * Globals.SCALAR));
        }

        /// <summary>
        /// Empty the sprites for user input and remake them to represent new updates.
        /// </summary>
        private void UpdateInputSprite()
        {
            int TotalWidth = 0;

            Texture2D TxtLeft;
            Texture2D TxtRight;

            if (playerInputString[0].Length > 0)
            {
                TxtLeft = textGenSML.GetText(playerInputString[0], game.GraphicsDevice);

                TotalWidth += TxtLeft.Width;
                playerInputSprite[0] = new GeneralSprite(TxtLeft, 1, 1, Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.UI_TEXT_LAYER);
            }

            if (playerInputString[1].Length > 0)
            {
                TxtRight = textGenSML.GetText(playerInputString[1], game.GraphicsDevice);

                TotalWidth += TxtRight.Width;
                playerInputSprite[1] = new GeneralSprite(TxtRight, 1, 1, Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.UI_TEXT_LAYER);
            }

            TotalWidth *= Globals.SCALAR; 

            playerInputLoc = new Vector2(Globals.OUT_FWIDTH / 2 - TotalWidth / 2,
                windowDrawPosition.Y + INPUT_TO_TOP * Globals.SCALAR);
        }

        /// <summary>
        /// Change the displayed string accroding to player input.
        /// Currently does not support Shift to alter texts. 
        /// </summary>
        /// <param name="key">Single input key</param>
        /// <returns>True if this is a valid input</returns>
        private bool UpdateInputText(Keys key)
        {
            bool HasChanged = false;
            char currentKey;
            string pattern = @".*(\d)";
            Regex rg = new Regex(pattern);

            // If last key pressing is not too long ago, skip this one
            if (inputSW.ElapsedMilliseconds < inputInterval) return HasChanged;
            else inputSW.Reset();

            // Check for delete and arrow keys 
            if(key == Keys.Back && playerInputString[0].Length > 0)
            {
                playerInputString[0] = playerInputString[0].Substring(0, playerInputString[0].Length - 1);
                HasChanged = true;
            }
            else if (key == Keys.Left && playerInputString[0].Length > 0)
            {
                // Move one character from left to right 
                playerInputString[1] = string.Concat(
                    playerInputString[0][playerInputString[0].Length - 1], 
                    playerInputString[1]);
                playerInputString[0] = playerInputString[0].Substring(0, playerInputString[0].Length - 1);
                HasChanged = true;
            }
            else if (key == Keys.Right && playerInputString[1].Length > 0)
            {
                // Move one character from right to left 
                playerInputString[0] = string.Concat(playerInputString[0], 
                    playerInputString[1][0]);
                playerInputString[1] = playerInputString[1].Substring(1, playerInputString[1].Length-1);
                HasChanged = true;
            }

            // Check for inputs 
            string currentKeyTr = key.ToString();
            currentKey = (char)currentKeyTr[0];

            if (currentKeyTr.Length > 1)
            {
                // If it's a number 
                MatchCollection matchedNumber = rg.Matches(currentKeyTr);
                if (matchedNumber.Count > 0)
                {
                    string digit = currentKeyTr.Substring(currentKeyTr.Length - 1);
                    playerInputString[0] = string.Concat(playerInputString[0], (char)digit[0]);
                    HasChanged = true;
                }
            }
            else if (char.IsLetter(currentKey))
            {
                // Convert all letter input into caps  
                playerInputString[0] = string.Concat(playerInputString[0], char.ToUpper(currentKey).ToString());
                HasChanged = true;
            }
            else if (textGenSML.IsValidInput(currentKey))
            {
                // Otherwise, just check if it is a valid input, if so, concate 
                playerInputString[0] = string.Concat(playerInputString[0], currentKey.ToString());
                HasChanged = true;
            }

            inputSW.Restart(); 
            return HasChanged; 
        }

        /// <summary>
        /// Update player's input and the corresponding display 
        /// </summary>
        private void UpdateKeyboardInput()
        {
            Keys[] PressedKeys = Keyboard.GetState().GetPressedKeys();
            bool HasChanged = false; 

            foreach(Keys key in PressedKeys)
            {
                HasChanged = UpdateInputText(key); 
            }

            if (HasChanged) UpdateInputSprite();
        }

        private void DrawPlayerInput()
        {

            if (playerInputString[0].Length > 0)
                playerInputSprite[0].Draw(spriteBatch, playerInputLoc, defualtTint);

            if( playerInputString[1].Length > 0)
            {
                Vector2 string2Offset = new Vector2(playerInputSprite[0].selfTexture.Width * Globals.SCALAR, 0);

                if(cursorOnDisplay)
                    playerInputCursor.Draw(spriteBatch, playerInputLoc + string2Offset, defualtTint);

                playerInputSprite[1].Draw(spriteBatch, playerInputLoc + string2Offset + inputCursorOffset, defualtTint);
            }

        }

        private void ExecuteCommand(Options Cmd)
        {
            switch (Cmd)
            {
                case Options.Cancel:
                    break;
                case Options.Confirm:
                    game.loadAndSave.SaveNow(string.Concat(playerInputString[0], playerInputString[1]));
                    ToggleActivity(false);
                    break;
                default:
                    break; 
            }
        }

        // ================================================================================
        // ================================ Public methods ================================
        // ================================================================================

        public bool IsEnabled()
        {
            return newWindowInSession; 
        }

        public void ToggleActivity(bool Target)
        {
            if (Target && !newWindowInSession)
            {
                InitilizeNewSession();
            }
            newWindowInSession = Target; 
        }

        public void SetMode(Mode Tar)
        {
            currentMode = Tar; 
        }

        /// <summary>
        /// Updating for the on hover and click event. 
        /// </summary>
        /// <param name="CurrentState"></param>
        /// <param name="CurrentLocation"></param>
        public void UpdateCursor(MouseState CurrentState, Vector2 CurrentLocation)
        {

            for(int i = 1; i < nameWindowTxRect.Count; i++)
            {
                // Check on hover 
                if (nameWindowTxRect[i].Contains(CurrentLocation) )
                {
                    if (!nameTextIsOnHover[i])
                    {
                        SoundFX.Instance.PlayTitleOnHover();
                        nameTextIsOnHover[i] = true;
                    }
                } 
                else nameTextIsOnHover[i] = false;

                // Check click an release 
                if(CurrentState.LeftButton == ButtonState.Pressed)
                {
                    LMBSession = true; 
                }
                else if (LMBSession && nameWindowTxRect[i].Contains(CurrentLocation))
                {
                    ExecuteCommand((Options)i);
                }
            }
        }

        public void Update()
        {
            UpdateKeyboardInput();

            if(cursorBlinkSW.ElapsedMilliseconds > cursorBlinkInterval)
            {
                cursorOnDisplay = !cursorOnDisplay;
                cursorBlinkSW.Restart();
            }
        }

        public void Draw()
        {
            // If it's not a new session, quit 
            if (!newWindowInSession) return;

            backgroundGray.Draw(spriteBatch, zeroVector, defualtTint * backgroundOpacity);

            windowNow.Draw(spriteBatch, windowDrawPosition, defualtTint);

            DrawPlayerInput(); 

            for (int i = 0; i < nameOptionTexts.Count; i++)
            {
                if (i > 0 && nameTextIsOnHover[i])
                    nameOptionTextsOnHover[i].Draw(spriteBatch, nameWindowTxLoc[i], defualtTint);
                else 
                    nameOptionTexts[i].Draw(spriteBatch, nameWindowTxLoc[i], defualtTint);
            }
        }
    }
}
