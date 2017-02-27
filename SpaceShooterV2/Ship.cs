using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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
        protected int _health;

        public Ship() { } //No use but to allow inheritance

        public Ship(int Width, int Height, byte TexNum, int xVelocity, int yVelocity)
        {
            _width = Width;
            _height = Height;
            _texNum = TexNum;
            _xVelocity = xVelocity;
            _yVelocity = yVelocity;
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D tex)
        {
            spriteBatch.Draw(tex,new Rectangle((int)_position.X,(int)_position.Y,_width,_height), Color.White);
        }

        public virtual void Update(GameTime gameTime)
        {
            _position.X += _xVelocity * gameTime.ElapsedGameTime.Ticks;
        }

        public virtual void Update(GameTime gameTime, KeyboardState curKeyState) { }

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
