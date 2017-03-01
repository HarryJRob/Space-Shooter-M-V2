﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceShooterV2
{
    class PlayerShip : Ship
    {
        private struct ControlScheme
        {
            public List<Keys> controls;
            public List<bool> keyStates;
        }

        private int WindowX;
        private int WindowY;

        private KeyboardState _previousKeyBoardState;
        private ControlScheme _controlScheme;

        public PlayerShip(int width, int height, byte texNum, int xVelocity, int yVelocity, byte playerID,string keyStr,int winX, int winY) : base(width, height,texNum, xVelocity, yVelocity)
        {
            _health = 5;
            WindowX = winX;
            WindowY = winY;

            #region Calculating Control Scheme

            Console.WriteLine("ID: {0}, Control Scheme: {1}", playerID,keyStr);

            _controlScheme.controls = new List<Keys>();
            _controlScheme.keyStates = new List<bool>();

            string[] _keys = keyStr.Split(',');

            foreach (string curStr in _keys)
            {
                _controlScheme.controls.Add(GetKeyCode(curStr));
                _controlScheme.keyStates.Add(false);
            }

            #endregion

            #region Calculating Starting Position
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
            #endregion
        }

        private Keys GetKeyCode(string curStr)
        {
            curStr = curStr.ToUpper();
            switch(curStr)
            {
                case "Q":
                    return Keys.Q;
                case "W":
                    return Keys.W;
                case "E":
                    return Keys.E;
                case "R":
                    return Keys.R;
                case "T":
                    return Keys.T;
                case "Y":
                    return Keys.Y;
                case "U":
                    return Keys.U;
                case "I":
                    return Keys.I;
                case "O":
                    return Keys.O;
                case "P":
                    return Keys.P;
                case "A":
                    return Keys.A;
                case "S":
                    return Keys.S;
                case "D":
                    return Keys.D;
                case "F":
                    return Keys.F;
                case "G":
                    return Keys.G;
                case "H":
                    return Keys.H;
                case "J":
                    return Keys.J;
                case "K":
                    return Keys.K;
                case "L":
                    return Keys.L;
                case "Z":
                    return Keys.Z;
                case "X":
                    return Keys.X;
                case "C":
                    return Keys.C;
                case "V":
                    return Keys.V;
                case "B":
                    return Keys.B;
                case "N":
                    return Keys.N;
                case "M":
                    return Keys.M;
                case "SPACE":
                    return Keys.Space;
                case "UP":
                    return Keys.Up;
                case "LEFT":
                    return Keys.Left;
                case "DOWN":
                    return Keys.Down;
                case "RIGHT":
                    return Keys.Right;
                case "ENTER":
                    return Keys.Enter;
                default:
                    return Keys.D0;
            }

        }

        public override void Update(GameTime gameTime, KeyboardState curKeyboardState)
        {
            #region Check Controls
            if (curKeyboardState.GetPressedKeys() != _previousKeyBoardState.GetPressedKeys())
            {
                for (int i = 0; i < _controlScheme.controls.Count; i++)
                {
                    if (curKeyboardState.IsKeyDown(_controlScheme.controls[i]))
                    {
                        _controlScheme.keyStates[i] = true;
                    }
                    else
                    {
                        _controlScheme.keyStates[i] = false;
                    }
                }
                _previousKeyBoardState = curKeyboardState;
            }
            #endregion

            #region Action Based on Control State 

            for (int i = 0; i < _controlScheme.keyStates.Count; i++)
            {
                if (_controlScheme.keyStates[i])
                {
                    switch (i)
                    {
                        case 0:
                            if (_position.Y - _yVelocity < 0)
                            {
                                _position.Y = 0;
                            }
                            else
                            {
                                _position.Y -= _yVelocity;
                            }
                            break;
                        case 1:
                            if (_position.X - _xVelocity < 0)
                            {
                                _position.X = 0;
                            }
                            else
                            {
                                _position.X -= _xVelocity;  
                            }
                            break;
                        case 2:
                            if (_position.Y - _height > WindowY)
                            {
                                _position.Y = WindowY + _height;
                            }
                            else
                            {
                                _position.Y += _xVelocity;
                            }
                            break;
                        case 3:
                            if (_position.X + _width > WindowX)
                            {
                                _position.X = WindowX - _width;
                            }
                            else
                            {
                                _position.X += _xVelocity;
                            }
                            break;
                    }
                }
            }

            #endregion
        }

    }
}
