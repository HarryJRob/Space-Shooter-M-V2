using Microsoft.Xna.Framework;

namespace SpaceShooterV2
{
    class Bullet : GameObject
    {
        public Bullet(int Width, int Height, byte TexNum, int xVelocity, int yVelocity): base(Width, Height, TexNum, xVelocity, yVelocity)
        {
            _position = new Vector2(20,200);
        }
    }
}
