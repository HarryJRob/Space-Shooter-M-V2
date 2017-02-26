using Microsoft.Xna.Framework;

namespace SpaceShooterV2
{
    class Ship
    {
        protected Vector2 _shipPosition;
        protected int _width, _height;
        protected byte _shipTexNum;
        protected bool _collision;
        protected int _velocity;

        public Ship(int width, int height, byte shipTexNum)
        {
            _width = width;
            _height = height;
            _shipTexNum = shipTexNum;
        }

        public Vector2 ShipPosition
        {
            get { return _shipPosition; }
        }

        public int Width
        {
            get { return _width; }
        }

        public int Height
        {
            get { return _height; }
        }

        public Rectangle BoundingBox
        {
            get { return new Rectangle((int)_shipPosition.X,(int)_shipPosition.Y,_width,_height);}
        }

        public bool Collision
        {
            set { _collision = value; }
        }
    }
}
