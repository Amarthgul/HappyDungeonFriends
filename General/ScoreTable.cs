using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyDungeon.General
{
    public class ScoreTable
    {
        private const int DEFAULT_SCORE = 10;
        private const double DEFAULT_DIFFICULTY_MULTI = 1.0; // Theoretically impossible to be used 

       
        private Dictionary<Globals.GameDifficulty, double> difficultyMultiplier
            = new Dictionary<Globals.GameDifficulty, double>() {
                {Globals.GameDifficulty.Idiot,   1.0 },
                {Globals.GameDifficulty.Normal,  1.5 }
            };


        // This chart represents single enemy/item
        // That is, the score recevived per kill/acquirement
        private Dictionary<int, int> scoreMappings = new Dictionary<int, int>()
        {
            // Items 
            {-1,   5  }, // ITEM_TORCH or Torch 
            {-10,  5  }, // ITEM_LINKEN or LikenSphere 
            {-20,  1  }, // ITEM_NOTE_SO or NoteSetOne 
            {-255, 1  }, // ITEM_GOLD or DroppedGold 
            // Enemies 
            {-156, 10 }, // ENEMY_STD or IEnemySTD
            {-257, 20 }  // ENEMY_BEAD or BloodBead
        };



        private static ScoreTable instance = new ScoreTable();
        public static ScoreTable Instance
        {
            get
            {
                return instance;
            }
        }
        private ScoreTable()
        {
        }



        /// <summary>
        /// Return the score of killing an enemy or achieving something
        /// </summary>
        /// <param name="QueryIndex">Index of the the otem</param>
        /// <param name="GameDifficulty">Difficulty setting</param>
        /// <returns></returns>
        public int getScore(int QueryIndex, Globals.GameDifficulty GameDifficulty)
        {
            // Score table has no direct access to the game class, so game difficulty 
            // is set as a parameter instead of a class attribute. 

            if (scoreMappings.ContainsKey(QueryIndex))
            {
                if (difficultyMultiplier.ContainsKey(GameDifficulty))
                    return (int)(scoreMappings[QueryIndex] * difficultyMultiplier[GameDifficulty]);
                else
                    return (int)(scoreMappings[QueryIndex] * DEFAULT_DIFFICULTY_MULTI);
            }

            return (int)(DEFAULT_SCORE * DEFAULT_DIFFICULTY_MULTI); 
        }
    }
}
