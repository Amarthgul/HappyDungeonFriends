using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace HappyDungeon
{
    /// <summary>
    /// Sounds effects, if not noted then it's from ZapSplat.com
    /// </summary>
    class SoundFX
    {

        public float volume { set; get; }
        public float pitchDefault { set; get; }
        public float panDefault { set; get; }

        // ================================================================================
        // ===================================== items ====================================
        // ================================================================================
        private SoundEffect[] itemGoldPickup;

        // ================================================================================
        // ==================================== Enemies ===================================
        // ================================================================================
        private SoundEffect[] enemyBBDie;
        private SoundEffect[] enemyBBMove;


        // ================================================================================
        // ================================= Levels and env ===============================
        // ================================================================================

        private static SoundFX instance = new SoundFX();
        public static SoundFX Instance {
            get { return instance; }
        }

        public SoundFX()
        {
            volume = 1.0f;
            pitchDefault = 0f;
            panDefault = 0f;
        }

        public void LoadAll(ContentManager Content)
        {
            itemGoldPickup = new SoundEffect[] { Content.Load<SoundEffect>("SFX/itemGoldPickup1") };

            enemyBBDie = new SoundEffect[] {    // All mono
                Content.Load<SoundEffect>("SFX/enemyBBDie1"),
                Content.Load<SoundEffect>("SFX/enemyBBDie2"),
                Content.Load<SoundEffect>("SFX/enemyBBDie3"),
                Content.Load<SoundEffect>("SFX/enemyBBDie4")
            };
            enemyBBMove = new SoundEffect[] {   // All mono
                Content.Load<SoundEffect>("SFX/enemyBBMove1"),
                Content.Load<SoundEffect>("SFX/enemyBBMove2"),
                Content.Load<SoundEffect>("SFX/enemyBBMove3"),
                Content.Load<SoundEffect>("SFX/enemyBBMove4"),
                Content.Load<SoundEffect>("SFX/enemyBBMove5"),
                Content.Load<SoundEffect>("SFX/enemyBBMove6")
            };
        }

        private void PlaySFX(SoundEffect SFX)
        {
            SFX.Play(volume, pitchDefault, panDefault);
        }

        private SoundEffect RandPick(SoundEffect[] List)
        {
            return List[Globals.RND.Next() % List.Length];
        }

        private void PlayAsStereo(SoundEffect SFX, float Volume, float Pan)
        {
            if (SFX == null) return;

            if (Math.Abs(Pan) < .5f)
            {
                float PanSide = .5f - Pan;
                SFX.Play(Volume, pitchDefault, Pan);
                SFX.Play(Volume, pitchDefault, PanSide);
            }
            else
            {
                SFX.Play(Volume, pitchDefault, Pan);
            }
            
        }

        // ================================================================================
        // ================================ Public methods ================================
        // ================================================================================

        public void PlayGoldPickupSFX()
        {
            SoundEffect GPU = RandPick(itemGoldPickup); ;
            PlaySFX(GPU);
        }

        public void PlayEnemyMoveSFX(int Index, float Volume, float Pan)
        {
            SoundEffect Target = null;
            switch (Index)
            {
                case Globals.ENEMY_BEAD:
                    Target = RandPick(enemyBBMove);
                    break;
                default:
                    break;
            }
            PlayAsStereo(Target, Volume, Pan);
        }

        public void PlayEnemyDieSFX(int Index, float Volume, float Pan)
        {
            SoundEffect Target = null; 
            switch (Index)
            {
                case Globals.ENEMY_BEAD:
                    Target = RandPick(enemyBBDie); 
                    break;
                default:
                    
                    break;
            }
            PlayAsStereo(Target, Volume, Pan);
            
            
        }

        public void PlayEnemyRangedAttack(int Index)
        {

        }

    }
}
