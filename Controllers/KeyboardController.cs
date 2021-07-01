﻿using System;
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

            controllerMappings = new Dictionary<Keys, ICommand>()
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
