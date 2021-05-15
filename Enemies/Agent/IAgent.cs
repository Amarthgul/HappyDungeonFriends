using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyDungeon
{
    /// <summary>
    /// Agent is the decision making part for the enemies. 
    /// The "brain", you can say. 
    /// </summary>
    public interface IAgent
    {

        Globals.Direction HandleBlockCollision(Globals.Direction FacingDir);

        void Update(MC MainChara);

    }
}
