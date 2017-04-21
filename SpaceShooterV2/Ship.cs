using Microsoft.Xna.Framework;

namespace SpaceShooterV2
{
    internal abstract class Ship : GameObject
    {
        protected int _health;
        protected int _dmgToTake;

        protected Ship(int width, int height, byte texNum, int xVelocity, int yVelocity)
            : base(width, height, texNum, xVelocity, yVelocity) { }

        public override void Update(GameTime gameTime)
        {
            if (_collision && _health > 0)
            {
                _health -= _dmgToTake;
                _collision = false;
            }

            base.Update(gameTime);
        }

        public int Health
        {
            get { return _health; }
        }

        public int dmgToTake
        {
            set { _dmgToTake = value; }
        }
    }
}
