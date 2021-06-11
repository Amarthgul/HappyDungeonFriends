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
            bool result = false;

            foreach (IBlock block in game.staticBlockList)
            {
                if (ProjRectangle.Intersects(block.GetRectangle()))
                    return true;
            }

            return result;
        }


        public bool CollidedWithPlayer()
        {
            bool result = false;




            return result;
        }

        public bool CollidedWithEnemy()
        {
            bool result = false;




            return result;
        }
    }
}
