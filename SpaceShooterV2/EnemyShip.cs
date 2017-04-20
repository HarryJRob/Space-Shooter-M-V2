using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace SpaceShooterV2
{
    internal abstract class EnemyShip : Ship
    {
        protected int _score;
        protected int _currentCoolDown;
        protected bool _willFire;
        protected int _bulVel;
        protected bool initialising = true;
        protected int _maxX;
        protected int _maxY;

        protected EnemyShip(int width, int height, byte texNum, int xVelocity, int yVelocity, int score, int MaxX, int maxY)
            : base(width, height, texNum, xVelocity, yVelocity)
        {
            _maxX = MaxX;
            _maxY = maxY;
            _score = score;
        }

        public override void Update(GameTime gameTime)
        {
            if (_collision && _health > 0)
            {
                _health -= 1;
                _collision = false;
            }
            if (_position.X + 1.4*_width > _maxX && initialising)
            {
                _position.X -= _width/60;
            }
            else if (_position.Y + 1.4*_height > _maxY && initialising)
            {
                _position.Y -= _height / 60;
            }
            else if (_position.Y < 0 && initialising)
            {
                _position.Y += _height / 60;
            }
            else
            {
                initialising = false;
            }

            base.Update(gameTime);
        }

        public int Score
        {
            get { return _score; }
        }

        public int GetBulVel
        {
            get { return _bulVel; }
        }

        public bool WillFire
        {
            get { return _willFire; }
            set { _willFire = value; }
        }
    }
}
