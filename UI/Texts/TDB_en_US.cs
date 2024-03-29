﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyDungeon.UI.Texts
{
    class TDB_en_US : TDB_BASE
    {
        // Should have 4 parts
        private string[] noteSetOne = new string[] {
            "This behavior makes it easier for formats like comma-separated values (CSV) files ",

            "multiple separator characters. The following example uses spaces, commas, " +
            "periods, colons, and tabs",

            "Natalie Portman 100 on the set of \"Leon The Professional",

            "nience method that lets you concatenate each element"
        };

        private bool[] noteSetOneMark = new bool[] { false, false, false, false };

        public TDB_en_US()
        {
            indexedDescription = new Dictionary<int, string> {
                { Globals.ITEM_TORCH, "A simple wooden stick with fabrics tied on top. Most likly left here by another poor soul." },
                { Globals.ITEM_LINKEN, "Shimmering orb, probably had its time in the past. Generates a shield that could absorb an attack" },
                { Globals.ITEM_NOTE_SO, "TEMP" },
                { Globals.ITEM_GOLD, "What can money do in this dark place?"}
            };
        }

        public override void Refresh()
        {
            noteSetOneMark = new bool[] { false, false, false, false };
        }


        public override string IndexedDescription(int Index)
        {
            return indexedDescription[Index];
        }

        public override string[] TitleOptions()
        {
            return new string[] { "Campaign", "Adventure", "Load", "Settings" };
        }

        public override string[] SettingOptions()
        {
            return new string[] { "Master Volume", "SFX Volume", "Music volume", "Difficulty", "Credits", "Reset", "Back", "Quit" };
        }

        public override string[] PauseOptions()
        {
            return new string[] { "Continue", "Settings", "Save", "Back to menu", "Quit" };
        }

        public override string[] DeathOptions()
        {
            return new string[] { "Restart", "Load from last save" };
        }

        public override string[] SaveAndLoadOptions()
        {
            return new string[] { "Load", "Save", "Override", "Back" };
        }

        public override string[] NameWindow()
        {
            return new string[] { "Give a name:", "Ja", "Nein" };
        }
        public override string DifficultyOptions(Globals.GameDifficulty DiffOption)
        {
            switch (DiffOption)
            {
                case Globals.GameDifficulty.Normal:
                    return "Normal";
                case Globals.GameDifficulty.Idiot:
                    return "Idiot";
                default:
                    return "";
            }
        }

        public override string NoteSetOneRand()
        {
            // If all notes are used, then return this
            if (noteSetOneMark.All(x => x == true))
                return "Everything on it looks blurry";

            // If there are still unused note, rand pick one
            int Index = Globals.RND.Next() % 4;
            while (noteSetOneMark[Index])
            {
                Index = Globals.RND.Next() % 4;
            }
            noteSetOneMark[Index] = true;

            return noteSetOne[Index];
        }
    }
}
