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
            return new string[] { "Campaign", "Adventure", "Settings" };
        }

    }
}
