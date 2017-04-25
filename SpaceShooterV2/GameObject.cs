using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceShooterV2
{
    internal abstract class GameObject
    {
        protected Vector2 _position;
        protected int _width, _height;
        protected byte _texNum;
        protected bool _collision;
        protected int _xVelocity;
        protected int _yVelocity;

        protected GameObject(int width, int height, byte texNum, int xVelocity, int yVelocity)
        {
            _width = width*height;
            _height = height;
            _texNum = texNum;
            _xVelocity = xVelocity;
            _yVelocity = yVelocity;
        }

        public virtual void Draw(SpriteBatch spriteBatch, Texture2D tex)
        {
            spriteBatch.Draw(tex, new Rectangle((int) _position.X, (int) _position.Y, _width, _height), Color.White);
        }

        public virtual void Update(GameTime gameTime)
        {
            _position.X += _xVelocity;
            _position.Y += _yVelocity;
        }

        public Rectangle BoundingBox
        {
            get { return new Rectangle((int) _position.X, (int) _position.Y, _width, _height); }
        }

        public bool Collision
        {
            set { _collision = value; }
            get { return _collision; }
        }

        public int TexNum
        {
            get { return _texNum; }
        }

        public Vector2 getCenterPoint
        {
            get { return new Vector2(_position.X + _width/2, _position.Y + _height/2); }
        }
    }
}
