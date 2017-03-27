using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

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

        private MenuState _curMenuState;
        private List<MenuButton> _menuButtons = new List<MenuButton>();
        private MouseState _curMouseState;

        public MainMenu(bool cameFromGame)
        {
            #region MenuState Logic

            switch (cameFromGame)
            {
                case false: 
                    _curMenuState = MenuState.Main;
                    break;
                case true:
                    _curMenuState = MenuState.GameOver;
                    break;
            }

            #endregion
        }

        public void Initialize() { }
        //Should load stuff once not every time the class is created
        public void LoadContent(ContentManager content) { }

        public void Update(GameTime gameTime)
        {
            _curMouseState = Mouse.GetState();
            foreach (MenuButton curMenuButton in _menuButtons.Where(item => item.IsActive))
            {
                curMenuButton.Update(_curMouseState);
            }
        }

        public void Draw(GameTime gameTime) { }
    }
}
