using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyDungeon.UI.Texts
{
    /// <summary>
    /// Interface Text Database.
    /// Each implemtation is a different language, so that it could (possibily) support
    /// several languages without too much effort. 
    /// </summary>
    public interface ITDB
    {
        string IndexedDescription(int index);


        /// <summary>
        /// Should be having "Campaign", "Adventure", and "Settings" 3 options. 
        /// </summary>
        /// <returns></returns>
        string[] TitleOptions();

        string[] SettingOptions();

        string[] PauseOptions();

        string[] DeathOptions();
    }
}
