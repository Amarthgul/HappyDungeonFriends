using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace HappyDungeon
{
    /// <summary>
    /// Allows attack more than one direction if you're fast enough, this is actually a design choice 
    /// </summary>
    class PlayerMeleeAttackCollision
    {

        private Game1 game;
        private SpriteBatch spriteBatch; 

        public PlayerMeleeAttackCollision(Game1 G)
        {
            game = G;
            spriteBatch = game.spriteBatch;
        }

        public void CheckMeleeAttack(Globals.Direction FacingDir, int MeleeRange, int MeleeDamage, Vector2 Position)
        {
            Rectangle CheckingRect = new Rectangle() ;
            int FinalRange = (MeleeRange + game.spellSlots.GetMeleeAttackRange()) * Globals.SCALAR;
            int DefaultSideRange = Globals.OUT_UNIT;

            switch(FacingDir)
            {
                case Globals.Direction.Left:
                    CheckingRect = new Rectangle(
                        (int)(Position.X - FinalRange), 
                        (int)(Position.Y),
                        FinalRange,
                        DefaultSideRange
                        );
                    break;
                case Globals.Direction.Right:
                    CheckingRect = new Rectangle(
                        (int)(Position.X + Globals.OUT_UNIT),
                        (int)(Position.Y),
                        FinalRange,
                        DefaultSideRange
                        );
                    break;
                case Globals.Direction.Up:
                    CheckingRect = new Rectangle(
                        (int)(Position.X),
                        (int)(Position.Y - FinalRange),
                        DefaultSideRange,
                        FinalRange
                        );
                    break;
                case Globals.Direction.Down:
                    CheckingRect = new Rectangle(
                        (int)(Position.X),
                        (int)(Position.Y + Globals.OUT_UNIT),
                        DefaultSideRange,
                        FinalRange
                        );
                    break;
                default:
                    break;
            }
            
            foreach (IEnemy enemy in game.enemyList)
            {
                Rectangle Inter = Rectangle.Intersect(CheckingRect, enemy.GetRectangle());

                if(Inter.Width > 0)
                {
                    enemy.TakeDamage(game.spellSlots.PlayerMeleeDamageInstance(MeleeDamage));
                }
            }

        }

    }
}
