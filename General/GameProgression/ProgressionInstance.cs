using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyDungeon.General.GameProgression
{
    public class ProgressionInstance
    {
        // Displayed attributes 
        public Texture2D savedThumbnail; 
        public Globals.GameLevel savedLevel;
        public DateTime savedTime;

        // Not displayed attributes 
        public int savedMapSize;
        public bool savedVirgin;

        public List<IEnemy> savedEnemyList { get; set; }          
        public List<IItem> savedCollectibleItemList { get; set; }  
        public List<IItem> savedBagItemList { get; set; }     
        public int savedGoldCount { get; set; }

        public RoomInfo[,] savedMapInfo { get; set; }

        public Vector2 savedPlayerPosition { get; set; }
        public Globals.Direction savedPlayerFacingDir { get; set; }
        public int savedhealth { get; set; }



    }
}
