using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
            PlayingMP
        }

        private GameState curState;

        public GameManager()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            curState = GameState.PlayingSP;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            if (!_transitioning)
            {
                if (curState == GameState.PlayingSP || curState == GameState.PlayingMP)
                {
                    #region Game Logic

                    if (CurGame == null && curState == GameState.PlayingSP)
                    {
                        CurGame = new MainGame(false, ref graphics, Window, spriteBatch);
                        CurGame.Initialize();
                        CurGame.LoadContent(Content);
                    }
                    else if (CurGame == null && curState == GameState.PlayingMP)
                    {
                        CurGame = new MainGame(true, ref graphics, Window, spriteBatch);
                        CurGame.Initialize();
                        CurGame.LoadContent(Content);
                    }
                    else
                    {
                        if (CurGame != null)
                        {
                            CurGame.Update(gameTime);
                            if (CurGame.IsDead)
                            {
                                CurGame = null;
                                curState = GameState.MainMenu;
                                //Change State
                            }
                        }
                    }

                    #endregion
                }
                else if (curState == GameState.MainMenu)
                {
                    if (CurMenu == null)
                    {
                        CurMenu = new MainMenu();
                        CurMenu.Initialize();
                        CurMenu.LoadContent(Content);
                    }
                }
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (!_transitioning)
            {
                GraphicsDevice.Clear(Color.SkyBlue);

                if ((curState == GameState.PlayingSP || curState == GameState.PlayingMP) && CurGame != null)
                    CurGame.Draw(gameTime);
                else if (curState == GameState.MainMenu && CurMenu != null)
                    CurMenu.Draw(gameTime);
            }
            base.Draw(gameTime);
        }
    }
}
