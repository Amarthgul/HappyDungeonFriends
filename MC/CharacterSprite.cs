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

        private GeneralSprite walking;
        private GeneralSprite attack;
        private GeneralSprite walkingWithTorch;
        private GeneralSprite attackWithTorch;
        // The sprite to use 
        private GeneralSprite currentMainSprite;
        private GeneralSprite lastMainSprite;

        private GeneralSprite torchFlame;
        private GeneralSprite torchShadow;
        private GeneralSprite torchAttackFlame;
        private List<GeneralSprite> additionalSprites;  
        private List<GeneralSprite> lastAddSprites;

        private Color defaultTint = Color.White;
        private Color damagedTint = Color.Red;
        private Color tintNow;

        public MC.primaryTypes primaryState { set; get; }
        public Globals.GeneralStates mcState { set; get; }
        public Globals.Direction facingDir { set; get; }

        public CharacterSprite(Game1 G)
        {
            game = G;

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

            lastAddSprites = new List<GeneralSprite>();
            additionalSprites = new List<GeneralSprite>();

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

            currentMainSprite = walking;

        }


        public void Draw()
        {

        }
    }
}
