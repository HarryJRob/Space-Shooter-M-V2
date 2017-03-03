using Microsoft.Xna.Framework;

namespace SpaceShooterV2
{
    class Bullet : GameObject
    {
        private bool _owner; //true indiciates player powed and false indicates enemy owned
        public Bullet(int Width, int Height, byte TexNum, int xVelocity, int yVelocity, bool Owner): base(Width, Height, TexNum, xVelocity, yVelocity)
        {
            _owner = Owner;
            _position = new Vector2(400,400);
        }

        public bool Owner
        {
            get { return _owner; }
        }
    }
}
