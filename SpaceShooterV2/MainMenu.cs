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
    internal class MainMenu
    {
        //Declarations
        //Required resources
        private readonly SpriteBatch _spriteBatch;
        private readonly GameWindow _window;
        private readonly List<MenuButton> _menuButtons = new List<MenuButton>();
        private List<Texture2D> _texList = new List<Texture2D>();
        private readonly SpriteFont _font;
        private KeyboardState _preKeyboardState;
        private bool _stateChanged;
        private readonly float _fontScale = 1920f;

        //Scoring
        private int _score = -1;
        private string[] _highscores = new string[9];
        private string _pathHighscores = "Content\\Highscores.txt";
        private int _highScoreEntryPos = -1;

        //Settings
        private string _pathSettings = "Content\\Settings.txt";
        private string[] _settingPlayer1 = new string[4];
        private string[] _settingPlayer2 = new string[4];
        private int _settingEntryPos = -1;

        //Difficulty
        private int _difficulty = 2;

        //State of the menu
        private enum MenuState
        {
            Main,
            Highscore,
            Options,
            PlayingSP,
            PlayingMP,
            HighscoreEntry,
            SettingEntry,
            DifficultyEntry
        }

        private MenuState _curMenuState;

        //Methods
        //Public Methods
        public MainMenu(SpriteBatch spriteBatch, GameWindow window, SpriteFont font)
        {
            _spriteBatch = spriteBatch;
            _window = window;
            _font = font;
            _curMenuState = MenuState.Main;
            _fontScale = (float)(_window.ClientBounds.Width)/_fontScale;
        }

        public MainMenu(SpriteBatch spriteBatch, GameWindow window, SpriteFont font, int score = -1)
        {
            _spriteBatch = spriteBatch;
            _window = window;
            _font = font;
            _stateChanged = true;
            _curMenuState = MenuState.Highscore;
            _score = score;
            _fontScale = (float)(_window.ClientBounds.Width) / _fontScale;
        }

        public void LoadContent(List<Texture2D> textureList)
        {
            _texList = textureList;

            _pathHighscores = AppDomain.CurrentDomain.BaseDirectory + _pathHighscores;
            _pathSettings = AppDomain.CurrentDomain.BaseDirectory + _pathSettings;
            
            AddButtons();
            LoadHighscores();
            LoadSettings();
        }

        public void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape) && _curMenuState != MenuState.Highscore)
            {
                Environment.Exit(-1);
            }

            MouseState curMouseState = Mouse.GetState();
            KeyboardState curKeyboardState = Keyboard.GetState();

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
                    _menuButtons[5].IsActive = true;
                    _menuButtons[4].IsActive = true;
                    _menuButtons[4].Text = "Highscores";
                    break;
                case MenuState.Highscore:
                    _menuButtons[4].IsActive = true;

                    #region Highscore achieved check

                    //Find highscore position
                    if (_score != -1)
                    {
                        for (int i = 1; i < _highscores.Length; i += 2)
                        {
                            if (Convert.ToInt16(_highscores[i]) < _score)
                            {
                                _highScoreEntryPos = i;
                                ShiftHighscores();
                                _highscores[_highScoreEntryPos - 1] = "Name Here";
                                _highscores[_highScoreEntryPos] = Convert.ToString(_score);
                                _curMenuState = MenuState.HighscoreEntry;
                                _highScoreEntryPos = _highScoreEntryPos - 1;
                                _score = -1;
                                break;
                            }
                        }
                    }

            #endregion

                    break;
                case MenuState.Options:
                    _menuButtons[4].IsActive = true;
                    _menuButtons[4].Text = "Continue";
                    _menuButtons[6].IsActive = true;
                    _menuButtons[6].Text = _settingPlayer1[0];
                    _menuButtons[7].IsActive = true;
                    _menuButtons[7].Text = _settingPlayer1[1];
                    _menuButtons[8].IsActive = true;
                    _menuButtons[8].Text = _settingPlayer1[2];
                    _menuButtons[9].IsActive = true;
                    _menuButtons[9].Text = _settingPlayer1[3];
                    _menuButtons[10].IsActive = true;
                    _menuButtons[10].Text = _settingPlayer1[4];
                    _menuButtons[11].IsActive = true;
                    _menuButtons[11].Text = _settingPlayer2[0];
                    _menuButtons[12].IsActive = true;
                    _menuButtons[12].Text = _settingPlayer2[1];
                    _menuButtons[13].IsActive = true;
                    _menuButtons[13].Text = _settingPlayer2[2];
                    _menuButtons[14].IsActive = true;
                    _menuButtons[14].Text = _settingPlayer2[3];
                    _menuButtons[15].IsActive = true;
                    _menuButtons[15].Text = _settingPlayer2[4];
                    break;
               case MenuState.HighscoreEntry:
                    _menuButtons[4].IsActive = true;
                    _menuButtons[4].Text = "Confirm";
                    
                    #region HighScore Entry

                        {
                            List<Keys> newKeysPressed = new List<Keys>();
                            foreach (Keys key in curKeyboardState.GetPressedKeys())
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

                    break;
                case MenuState.SettingEntry:
                    _menuButtons[4].IsActive = true;
                    _menuButtons[4].Text = "Confirm";
                    _menuButtons[6].IsActive = true;
                    _menuButtons[6].Text = _settingPlayer1[0];
                    _menuButtons[7].IsActive = true;
                    _menuButtons[7].Text = _settingPlayer1[1];
                    _menuButtons[8].IsActive = true;
                    _menuButtons[8].Text = _settingPlayer1[2];
                    _menuButtons[9].IsActive = true;
                    _menuButtons[9].Text = _settingPlayer1[3];
                    _menuButtons[10].IsActive = true;
                    _menuButtons[10].Text = _settingPlayer1[4];
                    _menuButtons[11].IsActive = true;
                    _menuButtons[11].Text = _settingPlayer2[0];
                    _menuButtons[12].IsActive = true;
                    _menuButtons[12].Text = _settingPlayer2[1];
                    _menuButtons[13].IsActive = true;
                    _menuButtons[13].Text = _settingPlayer2[2];
                    _menuButtons[14].IsActive = true;
                    _menuButtons[14].Text = _settingPlayer2[3];
                    _menuButtons[15].IsActive = true;
                    _menuButtons[15].Text = _settingPlayer2[4];

                    #region Setting Entry

                        {
                            List<Keys> newKeysPressed = new List<Keys>();
                            foreach (Keys key in curKeyboardState.GetPressedKeys())
                            {
                                if (!_preKeyboardState.GetPressedKeys().Contains(key))
                                {
                                    newKeysPressed.Add(key);
                                }

                                if (newKeysPressed.Count != 0)
                                {
                                    AddKeysToSettings(newKeysPressed);
                                }
                            }
                       }

                    #endregion

                    break;
                case MenuState.DifficultyEntry:
                    _menuButtons[16].IsActive = true;
                    _menuButtons[17].IsActive = true;
                    _menuButtons[18].IsActive = true;
                    _menuButtons[4].IsActive = true;
                    _menuButtons[4].Text = "Continue";
                    break;
            }

            #endregion

            #region Button Logic

            for (int i = 0; i < _menuButtons.Count; i++)
            {
                if (_menuButtons[i].IsActive && _menuButtons[i].IsClickable)
                {
                    _menuButtons[i].Update(curMouseState);

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
                                if (_curMenuState == MenuState.HighscoreEntry)
                                {
                                    _score = -1;
                                    if (_highscores[_highScoreEntryPos] == "Name Here" ||
                                        _highscores[_highScoreEntryPos] == "")
                                    {
                                        _highscores[_highScoreEntryPos] = "Anon";
                                    }
                                    SaveHighscores();
                                    _curMenuState = MenuState.Highscore;
                                }
                                else if (_curMenuState == MenuState.SettingEntry)
                                {
                                    SaveSettings();
                                    _curMenuState = MenuState.Options;                                   
                                }
                                else if (_curMenuState == MenuState.Main)
                                {
                                    _curMenuState = MenuState.Highscore;
                                }
                                else
                                {
                                    _curMenuState = MenuState.Main;
                                }
                                _stateChanged = true;
                                break;
                            case 5:
                                _curMenuState = MenuState.DifficultyEntry;
                                _stateChanged = true;
                                break;
                            case 6:
                                _settingEntryPos = 0;
                                _curMenuState = MenuState.SettingEntry;
                                _stateChanged = true;
                                break;
                            case 7:
                                _settingEntryPos = 1;
                                _curMenuState = MenuState.SettingEntry;
                                _stateChanged = true;
                                break;
                            case 8:
                                _settingEntryPos = 2;
                                _curMenuState = MenuState.SettingEntry;
                                _stateChanged = true;
                                break;
                            case 9:
                                _settingEntryPos = 3;
                                _curMenuState = MenuState.SettingEntry;
                                _stateChanged = true;
                                break;
                            case 10:
                                _settingEntryPos = 4;
                                _curMenuState = MenuState.SettingEntry;
                                _stateChanged = true;
                                break;
                            case 11:
                                _settingEntryPos = 5;
                                _curMenuState = MenuState.SettingEntry;
                                _stateChanged = true;
                                break;
                            case 12:
                                _settingEntryPos = 6;
                                _curMenuState = MenuState.SettingEntry;
                                _stateChanged = true;
                                break;
                            case 13:
                                _settingEntryPos = 7;
                                _curMenuState = MenuState.SettingEntry;
                                _stateChanged = true;
                                break;
                            case 14:
                                _settingEntryPos = 8;
                                _curMenuState = MenuState.SettingEntry;
                                _stateChanged = true;
                                break;
                            case 15:
                                _settingEntryPos = 9;
                                _curMenuState = MenuState.SettingEntry;
                                _stateChanged = true;
                                break;
                            case 16:
                                _difficulty = 1;
                                break;
                            case 17:
                                _difficulty = 2;
                                break;
                            case 18:
                                _difficulty = 3;
                                break;
                        }
                    }
                }

            }

            #endregion

            _preKeyboardState = curKeyboardState;
        }

        public void Draw(GameTime gameTime)
        {
            #region Draw Relevant Buttons
            foreach (MenuButton curMenuButton in _menuButtons.Where(item => item.IsActive))
            {
                curMenuButton.Draw(_spriteBatch,_texList[curMenuButton.TexNum], _font,_fontScale);
            }
            #endregion

            #region Draw Scoreboard On Gameover 
            if (_curMenuState == MenuState.Highscore || _curMenuState == MenuState.HighscoreEntry)
            {
                int lineSpacing = _window.ClientBounds.Height/40;

                _spriteBatch.DrawString(_font, "Highscores",
                    new Vector2(_window.ClientBounds.Width/2 - _font.MeasureString("Highscores").X/2, lineSpacing),
                    Color.White, 0f, new Vector2(0, 0), new Vector2(_fontScale, 1f), SpriteEffects.None, 0f); 

                for (int i = 0; i < _highscores.Length - 1; i+= 2)
                {
                    _spriteBatch.DrawString(_font, _highscores[i] + " : " + _highscores[i + 1], new Vector2(_window.ClientBounds.Width / 2 - _font.MeasureString(_highscores[i] + " : " + _highscores[i + 1]).X / 2, i * lineSpacing + 3 * lineSpacing), Color.White, 0f, new Vector2(0, 0), new Vector2(_fontScale, 1f), SpriteEffects.None, 0f);
                }
            }
            #endregion

            #region Draw Options info 
            if (_curMenuState == MenuState.Options || _curMenuState == MenuState.SettingEntry)
            {
                int lineSpacing = _window.ClientBounds.Height/8;

                _spriteBatch.DrawString(_font, "Up", new Vector2(_window.ClientBounds.Width / 2 - _font.MeasureString("Up").X / 2, _window.ClientBounds.Height / 1.5f - 4.2f * lineSpacing + _font.MeasureString("Up").Y / 2), Color.White, 0f, new Vector2(0, 0), new Vector2(_fontScale, 1f), SpriteEffects.None, 0f);
                _spriteBatch.DrawString(_font, "Left", new Vector2(_window.ClientBounds.Width / 2 - _font.MeasureString("Left").X / 2, _window.ClientBounds.Height / 1.5f - 3.1f * lineSpacing + _font.MeasureString("Left").Y / 2), Color.White, 0f, new Vector2(0, 0), new Vector2(_fontScale, 1f), SpriteEffects.None, 0f);
                _spriteBatch.DrawString(_font, "Down", new Vector2(_window.ClientBounds.Width / 2 - _font.MeasureString("Down").X / 2, _window.ClientBounds.Height / 1.5f - 2 * lineSpacing + _font.MeasureString("Down").Y / 2), Color.White, 0f, new Vector2(0, 0), new Vector2(_fontScale, 1f), SpriteEffects.None, 0f);
                _spriteBatch.DrawString(_font, "Right", new Vector2(_window.ClientBounds.Width / 2 - _font.MeasureString("Right").X / 2, _window.ClientBounds.Height / 1.5f - lineSpacing + _font.MeasureString("Right").Y / 2), Color.White, 0f, new Vector2(0, 0), new Vector2(_fontScale, 1f), SpriteEffects.None, 0f);
                _spriteBatch.DrawString(_font, "Shoot", new Vector2(_window.ClientBounds.Width / 2 - _font.MeasureString("Shoot").X / 2, _window.ClientBounds.Height / 1.5f + lineSpacing / 2 - _font.MeasureString("Shoot").Y / 2), Color.White, 0f, new Vector2(0, 0), new Vector2(_fontScale, 1f), SpriteEffects.None, 0f);
            }
            #endregion

            #region Draw Difficulty

            if (_curMenuState == MenuState.DifficultyEntry)
            {
                switch (_difficulty)
                {
                    case 1:
                        _spriteBatch.DrawString(_font, "Difficulty: Easy", new Vector2(_window.ClientBounds.Width / 2 - _font.MeasureString("Difficulty: Easy").X / 2, _window.ClientBounds.Height / 1.5f), Color.Ivory, 0f, new Vector2(0, 0), new Vector2(_fontScale, 1f), SpriteEffects.None, 0f);
                        break;
                    case 2:
                        _spriteBatch.DrawString(_font, "Difficulty: Medium", new Vector2(_window.ClientBounds.Width / 2 - _font.MeasureString("Difficulty: Medium").X / 2, _window.ClientBounds.Height / 1.5f), Color.Ivory, 0f, new Vector2(0, 0), new Vector2(_fontScale, 1f), SpriteEffects.None, 0f);
                        break;
                    case 3:
                        _spriteBatch.DrawString(_font, "Difficulty: Hard", new Vector2(_window.ClientBounds.Width / 2 - _font.MeasureString("Difficulty: Hard").X / 2, _window.ClientBounds.Height / 1.5f), Color.Ivory,0f,new Vector2(0,0),new Vector2(_fontScale,1f),SpriteEffects.None,0f);
                        break;
                }
            }

            #endregion
        }

        //Public Accessors
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

        public int GetDiffculty()
        {
            return _difficulty;
        }
        
        //Public Functions
        public string GetSettings()
        {
            string settingsString = "";

            for (int i = 0; i < _settingPlayer1.Length; i++)
            {
                if (i < _settingPlayer1.Length - 1)
                {
                    settingsString += _settingPlayer1[i];
                    settingsString += ",";
                }
                else
                    settingsString += _settingPlayer1[i];
            }

            settingsString += ":";

            for (int i = 0; i < _settingPlayer2.Length; i++)
            {
                if (i < _settingPlayer2.Length - 1)
                {
                    settingsString += _settingPlayer2[i];
                    settingsString += ",";
                }
                else
                    settingsString += _settingPlayer2[i];
            }

            return settingsString;
        }

        //Private Methods
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
            //Main Menu
            _menuButtons.Add(new MenuButton((double)_texList[0].Width / (double)_texList[0].Height, _window.ClientBounds.Height / 9, 0, new Vector2(_window.ClientBounds.Width / 4, _window.ClientBounds.Height / 2), "Single Player", true));
            _menuButtons.Add(new MenuButton((double)_texList[0].Width / (double)_texList[0].Height, _window.ClientBounds.Height / 9, 0, new Vector2(3*_window.ClientBounds.Width/4, _window.ClientBounds.Height / 2), "Multiplayer Player", true));
            _menuButtons.Add(new MenuButton((double)_texList[0].Width / (double)_texList[0].Height, _window.ClientBounds.Height / 12, 0, new Vector2(_window.ClientBounds.Width / 4, 3*_window.ClientBounds.Height/4), "Options", true));
            _menuButtons.Add(new MenuButton((double)_texList[0].Width / (double)_texList[0].Height, _window.ClientBounds.Height / 10, 0, new Vector2(_window.ClientBounds.Width / 2, _window.ClientBounds.Height / 8), "Space Shooter", false));
            _menuButtons.Add(new MenuButton((double)_texList[0].Width / (double)_texList[0].Height, _window.ClientBounds.Height / 14, 0, new Vector2(_window.ClientBounds.Width / 2, 7*_window.ClientBounds.Height /8), "Continue", true));
            _menuButtons.Add(new MenuButton((double)_texList[0].Width / (double)_texList[0].Height, _window.ClientBounds.Height / 12, 0, new Vector2(3*_window.ClientBounds.Width/4, 3*_window.ClientBounds.Height/4), "Difficulty", true));
            //Options
            //Player1
            _menuButtons.Add(new MenuButton((double)_texList[0].Width / (double)_texList[0].Height, _window.ClientBounds.Height / 16, 0, new Vector2(_window.ClientBounds.Width / 3, _window.ClientBounds.Height / 1.5f - 4.4f * (_window.ClientBounds.Height / 8)), "1", true));
            _menuButtons.Add(new MenuButton((double)_texList[0].Width / (double)_texList[0].Height, _window.ClientBounds.Height / 16, 0, new Vector2(_window.ClientBounds.Width / 3, _window.ClientBounds.Height / 1.5f - 3.3f * (_window.ClientBounds.Height / 8)), "2", true));
            _menuButtons.Add(new MenuButton((double)_texList[0].Width / (double)_texList[0].Height, _window.ClientBounds.Height / 16, 0, new Vector2(_window.ClientBounds.Width / 3, _window.ClientBounds.Height / 1.5f - 2.2f * (_window.ClientBounds.Height / 8)), "3", true));
            _menuButtons.Add(new MenuButton((double)_texList[0].Width / (double)_texList[0].Height, _window.ClientBounds.Height / 16, 0, new Vector2(_window.ClientBounds.Width / 3, _window.ClientBounds.Height / 1.5f - 1.1f * (_window.ClientBounds.Height / 8)), "4", true));
            _menuButtons.Add(new MenuButton((double)_texList[0].Width / (double)_texList[0].Height, _window.ClientBounds.Height / 16, 0, new Vector2(_window.ClientBounds.Width / 3, _window.ClientBounds.Height / 1.5f), "5", true));
            //Player2
            _menuButtons.Add(new MenuButton((double)_texList[0].Width / (double)_texList[0].Height, _window.ClientBounds.Height / 16, 0, new Vector2(2*_window.ClientBounds.Width / 3, _window.ClientBounds.Height / 1.5f - 4.4f * (_window.ClientBounds.Height / 8)), "6", true));
            _menuButtons.Add(new MenuButton((double)_texList[0].Width / (double)_texList[0].Height, _window.ClientBounds.Height / 16, 0, new Vector2(2*_window.ClientBounds.Width / 3, _window.ClientBounds.Height / 1.5f - 3.3f * (_window.ClientBounds.Height / 8)), "7", true));
            _menuButtons.Add(new MenuButton((double)_texList[0].Width / (double)_texList[0].Height, _window.ClientBounds.Height / 16, 0, new Vector2(2*_window.ClientBounds.Width / 3, _window.ClientBounds.Height / 1.5f - 2.2f * (_window.ClientBounds.Height / 8)), "8", true));
            _menuButtons.Add(new MenuButton((double)_texList[0].Width / (double)_texList[0].Height, _window.ClientBounds.Height / 16, 0, new Vector2(2*_window.ClientBounds.Width / 3, _window.ClientBounds.Height / 1.5f - 1.1f * (_window.ClientBounds.Height / 8)), "9", true));
            _menuButtons.Add(new MenuButton((double)_texList[0].Width / (double)_texList[0].Height, _window.ClientBounds.Height / 16, 0, new Vector2(2*_window.ClientBounds.Width / 3, _window.ClientBounds.Height / 1.5f), "10", true));
            //Difficulty Entry
            _menuButtons.Add(new MenuButton((double)_texList[0].Width / (double)_texList[0].Height, _window.ClientBounds.Height / 16, 0, new Vector2(_window.ClientBounds.Width / 4, _window.ClientBounds.Height / 2), "Easy", true));
            _menuButtons.Add(new MenuButton((double)_texList[0].Width / (double)_texList[0].Height, _window.ClientBounds.Height / 16, 0, new Vector2(_window.ClientBounds.Width / 2, _window.ClientBounds.Height / 2), "Medium", true));
            _menuButtons.Add(new MenuButton((double)_texList[0].Width / (double)_texList[0].Height, _window.ClientBounds.Height / 16, 0, new Vector2(3*_window.ClientBounds.Width / 4, _window.ClientBounds.Height / 2), "Hard", true));
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

        private void AddKeysToSettings(List<Keys> newKeysPressed)
        {
            if (_settingEntryPos > 4)
            {
                if (!_settingPlayer2.Contains(newKeysPressed[0].ToString()))
                _settingPlayer2[_settingEntryPos - 5] = newKeysPressed[0].ToString();
            }
            else
            {
                if (!_settingPlayer1.Contains(newKeysPressed[0].ToString()))
                _settingPlayer1[_settingEntryPos] = newKeysPressed[0].ToString();
            }
        }

        private void LoadHighscores()
        {
            try
            {
                if (File.Exists(_pathHighscores))
                {
                    using (StreamReader sr = new StreamReader(_pathHighscores))
                    {
                        string line = sr.ReadToEnd();
                        Debug.WriteLine(" Main Menu - Loaded: " + line);
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
                throw;
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
                        Debug.Write(" Main Menu - Saving: ");
                        for (int i = 0; i < _highscores.Length; i++)
                        {
                            if (i < _highscores.Length - 1)
                                sw.Write(_highscores[i] + ",");
                            else
                                sw.Write(_highscores[i]);

                            Debug.Write(_highscores[i] + ", ");
                        }
                    }
                    Debug.WriteLine("");
                }

            }
            catch (IOException exception)
            {
                Debug.WriteLine(" Main Menu - Error Saving: " + exception.Message);
                throw;
            }
        }

        private void LoadSettings()
        {
            //Load Format = Up,Left,Down,Right,Fire
            try
            {
                if (File.Exists(_pathSettings))
                {
                    using (StreamReader sr = new StreamReader(_pathSettings))
                    {
                        string Player1 = sr.ReadLine();
                        string Player2 = sr.ReadLine();
                        _settingPlayer1 = Player1.Split(',');
                        _settingPlayer2 = Player2.Split(',');

                        Debug.WriteLine(" Main Menu - Loaded: " + Player1);
                        Debug.WriteLine(" Main Menu - Loaded: " + Player2);
                    }
                }
            }
            catch (IOException exception)
            {
                Debug.WriteLine(" Main Menu - Error Loading: " + exception.Message);
                throw;
            }
        }

        private void SaveSettings()
        {
            try
            {
                if (File.Exists(_pathSettings))
                {
                    using (StreamWriter sw = new StreamWriter(_pathSettings))
                    {
                        Debug.Write(" Main Menu - Saving: ");
                        for (int i = 0; i < _settingPlayer1.Length; i++)
                        {
                            if (i < _settingPlayer1.Length - 1)
                                sw.Write(_settingPlayer1[i] + ",");
                            else
                                sw.Write(_settingPlayer1[i]);

                            Debug.Write(_settingPlayer1[i]);
                        }
                        sw.WriteLine("");
                        Debug.WriteLine("");
                        Debug.Write(" Main Menu - Saving: ");
                        for (int i = 0; i < _settingPlayer2.Length; i++)
                        {
                            if (i < _settingPlayer2.Length - 1)
                                sw.Write(_settingPlayer2[i] + ",");
                            else
                                sw.Write(_settingPlayer2[i]);

                            Debug.Write(_settingPlayer2[i]);
                        }
                        Debug.WriteLine("");
                    }
                }
            }
            catch (IOException exception)
            {
                Debug.WriteLine(" Main Menu - Error Saving: " + exception.Message);
                throw;
            }  
        }

        private void ShiftHighscores()
        {
            for (int i = 7; i > _highScoreEntryPos; i -= 1)
            {
                _highscores[i + 1] = _highscores[i - 1];
                _highscores[i + 2] = _highscores[i];
            }
        }
    }
}