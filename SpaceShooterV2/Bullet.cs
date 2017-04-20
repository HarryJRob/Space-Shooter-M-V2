using Microsoft.Xna.Framework;

namespace SpaceShooterV2
{
    internal class Bullet : GameObject
    {
        private readonly bool _owner; //true indiciates player owned and false indicates enemy owned
        public Bullet(int Width, int Height, byte TexNum, int xVelocity, int yVelocity,Vector2  startingPos , bool Owner): base(Width, Height, TexNum, xVelocity, yVelocity)
        {
            _owner = Owner;
            _position = new Vector2(startingPos.X, startingPos.Y - _height/2);
        }

        public bool Owner
        {
            get { return _owner; }
        }

        public int xVel
        {
            get { return _xVelocity; }
            set { _xVelocity = value; }
        }

        public int yVel
        {
            get { return _yVelocity;}
            set { _yVelocity = value; }
        }
    }
}
