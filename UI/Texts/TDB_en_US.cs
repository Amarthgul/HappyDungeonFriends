using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyDungeon.UI.Texts
{
    class TDB_en_US : ITDB
    {

        private Dictionary<int, string> indexedDescription = new Dictionary<int, string> {
            {Globals.ITEM_TORCH, "A simple wooden stick with fabrics tied on top" },
            {Globals.ITEM_LINKEN, "It wispers" },
            {Globals.ITEM_NOTE_SO, "torch" },
            
        };



        public TDB_en_US()
        {

        }

        public string IndexedDescription(int Index)
        {
            return indexedDescription[Index];
        }

        public string[] TitleOptions()
        {
            return new string[] { "Campaign", "Adventure", "Load", "Settings" };
        }

        public string[] SettingOptions()
        {
            return new string[] { "Master Volume", "SFX Volume", "Music volume", "Difficulty", "Save", "Load", "Back", "Quit" };
        }

        public string[] PauseOptions()
        {
            return new string[] { "Continue", "Settings", "Save", "Back to menu", "Quit" };
        }

        public string[] DeathOptions()
        {
            return new string[] { "Restart", "Load from last save" };
        }

        public string[] DifficultyOptions()
        {
            return new string[] { "Idiot", "Normal" };
        }
        public string DifficultyOptions(Globals.GameDifficulty DiffOption)
        {
            switch (DiffOption)
            {
                case Globals.GameDifficulty.Normal:
                    return "Normal";
                case Globals.GameDifficulty.Idiot:
                    return "Idiot";
                default:
                    return "";
            }
        }
    }
}
