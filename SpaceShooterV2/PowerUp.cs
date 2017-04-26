using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceShooterV2
{
    internal class PowerUp : GameObject
    {
        //Variables
        private readonly int _type;

        //Public Procedures
        public PowerUp(int Width, int Height, byte TexNum, int xVelocity, Vector2 startingPos, int type)
            : base(Width, Height, TexNum, xVelocity,0)
        {
            _position = startingPos;
            _type = type;
        }

        //Public Accessors
        public int Type
        {
            get { return _type;}
        }

    }
}
