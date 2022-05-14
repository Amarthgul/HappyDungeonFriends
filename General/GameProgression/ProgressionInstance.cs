using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HappyDungeon.General.GameProgression
{
    [Serializable]
    public struct ProgressionInstance
    {
        public string ID; 

        // Displayed attributes 
        public Texture2D savedThumbnail; 

        public Globals.GameLevel savedLevel;

        public DateTime savedTime;


        // Not displayed attributes 
        public int savedMapSize;
        public bool savedVirgin;
      
        public List<IItem> savedBagItemList { get; set; } 
        public IItem savedPrimary { get; set; }
        public List<IItem> savedUsable { get; set; }

        public int savedGoldCount { get; set; }
        public int savedScore { get; set; }

        public RoomInfo[,] savedMapInfo { get; set; }

        public int[] savedPlayerRoomIndex { get; set; }
        public Vector2 savedPlayerPosition { get; set; }
        public Globals.Direction savedPlayerFacingDir { get; set; }
        public int savedHealth { get; set; }

    }

    /// <summary>
    /// A more primary version of the ProgressionInstance, remade some varibles into 
    /// other types that can be serialized. 
    /// </summary>
    [Serializable]
    public struct SerializableInstance
    {
        public string ID;

        // Displayed attributes 
        public int savedLevel;

        public DateTime savedTime;

        // Not displayed attributes 
        public int savedMapSize;
        public bool savedVirgin;

        // The following dict it for items' " index : count "
        public List<int[]> savedBagItemList { get; set; }
        public int[] savedPrimary { get; set; }
        public List<int[]> savedUsable { get; set; }

        public int savedGoldCount { get; set; }
        public int savedScore { get; set; }

        public SerializableRoomInfo[,] savedMapInfo { get; set; }

        public int[] savedPlayerRoomIndex { get; set; }
        public Vector2 savedPlayerPosition { get; set; }


        public int savedPlayerFacingDir { get; set; }
        public int savedHealth { get; set; }
    }
}
