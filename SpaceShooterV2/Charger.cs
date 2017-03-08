using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SpaceShooterV2
{
    class Charger : EnemyShip
    {
        private const int _coolDownTotal = 40;
        private int _currentCoolDown;
        private List<int> _bulletListPos = new List<int>();
        private int _bulVel;

        public Charger(int width, int height, byte texNum,int bulVel, int score)
            : base(width, height, texNum, 0 , 0, score)
        {
            _bulVel = bulVel;
        }

        public override void Update(GameTime gameTime)
        {
            _currentCoolDown += 1;

            base.Update(gameTime);
        }

        public double GetAngleTwoPoints(Vector2 point1, Vector2 point2)
        {
            double xDif = point1.X - point2.X;
            double yDif = point1.Y - point2.Y;
            return Math.Atan2(yDif, xDif);
        }

        public int Firing()
        {
            if (_currentCoolDown == _coolDownTotal && _bulletListPos.Count == 4)
            {
                _currentCoolDown = 0;
                return 0;
            }
            if (_currentCoolDown == _coolDownTotal)
            {
                _currentCoolDown = 0;
                return 1;
            }
            return 2;
        }

        public List<int> getBulletReference
        {
            get { return _bulletListPos; }
        }

        public void addBulletReference(int bulletRef)
        {
            _bulletListPos.Add(bulletRef);
        }

        public int getBulVel
        {
            get { return _bulVel;}
        }
    }
}
