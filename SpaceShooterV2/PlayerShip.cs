using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceShooterV2
{
    class PlayerShip : Ship
    {
        private struct controlScheme
        {
            public List<Keys> Keys;
            public List<bool> KeyState;
        }

        private controlScheme _controlScheme = new controlScheme();

        public PlayerShip(int width, int height, byte texNum, int xVelocity, int yVelocity, byte playerID,List<Keys> keyList) : base(width, height,texNum, xVelocity, yVelocity)
        {
            _health = 5;

            _controlScheme.Keys = new List<Keys>();
            _controlScheme.KeyState = new List<bool>();

            _controlScheme.Keys = keyList;

            foreach (Keys curKey in _controlScheme.Keys)
            {
                _controlScheme.KeyState.Add(false);
            }

            if (playerID == 1)
            {
                _position.Y = height;
            }
            else if (playerID == 2)
            {
                _position.Y = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - height;
            }
            else
            {
                _position.Y = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height/2 - height/2;
            }
            _position.X += 40;
            
        }

        public override void Update(GameTime gameTime, KeyboardState curKeyboardState)
        {
                for (int i = 0; i < _controlScheme.Keys.Count; i++)
                {
                    if (curKeyboardState.IsKeyDown(_controlScheme.Keys[i]))
                    {
                        _controlScheme.KeyState[i] = true;
                        Console.WriteLine(i);
                    }
                    else
                    {
                        _controlScheme.KeyState[i] = false;
                    }
                }
        }
    }
}
