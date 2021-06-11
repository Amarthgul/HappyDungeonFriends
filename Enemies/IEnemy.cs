using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace HappyDungeon
{

    public interface IEnemy
    {

        void Update(MC MainChara);

        void Draw();

        void Turn(Globals.Direction NewDir);

        void TakeDamage(DamageInstance Damage);

        bool CanAttack();

        void Attack();

        void SetAttackInterval(int Pattern);

        Rectangle GetRectangle();

        int GetIndex();

        bool IsDead();

        DamageInstance DealCollisionDamage(); 
    }
}
