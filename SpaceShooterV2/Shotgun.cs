using Microsoft.Xna.Framework;

namespace SpaceShooterV2
{
    internal class Shotgun : EnemyShip
    {
        private const int CoolDownTotal = 90;

        public Shotgun(int width, int height, byte texNum, int bulVel, int score, int diffculty)
            : base(width, height, texNum, 0, 0, score)
        {
            _bulVel = bulVel;
            _health = 2 * diffculty;
            _score = score * diffculty;
            _position = new Vector2(1600,400);
        }

        public override void Update(GameTime gameTime)
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

            base.Update(gameTime);
        }
    }
}
