using System.Collections.Generic;
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

        private SpriteFont _font;
        private MouseState _curMouseState;
        private bool StateChanged;

        private enum MenuState
        {
            Main,
            GameOver,
            Options,
            LevelSelect,
            PlayingSP,
            PlayingMP,
        }

        private MenuState _curMenuState = MenuState.Main;

        public MainMenu(bool cameFromGame, SpriteBatch spriteBatch, GameWindow window, SpriteFont font)
        {
            _spriteBatch = spriteBatch;
            _window = window;
            _font = font;

            #region Adding Buttons
            // _menuButtons.Add(new MenuButton(,,, new Vector2(,)));
            _menuButtons.Add(new MenuButton(_window.ClientBounds.Width / 3, _window.ClientBounds.Height / 5, 1, new Vector2(_window.ClientBounds.Width / 20, _window.ClientBounds.Height / 2 - _window.ClientBounds.Height / 15),"Single Player"));
            _menuButtons.Add(new MenuButton(_window.ClientBounds.Width / 3, _window.ClientBounds.Height / 5, 1, new Vector2(_window.ClientBounds.Width - _window.ClientBounds.Width / 3 - _window.ClientBounds.Width / 20, _window.ClientBounds.Height / 2 - _window.ClientBounds.Height / 15), "Multiplayer Player"));
            _menuButtons.Add(new MenuButton(_window.ClientBounds.Width / 3, _window.ClientBounds.Height / 5, 1, new Vector2(_window.ClientBounds.Width / 2 - _window.ClientBounds.Width / 6, _window.ClientBounds.Height - _window.ClientBounds.Height / 3), "Options"));
            _menuButtons.Add(new MenuButton(_window.ClientBounds.Width / 5, _window.ClientBounds.Height / 8, 1, new Vector2(_window.ClientBounds.Width / 2 - _window.ClientBounds.Width / 10, _window.ClientBounds.Height / 1.2f), "Continue"));
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

            //Temporarya
            #region State Logic

            if (StateChanged)
            {
                StateChanged = false;
                DeactivateAll();
            }

            switch (_curMenuState)
            {
                case MenuState.Main:
                    _menuButtons[0].IsActive = true;
                    _menuButtons[1].IsActive = true;
                    _menuButtons[2].IsActive = true;
                    break;
                case MenuState.GameOver:
                    _menuButtons[3].IsActive = true;
                    break;
                case MenuState.LevelSelect:
                    break;
                case MenuState.Options:
                    _menuButtons[3].IsActive = true;
                    break;
            }
            #endregion

            #region Button Logic

            for (int i = 0; i < _menuButtons.Count; i++)
            {
                if (_menuButtons[i].IsActive)
                {
                    _menuButtons[i].Update(_curMouseState);

                    if (_menuButtons[i].IsClicked)
                    {
                        //One for each button
                        _menuButtons[i].IsClicked = false;
                        Debug.WriteLine(" Main Menu - Index {0} is clicked", i);

                        switch (i)
                        {
                            case 0:
                                _curMenuState = MenuState.PlayingSP;
                                StateChanged = true;
                                break;
                            case 1:
                                _curMenuState = MenuState.PlayingMP;
                                StateChanged = true;
                                break;
                            case 2:
                                _curMenuState = MenuState.Options;
                                StateChanged = true;
                                break;
                            case 3:
                                _curMenuState = MenuState.Main;
                                StateChanged = true;
                                break;
                        }
                    }
                }

            }

            #endregion
        }

        public void Draw(GameTime gameTime)
        {
            foreach (MenuButton curMenuButton in _menuButtons.Where(item => item.IsActive))
            {
                curMenuButton.Draw(_spriteBatch,_texList[curMenuButton.TexNum], _font);
            }
        }

        public int WillPlay()
        {
            if (_curMenuState == MenuState.PlayingSP)
            {
                return 1;
            }  
            if (_curMenuState == MenuState.PlayingMP)
            {
                return 2;
            }
            return 0;
        }

        private void DeactivateAll()
        {
            foreach (MenuButton curButton in _menuButtons)
            {
                curButton.IsActive = false;
            }
        }
    }
}
