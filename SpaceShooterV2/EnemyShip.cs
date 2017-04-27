using Microsoft.Xna.Framework;

namespace SpaceShooterV2
{
    internal abstract class EnemyShip : Ship
    {
        //Variables
        protected int _score;
        protected int _currentCoolDown;
        protected bool _willFire;
        protected int _xBulVel;
        protected bool initialising = true;
        protected int _maxX;
        protected int _maxY;

        //Public procedures
        protected EnemyShip(double widthByHeight, int height, byte texNum, int xVelocity, int yVelocity, int score, int maxX, int maxY, float startingPosition)
            : base(widthByHeight, height, texNum, xVelocity, yVelocity)
        {
            _maxX = maxX;
            _maxY = maxY;
            _score = score;
            _position = new Vector2(maxX + 0.5f*_width, startingPosition);
        }

        //Procedures Procedures
        public override void Update(GameTime gameTime)
        {

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

            if(_position.X + 1.1*_width < _maxX && _position.Y + _height < _maxY && _position.Y > 0)
            {
                initialising = false;
            }

            base.Update(gameTime);
        }

        //Public Accessors
        public int Score
        {
            get { return _score; }
        }

        public int GetBulXVel
        {
            get { return _xBulVel; }
        }

        public bool WillFire
        {
            get { return _willFire; }
            set { _willFire = value; }
        }
    }
}
