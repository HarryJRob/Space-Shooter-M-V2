using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceShooterV2
{
    class Ship
    {
        protected Vector2 _position;
        protected int _width, _height;
        protected byte _texNum;
        protected bool _collision;
        protected int _xVelocity;
        protected int _yVelocity;

        public Ship() { } //No use but to allow inheritance

        public Ship(int Width, int Height, byte TexNum, int xVelocity, int yVelocity)
        {
            _width = Width;
            _height = Height;
            _texNum = TexNum;
            _xVelocity = xVelocity;
            _yVelocity = yVelocity;
        }

        public void Draw(SpriteBatch _spriteBatch, Texture2D _tex)
        {
            _spriteBatch.Draw(_tex,new Rectangle((int)_position.X,(int)_position.Y,_width,_height), Color.White);
        }

        public virtual void Update(GameTime _gameTime)
        {
            _position.X += _xVelocity * _gameTime.ElapsedGameTime.Ticks;
        }

        public Rectangle BoundingBox
        {
            get { return new Rectangle((int)_position.X,(int)_position.Y,_width,_height);}
        }

        public bool Collision
        {
            set { _collision = value; }
        }
    }
}
