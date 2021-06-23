using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace HappyDungeon
{
    class TextureFactory
    {
        // Made all of them public b/c I'm lazy

        public ImageFile screenDebug;

        // ================================================================================
        // ============================== Levels and env ==================================
        // ================================================================================ 
        public ImageFile blockAllMight;
        public ImageFile[] roomBorder;
        public ImageFile[] roomDelightDirts; 
        public ImageFile roomDoors;
        public ImageFile fogOfWar;

        public ImageFile mcWalk;
        public ImageFile mcTorchWalk;
        public ImageFile mcAttack;
        public ImageFile mcAttackTorch; 

        // ================================================================================
        // ================================= UI elements ==================================
        // ================================================================================ 
        public ImageFile cursor;
        public ImageFile cursorRMBClickInMap; 

        public ImageFile uiFront;
        public ImageFile uiBack;
        public ImageFile miniRooms;
        public ImageFile playerNote;
        public ImageFile bagOnBar;
        public ImageFile goldOnBar;

        public ImageFile SelectionNote;
        public ImageFile BagViewBasic;
        public ImageFile BagViewUnderlay;

        public ImageFile titleText; 
        public ImageFile titleBackground;
        public ImageFile skyBackground;
        public ImageFile titleCloudFG;
        public ImageFile titleCloudMG;
        public ImageFile titleCloudBG;
        public ImageFile titleMountainBG;
        public ImageFile titleMountainFG; 

        public ImageFile settingBackground;
        public ImageFile settingSlider;
        public ImageFile settingSlideBar;
        public ImageFile settingSliderRed;
        public ImageFile settingSlideBarRed;
        public ImageFile settingArrow;

        public ImageFile fontDigitsSmall;
        public ImageFile fontDigitsSmallB;
        public ImageFile fontLarge;
        public ImageFile fontLargeBR;
        public ImageFile fontLargeWR;

        public ImageFile transitionUp;
        public ImageFile transitionDown; 

        public ImageFile SFX_fly; 

        // ================================================================================
        // =================================== Items ======================================
        // ================================================================================ 
        public ImageFile itemTorch;
        public ImageFile itemTorchFlame;
        public ImageFile itemTorchAttackFlame;
        public ImageFile itemTorchShadow;
        public ImageFile itemLikenSphere;
        public ImageFile itemLikenSphereOnBar;
        public ImageFile itemLikenSphereShield;
        public ImageFile itemNoteSetOne; 

        public ImageFile goldOnGround;
        public ImageFile goldOnGroundFX;

        // ================================================================================
        // ================================== Enemies =====================================
        // ================================================================================ 
        public ImageFile healthBarMinions; 
        public ImageFile enemyBead;
        public ImageFile enemyBeadDeath;

        public ImageFile enemySTD;
        public ImageFile enemySTDDie;
        public ImageFile enemySTDBurrow;
        public ImageFile enemySTDAttack;
        public ImageFile enemySTDProjectile; 

        private static TextureFactory instance = new TextureFactory();

        public static TextureFactory Instance {
            get  {
                return instance;
            }
        }

        private TextureFactory()
        {
        }

        public void LoadAll(ContentManager content){

            screenDebug = new ImageFile(content, "Images/Levels/ScreenDebug", 1, 1);

            blockAllMight = new ImageFile(content, "Images/Levels/blockAllMight", 16, 16);
            roomBorder =  new ImageFile[] { 
                new ImageFile(content, "Images/Levels/bloodBorder1", 1, 1), 
                new ImageFile(content, "Images/Levels/bloodBorder2", 1, 1)
                };
            roomDelightDirts = new ImageFile[] {
                new ImageFile(content, "Images/Levels/DelightDirt1", 1, 1),
                new ImageFile(content, "Images/Levels/DelightDirt2", 1, 1),
                new ImageFile(content, "Images/Levels/DelightDirt3", 1, 1),
                new ImageFile(content, "Images/Levels/DelightDirt4", 1, 1)
                };
            roomDoors = new ImageFile(content, "Images/Levels/bloodDoors", 4, 5);
            fogOfWar = new ImageFile(content, "Images/Levels/FoW", 1, 1);


            mcWalk = new ImageFile(content, "Images/MC/MC_walk", 4, 4);
            mcTorchWalk = new ImageFile(content, "Images/MC/MC_walk_Torch", 4, 4);
            mcAttack = new ImageFile(content, "Images/MC/MC_attack_bare", 4, 4);
            mcAttackTorch = new ImageFile(content, "Images/MC/MC_attack_torch", 4, 4);

            // --------------------------------------------------------------------------------
            // --------------------------------- UI elements ----------------------------------
            cursor = new ImageFile(content, "Images/UI/cursor", 1, 1);
            cursorRMBClickInMap = new ImageFile(content, "Images/UI/cursorRMBInMap", 4, 4);

            uiFront = new ImageFile(content, "Images/UI/HeadsupUIFront", 1, 1);
            uiBack = new ImageFile(content, "Images/UI/HeadsupUIBack", 1, 1);
            miniRooms = new ImageFile(content, "Images/UI/Minimap", 4, 4);
            playerNote = new ImageFile(content, "Images/UI/playerNotation", 4, 4);
            bagOnBar = new ImageFile(content, "Images/UI/bagOnBar", 1, 2);
            goldOnBar = new ImageFile(content, "Images/UI/goldOnBar", 1, 1);

            SelectionNote = new ImageFile(content, "Images/UI/selectionNote", 1, 1);
            BagViewBasic = new ImageFile(content, "Images/UI/BagViewBasic", 1, 1);
            BagViewUnderlay = new ImageFile(content, "Images/UI/BagViewUnderlay", 1, 1);

            titleText = new ImageFile(content, "Images/UI/title", 1, 1);
            titleBackground = new ImageFile(content, "Images/UI/titleBackground", 1, 1);
            skyBackground = new ImageFile(content, "Images/UI/skyBackground", 1, 1);
            titleCloudFG = new ImageFile(content, "Images/UI/titleCloudFG", 1, 1);
            titleCloudMG = new ImageFile(content, "Images/UI/titleCloudMG", 1, 1);
            titleCloudBG = new ImageFile(content, "Images/UI/titleCloudBG", 1, 1);
            titleMountainBG = new ImageFile(content, "Images/UI/titleMountainBG", 1, 1);
            titleMountainFG = new ImageFile(content, "Images/UI/titleMountainFG", 1, 1);

            settingBackground = new ImageFile(content, "Images/UI/SettingBase", 1, 1);
            settingSlider = new ImageFile(content, "Images/UI/slider", 1, 1);
            settingSlideBar = new ImageFile(content, "Images/UI/settingSliders", 1, 1);
            settingSliderRed = new ImageFile(content, "Images/UI/sliderRed", 1, 1);
            settingSlideBarRed = new ImageFile(content, "Images/UI/settingSlidersRed", 1, 1);
            settingArrow = new ImageFile(content, "Images/UI/settingArrows", 4, 4);

            fontDigitsSmall = new ImageFile(content, "Images/UI/Fonts/DigitsSmall", 1, 1);
            fontDigitsSmallB = new ImageFile(content, "Images/UI/Fonts/DigitsSmallBlack", 1, 1);
            fontLarge = new ImageFile(content, "Images/UI/Fonts/fontBig", 1, 1);
            fontLargeBR = new ImageFile(content, "Images/UI/Fonts/fontBigBR", 1, 1);
            fontLargeWR = new ImageFile(content, "Images/UI/Fonts/fontBigWR", 1, 1);


            transitionDown = new ImageFile(content, "Images/UI/transitionDown", 1, 1);
            transitionUp = new ImageFile(content, "Images/UI/transitionUp", 1, 1);
            SFX_fly = new ImageFile(content, "Images/UI/fly", 4, 4);

            // --------------------------------------------------------------------------------
            // ----------------------------------- Items --------------------------------------
            itemTorch = new ImageFile(content, "Images/Items/item_torch", 4, 4);
            itemTorchFlame = new ImageFile(content, "Images/Items/torchFlame", 4, 4);
            itemTorchAttackFlame = new ImageFile(content, "Images/Items/torchFlameAttack", 4, 4);
            itemTorchShadow = new ImageFile(content, "Images/Items/torchShhadows", 4, 4);
            itemLikenSphere = new ImageFile(content, "Images/Items/LinkenSphere", 1, 1);
            itemLikenSphereOnBar = new ImageFile(content, "Images/Items/LinkenSphereOnBar", 4, 4);
            itemLikenSphereShield = new ImageFile(content, "Images/Items/LinkenSphereShield", 4, 4);
            itemNoteSetOne = new ImageFile(content, "Images/Items/NoteSetOne", 4, 4);

            goldOnGround = new ImageFile(content, "Images/Items/goldOnGround", 4, 4);
            goldOnGroundFX = new ImageFile(content, "Images/Items/goldOnGroundFX", 4, 4);


            // --------------------------------------------------------------------------------
            // ---------------------------------- Enemies -------------------------------------
            healthBarMinions = new ImageFile(content, "Images/Enemies/HPBM", 1, 1);
            
            // Standard enemy base, used for debug and meta behavior test 
            enemySTD = new ImageFile(content, "Images/Enemies/enemyStd", 4, 4);
            enemySTDDie = new ImageFile(content, "Images/Enemies/enemyStdDie", 4, 4);
            enemySTDBurrow = new ImageFile(content, "Images/Enemies/enemyStdBurrow", 4, 4);
            enemySTDAttack = new ImageFile(content, "Images/Enemies/enemyStdAttack", 4, 4);
            enemySTDProjectile = new ImageFile(content, "Images/Enemies/enemyStdProjectile", 4, 4);

            enemyBead = new ImageFile(content, "Images/Enemies/bead", 4, 4);
            enemyBeadDeath = new ImageFile(content, "Images/Enemies/beadDeath", 4, 4);

        }

        /// <summary>
        /// Generate a blank texture filled with given color. 
        /// Proper use may decrease need for extra Photoshoping and loading. 
        /// </summary>
        /// <param name="Graphics">Unfortunately this requires a GraphicDevice class</param>
        /// <param name="Width">Width of the texture in pixel</param>
        /// <param name="Height">Height of the texture in pixel</param>
        /// <param name="Paint">Fill color, can be transparent</param>
        /// <returns>Generated texture</returns>
        public Texture2D GenerateTexture(GraphicsDevice Graphics, int Width, int Height, Func<int, Color> Paint)
        {
            Texture2D texture = new Texture2D(Graphics, Width, Height);

            Color[] data = new Color[Width * Height];
            for (int pixel = 0; pixel < data.Count(); pixel++)
                data[pixel] = Paint(pixel);

            texture.SetData(data);
            return texture;
        }

    }

    class ImageFile
    {
        public Texture2D texture { get; set; }
        public int R { get; set; }
        public int C { get; set; }

        public ImageFile(ContentManager content, string PN, int C, int R) {
            this.texture = content.Load<Texture2D>(PN);
            this.C = C;
            this.R = R;
        }
    }

}


