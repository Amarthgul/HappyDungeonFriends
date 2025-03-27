using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyDungeon
{
    class GenerateRoomJoy : Levels.GenerateRoomBasics
    {

        private bool _DEVMODE = true;

        // ================================================================================
        // ======================== Consts and frequently used ============================
        // ================================================================================

        private const int JOY_ENEMY_MAX = 5;



        public int[] tileList { get; set; } // For level delight


         

        public GenerateRoomJoy()
        {
            // Setup the template 
            roomDB = new Levels.RoomDB();

            enemyList = new int[]{
                Globals.ENEMY_BEAD
            };
            itemList = new int[] {

            };
            merchantItems = new int[] {

            };
            merchantCharaList = new int[] {

            };

            defaultBlock = 48;
            tileList = new int[] { 33, 34, 35, 36 };
            walkableBlockList = new int[] { 96 };
            solidBlockLIst = new int[] { 128, 144, 160, 161 };
            blackRoomInedx = 1;
            //bossIndex = Globals.BOSS_ENEMY;

        }



    }

}

