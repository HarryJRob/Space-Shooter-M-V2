using System;
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
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private MainGame CurGame;
        private MainMenu CurMenu;

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
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            _curState = GameState.PlayingSP;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
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

                    if (CurGame == null && _curState == GameState.PlayingSP)
                    {
                        CurGame = new MainGame(false, ref graphics, Window, spriteBatch);
                        CurGame.Initialize();
                        CurGame.LoadContent(Content);
                    }
                    else if (CurGame == null && _curState == GameState.PlayingMP)
                    {
                        CurGame = new MainGame(true, ref graphics, Window, spriteBatch);
                        CurGame.Initialize();
                        CurGame.LoadContent(Content);
                    }
                    else if (CurGame != null)
                    {
                            CurGame.Update(gameTime);
                            if (CurGame.IsDead)
                            {
                                CurGame = null;
                                _curState = GameState.Dead;
                            }
                    }

                    #endregion
                }
                else if (_curState == GameState.Dead || _curState == GameState.MainMenu)
                {
                    #region Menu Logic

                    if (CurMenu == null && _curState == GameState.Dead)
                    {
                        CurMenu = new MainMenu(true);
                        CurMenu.Initialize();
                        CurMenu.LoadContent(Content);
                    }
                    else if (CurMenu == null && _curState != GameState.Dead)
                    {
                        CurMenu = new MainMenu(true);
                        CurMenu.Initialize();
                        CurMenu.LoadContent(Content);
                    }
                    else if (CurMenu != null)
                    {
                        CurMenu.Update(gameTime);
                    }

                    #endregion
                }
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (!_transitioning)
            {
                GraphicsDevice.Clear(Color.SkyBlue);

                if ((_curState == GameState.PlayingSP || _curState == GameState.PlayingMP) && CurGame != null)
                    CurGame.Draw(gameTime);
                else if (_curState == GameState.MainMenu && CurMenu != null)
                    CurMenu.Draw(gameTime);
            }
            base.Draw(gameTime);
        }
    }
}
