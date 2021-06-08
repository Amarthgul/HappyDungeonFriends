using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyDungeon.UI.Texts
{
    /// <summary>
    /// French, fancy 
    /// </summary>
    class TDB_fr_FR : ITDB
    {

        private Dictionary<int, string> indexedDescription = new Dictionary<int, string> {

        };

        public TDB_fr_FR()
        {

        }

        public string IndexedDescription(int Index)
        {
            return indexedDescription[Index];
        }

    }
}
