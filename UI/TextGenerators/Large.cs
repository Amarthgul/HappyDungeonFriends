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
    /// Larger font with black rims
    /// </summary>
    public class Large : IText
    {
        protected const int HEIGHT = 11;       // To accommodate this height, sprite images needs extra spaces  
        protected const int ROW_LOC_LOW = 0;   
        protected const int ROW_LOC_CAP = 18;
        protected const int ROW_LOC_DIG = 35;  // Offset a bit to align
        protected const int ROW_LOC_PUN = 54;

        public Texture2D sourceTexts;

        public Texture2D textTure;

        protected Dictionary<char, int> digitWidth = new Dictionary<char, int>
        {
            {'0', 7 },
            {'1', 5 },
            {'2', 7 },
            {'3', 7 },
            {'4', 7 },
            {'5', 7 },
            {'6', 7 },
            {'7', 7 },
            {'8', 7 },
            {'9', 7 },
        };
        protected Dictionary<char, int> alpLowerWidth = new Dictionary<char, int>
        {
            {'a', 6 },
            {'b', 6 },
            {'c', 6 },
            {'d', 6 },
            {'e', 6 },
            {'f', 5 },
            {'g', 6 },
            {'h', 6 },
            {'i', 3 },
            {'j', 4 },
            {'k', 6 },
            {'l', 4 },
            {'m', 7 },
            {'n', 6 },
            {'o', 6 },
            {'p', 6 },
            {'q', 6 },
            {'r', 5 },
            {'s', 6 },
            {'t', 5 },
            {'u', 6 },
            {'v', 7 },
            {'w', 7 },
            {'x', 7 },
            {'y', 6 },
            {'z', 6 },
        };
        protected Dictionary<char, int> alpCapWidth = new Dictionary<char, int>
        {
            {'A', 7 },
            {'B', 7 },
            {'C', 7 },
            {'D', 7 },
            {'E', 6 },
            {'F', 6 },
            {'G', 7 },
            {'H', 7 },
            {'I', 5 },
            {'J', 5 },
            {'K', 7 },
            {'L', 6 },
            {'M', 9 },
            {'N', 7 },
            {'O', 7 },
            {'P', 7 },
            {'Q', 8 },
            {'R', 7 },
            {'S', 7 },
            {'T', 7 },
            {'U', 7 },
            {'V', 9 },
            {'W', 9 },
            {'X', 9 },
            {'Y', 7 },
            {'Z', 7 },
        };
        protected Dictionary<char, int> puncWidth = new Dictionary<char, int> 
        {
            {'!', 3 },
            {'\"', 5 },
            {'\'', 3 },
            {'(', 5 },
            {')', 5 },
            {'*', 5 },
            {'+', 7 },
            {',', 4 },
            {'-', 5 },
            {'.', 3 },
            {'/', 7 },
            {':', 3 },
            {';', 4 },
            {'<', 5 },
            {'=', 5 },
            {'>', 5 },
            {'?', 7 },
            {'%', 7 }
        };


        public int interval { set; get; }
        public int spaceSize { set; get; }

        public Large()
        {
            sourceTexts = TextureFactory.Instance.fontLarge.texture;

            interval = 0;
            spaceSize = 2;
        }

        protected int GetCharWidth(char TarChar)
        {
            if (alpLowerWidth.ContainsKey(TarChar))
                return alpLowerWidth[TarChar];
            else if (alpCapWidth.ContainsKey(TarChar))
                return alpCapWidth[TarChar];
            else if (digitWidth.ContainsKey(TarChar))
                return digitWidth[TarChar];
            else if (puncWidth.ContainsKey(TarChar))
                return puncWidth[TarChar];
            else
                return 0; 
        }

        protected int GetSrcLocation(char TarChar)
        {
            Dictionary<char, int> ExmDict;
            int Result = 0; 

            if (Char.IsDigit(TarChar)) ExmDict = digitWidth;
            else if (Char.IsLower(TarChar)) ExmDict = alpLowerWidth;
            else if (Char.IsUpper(TarChar)) ExmDict = alpCapWidth;
            else ExmDict = puncWidth;

            for (int i = 0; ExmDict.ElementAt(i).Key != TarChar; i++)
                Result += ExmDict.ElementAt(i).Value;

            return Result;

        }

        protected Texture2D GetCharTexture(char TarChar, GraphicsDevice G)
        {
            int RowLoc, CharWidth, CharSrcPosition;
            Texture2D SingleChar; 
            Rectangle SourceRect;
            Color[] Data;

            if (alpLowerWidth.ContainsKey(TarChar))
                RowLoc = ROW_LOC_LOW;
            else if (alpCapWidth.ContainsKey(TarChar))
                RowLoc = ROW_LOC_CAP;
            else if (digitWidth.ContainsKey(TarChar))
                RowLoc = ROW_LOC_DIG;
            else if (puncWidth.ContainsKey(TarChar))
                RowLoc = ROW_LOC_PUN;
            else
                return null;

            CharWidth = GetCharWidth(TarChar);
            SingleChar = TextureFactory.Instance.GenerateTexture(G, CharWidth, HEIGHT, pixel => Color.Transparent);
            Data = new Color[SingleChar.Width * SingleChar.Height];

            CharSrcPosition = GetSrcLocation(TarChar);
            SourceRect = new Rectangle(CharSrcPosition, RowLoc, CharWidth, HEIGHT);

            sourceTexts.GetData(0, SourceRect, Data, 0, Data.Length);
            SingleChar.SetData(Data);

            return SingleChar;
        }

        public Texture2D GetText(string Text, GraphicsDevice G) 
        {
            int FullWidth = 0;
            int Recorder = 0;
            Rectangle DestRectangle = new Rectangle(0, 0, 0, HEIGHT);
            

            // Calculate full width of the output texture 
            foreach (char c in Text)
            {
                if (alpLowerWidth.ContainsKey(c) || alpCapWidth.ContainsKey(c)
                    || digitWidth.ContainsKey(c) || puncWidth.ContainsKey(c))
                    FullWidth += GetCharWidth(c) + interval;
                else if (c == ' ')
                    FullWidth += spaceSize;
            }

            textTure = TextureFactory.Instance.GenerateTexture(G, FullWidth, HEIGHT, pixel => Color.Transparent);

            foreach (char c in Text)
            {
                if(c == ' ')
                {
                    Recorder += spaceSize;
                    continue;
                }

                Texture2D CharTextureNow = GetCharTexture(c, G);
                Color[] Data = new Color[CharTextureNow.Width * CharTextureNow.Height];
                CharTextureNow.GetData(Data);

                DestRectangle = new Rectangle(Recorder, 0, CharTextureNow.Width, HEIGHT);
                textTure.SetData(0, DestRectangle, Data, 0, Data.Length);

                Recorder += GetCharWidth(c);
            }

            return textTure; 
        }

        public bool IsValidInput(string Text)
        {
            foreach (char single in Text)
                if (!digitWidth.ContainsKey(single) || !alpLowerWidth.ContainsKey(single) ||
                !alpCapWidth.ContainsKey(single) || !puncWidth.ContainsKey(single))
                    return false; 

            return true;
        }

        public bool IsValidInput(char Single)
        {
            return digitWidth.ContainsKey(Single) || alpLowerWidth.ContainsKey(Single) ||
                alpCapWidth.ContainsKey(Single) || puncWidth.ContainsKey(Single);
        }
    }
}
