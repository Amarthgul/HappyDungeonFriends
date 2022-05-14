using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace HappyDungeon.General.GameProgression
{
    class GameSaver
    {
        

        public GameSaver()
        {

        }

        public void SaveInstance(SerializableInstance Instance, Texture2D Thumbnail)
        {
            
            string basePath = AppDomain.CurrentDomain.BaseDirectory;

            string savePath = Path.Combine(basePath, Settings.saveToPath);
            //string savePath = basePath + Settings.saveToPath; 

            if (!Directory.Exists(savePath))
                Directory.CreateDirectory(savePath);

            string output = JsonConvert.SerializeObject(Instance, Formatting.Indented);

            File.WriteAllText(Path.Combine(savePath, "rec.json"), output);

        }


        // ================================================================================
        // ============================== Private methods =================================
        // ================================================================================

    }
}
