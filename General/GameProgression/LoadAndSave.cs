using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyDungeon
{
    /// <summary>
    /// Load and save as a feature, different from the display 
    /// </summary>
    public class LoadAndSave
    {

        private Game1 game;

        private General.GameProgression.GameLoader loader;
        private General.GameProgression.GameSaver saver; 

        public LoadAndSave(Game1 G)
        {
            game = G; 
        }

        public General.GameProgression.ProgressionInstance[] GetSavedInstances()
        {
            return null; 
        }

    }
}
