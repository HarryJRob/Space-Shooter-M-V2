using Microsoft.Xna.Framework;

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

        public Vector2 getCenterPoint
        {
            get { return new Vector2(_position.X + _width/2,_position.Y + _height/2);}
        }
    }
}
