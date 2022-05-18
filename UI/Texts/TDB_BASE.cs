using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyDungeon.UI.Texts
{
    /// <summary>
    /// The basic, so that I don't have to go through every one of the language implementation 
    /// when adding methods to the interface. 
    /// </summary>
    class TDB_BASE : ITDB
    {

        protected Dictionary<int, string> indexedDescription = new Dictionary<int, string> {

        };

        public TDB_BASE()
        {

        }

        

        public virtual string IndexedDescription(int Index)
        {
            return indexedDescription[Index];
        }

        public virtual string[] TitleOptions()
        {
            return new string[] { };
        }

        public virtual string[] SettingOptions()
        {
            return new string[] { };
        }

        public virtual string[] PauseOptions()
        {
            return new string[] { };
        }

        public virtual string[] DeathOptions()
        {
            return new string[] { };
        }

        public virtual string[] SaveAndLoadOptions()
        {
            return new string[] { };
        }

        public virtual string[] NameWindow()
        {
            return new string[] { };
        }

        public virtual string DifficultyOptions(Globals.GameDifficulty DiffOption)
        {
            return " ";
        }

        public virtual string NoteSetOneRand()
        {
            return "";
        }
    }
}
