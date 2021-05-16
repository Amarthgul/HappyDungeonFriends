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

        public MC.primaryTypes primaryState { set; get; }
        public Globals.GeneralStates mcState { set; get; }
        public Globals.Direction facingDir { set; get; }

        public CharacterSprite(Game1 G)
        {
            game = G;
            spriteBatch = game.spriteBatch;

            LoadAllSprites();
        }

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

            torchFlame = new GeneralSprite(iTF.texture, iTF.C, iTF.R,
                Globals.WHOLE_SHEET, Globals.FRAME_CYCLE, Globals.MC_LAYER + 0.01f);
            torchFlame.positionOffset = Globals.SPRITE_OFFSET_2;

            torchShadow = new GeneralSprite(iTS.texture, iTS.C, iTS.R,
                Globals.WHOLE_SHEET, Globals.FRAME_CYCLE, Globals.MC_LAYER - 0.01f);
            torchFlame.positionOffset = Globals.SPRITE_OFFSET_2;

            torchAttackFlame = new GeneralSprite(iTAF.texture, iTAF.C, iTAF.R,
                Globals.WHOLE_SHEET, Globals.FRAME_CYCLE, Globals.ITEM_EFFECT_LAYER);
            torchAttackFlame.positionOffset = Globals.SPRITE_OFFSET_UNIT;

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

        private void ChangeAllSpriteDir()
        {
            foreach (GeneralSprite GS in allSprites)
            {
                GS.rowLimitation = (int)facingDir;
            }
        }

        private void UpdateSelectedSprites()
        {
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
        }

        private List<GeneralSprite> GetSpritesNow()
        {
            List<GeneralSprite> SpriteList = new List<GeneralSprite>();

            switch (mcState)
            {
                case Globals.GeneralStates.Attack:
                    if(primaryState == MC.primaryTypes.Torch)
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

        private List<GeneralSprite> HoldAndMoving()
        {
            List<GeneralSprite> SpriteList = new List<GeneralSprite>();
            if (primaryState == MC.primaryTypes.Torch)
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


        public void RefreshAttack()
        {
            attack.currentFrame = 0;
            attackWithTorch.currentFrame = 0;
            torchAttackFlame.currentFrame = 0;
        }

        public void Update(Globals.Direction D, Globals.GeneralStates S, MC.primaryTypes P)
        {
            facingDir = D;
            mcState = S;
            primaryState = P;

            ChangeAllSpriteDir();
            UpdateSelectedSprites();

        }

        public void Draw(SpriteBatch SB, Vector2 P)
        {
            foreach (GeneralSprite GS in GetSpritesNow())
                GS.Draw(SB, P, defaultTint);
        }
    }
}
