using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyDungeon.Commands
{
    class MoveCommand
    {

        /// <summary>
        /// Should there be any change in the implementation of move
        /// then I'd have to change it 4 times for each direction? 
        /// Not really possible for my lazy ass. 
        /// </summary>
        /// <param name="game">Game1</param>
        /// <param name="Direction">Direction to move</param>
        public void Move(Game1 game, Globals.Direction Direction)
        {
            if(game.mainChara.canTurn[(int)Direction])
                game.mainChara.ChangeDirection(Direction);

            if (!game.mainChara.moveRestricted)
                game.mainChara.Move();
        }
    }
}
