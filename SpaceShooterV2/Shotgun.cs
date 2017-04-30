using Microsoft.Xna.Framework;

namespace SpaceShooterV2
{
    internal class Shotgun : EnemyShip
    {
        //Variables
        private const int CoolDownTotal = 85;
        private const int VelocityScale = 35;
        private int _target = -1;

        //Public Procedures
        public Shotgun(double widthByHeight, int height, byte texNum, int bulVel, int score, int diffculty, int maxX, int maxY, float startingPosition)
            : base(widthByHeight, height, texNum, 0, 0, score, maxX, maxY, startingPosition)
        {
            _xBulVel = bulVel;
            _health = 2 * diffculty;
            _score = score * diffculty;
        }

        public void Update(GameTime gameTime, Vector2 playerPos)
        {
            if (!initialising)
            {
                #region Firing Logic
                // Obj: 1.ii.3 c
                if (_currentCoolDown < CoolDownTotal)
                {
                    _currentCoolDown += 1;
                }
                else
                {
                    _willFire = true;
                    _currentCoolDown = 0;
                }

                #endregion

                #region Moving Logic
                // Obj: 1.ii.3 a
                if (!(_position.X <= playerPos.X + 5*_width && _position.X >= playerPos.X + _width))
                {
                    if (playerPos.X + 5*_width < _position.X)
                    {
                        _position.X -= _width/VelocityScale;
                    }
                    else if (playerPos.X > _position.X && playerPos.X < _maxX)
                    {
                        _position.X += _width/VelocityScale;
                    }
                }

                if (
                    !(getCenterPoint.Y >= playerPos.Y - _height/VelocityScale &&
                      getCenterPoint.Y <= playerPos.Y + _height/VelocityScale))
                {
                    if (playerPos.Y < getCenterPoint.Y)
                    {
                        _position.Y -= _height/VelocityScale;
                    }
                    else if (playerPos.Y > getCenterPoint.Y && playerPos.Y < _maxY)
                    {
                        _position.Y += _height/VelocityScale;
                    }
                }

                #endregion
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
