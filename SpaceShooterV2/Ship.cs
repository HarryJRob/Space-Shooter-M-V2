namespace SpaceShooterV2
{
    class Ship : GameObject
    {
        protected int _health;

        public Ship(int width, int height, byte texNum, int xVelocity, int yVelocity) : base(width, height, texNum, xVelocity, yVelocity) { }
    }
}
