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
    class CharacterSprite
    {
        Game1 game;
        private SpriteBatch spriteBatch;

        private GeneralSprite walking;
        private GeneralSprite attack;
        private GeneralSprite walkingWithTorch;
        private GeneralSprite attackWithTorch;

        private GeneralSprite torchFlame;
        private GeneralSprite torchShadow;
        private GeneralSprite torchAttackFlame;

        private List<GeneralSprite> allSprites; 

        private Color defaultTint = Color.White;
        private Color damagedTint = Color.Red;
        private Color tintNow;

        private int oscillator = 0; 

        public Globals.primaryTypes primaryState { set; get; }
        public Globals.GeneralStates mcState { set; get; }
        public Globals.Direction facingDir { set; get; }

        public bool healthInflicting { set; get; }

        private Stopwatch torchSFXSW = new Stopwatch();
        private int torchSoundInterval = 900; 

        public CharacterSprite(Game1 G)
        {
            game = G;
            spriteBatch = game.spriteBatch;

            LoadAllSprites();

            torchSFXSW.Restart();
        }

        /// <summary>
        /// Read textures from the factory and load all the GeneralSprites
        /// </summary>
        private void LoadAllSprites()
        {
            // Initlize all IMs 
            ImageFile WalkingIM = TextureFactory.Instance.mcWalk;
            ImageFile Attack = TextureFactory.Instance.mcAttack;
            ImageFile WWT = TextureFactory.Instance.mcTorchWalk; // Walking With Torch 
            ImageFile AWT = TextureFactory.Instance.mcAttackTorch;

            ImageFile iTF = TextureFactory.Instance.itemTorchFlame;
            ImageFile iTS = TextureFactory.Instance.itemTorchShadow;
            ImageFile iTAF = TextureFactory.Instance.itemTorchAttackFlame;

            // Creating all sprites 
            walking = new GeneralSprite(WalkingIM.texture, WalkingIM.C, WalkingIM.R,
                Globals.WHOLE_SHEET, Globals.FRAME_CYCLE, Globals.MC_LAYER);
            walkingWithTorch = new GeneralSprite(WWT.texture, WWT.C, WWT.R,
                Globals.WHOLE_SHEET, Globals.FRAME_CYCLE, Globals.MC_LAYER);
            attack = new GeneralSprite(Attack.texture, Attack.C, Attack.R,
                Globals.WHOLE_SHEET, Globals.FRAME_CYCLE, Globals.MC_LAYER);
            attackWithTorch = new GeneralSprite(AWT.texture, AWT.C, AWT.R,
                Globals.WHOLE_SHEET, Globals.FRAME_CYCLE, Globals.MC_LAYER);
            attackWithTorch.frameDelay = 50; 

            torchFlame = new GeneralSprite(iTF.texture, iTF.C, iTF.R,
                Globals.WHOLE_SHEET, Globals.FRAME_CYCLE, Globals.MC_LAYER + 0.01f);
            torchFlame.positionOffset = Globals.SPRITE_OFFSET_2;

            torchShadow = new GeneralSprite(iTS.texture, iTS.C, iTS.R,
                Globals.WHOLE_SHEET, Globals.FRAME_CYCLE, Globals.MC_LAYER - 0.01f);
            torchFlame.positionOffset = Globals.SPRITE_OFFSET_2;

            torchAttackFlame = new GeneralSprite(iTAF.texture, iTAF.C, iTAF.R,
                Globals.WHOLE_SHEET, Globals.FRAME_CYCLE, Globals.ITEM_EFFECT_LAYER);
            torchAttackFlame.positionOffset = Globals.SPRITE_OFFSET_UNIT;
            torchAttackFlame.frameDelay = 50;

            allSprites = new List<GeneralSprite>() { 
                walking,
                walkingWithTorch,
                attack,
                attackWithTorch,
                torchFlame, 
                torchShadow, 
                torchAttackFlame
            };

        }

        /// <summary>
        /// The sprites are all 4x4 directional, so doesn't hurt just doing it on all of them.
        /// </summary>
        private void ChangeAllSpriteDir()
        {
            foreach (GeneralSprite GS in allSprites)
            {
                GS.rowLimitation = (int)facingDir;
            }
        }

        /// <summary>
        /// Depending on the state of the character and what she's holding,
        /// update some of the sprites.
        /// NOTE: special sprites are for primary items only, thus usable items' 
        /// effects are in their own item class and shall not be added here. 
        /// </summary>
        private void UpdateSelectedSprites()
        {
            // This switch only update the sprites
            // and doesn't deal with other state-specific conditions 
            switch (mcState)
            {
                case Globals.GeneralStates.Attack:
                    attackWithTorch.Update();
                    torchAttackFlame.Update();
                    attack.Update();
                    torchShadow.Update();
                    break;
                case Globals.GeneralStates.Broken: // Broken can not move and can do no shit
                    break;
                case Globals.GeneralStates.Damaged: // Broken can not move and can do no shit
                    break;
                case Globals.GeneralStates.Hold: // Broken can not move and can do no shit
                    torchFlame.Update();
                    torchShadow.Update();
                    break;
                case Globals.GeneralStates.Moving: // Broken can not move and can do no shit
                    walking.Update();
                    torchFlame.Update();
                    torchShadow.Update();
                    walkingWithTorch.Update();
                    break;
                case Globals.GeneralStates.Stunned: // Broken can not move and can do no shit
                    break;
                default:
                    break;
            }

            // If the character is taking damage, oscillate between red and default 
            if (healthInflicting)
            {
                tintNow = (oscillator % 2 == 0) ? defaultTint : damagedTint;
                oscillator++;
            }
            else
            {
                tintNow = defaultTint;
            }
            
        }

        /// <summary>
        /// Depending on the states and primary weapon, return a list of sprites 
        /// that should be drawn. 
        /// </summary>
        /// <returns>Sprites to draw in current condition</returns>
        private List<GeneralSprite> GetSpritesNow()
        {
            List<GeneralSprite> SpriteList = new List<GeneralSprite>();

            switch (mcState)
            {
                case Globals.GeneralStates.Attack:
                    if(primaryState == Globals.primaryTypes.Torch
                        && ((Torch)game.spellSlots.GetItem(-1)).torchOn)
                    {
                        SpriteList.Add(attackWithTorch);
                        SpriteList.Add(torchAttackFlame);
                        SpriteList.Add(torchShadow);
                    }
                    else
                    {
                        SpriteList.Add(attack);
                    }
                        
                    break;
                case Globals.GeneralStates.Broken:
                    break;
                case Globals.GeneralStates.Damaged:
                    SpriteList.Add(walking);
                    break;
                case Globals.GeneralStates.Hold:
                    SpriteList = HoldAndMoving();
                    break;
                case Globals.GeneralStates.Moving:
                    SpriteList = HoldAndMoving();
                    break;
                case Globals.GeneralStates.Stunned:
                    break;
                default:
                    break;
            }

            return SpriteList; 
        }

        /// <summary>
        /// Hold and moving are essentially the same, only difference being moving has 
        /// the move sprite updated while hold is to have it remain still. 
        /// </summary>
        /// <returns>The sprites for hold and movinf states</returns>
        private List<GeneralSprite> HoldAndMoving()
        {
            List<GeneralSprite> SpriteList = new List<GeneralSprite>();

            if (primaryState == Globals.primaryTypes.Torch
                && ((Torch)game.spellSlots.GetItem(-1)).torchOn)
            {
                SpriteList.Add(walkingWithTorch);
                SpriteList.Add(torchFlame);
                SpriteList.Add(torchShadow);
            }
            else
            {
                SpriteList.Add(walking);
            }
            return SpriteList; 
        }

        private bool IsMainSprite(GeneralSprite G)
        {
            bool result = false;

            result |= G.Equals(walking);
            result |= G.Equals(walkingWithTorch);

            return result;
        }

        private void UpdateSounds()
        {
            // Update for when the player character is holding the torch 
            if (primaryState == Globals.primaryTypes.Torch
                && ((Torch)game.spellSlots.GetItem(-1)).torchOn)
            {   
                // Note that 900 interval will accumulate some of the sound, making it louder by time 
                // It is assumed that player won't stay in the same room for too long
                // Or hold the torch for too long for it to become overly loud 
                if(torchSFXSW.ElapsedMilliseconds > torchSoundInterval)
                {
                    SoundFX.Instance.PlayMCTorchOn();
                    torchSFXSW.Restart();
                }
            }
        }

        // ================================================================================
        // ================================= Public methods ===============================
        // ================================================================================

        /// <summary>
        /// Since attack sprites are updated in some other cases, it might cause new attack
        /// to start from the middle of the animation, thus a refresh is needed to reset 
        /// the frame to the beginning.
        /// </summary>
        public void RefreshAttack()
        {
            attack.currentFrame = 0;
            attackWithTorch.currentFrame = 0;
            torchAttackFlame.currentFrame = 0;
        }

        public void Update(Globals.Direction D, Globals.GeneralStates S, Globals.primaryTypes P, bool DMG_On)
        {
            facingDir = D;
            mcState = S;
            primaryState = P;
            healthInflicting = DMG_On;

            ChangeAllSpriteDir();
            UpdateSelectedSprites();

            UpdateSounds();

        }

        public void Draw(SpriteBatch SB, Vector2 P)
        {
            foreach (GeneralSprite GS in GetSpritesNow())
            {
                if (IsMainSprite(GS))
                {
                    GS.Draw(SB, P, tintNow);
                }
                else
                {
                    GS.Draw(SB, P, defaultTint);
                }
                
            }
                
        }
    }
}
