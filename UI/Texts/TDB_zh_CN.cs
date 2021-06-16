using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyDungeon.UI.Texts
{
    /// <summary>
    /// I cannot even draw the simplified version (which would be around 3000 for this game?)
    /// let alone traditional chinese.
    /// </summary>
    class TDB_zh_CN : ITDB
    {

        private Dictionary<int, string> indexedDescription = new Dictionary<int, string> {

        };

        public TDB_zh_CN()
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
    }
}
