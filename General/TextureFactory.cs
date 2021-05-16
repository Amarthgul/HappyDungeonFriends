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

        // ================================================================================
        // ============================== Levels and env ==================================
        // ================================================================================ 
        public ImageFile blockAllMight;
        public ImageFile[] roomBorder;
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

        public ImageFile fontDigitsSmall;
        public ImageFile fontDigitsSmallB;
        public ImageFile fontLargeBR;

        // ================================================================================
        // =================================== Items ======================================
        // ================================================================================ 
        public ImageFile itemTorch;
        public ImageFile itemTorchFlame;
        public ImageFile itemTorchAttackFlame;
        public ImageFile itemTorchShadow;
        public ImageFile goldOnGround;
        public ImageFile goldOnGroundFX;

        // ================================================================================
        // ================================== Enemies =====================================
        // ================================================================================ 
        public ImageFile healthBarMinions; 
        public ImageFile enemyBead; 

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

            blockAllMight = new ImageFile(content, "Images/Levels/blockAllMight", 16, 16);
            roomBorder =  new ImageFile[] { 
                new ImageFile(content, "Images/Levels/bloodBorder1", 1, 1), 
                new ImageFile(content, "Images/Levels/bloodBorder2", 1, 1)
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

            fontDigitsSmall = new ImageFile(content, "Images/UI/Fonts/DigitsSmall", 1, 1);
            fontDigitsSmallB = new ImageFile(content, "Images/UI/Fonts/DigitsSmallBlack", 1, 1);
            fontLargeBR = new ImageFile(content, "Images/UI/Fonts/fontBigBR", 1, 1);

            // --------------------------------------------------------------------------------
            // ----------------------------------- Items --------------------------------------
            itemTorch = new ImageFile(content, "Images/Items/item_torch", 4, 4);
            itemTorchFlame = new ImageFile(content, "Images/Items/torchFlame", 4, 4);
            itemTorchAttackFlame = new ImageFile(content, "Images/Items/torchFlameAttack", 4, 4);
            itemTorchShadow = new ImageFile(content, "Images/Items/torchShhadows", 4, 4);
            goldOnGround = new ImageFile(content, "Images/Items/goldOnGround", 4, 4);
            goldOnGroundFX = new ImageFile(content, "Images/Items/goldOnGroundFX", 4, 4);


            // --------------------------------------------------------------------------------
            // ---------------------------------- Enemies -------------------------------------
            healthBarMinions = new ImageFile(content, "Images/Enemies/HPBM", 1, 1);
            enemyBead = new ImageFile(content, "Images/Enemies/bead", 4, 4);
        }

        /// <summary>
        /// Generate a blank texture filled with given color. 
        /// </summary>
        /// <param name="Graphics"></param>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        /// <param name="Paint"></param>
        /// <returns></returns>
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


