using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyDungeon.General.GameProgression
{
    public class Settings
    {
        // Length of the ID 
        public const int ID_LENGTH = 8; 

        /// <summary>
        /// The extension name, which is a
        /// </summary>
        public const string extensionName = ".json";

        /// <summary>
        /// Sub folders in which the game will try to search for saved game progressions.
        /// Please note that where to save is designated by saveToPath.  
        /// </summary>
        public static string[] subFolders = new string[] { 
            "",
            "Content", 
            "Content\\Saved",
            "Content\\Save"
        };

        /// <summary>
        /// The game will try to save to this path. 
        /// If this path does not exist, then will try to create one. 
        /// Please note that when trying to find saved game progressions, 
        /// the game will look into every path listed in subFolders. 
        /// </summary>
        public const string saveToPath = "Content\\Saved";



    }
}
