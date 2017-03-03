using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceShooterV2
{
    class GameObject
    {
        protected Vector2 _position;
        protected int _width, _height;
        protected byte _texNum;
        protected bool _collision;
        protected int _xVelocity;
        protected int _yVelocity;

        public GameObject() { }

        public GameObject(int Width, int Height, byte TexNum, int xVelocity, int yVelocity)
        {
            _width = Width * Height;
            _height = Height;
            _texNum = TexNum;
            _xVelocity = xVelocity;
            _yVelocity = yVelocity;
        }

        public virtual void Draw(SpriteBatch spriteBatch, Texture2D tex)
        {
            spriteBatch.Draw(tex, new Rectangle((int)_position.X, (int)_position.Y, _width, _height), Color.White);
        }

        public virtual void Update(GameTime gameTime)
        {
            _position.X += _xVelocity;
            _position.Y += _yVelocity;
        }

        public Rectangle BoundingBox
        {
            get { return new Rectangle((int)_position.X, (int)_position.Y, _width, _height); }
        }

        public bool Collision
        {
            set { _collision = value; }
            get { return _collision;}
        }

        public int TexNum
        {
            get { return _texNum;}
        }
    }
}
