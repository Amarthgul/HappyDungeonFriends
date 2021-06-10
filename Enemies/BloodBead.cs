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
    /// The earliest enemy came to be in this project, so some parts remains out-dated.  
    /// </summary>
    class BloodBead : IEnemySTD
    {
        private int beadKnockbackDist = 16;

        public BloodBead(Game1 G, Vector2 P) : base( G,  P) 
        {
            game = G;
            position = P;
            spriteBatch = game.spriteBatch;

            // ------------------------------------------------------------------
            // -------------------- Difference from STD--------------------------
            segmentedSpeed = new int[] {
                (int)(0.1 * Globals.SCALAR),
                (int)(0.3 * Globals.SCALAR),
                (int)(0.6 * Globals.SCALAR),
                (int)(0.3 * Globals.SCALAR) };
            collisionDMG = -12;
            horizontalTop = 4 * Globals.SCALAR;
            horizontalBot = 3 * Globals.SCALAR;
            sideShrink = 2 * Globals.SCALAR;
            useSegmentedSpeed = true;              // Speed varies with animation
            startWithHibernate = false;            // Not hibernating 
            isVisible = true;                      // Born visible
            canAttack = false;                     // Does not attack 
            selfState = Globals.GeneralStates.Moving; 
            // ------------------------------------------------------------------

            LoadSprites();
            UpdateRect();

            HPBar = new Enemies.EnemyHealthBar(game, selfType);

            brainAgent = new Enemies.AgentStupid(this);
            enemyBlockCollison = new EnemyBlockCollision(game);

            currentMoveIndex = 0;
            facingDir = (Globals.Direction)(Globals.RND.Next() % 4);
            movingSprite.rowLimitation = (int)facingDir;

            damageProtectionSW.Restart();
        }

        /// <summary>
        /// Since blood bead basic is rather stupid, its update method is simplified.  
        /// </summary>
        /// <param name="MainChara">Not used</param>
        public override void Update(MC MainChara)
        {
            // Change draw layers if player is lower in screen 
            float DrawLayer; 
            if(MainChara.position.Y > position.Y)
            {
                DrawLayer = layerAtBack;
            }
            else
            {
                DrawLayer = layerOnTop;
            }
            mainSprite.layer = DrawLayer;

            // Deal with live and die 
            if (currentHealth <= 0)
            {
                UpdateDeath();
            }
            else
            {
                // Change movement depending on player's stats 
                brainAgent.Update(MainChara);


                // Try to move 
                Move();
                // Turn if hit a block
                if (enemyBlockCollison.ValidMove(GetStagedRectangle()))
                {
                    mainSprite.Update();
                    currentMoveIndex = mainSprite.currentFrame % Globals.FRAME_CYCLE;

                    position += stagedMovement;
                    stagedMovement = new Vector2(0, 0);
                }
                else
                {
                    Turn(brainAgent.HandleBlockCollision(facingDir));
                }

            }

            // Update collision to follow the movement 
            UpdateRect();
            // Update HP if necessary 
            HPBar.Update(totalHealth, currentHealth);
        }

        /// <summary>
        /// Return a damage instance of on collision. 
        /// </summary>
        /// <returns>DamageInstance object</returns>
        public override DamageInstance DealCollisionDamage()
        {
            if (currentHealth == 0)
                return null; 

            DamageInstance DMG = new DamageInstance(collisionDMG, new Globals.DamageEffect [] { Globals.DamageEffect.Knockback });
            DMG.knowckbackDist = beadKnockbackDist;

            return DMG;
        }


        // ================================================================================
        // ================================ Private methods ===============================
        // ================================================================================

        protected override void LoadSprites()
        {
            ImageFile BB = TextureFactory.Instance.enemyBead;
            ImageFile BD = TextureFactory.Instance.enemyBeadDeath;

            movingSprite = new GeneralSprite(BB.texture, BB.C, BB.R,
                Globals.WHOLE_SHEET, Globals.FRAME_CYCLE, Globals.ENEMY_LAYER);
            movingSprite.frameDelay = 250;

            deathSprite = new GeneralSprite(BD.texture, BD.C, BD.R,
                    Globals.WHOLE_SHEET, Globals.FRAME_CYCLE, Globals.ENEMY_LAYER);
            deathSprite.positionOffset = new Vector2(-2, -2) * Globals.SCALAR;

            mainSprite = movingSprite; 
        }


    }
}
