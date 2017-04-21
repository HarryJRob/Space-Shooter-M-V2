using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceShooterV2
{
    internal class Bullet : GameObject
    {
        private readonly bool _owner; //true indiciates player owned and false indicates enemy owned
        private readonly int _dmg;

        public Bullet(int Width, int Height, byte TexNum, int xVelocity, int yVelocity,Vector2  startingPos , bool Owner, int dmg): base(Width, Height, TexNum, xVelocity, yVelocity)
        {
            _dmg = dmg;
            _owner = Owner;
            _position = new Vector2(startingPos.X, startingPos.Y - _height/2);
        }

        public override void Draw(SpriteBatch spriteBatch, Texture2D tex)
        {
            if (_dmg == 2)
            {
                spriteBatch.Draw(tex, new Rectangle((int) _position.X, (int) _position.Y, _width, _height), Color.DarkRed);
            }
            else
            {
                base.Draw(spriteBatch,tex);
            }
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

        public int Dmg
        {
            get { return _dmg; }
        }
    }
}
