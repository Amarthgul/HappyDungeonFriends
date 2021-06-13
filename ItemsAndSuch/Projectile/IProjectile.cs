using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


namespace HappyDungeon
{
    public interface IProjectile
    {
        void MarkAsMelee(Object Source);

        void Update();

        void Draw();

        bool Expired();

        bool IsMelee();

        Rectangle GetRectangle();

        Rectangle GetSrcRectangle();

        DamageInstance GetDamageInstance();
    }
}
