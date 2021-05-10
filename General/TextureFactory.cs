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

        // ================================================================================
        // ================================= UI elements ==================================
        // ================================================================================ 
        public ImageFile uiFront;
        public ImageFile uiBack;
        public ImageFile miniRooms;
        public ImageFile playerNote;

        // ================================================================================
        // =================================== Items ======================================
        // ================================================================================ 
        public ImageFile itemTorch;
        public ImageFile itemTorchFlame;
        public ImageFile itemTorchShadow; 


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


            uiFront = new ImageFile(content, "Images/UI/HeadsupUIFront", 1, 1);
            uiBack = new ImageFile(content, "Images/UI/HeadsupUIBack", 1, 1);
            miniRooms = new ImageFile(content, "Images/UI/Minimap", 4, 4);
            playerNote = new ImageFile(content, "Images/UI/playerNotation", 4, 4);


            itemTorch = new ImageFile(content, "Images/Items/item_torch", 4, 4);
            itemTorchFlame = new ImageFile(content, "Images/Items/torchFlame", 4, 4);
            itemTorchShadow = new ImageFile(content, "Images/Items/torchShhadows", 4, 4);
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


