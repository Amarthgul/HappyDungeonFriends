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
                {Keys.Q, new UsePrimaryCommand(game)},
                {Keys.W, new Use1stSlotCommand(game)},
                {Keys.E, new Use2ndSlotCommand(game)},
                {Keys.R, new Use3rdSlotCommand(game)},
                {Keys.Up, new MoveUpCommand(game)},
                {Keys.Left, new MoveLeftCommand(game)},
                {Keys.Down, new MoveDownCommand(game)},
                {Keys.Right, new MoveRightCommand(game)},

                {Keys.A, new AttackCommand(game)},

                {Keys.Tab, new DisplayMapCommand(game)},
                {Keys.LeftAlt, new AltDisplayCommand(game)},
            };
        }


        public void Update()
        {
            switch (game.gameState)
            {
                case Globals.GameStates.Running:
                    Keys[] PressedKeys = Keyboard.GetState().GetPressedKeys();

                    foreach (Keys key in PressedKeys)
                    {
                        if (controllerMappings.ContainsKey(key))
                        {
                            controllerMappings[key].execute();
                        }
                    }
                    break;
                case Globals.GameStates.TitleScreen:
                    break;
                default:
                    break; 
            }
            

        }
    }
}
