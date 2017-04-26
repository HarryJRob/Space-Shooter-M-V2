using Microsoft.Xna.Framework;

namespace SpaceShooterV2
{
    internal class Shotgun : EnemyShip
    {
        //Variables
        private const int CoolDownTotal = 90;
        private int _target = -1;

        //Public Procedures
        public Shotgun(int width, int height, byte texNum, int bulVel, int score, int diffculty, int maxX, int maxY, float startingPosition)
            : base(width, height, texNum, 0, 0, score, maxX, maxY, startingPosition)
        {
            _xBulVel = bulVel;
            _health = 2 * diffculty;
            _score = score * diffculty;
        }

        public void Update(GameTime gameTime, Vector2 playerPos)
        {
            if (!initialising)
            {
                if (_currentCoolDown < CoolDownTotal)
                {
                    _currentCoolDown += 1;
                }
                else
                {
                    _willFire = true;
                    _currentCoolDown = 0;
                }

                if (!(_position.X <= playerPos.X + 1.5f*_width && _position.X >= playerPos.X + _width))
                {
                    if (playerPos.X + 1.5*_width < _position.X)
                    {
                        _position.X -= _width/45;
                    }
                    else if (playerPos.X  > _position.X && playerPos.X< _maxX)
                    {
                        _position.X += _width / 45;
                    }
                }
                //x >= 1 && x <= 100
                if (!(_position.Y >= playerPos.Y - _height/15 && _position.Y <= playerPos.Y + _height/15))
                {
                    if (playerPos.Y < _position.Y)
                    {
                        _position.Y -= _height / 45;
                    }
                    else if (playerPos.Y > _position.Y && playerPos.Y < _maxY)
                    {
                        _position.Y += _height / 45;
                    }
                }
            }
            base.Update(gameTime);
        }

        //Public Accessors
        public int Target
        {
            get { return _target; }
            set { _target = value; }
        }
    }
}
