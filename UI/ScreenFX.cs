﻿using System;
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
    public class ScreenFX
    {

        private Game1 game;
        private SpriteBatch spriteBatch;

        private int respawnPossibility = 5; //5 percent 
        private int maxFlyCount = 3;
        private List<UI.Effects.ScreenFlyFX> flyList; 

        public ScreenFX(Game1 G)
        {
            game = G;
            spriteBatch = game.spriteBatch;
            flyList = new List<UI.Effects.ScreenFlyFX>();

        }

        public void Update()
        {
            if (flyList.Count < maxFlyCount && Globals.RND.Next(100) < respawnPossibility)
            {
                flyList.Add(new UI.Effects.ScreenFlyFX(spriteBatch));
            }

            // Remove all expired flies 
            flyList.RemoveAll(c => c.Finished());

            foreach (UI.Effects.ScreenFlyFX fly in flyList)
            {
                fly.Update();
            }
        }

        public void Draw(Vector2 FocusPos)
        {

            foreach (UI.Effects.ScreenFlyFX fly in flyList)
            {
                if (FlyVisible(FocusPos, fly.GetPosition()))
                    fly.Draw();
            }
        }

        private bool FlyVisible(Vector2 PlayerPos, Vector2 FlyPos)
        {
            float Range = (game.fogOfWar.GetRange() + 0.5f) * Globals.OUT_UNIT;

            return (Math.Abs(PlayerPos.X - FlyPos.X) + Math.Abs(PlayerPos.Y - FlyPos.Y)) < Range;
        }


    }

    

}