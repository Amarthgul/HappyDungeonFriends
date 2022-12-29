using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace HappyDungeon

{
    public class Globals
    {

        public const bool DEBUGGING = true;

        // Version lower than 1.0 means campign is not yet finished 
        // and the game is mostly still in dev. 
        public const float VERSION = 0.11f;

        // ================================================================================
        // ============================== Gameplay Options ================================
        // ================================================================================

        /// <summary>
        /// When this is enabled, enemy can move even while the player is not active
        /// (i.e. in bag view etc.)
        /// </summary>
        public const bool REAL_TIME_ACTION = false;

        // ================================================================================
        // =========================== Game Size and Display ==============================
        // ================================================================================
        // Original sizes, used in texture control 
        public const int ORIG_UNIT = 16;          // Pixel unit
        public const int ORIG_GWIDTH = 256;       // Width of the `G`ame area 
        public const int ORIG_GHEIGHT = 176;      // Height of the `G`ame area 
        public const int ORIG_HEADSUP = 16;       // Headsup display height
        public const int ORIG_FWIDTH = 256;       // Width of the `F`ull window 
        public const int ORIG_FHEIGHT = ORIG_GHEIGHT + ORIG_HEADSUP; // Height of the `F`ull window 
        public const int ORIG_BORDER = 32;        // Size of the border, where player cannot go in 

        // How much bigger the output would be 
        public const int SCALAR = 4;

        // Sizes after scaling, used in moving calculation and drawing
        public const int OUT_UNIT = ORIG_UNIT * SCALAR;         // Pixel unit after scaled 
        public const int OUT_GWIDTH = ORIG_GWIDTH * SCALAR;
        public const int OUT_GHEIGHT = ORIG_GHEIGHT * SCALAR;
        public const int OUT_HEADSUP = ORIG_HEADSUP * SCALAR;
        public const int OUT_FWIDTH = ORIG_FWIDTH * SCALAR;
        public const int OUT_FHEIGHT = ORIG_FHEIGHT * SCALAR;
        public const int OUT_BORDER = ORIG_BORDER * SCALAR;    // Width of the map border after scaled 


        // ================================================================================
        // ============================== Magical numbers =================================
        // ================================================================================
        public const int LOAD_SAVED = 6; 

        // ================================================================================
        // ============================== Sprites Related =================================
        // ================================================================================
        public const int WHOLE_SHEET = -1; // Use the entire sheet when updating frames 
        public const int ONE_FRAME = 1;
        public const int FRAME_CYCLE = 4;  // By default each animation cycle has 4 frames 
        public static Vector2 SPRITE_OFFSET_2 = new Vector2(-2 * SCALAR, -2 * SCALAR);
        public static Vector2 SPRITE_OFFSET_UNIT = new Vector2(-ORIG_UNIT * SCALAR, -ORIG_UNIT * SCALAR);

        // ================================================================================
        // ============================== Levels and Rooms ================================
        // ================================================================================
        // Room tile dimensions 
        public const int RTILE_ROW = 7;
        public const int RTILE_COLUMN = 12;

        // Random Room Generation 
        public const int MAX_DIR_COMBO = 3;

        // ================================================================================
        // ====================== Item, enemy, and block indexes ==========================
        // ================================================================================

        public const int NULL_INDEX = 0; 

        /// <summary>
        /// Solid blocks are block with collision box and an index higher than 127 
        /// Blocks without collision box are btween 127 to 0 (inclusive)
        /// -1 ~ -255 are items 
        /// -256 and smaller are enemies or NPCs 
        /// </summary>

        // Items 
        public const int ITEM_TORCH =     -1;
        public const int ITEM_LINKEN =   -10;
        public const int ITEM_NOTE_SO =  -20;
        public const int ITEM_STD =     -254;
        public const int ITEM_GOLD =    -255;

        public const int ITEM_BOUND = -256;

        //Enemy or MPC 
        public const int ENEMY_STD =   -256;
        public const int ENEMY_BEAD =  -257;
        /// <summary>
        /// Please note adding or updating the enemy index would very possibily also require 
        /// updates in ScoreTable (Enemies.Database.ScoreTable).
        /// Score are kept in a seperate file becasue it's not an essential feature, and also
        /// to reduce the size and amount of items in this Globals. 
        /// </summary>

        // Blocks 
        public const int STARE_BLOCK_1 = 128;
        public const int STARE_BLOCK_2 = 144; 
        public const int SOLID_BLOCK_BOUND = 128;

        // ================================================================================
        // ==================================== UIs =======================================
        // ================================================================================
        public const Language GAME_LANGUAGE = Language.English;  
        public const int SLOT_SIZE = 3;
        public const int BAG_SIZE = 24;
        public const int BAG_ROW = 4;
        public const int BAG_COL = 6; 
        public static Vector2[] itemSlotsLocation = new Vector2[] {
            new Vector2(114, 1),
            new Vector2(135, 1),
            new Vector2(156, 1) };

        public const int TRANSITION_SINGLE = 500;
        public const int TRANSITION_HOLD = 100;
        public const int TRANSITION_TOTAL = TRANSITION_SINGLE * 2 + TRANSITION_HOLD;

        // ================================================================================
        // ================================ Draw Layers ===================================
        // ================================================================================
        // It is quite easy for monogame to mess it up when 2 draw calls are in same layer
        // so I tried to be as specific as possible 
        public const float MAP_LAYER = 0.1f;         // The beackground/maps/rooms
        public const float GROUND_EFFECTS = 0.11f; 
        public const float BLOCKS_LAYER = 0.2f;      // Blocks and evnironments 
        public const float ITEM_LAYER = 0.45f;       // items are always beneth 
        public const float MC_LAYER = 0.5f;          // Main character 
        public const float ENEMY_LAYER = 0.51f;      // Enemies always "on top of" the player 
        public const float ITEM_EFFECT_LAYER = 0.52f;
        public const float MAP_OVERLAY = 0.6f;       // Overlay of the border for door obscure effect 
        public const float FOW_LAYER = 0.65f;        // Fog of War
        
        public const float UI_UNDER = 0.75f;         // Down-most layer for UI
        public const float UI_SIG = 0.77f;           // Signifier when player if taking damage 
        public const float UI_MID = 0.78f;           // Mostly for item masks 
        public const float UI_SLOTS = 0.79f;         // Items in the slot 
        public const float UI_SLOTS_CD = 0.795f;     // Item CDs 
        public const float UI_LAYER = 0.8f;          // UI front panel layer
        public const float UI_ICONS = 0.81f;         // Bag and gold icon, and moving items

        public const float UI_MINIMAP = 0.85f;       // Minimap layer
        public const float UI_MINIMAP_PLAYER = 0.86f;// Player noation layer, ensure it's on top

        public const float INTERACTIVE_BG_LAYER = 0.865f;// Gray under the interactive window 
        public const float INTERACTIVE_WIN_LAYER = 0.87f;// Interactive window 

        public const float UI_TEXT_SHADOW = 0.88f;   // UI text drop shadow, nromally black 
        public const float UI_TEXT_LAYER = 0.89f;    // UI text, normally white
        public const float UI_ALT_TEXT = 0.891f;     // Alt display of the texts 

        public const float CURSOR_RESPONSE = 0.90f;   // Effects triggered by some mouse click 
        public const float CURSOR_LAYER = 0.91f;      // Cursor layer 
        public const float TRANSIT_LAYER = 0.95f; 
        public const float DEBUG_LAYER = 0.999f;      // Debug layer, put on top-most 


        // ================================================================================
        // ==================================== Misc ======================================
        // ================================================================================
        // Speed metre 
        public const int SPEED_BASELINE = 1 * SCALAR;
        // Time realted 
        public const int ITEM_HOLD = 100;     // Items cannot be picked up before this time ends 
        public const int KEYBOARD_HOLD = 100; // Keys are pressed may become protected for this amount of time
        public const int KB_STATE_HOLD = 200; // For keys that triggers game state change
        public const int FRAME_DELAY = 125;   // 8 fps 

        public static Random RND = new Random();

        public enum primaryTypes { None, Torch };

        public enum RoomTypes { Start, Boss, Merchant, Normal };
        public enum EnemyTypes { Minion, Elite, Boss };
        public enum ItemType { Primary, Usable, Junk };
        public enum Direction { Left, Right, Up, Down, None };
        public enum GameStates { TitleScreen, Running, RoomTransitioning, 
            Setting, Bag, GameOver, Conversation, Pause, LoadAndSave, Overlay }
        public enum GameLevel { Delight, Joy, Bliss };
        public enum GameDifficulty { Idiot, Normal };
        public enum GeneralStates { Moving, Hold, Attack, Damaged, Broken, Stunned, Dead }
        public enum AttackType { Melee, Ranged, Target, None };
        public enum DamageEffect { Stun, Knockback, Break, None };
        public enum DamageType { None, Burn };

        /// Chinese and Japanese sprites are too difficult to make and I don't
        /// speak Spanish or French, so English is the only one language 
        /// currently the game supports (till v0.11)
        public enum Language { English, Spanish, Chinese, Japanese, French, Deutsch };

        // For marking and displaying special character 
        public enum SpecialCharacters { InputCursor, InvalidCharacter };

        public enum KeyboardControl { Tradition, RPG };

        // --------------------------------------------------------------------------------
        // -------------------------------- Iterators -------------------------------------
        // Created for quick access or iteration 
        public static List<Direction> FourDirIter = new List<Direction>() 
            { Direction.Left, Direction.Right, Direction.Up, Direction.Down};

        public static List<GameDifficulty> DifficultyIter = new List<GameDifficulty>() 
            { GameDifficulty.Normal, GameDifficulty.Idiot };

        public static List<GameStates> Displays = new List<GameStates> { 
            GameStates.Bag, GameStates.GameOver, 
            GameStates.Pause, GameStates.Setting,
            GameStates.TitleScreen
        };
    }
}
