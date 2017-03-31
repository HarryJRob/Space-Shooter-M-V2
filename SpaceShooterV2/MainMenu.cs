﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceShooterV2
{
    class MainMenu
    {
        private SpriteBatch _spriteBatch;
        private GameWindow _window;

        private List<MenuButton> _menuButtons = new List<MenuButton>();
        private List<Texture2D> _texList = new List<Texture2D>();

        private MouseState _curMouseState;

        private enum MenuState
        {
            Main,
            GameOver,
            Options,
            LevelSelect,
        }

        private MenuState _curMenuState;

        private const int MenuScale = 2;

        public MainMenu(bool cameFromGame, SpriteBatch spriteBatch, GameWindow window)
        {
            _spriteBatch = spriteBatch;
            _window = window;

            #region Adding Buttons
            // _menuButtons.Add(new MenuButton(,,, new Vector2(,)));
            _menuButtons.Add(new MenuButton((int)(_window.ClientBounds.Width / 2.5f), _window.ClientBounds.Height / 4, 1, new Vector2(_window.ClientBounds.Width / 20, _window.ClientBounds.Height / 2 - _window.ClientBounds.Height / 7)));
            _menuButtons.Add(new MenuButton((int)(_window.ClientBounds.Width / 2.5f), _window.ClientBounds.Height / 4, 1, new Vector2(_window.ClientBounds.Width - _window.ClientBounds.Width / 2.5f - _window.ClientBounds.Width / 20, _window.ClientBounds.Height / 2 - _window.ClientBounds.Height / 7)));
            _menuButtons[0].IsActive = true;
            _menuButtons[1].IsActive = true;

            #endregion

            #region MenuState Logic

            switch (cameFromGame)
            {
                case false: 
                    _curMenuState = MenuState.Main;
                    break;
                case true:
                    _curMenuState = MenuState.GameOver;
                    break;
            }

            #endregion
        }

        //Should load stuff once not every time the class is created
        public void LoadContent(List<Texture2D> textureList)
        {
            _texList = textureList;
        }

        public void Update(GameTime gameTime)
        {
            _curMouseState = Mouse.GetState();

            #region State Logic

            switch (_curMenuState)
            {
                case MenuState.Main:
                    break;
                case MenuState.GameOver:
                    break;
                case MenuState.LevelSelect:
                    break;
                case MenuState.Options:
                    break;
            }
            #endregion

            #region Button Logic

            for (int i = 0; i < _menuButtons.Count; i++)
            {
                if (_menuButtons[i].IsActive)
                {
                    _menuButtons[i].Update(_curMouseState);
                }

                if (_menuButtons[i].IsClicked)
                {
                    //One for each button
                    Debug.WriteLine("Index {0} is clicked",i);
                    _menuButtons[i].IsClicked = false;
                    switch (i)
                    {
                        case 0:
                            break;
                        case 1:
                            break;
                        case 2:
                            break;
                        case 3:
                            break;
                    }
                }
            }

            #endregion
        }

        public void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();
            foreach (MenuButton curMenuButton in _menuButtons.Where(item => item.IsActive))
            {
                curMenuButton.Draw(_spriteBatch,_texList[curMenuButton.TexNum]);
            }
            _spriteBatch.End();
        }
    }
}
