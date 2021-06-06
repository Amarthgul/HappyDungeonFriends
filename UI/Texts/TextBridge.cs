using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyDungeon
{
    /// <summary>
    /// If you want to add language support, add it here in this class 
    /// (need corresponding sprites and database)
    /// </summary>
    class TextBridge
    {
        public Globals.Language Language { set; get; }


        private string notFound = "TextNotFound";
        private UI.Texts.TDB_en_US TDB_English;
        

        private static TextBridge instance = new TextBridge();
        public static TextBridge Instance
        {
            get
            {
                return instance;
            }
        }

        public void Init(Game1 G)
        {
            Language = G.gameLanguage;
            TDB_English = new UI.Texts.TDB_en_US();
        }

        public string GetIndexedDescrption(int Index)
        {
            switch (Language)
            {
                case Globals.Language.English:
                    return TDB_English.IndexedDescription(Index);
                default:
                    return notFound;
            }
        }

    }
}
