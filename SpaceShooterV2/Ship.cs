namespace SpaceShooterV2
{
    internal abstract class Ship : GameObject
    {
        protected int _health;

        protected Ship(int width, int height, byte texNum, int xVelocity, int yVelocity)
            : base(width, height, texNum, xVelocity, yVelocity) { }

        public int Health
        {
            get { return _health; }
        }
    }
}
