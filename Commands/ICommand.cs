using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyDungeon
{
    /// <summary>
    /// Command interface. 
    /// Classes based on this interface has only 1 method: execute.
    /// Other features are achived by commands not from ICommand. 
    /// </summary>
    public interface ICommand
    {
        void execute();
    }
}
