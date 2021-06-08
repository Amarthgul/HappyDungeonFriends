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
            {Globals.ITEM_TORCH, "torch" },
            {Globals.ITEM_LINKEN, "torch" },
            {Globals.ITEM_NOTE_SO, "torch" },
            {Globals.ITEM_TORCH, "torch" },
            {Globals.ITEM_TORCH, "torch" },
            {Globals.ITEM_TORCH, "torch" },
        };

        public TDB_en_US()
        {

        }

        public string IndexedDescription(int Index)
        {
            return "Helloworld";
        }

    }
}
