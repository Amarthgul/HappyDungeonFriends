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

        private const int UPDATE_DELAY = 4;

        private const int TRANSITION_STEP_Y = Globals.SCALAR * 2;
        private const int TRANSITION_STEP_X = Globals.SCALAR * 4;
 
        // ================================================================================
        // ========================= Infomation about the room ============================
        // ================================================================================
        public RoomInfo roomInfo;
        // Doors related. Left, Right, Up, Down as in global settings 
        private bool[] doorOpen = { false, false, false, false };    // Highest priority 
        private bool[] doorHole = { false, false, false, false };    // Second in command 
        private bool[] doorLocked = { false, false, false, false };    // Lowest priority  
        private bool[] doorMys = { false, false, false, false };

        // ================================================================================
        // ============================ Textures and display ==============================
        // ================================================================================
        public Texture2D nextLevelTexture;
        public Texture2D levelTexture;     // Can be accessed to make map transitioning 
        private Texture2D roomOverlay; 
        private Color defaultTint = Color.White;

        Levels.RoomTextureGenerator roomTexture;  // Generate and update the textures 

        // ================================================================================
        // ================= Abstract resources, parameters and states ====================
        // ================================================================================
        private SpriteBatch spriteBatch;
        private Game1 game;

        // Transition 
        private bool transtionFinished;
        private System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
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

            roomTexture = new Levels.RoomTextureGenerator(game, roomInfo);

            levelTexture = roomTexture.GetRoomTexture();
            roomOverlay = roomTexture.GenerateOverlay();

            doorHole = roomInfo.Holes;
            doorLocked = roomInfo.LockedDoors;
            doorMys = roomInfo.MysteryDoors;
            doorOpen = roomInfo.OpenDoors;
        }


        // For explosions to add a hole on the wall 
        public void AddHole(int Dir)
        {
            doorHole[Dir] = true;
            doorOpen[Dir] = false;
            levelTexture = roomTexture.UpdateDrawDoors();
        }

        // Open a mys door 
        public void OpenMysDoor(int Dir)
        {
            game.roomCycler.OpenMysDoor((Globals.Direction)Dir);

            doorOpen = game.roomCycler.GetCurrentRoomInfo().OpenDoors;
            doorMys = game.roomCycler.GetCurrentRoomInfo().MysteryDoors;

            levelTexture = roomTexture.UpdateDrawDoors();
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

                // Overlay effects like dirt 
                roomTexture.DrawGroundOverlays();

                // Overlay appears only in Joy and Delight 
                if (game.gameLevel == Globals.GameLevel.Delight || game.gameLevel == Globals.GameLevel.Joy)
                {
                    spriteBatch.Draw(roomOverlay, new Vector2(0, Globals.OUT_HEADSUP), null, defaultTint,
                    0, Vector2.Zero, Globals.SCALAR, SpriteEffects.None, Globals.MAP_OVERLAY);
                }
            }

        }

    }
}
