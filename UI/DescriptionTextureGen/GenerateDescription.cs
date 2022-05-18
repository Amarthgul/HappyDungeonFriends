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
    /// For items in the bag, this generates a box showing item descriptions 
    /// </summary>
    public class GenerateDescription
    {
        private const int WHOLE_WIDTH = 84;
        private const int WIDTH_TEXT_OFFSET = 3; 
        private const int SINGLE_ROW_HEIGHT = 7;
        private const int ROW_INTERVAL = 3;
        private const int TERMINAL_HEIGHT = 4; 

        private const int SINGLE_ROW_CHAR = 20;
        private const int MAX_LINE = 10; 

        char[] delimiterChars = { ' ' };

        private GraphicsDevice graphics;
        private string description;
        private List<string> segments = new List<string>();

        private UI.Small textgen = new UI.Small();

        private Texture2D texts;
        private Texture2D box; 

        private Color defaultFill = Color.Transparent;

        public GenerateDescription(GraphicsDevice G, IItem I)
        {
            graphics = G;
            description = I.GetItemDescription();

            Segmentation();
            GenerateText();
            GenerateBox();
        }

        /// <summary>
        /// Spilt the string into sentences that each fits the width of the box
        /// </summary>
        private void Segmentation()
        {
            string[] AllWords = description.Split(delimiterChars);
            List<string> Currentline = new List<string>();
            int CurrentLineLength = 0;

            foreach (string Word in AllWords)
            {
                if (CurrentLineLength + Word.Length + 1 > SINGLE_ROW_CHAR)
                {
                    segments.Add(string.Join(" ", Currentline.ToArray()));
                    Currentline = new List<string>();
                    CurrentLineLength = 0;
                    
                }
                Currentline.Add(Word);
                CurrentLineLength += Word.Length + 1;
            }
            segments.Add(string.Join(" ", Currentline.ToArray()));
        }

        /// <summary>
        /// Generate the texture for the box
        /// </summary>
        private void GenerateText()
        {
            int Width = 1;
            int Height = segments.Count * (SINGLE_ROW_HEIGHT + ROW_INTERVAL) - ROW_INTERVAL;

            foreach(string Sentence in segments)
            {
                int LineWidth = textgen.GetText(Sentence, graphics).Width;
                if (LineWidth > Width)
                    Width = LineWidth;
            }

            Texture2D Result = TextureFactory.Instance.GenerateTexture(graphics, Width, Height, pixel => default);

            for (int i = 0; i < segments.Count; i++)
            {
                Texture2D CurrentLine = textgen.GetText(segments[i], graphics);
                Color[] Data = new Color[CurrentLine.Width * CurrentLine.Height];
                CurrentLine.GetData(Data);

                Rectangle DestRect = new Rectangle(0, i * (SINGLE_ROW_HEIGHT + ROW_INTERVAL), CurrentLine.Width, CurrentLine.Height);
                Result.SetData(0, DestRect, Data, 0, Data.Length);
            }

            texts = Result;
        }

        /// <summary>
        /// Generate the texture for the underlaying box
        /// </summary>
        private void GenerateBox()
        {
            int Height = texts.Height + 2 * TERMINAL_HEIGHT; 
            Texture2D Result = TextureFactory.Instance.GenerateTexture(graphics, WHOLE_WIDTH, Height, pixel => default);

            Texture2D Holder = TextureFactory.Instance.bagOnHoverBoxTop.texture;
            Color[] Data = new Color[Holder.Width * Holder.Height]; 
            Holder.GetData(Data);
            Result.SetData(0, new Rectangle(0, 0, Holder.Width, Holder.Height), Data, 0, Data.Length);

            Holder = TextureFactory.Instance.bagOnHoverBoxMid.texture;
            Data = new Color[Holder.Width * Holder.Height];
            Holder.GetData(Data);
            for (int i = 0; i < texts.Height; i++)
            {
                Rectangle DestRect = new Rectangle(0, TERMINAL_HEIGHT + i, Holder.Width, Holder.Height);
                Result.SetData(0, DestRect, Data, 0, Data.Length);
            }

            Holder = TextureFactory.Instance.bagOnHoverBoxBot.texture;
            Data = new Color[Holder.Width * Holder.Height];
            Holder.GetData(Data);
            Result.SetData(0, new Rectangle(0, Result.Height - TERMINAL_HEIGHT, Holder.Width, Holder.Height), 
                Data, 0, Data.Length);

            box = Result;
        }

        /// <summary>
        /// Calculate the offset of the text, make it aligh to the middle 
        /// </summary>
        /// <returns>Suitable offset for the text</returns>
        public Vector2 TextPositionOffset()
        {
            Vector2 Result = new Vector2(0, 4);

            Result.X = (WHOLE_WIDTH - texts.Width) / 2;

            return Result;
        }

        public Texture2D GetTexture()
        {
            return texts;
        }

        public Texture2D GetBox()
        {
            return box;
        }

    }
}
