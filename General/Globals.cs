﻿using System;
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
        // ============================== Sprites Related =================================
        // ================================================================================
        public const int WHOLE_SHEET = -1; // Use the entire sheet when updating frames 
        public const int ONE_FRAME = 1;
        public const int FRAME_CYCLE = 4;  // By default each animation cycle has 4 frames 
        public static Vector2 SPRITE_OFFSET_2 = new Vector2(-2 * Globals.SCALAR, -2 * Globals.SCALAR);

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
        // -1 ~ -255 are items 
        public const int ITEM_TORCH = -1;
        public const int ITEM_GOLD = -255;

        public const int ITEM_BOUND = -256; 
        // -256 and smaller are enemies or NPCs 


        // Blocks are these bigger or equal to 0

        // Solid blocks are block with index higher then 127 
        public const int SOLID_BLOCK_BOUND = 128; 

        // ================================================================================
        // ==================================== UIs =======================================
        // ================================================================================
        public const int SLOT_SIZE = 3;

        // ================================================================================
        // ================================ Draw Layers ===================================
        // ================================================================================
        // It is quite easy for monogame to mess it up when 2 draw calls are in same layer
        // so I tried to be as specific as possible 
        public const float MAP_LAYER = 0.1f;         // The beackground/maps/rooms
        public const float BLOCKS_LAYER = 0.2f;      // Blocks and evnironments 
        public const float ITEM_LAYER = 0.45f;       // items are always beneth 
        public const float MC_LAYER = 0.5f;          // Main character 
        public const float ENEMY_LAYER = 0.55f;      // Enemies always "on top of" the player 
        public const float MAP_OVERLAY = 0.6f;      // Overlay of the border for door obscure effect 
        public const float FOW_LAYER = 0.65f;         // Fog of War
        

        public const float UI_UNDER = 0.75f;         // Down-most layer for UI
        public const float UI_MID = 0.78f;           // Mostly for item masks 
        public const float UI_SLOTS = 0.79f;         // Items in the slot 
        public const float UI_LAYER = 0.8f;          // UI front panel layer
        public const float UI_ICONS = 0.81f;         // Bag and gold icon

        public const float UI_MINIMAP = 0.85f;       // Minimap layer
        public const float UI_MINIMAP_PLAYER = 0.86f;// Player noation layer, ensure it's on top

        public const float UI_TEXT_SHADOW = 0.88f;   // UI text drop shadow, nromally black 
        public const float UI_TEXT_LAYER = 0.89f;    // UI text, normally white
        public const float UI_ALT_TEXT = 0.891f;     // Alt display of the texts 

        public const float CURSOR_RESPONSE = 0.90f;   // Effects triggered by some mouse click 
        public const float CURSOR_LAYER = 0.91f;     // Cursor layer 
        public const float DEBUG_LAYER = 0.95f;      // Debug layer, put on top-most 


        // ================================================================================
        // ==================================== Misc ======================================
        // ================================================================================
        // Speed metre 
        public const int SPEED_BASELINE = 1 * SCALAR;
        // Time realted 
        public const int ITEM_HOLD = 100;    // Items cannot be picked up before this time ends 
        public const int KEYBOARD_HOLD = 100;
        public const int FRAME_DELAY = 125;  // 8 fps 

        public static Random RND = new Random();

        public enum RoomTypes { Start, Boss, Merchant, Normal };
        public enum EnemyTypes { Minion, Elite, Boss };
        public enum ItemType { Primary, Usable, Junk };
        public enum Direction { Left, Right, Up, Down, None };
        public enum GameStates { TitleScreen, Running, RoomTransitioning, Setting, Bag, GameOver, Conversation }
        public enum GameLevel { Delight, Joy, Bliss };
        public enum GameDifficulty { Idiot, Normal };
        public enum GeneralStates { Moving, Hold, Attack, Damaged, Broken, Dead }

        // Four direction iterator 
        public static List<Direction> FourDirIter = new List<Direction>() 
            { Direction.Left, Direction.Right, Direction.Up, Direction.Down};

    }
}
