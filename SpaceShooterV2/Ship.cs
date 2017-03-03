namespace SpaceShooterV2
{
    class Ship : GameObject
    {
        protected int _health;
        protected bool _firing;

        public Ship() { } //No use but to allow inheritance

        public Ship(int width, int height, byte texNum, int xVelocity, int yVelocity) : base(width, height, texNum, xVelocity, yVelocity) { }

        public bool Firing
        {
            get { return _firing; }
            set { _firing = value; }
        }
    }
}
