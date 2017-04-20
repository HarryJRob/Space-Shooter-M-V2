using Microsoft.Xna.Framework;

namespace SpaceShooterV2
{
    internal abstract class EnemyShip : Ship
    {
        protected int _score;
        protected int _currentCoolDown;
        protected bool _willFire;
        protected int _bulVel;

        protected EnemyShip(int width, int height, byte texNum, int xVelocity, int yVelocity, int score)
            : base(width, height, texNum, xVelocity, yVelocity)
        {
            _score = score;
        }

        public override void Update(GameTime gameTime)
        {
            if (_collision && _health > 0)
            {
                _health -= 1;
                _collision = false;
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
