using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SpaceShooterV2
{
    class Charger : EnemyShip
    {
        private enum fireState
        {
            Firing,
            Charging,
            Charged,
            Fired,
        }

        private fireState _curState = fireState.Charging;
        private const int _coolDownTotal = 40;
        private int _currentCoolDown;
        private int _curCharge;
        private List<int> _bulletListPos = new List<int>();
        private int _bulVel;

        public Charger(int width, int height, byte texNum,int bulVel, int score)
            : base(width, height, texNum, 0 , 0, score)
        {
            _bulVel = bulVel;
            _position = new Vector2(400,400);
        }

        public override void Update(GameTime gameTime)
        {
            if (_currentCoolDown < _coolDownTotal && _curState == fireState.Charging)
            {
                _currentCoolDown += 1;
            }
            if (_currentCoolDown >= _coolDownTotal && _curState == fireState.Charging)
            {
                _curState = fireState.Charged;
                _curCharge += 1;
            }

            if (_curCharge >= 4)
            {
                _curState = fireState.Firing;
            }

            if (_curState == fireState.Fired && _bulletListPos.Count == 0)
            {
                _curState = fireState.Charging;
            }
            base.Update(gameTime);
        }

        public double GetAngleTwoPoints(Vector2 point1, Vector2 point2)
        {
            double xDif = point1.X - point2.X;
            double yDif = point1.Y - point2.Y;
            return Math.Atan2(yDif, xDif);
        }

        public List<int> GetBulletReference
        {
            get { return _bulletListPos; }
        }

        public void AddBulletReference(int bulletRef)
        {
            _bulletListPos.Add(bulletRef);
        }

        public int getBulVel
        {
            get { return _bulVel;}
        }

        public int getState()
        {
            if (_curState == fireState.Firing)
            {
                _curState = fireState.Fired;
                return 0;
            }
            if (_curState == fireState.Charged)
            {
                _curState = fireState.Charging;
                return 1;
            }
            return 2;
        }
    }
}
