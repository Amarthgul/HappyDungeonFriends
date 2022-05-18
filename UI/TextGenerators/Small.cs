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
    /// Small font is mostly for descriptions and other text-heavy parts. 
    /// </summary>
    public class Small : IText
    {

        protected const int HEIGHT = 7;        // To accommodate, sprites needs extra spaces  
        protected const int ROW_LOC_LOW = 4;
        protected const int ROW_LOC_CAP = 13;
        protected const int ROW_LOC_DIG = 22;
        protected const int ROW_LOC_PUN = 31;

        protected bool upperCaseOnly = true;

        public Texture2D sourceTexts;

        public Texture2D textTure;

        protected Dictionary<char, int> digitWidth = new Dictionary<char, int>
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
        };
        protected Dictionary<char, int> alpLowerWidth = new Dictionary<char, int>
        {
            {'a', 3 },
            {'b', 3 },
            {'c', 3 },
            {'d', 3 },
            {'e', 3 },
            {'f', 3 },
            {'g', 3 },
            {'h', 3 },
            {'i', 1 },
            {'j', 2 },
            {'k', 3 },
            {'l', 2 },
            {'m', 5 },
            {'n', 3 },
            {'o', 3 },
            {'p', 3 },
            {'q', 3 },
            {'r', 2 },
            {'s', 3 },
            {'t', 3 },
            {'u', 3 },
            {'v', 3 },
            {'w', 5 },
            {'x', 3 },
            {'y', 3 },
            {'z', 3 },
        };
        protected Dictionary<char, int> alpCapWidth = new Dictionary<char, int>
        {
            {'A', 3 },
            {'B', 3 },
            {'C', 3 },
            {'D', 3 },
            {'E', 3 },
            {'F', 3 },
            {'G', 3 },
            {'H', 3 },
            {'I', 3 },
            {'J', 2 },
            {'K', 3 },
            {'L', 3 },
            {'M', 5 },
            {'N', 4 },
            {'O', 4 },
            {'P', 3 },
            {'Q', 4 },
            {'R', 3 },
            {'S', 3 },
            {'T', 3 },
            {'U', 3 },
            {'V', 3 },
            {'W', 5 },
            {'X', 3 },
            {'Y', 3 },
            {'Z', 4 },
        };
        protected Dictionary<char, int> puncWidth = new Dictionary<char, int>
        {
            {'!', 1 },
            {'\"', 1 },
            {'\'', 1 },
            {'(', 2 },
            {')', 2 },
            {'*', 2 },
            {'+', 3 },
            {',', 2 },
            {'-', 2 },
            {'.', 1 },
            {'/', 3 },
            {':', 1 },
            {';', 2 },
            {'<', 3 },
            {'=', 2 },
            {'>', 3 },
            {'?', 3 },
            {'%', 3 }
        };


        public int interval { set; get; }
        public int spaceSize { set; get; }
        public int widthOffset { set; get; }
        public int heightOffset{ set; get; }
        public Small()
        {
            sourceTexts = TextureFactory.Instance.fontSmall.texture;

            interval = 1;
            spaceSize = 3;
            widthOffset = 0;  // When it has a rim
            heightOffset = 0; // When it has a rim 

        }

        protected int GetCharWidth(char TarChar)
        {
            int result = 0;

            if (alpLowerWidth.ContainsKey(TarChar))
                result = alpLowerWidth[TarChar];
            else if (alpCapWidth.ContainsKey(TarChar))
                result = alpCapWidth[TarChar];
            else if (digitWidth.ContainsKey(TarChar))
                result = digitWidth[TarChar];
            else if (puncWidth.ContainsKey(TarChar))
                result = puncWidth[TarChar];
            else
                return 0;

            result += 2 * widthOffset; 

            return result;
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
                Result += ExmDict.ElementAt(i).Value + interval + 2 * widthOffset; // Small basic has 1 offset between each character

            return Result;

        }

        protected Texture2D GetCharTexture(char TarChar, GraphicsDevice G)
        {
            int RowLoc, CharWidth, CharSrcPosition;
            Texture2D SingleChar;
            Rectangle SourceRect;
            Color[] Data;

            if (alpLowerWidth.ContainsKey(TarChar))
                RowLoc = ROW_LOC_LOW - heightOffset;
            else if (alpCapWidth.ContainsKey(TarChar))
                RowLoc = ROW_LOC_CAP - heightOffset;
            else if (digitWidth.ContainsKey(TarChar))
                RowLoc = ROW_LOC_DIG - heightOffset;
            else if (puncWidth.ContainsKey(TarChar))
                RowLoc = ROW_LOC_PUN - heightOffset;
            else
                return null;

            CharWidth = GetCharWidth(TarChar);
            SingleChar = TextureFactory.Instance.GenerateTexture(G, CharWidth, HEIGHT + 2 * heightOffset, pixel => Color.Transparent);
            Data = new Color[SingleChar.Width * SingleChar.Height];

            CharSrcPosition = GetSrcLocation(TarChar);
            SourceRect = new Rectangle(CharSrcPosition, RowLoc, CharWidth, HEIGHT + 2 * heightOffset);

            sourceTexts.GetData(0, SourceRect, Data, 0, Data.Length);
            SingleChar.SetData(Data);

            return SingleChar;
        }

        public Texture2D GetText(string Text, GraphicsDevice G)
        {
            int FullWidth = 0;
            int Recorder = 0;
            Rectangle DestRectangle = new Rectangle(0, 0, 0, HEIGHT + 2 * heightOffset);

            if (upperCaseOnly)
                Text = Text.ToUpper();

            // Calculate full width of the output texture 
            foreach (char c in Text)
            {
                if (alpLowerWidth.ContainsKey(c) || alpCapWidth.ContainsKey(c)
                    || digitWidth.ContainsKey(c) || puncWidth.ContainsKey(c))
                    FullWidth += GetCharWidth(c) + interval;
                else if (c == ' ')
                    FullWidth += spaceSize;
            }

            // generate the blank texture 
            textTure = TextureFactory.Instance.GenerateTexture(G, FullWidth, HEIGHT + 2 * heightOffset, pixel => Color.Transparent);

            foreach (char c in Text)
            {

                if (c == ' ')
                {
                    Recorder += spaceSize;
                    continue;
                }

                Texture2D CharTextureNow = GetCharTexture(c, G);
                Color[] Data = new Color[CharTextureNow.Width * CharTextureNow.Height];
                CharTextureNow.GetData(Data);

                DestRectangle = new Rectangle(Recorder, 0, CharTextureNow.Width, HEIGHT + 2 * heightOffset);
                textTure.SetData(0, DestRectangle, Data, 0, Data.Length);

                Recorder += GetCharWidth(c) + interval;
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

