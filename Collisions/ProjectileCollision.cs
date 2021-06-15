using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


namespace HappyDungeon
{
    class ProjectileCollision
    {

        private Game1 game;


        public ProjectileCollision(Game1 G)
        {
            game = G; 

        }

        /// <summary>
        /// Collision with environments. 
        /// Since the walls are represented as blocks, only static blcok list is used. 
        /// </summary>
        /// <param name="ProjRectangle">The rectangle of the projectile</param>
        /// <returns>True if it hits some blocks</returns>
        public bool CollidedWithEnv(Rectangle ProjRectangle)
        {

            foreach (IBlock block in game.staticBlockList)
            {
                if (ProjRectangle.Intersects(block.GetRectangle()))
                    return true;
            }

            return false;
        }


        public bool CollidedWithPlayer(Rectangle ProjRectangle)
        {

            if (game.mainChara.GetRectangle().Intersects(ProjRectangle))
                return true;

            return false;
        }

        public bool CollidedWithEnemy(Rectangle ProjRectangle)
        {

            foreach (IEnemy enemy in game.staticBlockList)
            {
                if (ProjRectangle.Intersects(enemy.GetRectangle()))
                    return true;
            }


            return false;
        }
    }
}
