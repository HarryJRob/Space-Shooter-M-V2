using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
        private bool _stateChanged;
        private int _score = -1;
        private string[] _highscores = new string[9];
        private string _pathHighscores = "Content\\Highscores.txt";

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

        public MainMenu(SpriteBatch spriteBatch, GameWindow window, SpriteFont font)
        {
            _spriteBatch = spriteBatch;
            _window = window;
            _font = font;
            _curMenuState = MenuState.Main;
        }

        public MainMenu(SpriteBatch spriteBatch, GameWindow window, SpriteFont font, int Score)
        {
            _spriteBatch = spriteBatch;
            _window = window;
            _font = font;
            _stateChanged = true;
            _curMenuState = MenuState.GameOver;
            _score = Score;
        }

        private void AddButtons()
        {
            _menuButtons.Add(new MenuButton(_window.ClientBounds.Width / 3, _window.ClientBounds.Height / 5, 1, new Vector2(_window.ClientBounds.Width / 20, _window.ClientBounds.Height / 2 - _window.ClientBounds.Height / 15), "Single Player", true));
            _menuButtons.Add(new MenuButton(_window.ClientBounds.Width / 3, _window.ClientBounds.Height / 5, 1, new Vector2(_window.ClientBounds.Width - _window.ClientBounds.Width / 3 - _window.ClientBounds.Width / 20, _window.ClientBounds.Height / 2 - _window.ClientBounds.Height / 15), "Multiplayer Player", true));
            _menuButtons.Add(new MenuButton(_window.ClientBounds.Width / 3, _window.ClientBounds.Height / 5, 1, new Vector2(_window.ClientBounds.Width / 2 - _window.ClientBounds.Width / 6, _window.ClientBounds.Height - _window.ClientBounds.Height / 3), "Options", true));
            _menuButtons.Add(new MenuButton(_window.ClientBounds.Width / 3, _window.ClientBounds.Height / 10, 1, new Vector2(_window.ClientBounds.Width/3, _window.ClientBounds.Height/8), "Space Shooter", false));
            _menuButtons.Add(new MenuButton(_window.ClientBounds.Width / 5, _window.ClientBounds.Height / 8, 1, new Vector2(_window.ClientBounds.Width / 2 - _window.ClientBounds.Width / 10, _window.ClientBounds.Height / 1.2f), "Continue", true));
           }

        public void LoadContent(List<Texture2D> textureList)
        {
            _texList = textureList;

            AddButtons();
            //Cant be hardwired
            _pathHighscores = AppDomain.CurrentDomain.BaseDirectory + _pathHighscores;
            _pathHighscores = @"C:\Users\Harry\Source\Repos\Space-Shooter-M-V2\SpaceShooterV2\Content\Highscores.txt";
            #region Load Highscores
            try
            {
                if (File.Exists(_pathHighscores))
                {
                    using (StreamReader sr = new StreamReader(_pathHighscores))
                    {
                        string line = sr.ReadToEnd();
                        Debug.WriteLine("Main Menu - " + line);
                    }
                }
                else
                {
                    File.Create(_pathHighscores);
                }
            }
            catch (IOException exception)
            {
                Debug.WriteLine("Main Menu - " + exception.Message);
            }
            #endregion
        }

        public void Update(GameTime gameTime)
        {
            _curMouseState = Mouse.GetState();

            #region State Logic

            if (_stateChanged)
            {
                _stateChanged = false;
                DeactivateAll();
            }

            switch (_curMenuState)
            {
                case MenuState.Main:
                    _menuButtons[0].IsActive = true;
                    _menuButtons[1].IsActive = true;
                    _menuButtons[2].IsActive = true;
                    _menuButtons[3].IsActive = true;
                    break;
                case MenuState.GameOver:
                    _menuButtons[4].IsActive = true;
                    break;
                case MenuState.LevelSelect:
                    break;
                case MenuState.Options:
                    _menuButtons[4].IsActive = true;
                    break;
            }
            #endregion

            #region Button Logic

            for (int i = 0; i < _menuButtons.Count; i++)
            {
                if (_menuButtons[i].IsActive && _menuButtons[i].IsClickable)
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
                                _stateChanged = true;
                                break;
                            case 1:
                                _curMenuState = MenuState.PlayingMP;
                                _stateChanged = true;
                                break;
                            case 2:
                                _curMenuState = MenuState.Options;
                                _stateChanged = true;
                                break;
                            case 4:
                                _curMenuState = MenuState.Main;
                                _stateChanged = true;
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
            foreach (MenuButton curButton in _menuButtons.Where(item => item.IsActive))
            {
                curButton.IsActive = false;
            }
        }
    }
}
