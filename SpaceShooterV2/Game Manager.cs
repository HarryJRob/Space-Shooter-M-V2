using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceShooterV2
{
    class Game_Manager : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private MainGame CurGame;

        private enum GameState
        {
            MainMenu,
            PlayingSP,
            PlayingMP
        }

        private GameState curState;

        public Game_Manager()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            curState = GameState.PlayingSP;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //CurGame = new MainGame(false,ref graphics, Window, spriteBatch);
            //CurGame.Initialize();
            //CurGame.LoadContent(Content);
        }

        protected override void Update(GameTime gameTime)
        {
            if (curState == GameState.PlayingSP)
            {
                if (CurGame == null)
                {
                    CurGame = new MainGame(false,ref graphics,Window,spriteBatch);
                    CurGame.Initialize();
                    CurGame.LoadContent(Content);
                }
                CurGame.Update(gameTime);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (curState == GameState.PlayingSP || curState == GameState.PlayingMP)
                CurGame.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}
