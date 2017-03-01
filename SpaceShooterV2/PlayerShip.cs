using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceShooterV2
{
    class PlayerShip : Ship
    {
        public PlayerShip(int Width, int Height, byte TexNum, int xVelocity, int yVelocity, byte playerID,string keyList) : base(Width, Height,TexNum, xVelocity, yVelocity)
        {
            _health = 5;

            if (playerID == 1)
            {
                _position.Y = Height;
            }
            else if (playerID == 2)
            {
                _position.Y = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - Height;
            }
            else
            {
                _position.Y = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height/2 - Height/2;
            }
            _position.X += 40;
            
        }

        public override void Update(GameTime _gameTime)
        {
            base.Update(_gameTime); 
        }
    }
}
