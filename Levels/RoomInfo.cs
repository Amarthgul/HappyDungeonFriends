using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyDungeon
{
    public class RoomInfo
    {
        public int[,] Arrangement { get; set; }

        public Globals.RoomTypes Type{ get; set; }

        public bool[] LockedDoors { get; set; }
        public bool[] Holes { get; set; }
        public bool[] MysteryDoors { get; set; }
        public bool[] OpenDoors { get; set; }

        public int DefaultBlock { get; set; }
    }
}
