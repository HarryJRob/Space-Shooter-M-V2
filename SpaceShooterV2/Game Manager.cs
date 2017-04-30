using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

/*Comments are objectives
 * e.g. obj: 1.9
 * Show the logic of complex algorithms - brief explanation
 */

namespace SpaceShooterV2
{
    internal class GameManager : Game
    {
        //Variables
        //Required Resources
        private SpriteBatch _spriteBatch;
        private readonly List<Texture2D> _mainTexList = new List<Texture2D>();
        private SpriteFont _font;
        private MainGame _curGame;
        private MainMenu _curMenu;

        //Passed between menu and game
        private int _scoreFromGame = -1;
        private string _settings;
        private int _difficulty;

        //State of the program
        private enum GameState
        {
            MainMenu,
            PlayingSP,
            PlayingMP,
            Dead
        }

        private GameState _curState;

        //Public Procedures
        public GameManager()
        {
            var graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            _curState = GameState.MainMenu;

            #region FullScreen
            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            Window.AllowUserResizing = false;
            //graphics.ToggleFullScreen();
            #endregion
        }

        //Protected Procedure
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            #region Load Textures

            try
            {

                _font = Content.Load<SpriteFont>("Game Resources/KenVector");
                
                //0 - 13 = MainGame tex (Will need to be greater and background does not need to be passed but would break some of the existing code if removed)
                _mainTexList.Add(Content.Load<Texture2D>("Game Resources/CollisionArea"));
                _mainTexList.Add(Content.Load<Texture2D>("Game Resources/Backgrounds/BackGround"));
                _mainTexList.Add(Content.Load<Texture2D>("Game Resources/Ships/PlayerShip"));
                _mainTexList.Add(Content.Load<Texture2D>("Game Resources/Bullets/LongBullet"));
                _mainTexList.Add(Content.Load<Texture2D>("Game Resources/Bullets/RoundBullet"));
                _mainTexList.Add(Content.Load<Texture2D>("Game Resources/GameUI/HealthBarPiece"));
                _mainTexList.Add(Content.Load<Texture2D>("Game Resources/PowerUps/PowerUpHeal"));
                _mainTexList.Add(Content.Load<Texture2D>("Game Resources/PowerUps/PowerUpDamage"));
                _mainTexList.Add(Content.Load<Texture2D>("Game Resources/Bullets/BulletDamageBoost"));
                _mainTexList.Add(Content.Load<Texture2D>("Game Resources/Ships/Charger"));
                _mainTexList.Add(Content.Load<Texture2D>("Game Resources/Ships/Shotgun"));
                _mainTexList.Add(Content.Load<Texture2D>("Game Resources/Ships/Bomber"));
                _mainTexList.Add(Content.Load<Texture2D>("Game Resources/GameUI/CoolDownBarPiece"));
                _mainTexList.Add(Content.Load<Texture2D>("Game Resources/Ships/PlayerShip2"));

                //14-15 = MainMenu tex
                _mainTexList.Add(Content.Load<Texture2D>("Game Resources/Menu/MenuButton"));

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                throw;
            }

            #endregion
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape) && (_curState == GameState.PlayingSP || _curState == GameState.PlayingMP))
            {
                _curState = GameState.Dead; 
            }


            if (_curState == GameState.PlayingSP || _curState == GameState.PlayingMP)
                {
                    #region Game Logic

                    if (IsMouseVisible)
                    {
                        IsMouseVisible = false;
                    }
                    if (_curMenu != null)
                    {
                        _curMenu = null;
                    }

                    if (_curGame == null && _curState == GameState.PlayingSP)
                    {
                        _curGame = new MainGame(false, Window, _spriteBatch, _settings, _difficulty);
                        _curGame.Initialize();
                        _curGame.LoadContent(_mainTexList.GetRange(0,14),_font);
                    }
                    else if (_curGame == null && _curState == GameState.PlayingMP)
                    {
                        _curGame = new MainGame(true, Window, _spriteBatch, _settings, _difficulty);
                        _curGame.Initialize();
                        _curGame.LoadContent(_mainTexList.GetRange(0, 14), _font);
                    }
                    else if (_curGame != null)
                    {
                            _curGame.Update(gameTime);
                            if (_curGame.IsDead)
                            {
                                _scoreFromGame = _curGame.TotalScore;
                                _curGame = null;
                                _curState = GameState.Dead;
                            }
                    }

                    #endregion
                }
                else if (_curState == GameState.Dead || _curState == GameState.MainMenu)
                {
                    #region Menu Logic

                    if (IsMouseVisible == false)
                    {
                        IsMouseVisible = true;
                    }
                    if (_curGame != null)
                    {
                        _curGame = null;
                    }

                    if (_curMenu == null && _curState == GameState.Dead)
                    {
                        _curState = GameState.MainMenu;
                        _curMenu = new MainMenu(_spriteBatch, Window, _font,_scoreFromGame);
                        _curMenu.LoadContent(_mainTexList.GetRange(14, 1));
                    }
                    else if (_curMenu == null && _curState != GameState.Dead)
                    {
                        _curMenu = new MainMenu(_spriteBatch,Window,_font);
                        _curMenu.LoadContent(_mainTexList.GetRange(14, 1));
                    }
                    else if (_curMenu != null)
                    {
                        _curMenu.Update(gameTime);
                        if (_curMenu.WillPlay() == 1)
                        {
                            _settings = _curMenu.GetSettings();
                            _difficulty = _curMenu.GetDiffculty();
                            _curState = GameState.PlayingSP;
                        }
                        else if (_curMenu.WillPlay() == 2)
                        {
                            _settings = _curMenu.GetSettings();
                            _difficulty = _curMenu.GetDiffculty();
                            _curState = GameState.PlayingMP;
                        }
                    }

                    #endregion
                }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();
            _spriteBatch.Draw(_mainTexList[1], new Rectangle(0, 0, Window.ClientBounds.Width, Window.ClientBounds.Height), Color.White);

                if ((_curState == GameState.PlayingSP || _curState == GameState.PlayingMP) && _curGame != null)
                    _curGame.Draw(gameTime);
                else if (_curState == GameState.MainMenu && _curMenu != null)
                    _curMenu.Draw(gameTime);

            base.Draw(gameTime);
            _spriteBatch.End();
        }
    }
}
