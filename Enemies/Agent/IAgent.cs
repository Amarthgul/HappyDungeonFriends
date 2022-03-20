using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyDungeon
{
    /// <summary>
    /// Agent is the decision making part for the enemies, 
    /// the "brain", one can say. 
    /// </summary>
    public interface IAgent
    {

        void HandleBlockCollision(Globals.Direction FacingDir);

        void Update(MC MainChara);

    }
}
