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
    public class MouseCursor
    {
        private const long MIN_INTERVAL = 150;
        private const int FRAME_LENGTH = 20; 
        private const long LASTING_TIME = FRAME_LENGTH * 17;

        Game1 game;
        private SpriteBatch spriteBatch;

        private GeneralSprite cursor;


        private Stopwatch creationTimer;
        private bool canCreateNow = true;
        private List<RightClikInMapNote> effectList;
        private Vector2 positionOffset = new Vector2(-7, -10);

        private Vector2 position;

        private Color defaultTint = Color.White;

        public MouseCursor(Game1 G)
        {
            game = G;
            spriteBatch = game.spriteBatch;

            creationTimer = new Stopwatch();
            effectList = new List<RightClikInMapNote>();

            position = new Vector2(0, 0);

            SetUpSprites();

        }

        /// <summary>
        /// Setup initial sprites 
        /// </summary>
        private void SetUpSprites()
        {
            ImageFile MC = TextureFactory.Instance.cursor;

            cursor = new GeneralSprite(MC.texture, MC.C, MC.R,
                Globals.WHOLE_SHEET, Globals.ONE_FRAME, Globals.CURSOR_LAYER);
        }

        /// <summary>
        /// Create a new click on map effect sprite.
        /// </summary>
        /// <returns>Click on map sprite</returns>
        private GeneralSprite AddNewRMBMapClick()
        {
            ImageFile RMBIM = TextureFactory.Instance.cursorRMBClickInMap;

            GeneralSprite RMBMapClick = new GeneralSprite(RMBIM.texture, RMBIM.C, RMBIM.R,
                Globals.WHOLE_SHEET, RMBIM.C * RMBIM.R, Globals.CURSOR_RESPONSE);
            RMBMapClick.frameDelay = FRAME_LENGTH; 

            return RMBMapClick;
        }

        /// <summary>
        /// Called when an right click event is encountered. 
        /// </summary>
        /// <param name="CreationLocation">Current location of the cursor</param>
        public void RightClick(Vector2 CreationLocation)
        {

            if (canCreateNow)
            {
                Vector2 adjustedPosition = CreationLocation + positionOffset * Globals.SCALAR; 
                Stopwatch NewTimer = new Stopwatch();
                NewTimer.Restart();
                effectList.Add(new RightClikInMapNote(NewTimer, AddNewRMBMapClick(), adjustedPosition));

                creationTimer.Restart();
                canCreateNow = false;
            }
            
        }

        public void SetPosition(Vector2 NewPos)
        {
            position = NewPos;
        }

        public void Update()
        {
            if (creationTimer.ElapsedMilliseconds > MIN_INTERVAL)
                canCreateNow = true;

            foreach (RightClikInMapNote effect in effectList)
            {
                effect.Update();
            }

            effectList.RemoveAll(c => c.timer.ElapsedMilliseconds > LASTING_TIME);

        }


        public void Draw()
        {
            foreach (RightClikInMapNote effect in effectList)
            {
                effect.Draw(spriteBatch);
            }

            cursor.Draw(spriteBatch, position, defaultTint);
        }
    }



    class RightClikInMapNote
    {
        public Stopwatch timer;
        public GeneralSprite noteSprite;
        public Vector2 position; 

        public RightClikInMapNote(Stopwatch T, GeneralSprite S, Vector2 P)
        {
            timer = T;
            noteSprite = S;
            position = P; 
        }

        public void Update()
        {
            noteSprite.Update();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            noteSprite.Draw(spriteBatch, position, Color.White);
        }
    }
}
