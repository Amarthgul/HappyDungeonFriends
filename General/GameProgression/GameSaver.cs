using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace HappyDungeon.General.GameProgression
{
    class GameSaver
    {
        

        public GameSaver()
        {

        }

        public void SaveInstance(SerializableInstance Instance)
        {
            
            string basePath = AppDomain.CurrentDomain.BaseDirectory;

            string savePath = Path.Combine(basePath, Settings.saveToPath);
            //string savePath = basePath + Settings.saveToPath; 

            if (!Directory.Exists(savePath))
                Directory.CreateDirectory(savePath);

            string output = JsonConvert.SerializeObject(Instance);

            File.WriteAllText(Path.Combine(savePath, "rec.json"), output);

        }

        //public string UpdatePaths(string Paths)
        //{

        //}

        // ================================================================================
        // ============================== Private methods =================================
        // ================================================================================

        private void ParseSaveinstance(ProgressionInstance Instance)
        {

        }
    }
}
