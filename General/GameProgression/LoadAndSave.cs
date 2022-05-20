using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace HappyDungeon
{
    /// <summary>
    /// Load and save as a feature, this runs underneath the load and save display. 
    /// </summary>
    public class LoadAndSave
    {

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
            savedInstances = new List<General.GameProgression.ProgressionInstance>();

            savePaths = new List<string>();
            savePaths.Add(SaveToPath);
            foreach (string subPath in General.GameProgression.Settings.subFolders)
            {
                savePaths.Add(Path.Combine(BasePath, subPath));
            }

            UpdateSavedInstances();
        }

        
        /// <summary>
        /// Initiate an attempt to save 
        /// </summary>
        public void SaveNow(string SaveFileName)
        {
            General.GameProgression.ProgressionInstance Progression = TakeSnapshot(SaveFileName);

            saver.SaveInstance(SerializeInstance(Progression), Progression.savedThumbnail);

            UpdateSavedInstances();
        }


        /// <summary>
        /// Override a given record. 
        /// Essentially the same as SaveNow, but replacing the name and ID with given record.
        /// </summary>
        /// <param name="Target">Which one to be overriden</param>
        public void OverrideNow(General.GameProgression.ProgressionInstance Target)
        {
            General.GameProgression.ProgressionInstance Progression = TakeSnapshot(Target.saveName);
            Progression.ID = Target.ID;

            saver.SaveInstance(SerializeInstance(Progression), Progression.savedThumbnail);

            UpdateSavedInstances();
        }

        /// <summary>
        /// Load a saved game stats, current progression (if has any) will be overriden.
        /// Note that the game also needs to be reset in order to make them effective. 
        /// </summary>
        /// <param name="Target">What to load</param>
        public void LoadNow(General.GameProgression.ProgressionInstance Target)
        {
            game.gameLevel = Target.savedLevel;

            game.mapSize = Target.savedMapSize;
            game.virgin = Target.savedVirgin;

            for (int i = 0; i < Globals.BAG_SIZE; i++)
                game.spellSlots.PutIntoBag(Target.savedBagItemList[i], i);
            game.spellSlots.PutIntoPrimary(Target.savedPrimary);
            for (int i = 0; i < Globals.SLOT_SIZE; i++)
                game.spellSlots.PutIntoUsable(Target.savedUsable[i], i);
            

            game.goldCount = Target.savedGoldCount;
            game.gameScore = Target.savedScore;

            game.roomCycler.currentMapSet = Target.savedMapInfo;
            game.roomCycler.MoveIntoRoom( Target.savedPlayerRoomIndex[0], Target.savedPlayerRoomIndex[1]);

            game.mainChara.position = Target.savedPlayerPosition;
            game.mainChara.facingDir = Target.savedPlayerFacingDir;
            game.mainChara.currentHealth = Target.savedHealth;

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

                // If this record exists then remove the old one to override it
                if (savedInstances.Exists(x => x.ID == iter.ID)) 
                {
                    savedInstances.Remove(savedInstances.Find(x => x.ID == iter.ID));
                }

                // If this record is unique so far, add it into the saved 
                savedInstances.Add(iter);
            }

            return savedInstances; 
        }

        // ================================================================================
        // ============================== Private methods =================================
        // ================================================================================

        /// <summary>
        /// Dissect a Roominfo into several parts for serialization 
        /// </summary>
        /// <param name="Target">A single RoomInfo</param>
        /// <returns>SerializableRoomInfo containing serializable contents</returns>
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
            Result.Explored = Target.Explored; 
            Result.LockedDoors = Target.LockedDoors;
            Result.Holes = Target.Holes;
            Result.MysteryDoors = Target.MysteryDoors;
            Result.OpenDoors = Target.OpenDoors;
            Result.DefaultBlock = Target.DefaultBlock;

            return Result;
        }

        /// <summary>
        /// Parse a SerializableRoomInfo into a RoomInfo
        /// </summary>
        /// <param name="Target">One SerializableRoomInfo</param>
        /// <returns>Deserialized RoomInfo</returns>
        public RoomInfo DeserializeRoomInfo(SerializableRoomInfo Target)
        {
            if (Target == null) return null;

            RoomInfo Result = new RoomInfo();

            Result.Arrangement = new int[Target.Arrangement.GetLength(0), Target.Arrangement.GetLength(1)];
            for (int i = 0; i < Target.Arrangement.GetLength(0); i++)
            {
                for (int j = 0; j < Target.Arrangement.GetLength(1); j++)
                {
                    Result.Arrangement[i, j] = Target.Arrangement[i, j];
                }
            }

            Result.Type = (Globals.RoomTypes)Target.Type;
            Result.Explored = Target.Explored;
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
        private General.GameProgression.ProgressionInstance TakeSnapshot(string Name)
        {
            General.GameProgression.ProgressionInstance Snapshot;

            Snapshot = new General.GameProgression.ProgressionInstance();

            // ID is random generated number truncated into certain length 
            Snapshot.ID = (Globals.RND.NextDouble() * Math.Pow(10, 
                General.GameProgression.Settings.ID_LENGTH)).ToString().Substring(0, General.GameProgression.Settings.ID_LENGTH);
            Snapshot.saveName = Name; 

            Snapshot.savedThumbnail = game.screenFX.partialScreenshot;
            Snapshot.thumbnailSize = new int[] { Snapshot.savedThumbnail.Width, Snapshot.savedThumbnail.Height };

            Snapshot.savedLevel = game.gameLevel;
            Snapshot.savedTime = DateTime.Now;
            
            Snapshot.savedMapSize = game.mapSize;
            Snapshot.savedVirgin = game.virgin;

            Snapshot.savedBagItemList = game.spellSlots.bag;
            Snapshot.savedPrimary = game.spellSlots.primary;
            Snapshot.savedUsable = game.spellSlots.itemSlots;

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

            Serialized.savedThumbnail = new Color[Source.savedThumbnail.Width * Source.savedThumbnail.Height];
            Source.savedThumbnail.GetData(Serialized.savedThumbnail);
            Serialized.thumbnailSize = Source.thumbnailSize; 

            Serialized.ID = Source.ID;
            Serialized.saveName = Source.saveName;
            Serialized.savedLevel = (int)Source.savedLevel;
            Serialized.savedTime = Source.savedTime;
            Serialized.savedMapSize = Source.savedMapSize; 
            Serialized.savedVirgin = Source.savedVirgin;


            // -------------------------------------------------------------------------------
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
            // -------------------------------------------------------------------------------


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

        /// <summary>
        /// Parse a SerializableInstance into a ProgressionInstance
        /// </summary>
        /// <param name="Source">Progression in SerializableInstance form</param>
        /// <returns>ProgressionInstance form</returns>
        private General.GameProgression.ProgressionInstance DeserializeInstance(General.GameProgression.SerializableInstance Source)
        {
            General.GameProgression.ProgressionInstance Result = new General.GameProgression.ProgressionInstance();

            Result.ID = Source.ID;
            Result.saveName = Source.saveName;

            Result.savedThumbnail = new Texture2D(game.GraphicsDevice, Source.thumbnailSize[0], Source.thumbnailSize[1]);
            Result.savedThumbnail.SetData(Source.savedThumbnail);

            Result.savedLevel = (Globals.GameLevel)Source.savedLevel;
            Result.savedTime = Source.savedTime;
            Result.savedMapSize = Source.savedMapSize;
            Result.savedVirgin = Source.savedVirgin;

            // -------------------------------------------------------------------------------
            // Items in the bag 
            Result.savedBagItemList = new List<IItem>();
            for(int i = 0; i < Globals.BAG_SIZE; i++)
            {
                if (Source.savedBagItemList[i] == null)
                    Result.savedBagItemList.Add(null);
                else
                    Result.savedBagItemList.Add(RecreateItem(Source.savedBagItemList[i][0], Source.savedBagItemList[i][1]));
            }

            // Item on primary slot 
            if (Source.savedPrimary[0] == Globals.NULL_INDEX && Source.savedPrimary[1] == Globals.NULL_INDEX)
                Result.savedPrimary = null;
            else
                Result.savedPrimary = RecreateItem(Source.savedPrimary[0], Source.savedPrimary[1]);

            // Items on the 3 top slots
            Result.savedUsable = new List<IItem>();
            for (int i = 0; i < Globals.SLOT_SIZE; i++)
            {
                if (Source.savedUsable[i][0] == Globals.NULL_INDEX && Source.savedUsable[i][1] == Globals.NULL_INDEX)
                    Result.savedUsable.Add(null);
                else
                    Result.savedUsable.Add(RecreateItem(Source.savedUsable[i][0], Source.savedUsable[i][1]));
            }

            Result.savedGoldCount = Source.savedGoldCount;
            Result.savedScore = Source.savedScore;

            Result.savedMapInfo = new RoomInfo[Source.savedMapInfo.GetLength(0), Source.savedMapInfo.GetLength(1)];
            for (int i = 0; i < Source.savedMapInfo.GetLength(0); i++)
            {
                for (int j = 0; j < Source.savedMapInfo.GetLength(1); j++)
                {
                    Result.savedMapInfo[i, j] = DeserializeRoomInfo(Source.savedMapInfo[i, j]);
                }
            }

            Result.savedPlayerRoomIndex = Source.savedPlayerRoomIndex;
            Result.savedPlayerPosition = Source.savedPlayerPosition;
            Result.savedPlayerFacingDir = (Globals.Direction)Source.savedPlayerFacingDir;
            Result.savedHealth = Source.savedHealth; 

            return Result; 
        }

        /// <summary>
        /// Given item index and count, recreate this item from read saved file.
        /// </summary>
        /// <param name="Index">Item index</param>
        /// <param name="Count">The count of such item</param>
        /// <returns>The item instance in its class</returns>
        private IItem RecreateItem(int Index, int Count)
        {
            IItem Result;
            Vector2 Placeholder = new Vector2(); 

            switch (Index)
            {
                case Globals.ITEM_TORCH:
                    Result = new Torch(game, Placeholder); 
                    break;
                case Globals.ITEM_LINKEN:
                    Result = new LinkenSphere(game, Placeholder);
                    break;
                case Globals.ITEM_NOTE_SO:
                    Result = new NoteSetOne(game, Placeholder);
                    break;
                case Globals.ITEM_GOLD:
                    Result = new DroppedGold(game, Placeholder);
                    break;
                case Globals.ITEM_STD:
                    Result = new IItemSTD(game, Placeholder);
                    break; 
                default:
                    Result = null;
                    break;
            }

            return Result; 
        }

    }
}
