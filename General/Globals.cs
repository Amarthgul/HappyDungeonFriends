using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace HappyDungeon
{
    public class Globals
    {
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
        // =============================== Sprites Basic ==================================
        // ================================================================================
        public const int WHOLE_SHEET = -1; // Use the entire sheet when updating frames 
        public const int ONE_FRAME = 1;
        public const int FRAME_CYCLE = 4;  // By default each animation cycle has 4 frames 

        // ================================================================================
        // ============================== Levels and Rooms ================================
        // ================================================================================
        // Room tile dimensions 
        public const int RTILE_ROW = 7;
        public const int RTILE_COLUMN = 12;

        // Random Room Generation 
        public const int MAX_DIR_COMBO = 3;

        // ================================================================================
        // =========================== Item and enemy indexes =============================
        // ================================================================================
        // -1 ~ -255 are items 
        public const int ITEM_TORCH = -1; 

        // -256 and smaller are enemies or NPCs 

        // ================================================================================
        // ================================ Draw Layers ===================================
        // ================================================================================
        public const float MAP_LAYER = 0.1f;    // The beackground/maps/rooms
        public const float BLOCKS_LAYER = 0.2f; // Blocks and evnironments 
        public const float MC_LAYER = 0.5f;     // Main character 
        public const float MIX_LAYER = 0.6f;    // items and enemies 
        public const float FOW_LAYER = 0.7f;    // Fog of War
        public const float UI_LAYER = 0.8f;
        public const float UI_MINIMAP = 0.85f; 
        public const float DEBUG_LAYER = 0.95f;


        // ================================================================================
        // ==================================== Misc ======================================
        // ================================================================================
        // Speed metre 
        public const int SPEED_BASELINE = 1 * SCALAR;
        // Time realted 
        public const int KEYBOARD_HOLD = 50;
        public const int FRAME_DELAY = 125; // 8 fps 

        public static Random RND = new Random();

        public enum RoomTypes { Start, Boss, Merchant, Normal };
        public enum EnemyTypes { Minion, Elite, Boss };
        public enum Weapons { Torch };
        public enum Direction { Left, Right, Up, Down, None };
        public enum GameStates { TitleScreen, Running, RoomTransitioning, Setting, Bag, GameOver, Story }
        public enum GeneralStates { Moving, Hold, Attack, Damaged, Broken, Dead }


        public static List<Direction> FourDirIter = new List<Direction>() 
            { Direction.Left, Direction.Right, Direction.Up, Direction.Down};

    }
}
