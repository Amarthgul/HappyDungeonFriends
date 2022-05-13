using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace HappyDungeon.General.GameProgression
{
    class GameLoader
    {


        public GameLoader()
        {
            
        }


        public List<string> GetFiles(List<String> PathToSeek)
        {
            string pa = AppDomain.CurrentDomain.BaseDirectory;
            List<string> files = new List<string>();

            foreach (string path in PathToSeek)
            {
                if (Directory.Exists(path))
                {
                    foreach (string file in Directory.GetFiles(path))
                    {
                        if (Path.GetExtension(file) == Settings.extensionName)
                            files.Add(file);
                    }   
                }
            }
            return files; 
        }

        /// <summary>
        /// Try to deserialize files into SerializableInstance
        /// </summary>
        /// <param name="FilesToRead">Paths of the json files</param>
        /// <returns>SerializableInstance for all that can be deserialized</returns>
        public List<SerializableInstance> GetSerializableInstance(List<String> FilesToRead)
        {
            List<SerializableInstance> Result = new List<SerializableInstance>();

            foreach (string FilePath in FilesToRead)
            {
                try
                {
                    string text = File.ReadAllText(FilePath);
                    SerializableInstance iter = JsonConvert.DeserializeObject<SerializableInstance>(text);
                    Result.Add(iter);
                }
                catch (Exception)
                {
                    continue;
                }
            }
            return Result; 
        }


    }
}
