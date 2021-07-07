using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;

namespace HappyDungeon
{
    /// <summary>
    /// Sounds effects, if not noted then it's from ZapSplat.com
    /// </summary>
    class SoundFX
    {
        private Stopwatch BGMSW = new Stopwatch();
        private int BGMInterval = 9500;
        private bool fisrtStartUp = true;

        public float volume { set; get; }
        public float pitchDefault { set; get; }
        public float panDefault { set; get; }

        public static float masterVolume;
        public static float SFXVolume;
        public static float musicVolume;

        // ================================================================================
        // ================================ Main character ================================
        // ================================================================================
        private SoundEffect[] mcAttackWithTorch;
        private SoundEffect[] mcWalkWithTorch;
        private SoundEffect[] mcWalkLDelight; 

        // ================================================================================
        // ===================================== items ====================================
        // ================================================================================
        private SoundEffect[] itemGoldPickup;
        private SoundEffect[] itemLikenPickup; 
        private SoundEffect[] itemLinkenOn;
        private SoundEffect[] itemLinkenBreak;
        private SoundEffect[] itemLikenRad; 

        // ================================================================================
        // ==================================== Enemies ===================================
        // ================================================================================
        private SoundEffect[] enemyBBDie;
        private SoundEffect[] enemyBBMove;


        // ================================================================================
        // ================================= Levels and env ===============================
        // ================================================================================
        private SoundEffect[] envOpenMysDoor; 

        // ================================================================================
        // ======================================= UI =====================================
        // ================================================================================
        private SoundEffect[] bagLMBSlect;
        private SoundEffect[] bagLMBRelease;
        private SoundEffect[] bagOnhover;

        private SoundEffect[] settingSliderClick; 

        private SoundEffect[] titleOnHover;
        private SoundEffect[] titleClick;

        // ================================================================================
        // ====================================== BGM =====================================
        // ================================================================================
        private SoundEffect[] LV1BGM;

        private static SoundFX instance = new SoundFX();
        public static SoundFX Instance {
            get { return instance; }
        }

        public SoundFX()
        {
            volume = 1.0f;
            pitchDefault = 0f;
            panDefault = 0f;

            BGMSW.Restart();
        }

        public void LoadAll(ContentManager Content)
        {
            // --------------------------------------------------------------------------------
            // ------------------------------ Main character ----------------------------------
            mcAttackWithTorch = new SoundEffect[] {
                Content.Load<SoundEffect>("SFX/MC/mcAttackTorch1"),
                Content.Load<SoundEffect>("SFX/MC/mcAttackTorch2"),
                Content.Load<SoundEffect>("SFX/MC/mcAttackTorch3")
            };
            mcWalkWithTorch = new SoundEffect[] {
                Content.Load<SoundEffect>("SFX/MC/mcWalkWithTorch1"),
                Content.Load<SoundEffect>("SFX/MC/mcWalkWithTorch2"),
                Content.Load<SoundEffect>("SFX/MC/mcWalkWithTorch3"),
                Content.Load<SoundEffect>("SFX/MC/mcWalkWithTorch4"),
                Content.Load<SoundEffect>("SFX/MC/mcWalkWithTorch5"),
                Content.Load<SoundEffect>("SFX/MC/mcWalkWithTorch6"),
                Content.Load<SoundEffect>("SFX/MC/mcWalkWithTorch7"),
                Content.Load<SoundEffect>("SFX/MC/mcWalkWithTorch8"),
                Content.Load<SoundEffect>("SFX/MC/mcWalkWithTorch9"),
                Content.Load<SoundEffect>("SFX/MC/mcWalkWithTorch10"),
                Content.Load<SoundEffect>("SFX/MC/mcWalkWithTorch11"),
                Content.Load<SoundEffect>("SFX/MC/mcWalkWithTorch12")
            };

            mcWalkLDelight = new SoundEffect[] { // Single footstep, boot, soft and gentle on concrete
                Content.Load<SoundEffect>("SFX/MC/mcWalkDelight1"),
                Content.Load<SoundEffect>("SFX/MC/mcWalkDelight2"),
                Content.Load<SoundEffect>("SFX/MC/mcWalkDelight3"),
                Content.Load<SoundEffect>("SFX/MC/mcWalkDelight4"),
                Content.Load<SoundEffect>("SFX/MC/mcWalkDelight5"),
                Content.Load<SoundEffect>("SFX/MC/mcWalkDelight6"),
                Content.Load<SoundEffect>("SFX/MC/mcWalkDelight7"),
                Content.Load<SoundEffect>("SFX/MC/mcWalkDelight8")
            };

            // --------------------------------------------------------------------------------
            // --------------------------------- Items ----------------------------------------
            itemGoldPickup = new SoundEffect[] { 
                Content.Load<SoundEffect>("SFX/Items/itemGoldPickup1"),
                Content.Load<SoundEffect>("SFX/Items/itemGoldPickup2"),
                Content.Load<SoundEffect>("SFX/Items/itemGoldPickup3"),
                Content.Load<SoundEffect>("SFX/Items/itemGoldPickup4"),
                Content.Load<SoundEffect>("SFX/Items/itemGoldPickup5")
            };
            itemLikenPickup = new SoundEffect[] {
                Content.Load<SoundEffect>("SFX/Items/itemLinkenPickup1")
            };
            itemLinkenOn = new SoundEffect[] {
                Content.Load<SoundEffect>("SFX/Items/itemLinkenOn1"),
                Content.Load<SoundEffect>("SFX/Items/itemLinkenOn2")
            };
            itemLinkenBreak = new SoundEffect[] {
                Content.Load<SoundEffect>("SFX/Items/itemLinkenBreak1"),
                Content.Load<SoundEffect>("SFX/Items/itemLinkenBreak2"),
                Content.Load<SoundEffect>("SFX/Items/itemLinkenBreak3"),
                Content.Load<SoundEffect>("SFX/Items/itemLinkenBreak4")
            };
            itemLikenRad = new SoundEffect[] {
                Content.Load<SoundEffect>("SFX/Items/ItemLikenRadiating1"),
                Content.Load<SoundEffect>("SFX/Items/ItemLikenRadiating2"),
                Content.Load<SoundEffect>("SFX/Items/ItemLikenRadiating3"),
                Content.Load<SoundEffect>("SFX/Items/ItemLikenRadiating4"),
                Content.Load<SoundEffect>("SFX/Items/ItemLikenRadiating5"),
                Content.Load<SoundEffect>("SFX/Items/ItemLikenRadiating6")
            };


            // --------------------------------------------------------------------------------
            // -------------------------------- Enemies ---------------------------------------
            enemyBBDie = new SoundEffect[] {    // All mono
                Content.Load<SoundEffect>("SFX/Enemies/enemyBBDie1"),
                Content.Load<SoundEffect>("SFX/Enemies/enemyBBDie2"),
                Content.Load<SoundEffect>("SFX/Enemies/enemyBBDie3"),
                Content.Load<SoundEffect>("SFX/Enemies/enemyBBDie4")
            };
            enemyBBMove = new SoundEffect[] {   // All mono
                Content.Load<SoundEffect>("SFX/Enemies/enemyBBMove1"),
                Content.Load<SoundEffect>("SFX/Enemies/enemyBBMove2"),
                Content.Load<SoundEffect>("SFX/Enemies/enemyBBMove3"),
                Content.Load<SoundEffect>("SFX/Enemies/enemyBBMove4"),
                Content.Load<SoundEffect>("SFX/Enemies/enemyBBMove5"),
                Content.Load<SoundEffect>("SFX/Enemies/enemyBBMove6")
            };

            // --------------------------------------------------------------------------------
            // ------------------------------- Level and env ----------------------------------
            envOpenMysDoor = new SoundEffect[] {
                Content.Load<SoundEffect>("SFX/Env/roomDoorOpen0"),
                Content.Load<SoundEffect>("SFX/Env/roomDoorOpen1"),
                Content.Load<SoundEffect>("SFX/Env/roomDoorOpen2"),
                Content.Load<SoundEffect>("SFX/Env/roomDoorOpen3")
            };

            // --------------------------------------------------------------------------------
            // ----------------------------------- UI -----------------------------------------
            bagLMBSlect = new SoundEffect[] {    
                Content.Load<SoundEffect>("SFX/UI/bagLMBClick1") };
            bagLMBRelease = new SoundEffect[] {    
                Content.Load<SoundEffect>("SFX/UI/bagLMBRelease1") };
            bagOnhover = new SoundEffect[] {
                Content.Load<SoundEffect>("SFX/UI/bagOnhover1") };

            settingSliderClick = new SoundEffect[] {
                Content.Load<SoundEffect>("SFX/UI/settingSliderClicking")};

            titleOnHover = new SoundEffect[] {
                Content.Load<SoundEffect>("SFX/UI/titleOnHover1")  };
            titleClick = new SoundEffect[] {
                Content.Load<SoundEffect>("SFX/UI/titleClick1") };

            // --------------------------------------------------------------------------------
            // ----------------------------------- BGM ----------------------------------------
            LV1BGM = new SoundEffect[] {
                Content.Load<SoundEffect>("BGM/lv1BGM1"),
                Content.Load<SoundEffect>("BGM/lv1BGM2"),
                Content.Load<SoundEffect>("BGM/lv1BGM3"),
                Content.Load<SoundEffect>("BGM/lv1BGM4"),
                Content.Load<SoundEffect>("BGM/lv1BGM5"),
                Content.Load<SoundEffect>("BGM/lv1BGM6"),
                Content.Load<SoundEffect>("BGM/lv1BGM7"),
                Content.Load<SoundEffect>("BGM/lv1BGM8"),
            };
        }

        /// <summary>
        /// Play method for SFX and SFX only. 
        /// </summary>
        /// <param name="SFX">SFX to play</param>
        /// <param name="Volume">Given volume, controled by whoever writes it</param>
        private void PlayInVolume(SoundEffect SFX, float Volume)
        {
            SFX.Play(Volume * masterVolume * SFXVolume, pitchDefault, panDefault);
        }

        /// <summary>
        /// Randomly pick an SFX from a given list. 
        /// </summary>
        /// <param name="List">List to pick</param>
        /// <returns>One of the SFX in the list</returns>
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
                SFX.Play(Volume * masterVolume, pitchDefault, Pan);
                SFX.Play(Volume * masterVolume, pitchDefault, PanSide);
            }
            else
            {
                SFX.Play(Volume * masterVolume, pitchDefault, Pan);
            }
            
        }

        // ================================================================================
        // ================================ Public methods ================================
        // ================================================================================

        public void UpdateBGM()
        {
            if (fisrtStartUp)
            {
                fisrtStartUp = false; 
                PlayInVolume(RandPick(LV1BGM), .25f);
                BGMSW.Restart();
            }
            else if(BGMSW.ElapsedMilliseconds > BGMInterval)
            {
                PlayInVolume(RandPick(LV1BGM), .25f);
                BGMSW.Restart();
            }
        }

        public void SetVolume(float[] Vols)
        {
            masterVolume = Vols[0];
            SFXVolume = Vols[1];
            musicVolume = Vols[2];
        }

        public void PlayMCAttack(MC Player)
        {
            switch (Player.primaryState)
            {
                case Globals.primaryTypes.Torch:
                    PlayInVolume(RandPick(mcAttackWithTorch), 1f);
                    break;
                default:
                    break;
            }
        }

        public void PlayMCTorchOn()
        {
            PlayInVolume(RandPick(mcWalkWithTorch), 1f);
        }

        public void PlayMCWalkLevelDelight()
        {
            PlayInVolume(RandPick(mcWalkLDelight), .15f);
        }

        public void PlayGoldPickupSFX()
        {
            PlayInVolume(RandPick(itemGoldPickup), 1f);
        }

        public void PlayitemPickup(int Index)
        {
            switch (Index)
            {
                case Globals.ITEM_LINKEN:
                    PlayInVolume(RandPick(itemLikenPickup), .25f);
                    break;
                case Globals.ITEM_NOTE_SO:
                    break;
                case Globals.ITEM_TORCH:
                    break;
                default:
                    break;
            }
        }

        public void PlayItemLikenOn()
        {
            PlayInVolume(RandPick(itemLinkenOn), .5f);
        }

        public void PlayItemLinkenBreak()
        {
            PlayInVolume(RandPick(itemLinkenBreak), .5f);
        }

        public void PlayitemLinkenRadiating()
        {
            PlayInVolume(RandPick(itemLikenRad), .025f);
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

        public void PlayBagLMBSelect()
        {
            PlayInVolume(RandPick(bagLMBSlect), 1f);
        }
        public void PlayBagLMBRelease()
        {
            PlayInVolume(RandPick(bagLMBRelease), 1f);
        }
        public void PlayBagItemOnhover()
        {
            PlayInVolume(RandPick(bagOnhover), 1f);
        }

        public void PlaySettingSliderClick()
        {
            PlayInVolume(RandPick(settingSliderClick), .5f);
        }

        public void PlayTitleOnHover()
        {
            PlayInVolume(RandPick(titleOnHover), .25f);
        }
        public void PlayTitleClick()
        {
            PlayInVolume(RandPick(titleClick), 1f);
        }

        public void PlayEnvOpenMysDoor(Globals.Direction Dir)
        {
            PlayInVolume(envOpenMysDoor[(int)Dir], 1f);
        }
    }
}
