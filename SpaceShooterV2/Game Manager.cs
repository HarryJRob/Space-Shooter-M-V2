using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

/*Comments are objectives
 * e.g. obj: 1.9
 * Show the logic of complex algorithms - brief explanation
 */

namespace SpaceShooterV2
{
    class GameManager : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private List<Texture2D> _mainTexList = new List<Texture2D>();
        private SpriteFont _font;

        private MainGame _curGame;
        private MainMenu _curMenu;

        private bool _transitioning = false; //Can be used for animation later
        private int _transitionFrameCount = 0;

        private enum GameState
        {
            MainMenu,
            PlayingSP,
            PlayingMP,
            Dead
        }

        private GameState _curState;

        public GameManager()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            _curState = GameState.MainMenu;

            #region FullScreen
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            Window.AllowUserResizing = false;
            _graphics.ToggleFullScreen();
            #endregion
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            #region Load Game Textures

                _font = Content.Load<SpriteFont>("Game Resources/Arial");

                //0 - 5 = MainGame tex (Will need to be greater and background does not need to be passed but would break some of the existing code if removed)
                _mainTexList.Add(Content.Load<Texture2D>("Game Resources/CollisionArea"));
                _mainTexList.Add(Content.Load<Texture2D>("Game Resources/Backgrounds/BackGround"));
                _mainTexList.Add(Content.Load<Texture2D>("Game Resources/Ships/ship"));
                _mainTexList.Add(Content.Load<Texture2D>("Game Resources/Bullets/LongBullet"));
                _mainTexList.Add(Content.Load<Texture2D>("Game Resources/Bullets/RoundBullet"));
                _mainTexList.Add(Content.Load<Texture2D>("Game Resources/Ships/HealthBarPiece"));
            #endregion
        }

        protected override void Update(GameTime gameTime)
        {
            if ((GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed) ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Environment.Exit(-1);

            if (!_transitioning)
            {
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
                        _curGame = new MainGame(false, ref _graphics, Window, _spriteBatch);
                        _curGame.Initialize();
                        _curGame.LoadContent(_mainTexList.GetRange(0,6),_font);
                    }
                    else if (_curGame == null && _curState == GameState.PlayingMP)
                    {
                        _curGame = new MainGame(true, ref _graphics, Window, _spriteBatch);
                        _curGame.Initialize();
                        _curGame.LoadContent(_mainTexList.GetRange(0, 6), _font);
                    }
                    else if (_curGame != null)
                    {
                            _curGame.Update(gameTime);
                            if (_curGame.IsDead)
                            {
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
                        _curMenu = new MainMenu(true, _spriteBatch, Window);
                        _curMenu.LoadContent(_mainTexList.GetRange(0, 6));
                    }
                    else if (_curMenu == null && _curState != GameState.Dead)
                    {
                        _curMenu = new MainMenu(false,_spriteBatch,Window);
                        _curMenu.LoadContent(_mainTexList.GetRange(0, 6));
                    }
                    else if (_curMenu != null)
                    {
                        _curMenu.Update(gameTime);
                        if (_curMenu.WillPlay() == 1)
                        {
                            _curState = GameState.PlayingSP;
                        }
                        else if (_curMenu.WillPlay() == 2)
                        {
                            _curState = GameState.PlayingMP;
                        }
                    }

                    #endregion
                }
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();
            _spriteBatch.Draw(_mainTexList[1], new Rectangle(0, 0, Window.ClientBounds.Width, Window.ClientBounds.Height), Color.White);
            //_spriteBatch.DrawString(_font, "Test 123", new Vector2(Window.ClientBounds.Width/2 - _font.MeasureString("Test 123").X/2, 0), Color.Black);

            if (!_transitioning)
            {
                GraphicsDevice.Clear(Color.SkyBlue);

                if ((_curState == GameState.PlayingSP || _curState == GameState.PlayingMP) && _curGame != null)
                    _curGame.Draw(gameTime);
                else if (_curState == GameState.MainMenu && _curMenu != null)
                    _curMenu.Draw(gameTime);
            }
            base.Draw(gameTime);
            _spriteBatch.End();
        }
    }
}
