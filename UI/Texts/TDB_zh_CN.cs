using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyDungeon.UI.Texts
{
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

    }
}
