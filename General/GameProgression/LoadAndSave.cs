using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace HappyDungeon
{
    /// <summary>
    /// Load and save as a feature, different from the display. 
    /// </summary>
    public class LoadAndSave
    {

        private const int PAIR = 2; 

        private Game1 game;

        private List<String> savePaths;    // Where the files will be saved to 

        private List<string> instancePaths; // The path of saved instances including file names  
        private List<General.GameProgression.ProgressionInstance> savedInstances;

        private General.GameProgression.GameLoader loader;
        private General.GameProgression.GameSaver saver; 

        public LoadAndSave(Game1 G)
        {
            game = G;

            saver = new General.GameProgression.GameSaver();
            loader = new General.GameProgression.GameLoader();

            string BasePath = AppDomain.CurrentDomain.BaseDirectory;
            string SaveToPath = Path.Combine(BasePath, General.GameProgression.Settings.saveToPath);

            instancePaths = new List<string>();

            savePaths = new List<string>();
            savePaths.Add(SaveToPath);
            foreach (string subPath in General.GameProgression.Settings.subFolders)
            {
                savePaths.Add(Path.Combine(BasePath, subPath));
            }

            UpdateSavedInstances();
        }

        
        public void SaveNow()
        {
            General.GameProgression.ProgressionInstance Progression = TakeSnapshot();

            saver.SaveInstance(SerializeInstance(Progression));

            UpdateSavedInstances();
        }


        /// <summary>
        /// Update the saved instances by refreshing and scan for new saves.
        /// </summary>
        /// <returns>Saved instances, if there are any</returns>
        public List<General.GameProgression.ProgressionInstance> UpdateSavedInstances()
        {
            instancePaths = loader.GetFiles(savePaths);
            List<General.GameProgression.SerializableInstance> SerializableInstances = loader.GetSerializableInstance(instancePaths);

            foreach(General.GameProgression.SerializableInstance Serializable in SerializableInstances)
            {
                General.GameProgression.ProgressionInstance iter = DeserializeInstance(Serializable); 
            }

            return savedInstances; 
        }

        // ================================================================================
        // ============================== Private methods =================================
        // ================================================================================
        public SerializableRoomInfo SerializeRoomInfo(RoomInfo Target)
        {
            if (Target == null) return null; 

            SerializableRoomInfo Result = new SerializableRoomInfo();

            Result.Arrangement = new int[Target.Arrangement.GetLength(0), Target.Arrangement.GetLength(1)];
            for (int i = 0; i < Target.Arrangement.GetLength(0); i++)
            {
                for (int j = 0; j< Target.Arrangement.GetLength(1); j++)
                {
                    Result.Arrangement[i, j] = Target.Arrangement[i, j];
                }
            }

            Result.Type = (int)Target.Type;
            Result.LockedDoors = Target.LockedDoors;
            Result.Holes = Target.Holes;
            Result.MysteryDoors = Target.MysteryDoors;
            Result.OpenDoors = Target.OpenDoors;
            Result.DefaultBlock = Target.DefaultBlock;

            return Result;
        }

        /// <summary>
        /// Create the initial snapshot ProgressionInstance. 
        /// </summary>
        /// <returns>ProgressionInstance for current game</returns>
        private General.GameProgression.ProgressionInstance TakeSnapshot()
        {
            General.GameProgression.ProgressionInstance Snapshot;

            Snapshot = new General.GameProgression.ProgressionInstance();

            Snapshot.ID = (Globals.RND.NextDouble() * Math.Pow(10, General.GameProgression.Settings.ID_LENGTH)).ToString().Substring(0, General.GameProgression.Settings.ID_LENGTH);

            Snapshot.savedThumbnail = game.screenFX.screenCapbackup;
            Snapshot.savedLevel = game.gameLevel;
            Snapshot.savedTime = DateTime.Now;

            Snapshot.savedMapSize = game.mapSize;
            Snapshot.savedVirgin = game.virgin;

            Snapshot.savedBagItemList = game.spellSlots.bag.ToList();
            Snapshot.savedPrimary = game.spellSlots.GetItem(-1);
            Snapshot.savedUsable = game.spellSlots.itemSlots.ToList();

            Snapshot.savedGoldCount = game.goldCount;
            Snapshot.savedScore = game.gameScore; 

            Snapshot.savedMapInfo = game.roomCycler.currentMapSet;

            Snapshot.savedPlayerRoomIndex = game.roomCycler.currentLocationIndex; 
            Snapshot.savedPlayerPosition = game.mainChara.position;
            Snapshot.savedPlayerFacingDir = game.mainChara.facingDir;
            Snapshot.savedHealth = game.mainChara.currentHealth; 

            return Snapshot; 
        }


        /// <summary>
        /// Produced a serializable instance of game progression. 
        /// </summary>
        /// <param name="Source"></param>
        /// <returns></returns>
        private General.GameProgression.SerializableInstance SerializeInstance(General.GameProgression.ProgressionInstance Source)
        {
            General.GameProgression.SerializableInstance Serialized;
            Serialized = new General.GameProgression.SerializableInstance();

            Serialized.ID = Source.ID; 

            Serialized.savedLevel = (int)Source.savedLevel;
            Serialized.savedVirgin = Source.savedVirgin;

            // Items in the bag 
            Serialized.savedBagItemList = new List<int[]>();
            for(int i = 0; i < Globals.BAG_SIZE; i++)
            {
                if (Source.savedBagItemList[i] == null)
                    Serialized.savedBagItemList.Add(new int[]{ Globals.NULL_INDEX, Globals.NULL_INDEX});
                else
                    Serialized.savedBagItemList.Add(new int[] { Source.savedBagItemList[i].SelfIndex(),
                        Source.savedBagItemList[i].GetCount() });
            }

            // Item on primary slot 
            if (Source.savedPrimary == null)
                Serialized.savedPrimary = new int[]{ Globals.NULL_INDEX, Globals.NULL_INDEX};
            else
                Serialized.savedPrimary= new int[] { Source.savedPrimary.SelfIndex(), Source.savedPrimary.GetCount() };

            // Items on the 3 top slots
            Serialized.savedUsable = new List<int[]>();
            for (int i = 0; i < Globals.SLOT_SIZE; i++)
            {
                if (Source.savedUsable[i] == null)
                    Serialized.savedUsable.Add(new int[] { Globals.NULL_INDEX, Globals.NULL_INDEX });
                else
                    Serialized.savedUsable.Add(new int[] { Source.savedUsable[i].SelfIndex(), 
                        Source.savedUsable[i].GetCount() });
            }

            Serialized.savedGoldCount = Source.savedGoldCount;
            Serialized.savedScore = Source.savedScore;


            Serialized.savedMapInfo = new SerializableRoomInfo[Source.savedMapInfo.GetLength(0), Source.savedMapInfo.GetLength(1)];
            for (int i = 0; i < Source.savedMapInfo.GetLength(0); i++)
            {
                for (int j = 0; j < Source.savedMapInfo.GetLength(1); j++)
                {
                    Serialized.savedMapInfo[i, j] = SerializeRoomInfo(Source.savedMapInfo[i, j]);
                }
            }

            Serialized.savedPlayerRoomIndex = Source.savedPlayerRoomIndex;
            Serialized.savedPlayerPosition = Source.savedPlayerPosition;

            Serialized.savedPlayerFacingDir = (int)Source.savedPlayerFacingDir;
            Serialized.savedHealth = Source.savedHealth;

            return Serialized; 
        }


        private General.GameProgression.ProgressionInstance DeserializeInstance(General.GameProgression.SerializableInstance Source)
        {
            General.GameProgression.ProgressionInstance Result = new General.GameProgression.ProgressionInstance();



            return Result; 
        }

    }
}
