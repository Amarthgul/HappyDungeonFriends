using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyDungeon.UI.Texts
{
    /// <summary>
    /// I really want to add JP support but drawing 100+ characters are damn hard 
    /// </summary>
    class TDB_ja_JP : ITDB
    {

        private Dictionary<int, string> indexedDescription = new Dictionary<int, string> {

        };

        public TDB_ja_JP()
        {

        }

        public string IndexedDescription(int Index)
        {
            return indexedDescription[Index];
        }

        public string[] TitleOptions()
        {
            return new string[] { };
        }


        public string[] SettingOptions()
        {
            return new string[] { };
        }

        public string[] PauseOptions()
        {
            return new string[] { };
        }

        public string[] DeathOptions()
        {
            return new string[] { };
        }

        public string DifficultyOptions(Globals.GameDifficulty DiffOption)
        {
            return " ";
        }

    }

}
