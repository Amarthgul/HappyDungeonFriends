using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyDungeon
{
    class JunkItemTexts
    {




        private Game1 game; 

        public JunkItemTexts(Game1 G)
        {
            game = G; 

            switch (game.gameLanguage)
            {
                case Globals.Language.English:
                    LoadText_en_US();
                    break;
                case Globals.Language.Spanish:
                    break;
                default:
                    break; 
            }
        }


        private void LoadText_en_US()
        {

        }
    }
}
