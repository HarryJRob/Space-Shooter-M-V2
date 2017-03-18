using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceShooterV2
{
    class MenuButton
    {
        private readonly int _width;
        private readonly int _height;
        private readonly int _texNum;
        private readonly Vector2 _position;
        private bool _clicked;

        public MenuButton(int width, int height, int texNum, Vector2 position)
        {
            _width = width;
            _height = height;
            _texNum = texNum;
            _position = position;
        }

        public void Update(MouseState curMouseState)
        {
            if (curMouseState.LeftButton == ButtonState.Pressed)
            {
                if (curMouseState.X >= _position.X && curMouseState.X < _position.X + _width && curMouseState.Y > _position.Y && curMouseState.Y < _position.Y + _height)
                {
                    _clicked = true;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D tex)
        {
            spriteBatch.Draw(tex,new Rectangle((int)_position.X,(int)_position.X,_width,_height),Color.White);
        }

        public bool IsClicked
        {
            get { return _clicked; }
            set { _clicked = value; }
        }

        public int TexNum
        {
            get { return _texNum; }
        }
    }
}
