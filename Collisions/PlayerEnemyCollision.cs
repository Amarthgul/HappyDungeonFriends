using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace HappyDungeon
{
    class PlayerEnemyCollision
    {

        private Game1 game;
        List<IEnemy> enemyList;

        public PlayerEnemyCollision(Game1 G)
        {
            game = G;
            enemyList = G.enemyList;

        }

        public bool CheckEnemyCollision(Rectangle PlayerCollisionRect)
        {
            bool result = false;

            foreach (IEnemy enemy in enemyList)
            {
                // Skip this one if it's dead 
                if (enemy.IsDead())
                    continue;

                Rectangle Intersection = Rectangle.Intersect(PlayerCollisionRect, enemy.GetRectangle());

                if (Intersection.Width > 0)
                {
                    result = true;
                    Globals.Direction DamageFrom = Globals.Direction.None; 

                    if(Intersection.Width > Intersection.Height)
                    {
                        if (PlayerCollisionRect.Y > enemy.GetRectangle().Y)
                            DamageFrom = Globals.Direction.Up;
                        else
                            DamageFrom = Globals.Direction.Down;
                    }
                    else
                    {
                        if (PlayerCollisionRect.X > enemy.GetRectangle().X)
                            DamageFrom = Globals.Direction.Left;
                        else
                            DamageFrom = Globals.Direction.Right;
                    }

                    game.mainChara.TakeCollisionDamage(enemy.DealCollisionDamage(), DamageFrom);
                }
            }

            return result;
        }
    }
}
