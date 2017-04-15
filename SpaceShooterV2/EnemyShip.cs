namespace SpaceShooterV2
{
    internal abstract class EnemyShip : Ship
    {
        protected int _score;
        protected int _diffculty;

        protected EnemyShip(int width, int height, byte texNum, int xVelocity, int yVelocity, int score)
            : base(width, height, texNum, xVelocity, yVelocity)
        {
            _score = score;
        }

        public int Score
        {
            get { return _score; }
        }
    }
}
