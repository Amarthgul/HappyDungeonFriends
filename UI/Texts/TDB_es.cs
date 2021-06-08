using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyDungeon.UI.Texts
{
    /// <summary>
    /// Spanish, god I know not a single word of this 
    /// </summary>
    class TDB_es : ITDB
    {

        private Dictionary<int, string> indexedDescription = new Dictionary<int, string> {

        };

        public TDB_es()
        {

        }

        public string IndexedDescription(int Index)
        {
            return indexedDescription[Index];
        }

    }
}
