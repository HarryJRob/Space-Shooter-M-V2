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
        private bool _stateChanged;
        private int _score = -1;
        private string[] _highscores = new string[9];
        private string _pathHighscores = "Content\\Highscores.txt";
        private int _highScoreEntryPos = -1;
        private KeyboardState _preKeyboardState;

        private enum MenuState
        {
            Main,
            GameOver,
            Options,
            PlayingSP,
            PlayingMP,
            HighscoreEntry,
        }

        private MenuState _curMenuState = MenuState.Main;

        public MainMenu(SpriteBatch spriteBatch, GameWindow window, SpriteFont font)
        {
            _spriteBatch = spriteBatch;
            _window = window;
            _font = font;
            _curMenuState = MenuState.Main;
        }

        public MainMenu(SpriteBatch spriteBatch, GameWindow window, SpriteFont font, int score)
        {
            _spriteBatch = spriteBatch;
            _window = window;
            _font = font;
            _stateChanged = true;
            _curMenuState = MenuState.GameOver;
            _score = score;
        }

        public void LoadContent(List<Texture2D> textureList)
        {
            _texList = textureList;

            AddButtons();
            LoadHighscores();
        }

        public void Update(GameTime gameTime)
        {
            MouseState _curMouseState = Mouse.GetState();
            KeyboardState _curKeyboardState = Keyboard.GetState();

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
                    
                    #region Highscore achieved check
                    if (_score != -1)
                    {
                        for (int i = 1; i < _highscores.Length; i += 2)
                        {
                            if (Convert.ToInt16(_highscores[i]) < _score)
                            {
                                _highscores[i - 1] = "Name Here";
                                _highscores[i] = Convert.ToString(_score);
                                _curMenuState = MenuState.HighscoreEntry;
                                _highScoreEntryPos = i - 1;
                                break;
                            }
                        }
                    }
                    #endregion

                    break;
                case MenuState.Options:
                    _menuButtons[4].IsActive = true;
                    break;
               case MenuState.HighscoreEntry:
                    _menuButtons[4].IsActive = true;
                    _menuButtons[4].Text = "Confirm Entry";
                    break;
            }

            #region HighScore Entry
            if (_curMenuState == MenuState.HighscoreEntry)
            {
                List<Keys> newKeysPressed = new List<Keys>();
                foreach (Keys key in _curKeyboardState.GetPressedKeys())
                {
                    if (!_preKeyboardState.GetPressedKeys().Contains(key))
                    {
                        newKeysPressed.Add(key);
                    }
                }

                if (newKeysPressed.Count != 0)
                {
                    if (_highscores[_highScoreEntryPos] == "Name Here")
                    {
                        _highscores[_highScoreEntryPos] = "";
                    }
                    AddKeysToHighscore(newKeysPressed);
                }
            }
            #endregion

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
                        Debug.WriteLine(" Main Menu - Button Index {0} is clicked", i);

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
                                if (_curMenuState != MenuState.HighscoreEntry)
                                    _curMenuState = MenuState.Main;
                                else
                                {
                                    _score = -1;
                                    if (_highscores[_highScoreEntryPos] == "Name Here" || _highscores[_highScoreEntryPos] == "")
                                    {
                                        _highscores[_highScoreEntryPos] = "Anon";
                                    }
                                    SaveHighscores();
                                    _curMenuState = MenuState.GameOver;
                                }
                                _stateChanged = true;
                                break;
                        }
                    }
                }

            }

            #endregion

            _preKeyboardState = _curKeyboardState;
        }

        public void Draw(GameTime gameTime)
        {
            #region Draw Relevant Buttons
            foreach (MenuButton curMenuButton in _menuButtons.Where(item => item.IsActive))
            {
                curMenuButton.Draw(_spriteBatch,_texList[curMenuButton.TexNum], _font);
            }
            #endregion

            #region Draw scoreboard on gameover 
            if (_curMenuState == MenuState.GameOver || _curMenuState == MenuState.HighscoreEntry)
            {
                int lineSpacing = _window.ClientBounds.Height/40;

                _spriteBatch.DrawString(_font, "Highscores",
                    new Vector2(_window.ClientBounds.Width/2 - _font.MeasureString("Highscores").X/2, lineSpacing),
                    Color.White); 

                for (int i = 0; i < _highscores.Length - 1; i+= 2)
                {
                    _spriteBatch.DrawString(_font, _highscores[i] + " : " + _highscores[i + 1], new Vector2(_window.ClientBounds.Width / 2 - _font.MeasureString(_highscores[i] + " : " + _highscores[i + 1]).X/2,i * lineSpacing + 3*lineSpacing), Color.White);
                }
            }
            #endregion
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
            _menuButtons[4].Text = "Continue";
        }

        private void AddButtons()
        {
            _menuButtons.Add(new MenuButton(_window.ClientBounds.Width / 3, _window.ClientBounds.Height / 5, 1, new Vector2(_window.ClientBounds.Width / 20, _window.ClientBounds.Height / 2 - _window.ClientBounds.Height / 15), "Single Player", true));
            _menuButtons.Add(new MenuButton(_window.ClientBounds.Width / 3, _window.ClientBounds.Height / 5, 1, new Vector2(_window.ClientBounds.Width - _window.ClientBounds.Width / 3 - _window.ClientBounds.Width / 20, _window.ClientBounds.Height / 2 - _window.ClientBounds.Height / 15), "Multiplayer Player", true));
            _menuButtons.Add(new MenuButton(_window.ClientBounds.Width / 3, _window.ClientBounds.Height / 5, 1, new Vector2(_window.ClientBounds.Width / 2 - _window.ClientBounds.Width / 6, _window.ClientBounds.Height - _window.ClientBounds.Height / 3), "Options", true));
            _menuButtons.Add(new MenuButton(_window.ClientBounds.Width / 3, _window.ClientBounds.Height / 10, 1, new Vector2(_window.ClientBounds.Width / 3, _window.ClientBounds.Height / 8), "Space Shooter", false));
            _menuButtons.Add(new MenuButton(_window.ClientBounds.Width / 5, _window.ClientBounds.Height / 8, 1, new Vector2(_window.ClientBounds.Width / 2 - _window.ClientBounds.Width / 10, _window.ClientBounds.Height / 1.2f), "Continue", true));
        }

        private void AddKeysToHighscore(List<Keys> newKeysPressed)
        {
            foreach (Keys key in newKeysPressed)
            {
                if (key == Keys.Back && _highscores[_highScoreEntryPos].Length != 0)
                {
                    _highscores[_highScoreEntryPos] =
                        _highscores[_highScoreEntryPos].Remove(_highscores[_highScoreEntryPos].Length - 1);
                }
                else if (_highscores[_highScoreEntryPos].Length <= 5 && (_highscores[_highScoreEntryPos] + key.ToString()).Length <= 5)
                {
                    _highscores[_highScoreEntryPos] += key.ToString();
                }
            } 
        }

        private void LoadHighscores()
        {
            _pathHighscores = AppDomain.CurrentDomain.BaseDirectory + _pathHighscores;

            try
            {
                if (File.Exists(_pathHighscores))
                {
                    using (StreamReader sr = new StreamReader(_pathHighscores))
                    {
                        string line = sr.ReadToEnd();
                        Debug.WriteLine("Main Menu - Loaded: " + line);
                        _highscores = line.Split(',');
                    }
                }
                else
                {
                    File.Create(_pathHighscores);
                }
            }
            catch (IOException exception)
            {
                Debug.WriteLine(" Main Menu - Error Loading: " + exception.Message);
            }
        }

        private void SaveHighscores()
        {
            try
            {
                if (File.Exists(_pathHighscores))
                {
                    using (StreamWriter sw = new StreamWriter(_pathHighscores))
                    {
                        for (int i = 0; i < _highscores.Length; i++)
                        {
                            if (i < _highscores.Length - 1)
                                sw.Write(_highscores[i] + ",");
                            else
                                sw.Write(_highscores[i]);

                            Debug.WriteLine(" Main Menu - Saving: " + _highscores[i]);
                        }
                    }
                }

            }
            catch (IOException exception)
            {
                Debug.WriteLine(" Main Menu - Error Saving: " + exception.Message);
            }
        }
    }
}
