using Microsoft.Xna.Framework;

namespace SpaceShooterV2
{
    internal class Shotgun : EnemyShip
    {
        private const int CoolDownTotal = 90;

        public Shotgun(int width, int height, byte texNum, int bulVel, int score, int diffculty, int maxX, int maxY)
            : base(width, height, texNum, 0, 0, score, maxX, maxY)
        {
            _bulVel = bulVel;
            _health = 2 * diffculty;
            _score = score * diffculty;
            _position = new Vector2(5000,2000);
        }

        public void Update(GameTime gameTime, Vector2 playerPos)
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

            if (!(_position.X >= playerPos.X - 3* _width && _position.X <= playerPos.X + 3*_width))
            {
                if (playerPos.X + 3*_width < _position.X)
                {
                    _position.X -= _width/60;
                }
                else if (playerPos.X + 3*_width > _position.X && playerPos.X + 3*_width < _maxX)
                {
                    _position.X += _width/60;
                }
            }
            //x >= 1 && x <= 100
            if (!(_position.Y >= playerPos.Y - _height/15 && _position.Y <= playerPos.Y + _height/15))
            {
                if (playerPos.Y < _position.Y)
                {
                        _position.Y -= _height/60;
                }
                else if (playerPos.Y > _position.Y && playerPos.Y < _maxY)
                {
                        _position.Y += _height/60;
                }
            }
            base.Update(gameTime);
        }
    }
}
