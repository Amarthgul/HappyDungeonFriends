using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace HappyDungeon
{
    class KeyboardController : IController
    {
        private Game1 game { get; set; }
        private Dictionary<Keys, ICommand> controllerMappings;

        public KeyboardController(Game1 game)
        {
            this.game = game;

            //Add all default controls
            controllerMappings = new Dictionary<Keys, ICommand>()
            {
                //PlayerMovement
                {Keys.W, new MoveUpCommand(game)},
                {Keys.A, new MoveLeftCommand(game)},
                {Keys.S, new MoveDownCommand(game)},
                {Keys.D, new MoveRightCommand(game)},
                {Keys.Up, new MoveUpCommand(game)},
                {Keys.Left, new MoveLeftCommand(game)},
                {Keys.Down, new MoveDownCommand(game)},
                {Keys.Right, new MoveRightCommand(game)},

                {Keys.Tab, new DisplayMapCommand(game)},
            };
        }


        public void Update()
        {
            Keys[] PressedKeys = Keyboard.GetState().GetPressedKeys();

            foreach (Keys key in PressedKeys)
            {
                if (controllerMappings.ContainsKey(key))
                {
                    controllerMappings[key].execute();
                }
            }

        }
    }
}
