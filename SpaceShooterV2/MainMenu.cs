using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace SpaceShooterV2
{
    class MainMenu
    {
        private enum MenuState
        {
            Main,
            GameOver,
            Options,
            LevelSelect,
        }

        private MenuState curMenuState;

        public MainMenu(bool cameFromGame)
        {
            #region MenuState Logic

            switch (cameFromGame)
            {
                case false: 
                    curMenuState = MenuState.Main;
                    break;
                case true:
                    curMenuState = MenuState.GameOver;
                    break;
            }

            #endregion
        }

        public void Initialize() { }

        public void LoadContent(ContentManager content) { }

        public void Update(GameTime gameTime) { }

        public void Draw(GameTime gameTime) { }
    }
}
