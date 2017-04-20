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

        protected EnemyShip(int width, int height, byte texNum, int xVelocity, int yVelocity, int score, int MaxX, int maxY, float startingPosition)
            : base(width, height, texNum, xVelocity, yVelocity)
        {
            _maxX = MaxX;
            _maxY = maxY;
            _score = score;
            _position = new Vector2(MaxX + 0.5f*_width, startingPosition);
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
            
            if (_position.Y + 1.4*_height > _maxY && initialising)
            {
                _position.Y -= _height / 60;
            }
            if (_position.Y < 0 && initialising)
            {
                _position.Y += _height / 60;
            }

            if(_position.X < _maxX && _position.Y < _maxY && _position.Y > 0)
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
