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
    /// I text is for text sprite generation. 
    /// Only generates words, control characters like new line and tab are not considered. 
    /// </summary>
    public interface IText
    {

        Texture2D GetText(string Text, GraphicsDevice G);

        bool IsValidInput(string Text);

        bool IsValidInput(char Single);

    }
}
