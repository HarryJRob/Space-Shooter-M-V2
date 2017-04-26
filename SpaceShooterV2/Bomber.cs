using System;
using Microsoft.Xna.Framework;

namespace SpaceShooterV2
{
    class Bomber : EnemyShip
    {
        //Variables
        private double _fireAngle = 0.5*Math.PI-Math.PI/8;
        private const int CoolDownTotal = 10;

        //Public Procedures
        public Bomber(int width, int height, byte texNum, int bulVel, int score, int diffculty, int maxX, int maxY,
            float startingPosition)
            : base(width, height, texNum, 0, 0, score, maxX, maxY, startingPosition)
        {
            _xBulVel = bulVel;
            _health = 2 * diffculty;
            _score = score * diffculty;
        }

        public override void Update(GameTime gameTime)
        {
            if (!initialising)
            {
                if (_currentCoolDown < CoolDownTotal)
                {
                    _currentCoolDown += 1;
                }
                else
                {
                    _willFire = true;
                    _fireAngle += Math.PI/8;
                    _fireAngle = _fireAngle%(2*Math.PI);
                    _currentCoolDown = 0;
                }

                if (_position.X + _width < 0)
                {
                    _position.X = _maxX;
                    Random rnd = new Random();
                    _position.Y = rnd.Next(0, _maxY - _height + 1);
                }

                if (_xVelocity == 0)
                {
                    _xVelocity = -_width / 70;
                }
            }
            base.Update(gameTime);
        }

        //Public Accessors
        public double FireAngle
        {
            get { return _fireAngle; }
            set { _fireAngle = value; }
        }
    }
}
