using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyDungeon
{
    public interface IItem
    {

        void Collect();

        void UseItem();
    }
}
