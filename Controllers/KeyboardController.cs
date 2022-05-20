using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace HappyDungeon
{
    class KeyboardController : IController
    {
        private Game1 game { get; set; }

        private Dictionary<Keys, ICommand> controllerMappings;
        private Dictionary<Keys, ICommand> controllerMappingsDefault;
        private Dictionary<Keys, ICommand> controllerMappingsRPG;

        public KeyboardController(Game1 game)
        {
            this.game = game;
            controllerMappings = new Dictionary<Keys, ICommand>(); 

            controllerMappingsDefault = new Dictionary<Keys, ICommand>()
            {
                {Keys.Q, new UsePrimaryCommand(game)},
                {Keys.W, new Use1stSlotCommand(game)},
                {Keys.E, new Use2ndSlotCommand(game)},
                {Keys.R, new Use3rdSlotCommand(game)},
                {Keys.Up, new MoveUpCommand(game)},
                {Keys.Left, new MoveLeftCommand(game)},
                {Keys.Down, new MoveDownCommand(game)},
                {Keys.Right, new MoveRightCommand(game)},
                {Keys.Enter, new EnterConfirmCommand(game)},

                {Keys.A, new AttackCommand(game)},
                {Keys.B, new DisplayBagCommand(game)},

                {Keys.Escape, new EscCommand(game)},
                {Keys.Tab, new DisplayMapCommand(game)},
                {Keys.LeftAlt, new AltDisplayCommand(game)},
            };

            controllerMappingsRPG = new Dictionary<Keys, ICommand>()
            {
                {Keys.D1, new UsePrimaryCommand(game)},
                {Keys.D2, new Use1stSlotCommand(game)},
                {Keys.D3, new Use2ndSlotCommand(game)},
                {Keys.D4, new Use3rdSlotCommand(game)},
                {Keys.W, new MoveUpCommand(game)},
                {Keys.A, new MoveLeftCommand(game)},
                {Keys.S, new MoveDownCommand(game)},
                {Keys.D, new MoveRightCommand(game)},
                {Keys.Enter, new EnterConfirmCommand(game)},

                {Keys.Space, new AttackCommand(game)},
                {Keys.B, new DisplayBagCommand(game)},

                {Keys.Escape, new EscCommand(game)},
                {Keys.Tab, new DisplayMapCommand(game)},
                {Keys.LeftAlt, new AltDisplayCommand(game)},
            };

            switch (game.keyboardControl)
            {
                case Globals.KeyboardControl.RPG:
                    controllerMappings = controllerMappingsRPG;
                    break;
                case Globals.KeyboardControl.Tradition:
                    controllerMappings = controllerMappingsDefault;
                    break;
                default:
                    controllerMappings = controllerMappingsDefault;
                    break;
            }
                

        }


        public void Update()
        {
            // Overlay window uses it own keyboard input and is not presented here 
            if (game.overlay.IsEnabled()) return;

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
