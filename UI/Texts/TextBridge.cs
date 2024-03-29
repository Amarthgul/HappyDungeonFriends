﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace HappyDungeon
{
    /// <summary>
    /// If you want to add language support, add it here in this class 
    /// (need corresponding sprites and database)
    /// </summary>
    class TextBridge
    {
        public Globals.Language Language { set; get; }

        private GraphicsDevice graphicsDevice;
        private string notFound = "TextNotFound";

        private UI.Texts.TDB_en_US TDB_English;
        private UI.Texts.TDB_es TDB_Spanish;
        private UI.Texts.TDB_fr_FR TDB_French;
        private UI.Texts.TDB_ja_JP TDB_Japanese;
        private UI.Texts.TDB_zh_CN TDB_Chinese; 

        private UI.Texts.ITDB textDataBase; 

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
            graphicsDevice = G.GraphicsDevice;

            // In case someone in the future want to make the game capable of
            // switching language mid game 
            TDB_English = new UI.Texts.TDB_en_US();
            TDB_Spanish = new UI.Texts.TDB_es();
            TDB_French = new UI.Texts.TDB_fr_FR();
            TDB_Japanese = new UI.Texts.TDB_ja_JP();
            TDB_Chinese = new UI.Texts.TDB_zh_CN();

            switch (Language)
            {
                case Globals.Language.English:
                    textDataBase = TDB_English;
                    break;
                case Globals.Language.Chinese:
                    textDataBase = TDB_Chinese;
                    break;
                case Globals.Language.French:
                    textDataBase = TDB_French;
                    break;
                case Globals.Language.Japanese:
                    textDataBase = TDB_Japanese;
                    break;
                case Globals.Language.Spanish:
                    textDataBase = TDB_Spanish;
                    break; 
                default:
                    break;
            }
        }

        /// <summary>
        /// Refresh text database. 
        /// Some of the texts are created using flags, which might have been flagged 
        /// previsuly or in other instances. This method refreshes these flags so that
        /// they can be used again. 
        /// </summary>
        public void Refresh()
        {
            textDataBase.Refresh();
        }

        public string GetIndexedDescrption(int Index)
        {
            return textDataBase.IndexedDescription(Index);
        }

        public string GetTitleScreenOption(int Index)
        {
            return textDataBase.TitleOptions()[Index];
        }

        public string GetLoadAndSaveOptions(int Index)
        {
            return textDataBase.SaveAndLoadOptions()[Index];
        }

        public string[] GetSettingOptions()
        {
            return textDataBase.SettingOptions();
        }

        public string[] GetPauseOptions()
        {
            return textDataBase.PauseOptions();
        }

        public string[] GetNameWindowTexts()
        {
            return textDataBase.NameWindow(); 
        }

        public string GetDifficultyOptions(Globals.GameDifficulty DiffOption)
        {
            return textDataBase.DifficultyOptions(DiffOption);
        }

        public string GetNoteSetOneRandDescription()
        {
            return textDataBase.NoteSetOneRand();
        }

        public string[] GetgameOverOptions()
        {
            return textDataBase.DeathOptions();
        }

    }
}
