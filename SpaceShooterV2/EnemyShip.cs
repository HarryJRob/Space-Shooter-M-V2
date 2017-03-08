namespace SpaceShooterV2
{
    class EnemyShip : Ship
    {
        private int _score;

        public EnemyShip(int width, int height, byte texNum, int xVelocity, int yVelocity, int score)
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
