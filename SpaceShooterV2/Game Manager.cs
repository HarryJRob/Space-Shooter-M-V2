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
            if (curState == GameState.PlayingSP || curState == GameState.PlayingMP)
            {
                #region Game Logic
                if (CurGame == null && curState == GameState.PlayingSP)
                {
                    CurGame = new MainGame(false,ref graphics,Window,spriteBatch);
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
                            //Change State
                        }
                    }
                }
                #endregion
            }
            else if (curState == GameState.MainMenu)
            {
                
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if ((curState == GameState.PlayingSP || curState == GameState.PlayingMP) && CurGame != null)
                CurGame.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}
