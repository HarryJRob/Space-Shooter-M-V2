using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceShooterV2
{
    internal class PowerUp : GameObject
    {
        private readonly int _type;
        public PowerUp(int Width, int Height, byte TexNum, int xVelocity, Vector2 startingPos, int type)
            : base(Width, Height, TexNum, xVelocity,0)
        {
            _position = startingPos;
            _type = type;
        }

        public int type
        {
            get { return _type;}
        }

        public override void Draw(SpriteBatch spriteBatch, Texture2D tex)
        {
            spriteBatch.Draw(tex, new Rectangle((int)_position.X, (int)_position.Y, _width, _height), Color.Red);
        }
    }
}
