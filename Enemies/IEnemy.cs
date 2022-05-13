using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace HappyDungeon
{

    /// <summary>
    /// IEnemy represents the "outside" of an enemy class, 
    /// i.e. how they exchange infomation with other characters. 
    /// The inside is handled by "IAgent" class and its derivatives.
    /// </summary> 
    public interface IEnemy
    {

        void Update(MC MainChara);

        void Draw();

        void SpeedChange(int NewSpeed); 

        void Turn(Globals.Direction NewDir);

        void TakeDamage(DamageInstance Damage);

        bool CanAttack();

        void Attack();

        void SetAttackInterval(int Pattern);

        Rectangle GetRectangle();

        Vector2 GetPosition();

        int GetIndex();

        bool IsDead();

        DamageInstance DealCollisionDamage();

        int GetKillScore();
    }
}
