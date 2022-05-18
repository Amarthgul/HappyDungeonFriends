using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace HappyDungeon.UI
{
    /// <summary>
    /// Generates digits for small areas.
    /// </summary>
    class DigitsSmall : IText
    {
        private const int HEIGHT = 5;

        private Texture2D sourceDigits;
        private Texture2D sourceDigitsB;

        private Texture2D textTure;
        private Dictionary<char, int> digitWidth = new Dictionary<char, int>
        {
            {'0', 3 },
            {'1', 1 },
            {'2', 3 },
            {'3', 3 },
            {'4', 3 },
            {'5', 3 },
            {'6', 3 },
            {'7', 3 },
            {'8', 3 },
            {'9', 3 },
            {'/', 3 },
        };

        public bool useBlack { set; get; }
        public int interval { set; get; }

        public DigitsSmall()
        {
            sourceDigits = TextureFactory.Instance.fontDigitsSmall.texture;
            sourceDigitsB = TextureFactory.Instance.fontDigitsSmallB.texture;

            interval = 1;
            useBlack = false;
        }

        /// <summary>
        /// Given a character, find where to clip from source image
        /// </summary>
        /// <param name="C">Character to find</param>
        /// <returns>the X location of the char in source image</returns>
        private int GetCharLocation(char C)
        {
            int result = 0;

            for (int i = 0; digitWidth.ElementAt(i).Key != C; i++)
                result += digitWidth.ElementAt(i).Value;

            return result; 
        }

        /// <summary>
        /// Given a series of digits, get their renderable texture.
        /// </summary>
        /// <param name="Text">Target string of digits</param>
        /// <param name="G">GraphicDevice used to make texture</param>
        /// <returns></returns>
        public Texture2D GetText(string Text, GraphicsDevice G)
        {
            int FullWidth = 0;
            int Recorder = 0;
            Rectangle SourceRectangle = new Rectangle(0, 0, 0, HEIGHT) ;
            Rectangle DestRectangle = new Rectangle(0, 0, 0, HEIGHT);
            
            // get width 
            foreach (char c in Text)
            {
                if (digitWidth.ContainsKey(c))
                    FullWidth += digitWidth[c] + interval;
            }

            // Create a blank texture for pasting 
            textTure = TextureFactory.Instance.GenerateTexture(G, FullWidth, HEIGHT, pixel => Color.Transparent);

            // Iterate through and paste each into the final texture 
            foreach(char c in Text)
            {
                if (digitWidth.ContainsKey(c))
                {
                    Color[] Data = new Color[digitWidth[c] * HEIGHT];

                    SourceRectangle.X = GetCharLocation(c);
                    SourceRectangle.Width = digitWidth[c];
                    DestRectangle.X = Recorder;
                    DestRectangle.Width = digitWidth[c];

                    if(useBlack)
                        sourceDigitsB.GetData<Color>(0, SourceRectangle, Data, 0, Data.Length);
                    else
                        sourceDigits.GetData<Color>(0, SourceRectangle, Data, 0, Data.Length);

                    textTure.SetData(0, DestRectangle, Data, 0, Data.Length);

                    Recorder += digitWidth[c] + interval; 
                }
            }

            return textTure; 
        }

        public bool IsValidInput(string Text)
        {
            foreach (char single in Text)
                if (!digitWidth.ContainsKey(single))
                    return false;

            return true;
        }

        public bool IsValidInput(char Single)
        {
            return digitWidth.ContainsKey(Single);
        }
    }
}
