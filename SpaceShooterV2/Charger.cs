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
        }

        private fireState _curState = fireState.Charging;
        private bool _willFire;
        private const int _coolDownTotal = 40;
        private int _currentCoolDown;
        private int _curCharge;
        private int _bulVel;

        public Charger(int width, int height, byte texNum,int bulVel, int score)
            : base(width, height, texNum, 0 , 0, score)
        {
            _bulVel = bulVel;
            _position = new Vector2(400,400);
        }

        public override void Update(GameTime gameTime)
        {
            if (_currentCoolDown < _coolDownTotal)
            {
                _currentCoolDown += 1;
            }
            else if (_curState == fireState.Charging && _currentCoolDown >= _coolDownTotal)
            {
                _currentCoolDown = 0;
                _curCharge += 1;
            }

            if (_curCharge >= 4)
            {
                _curState = fireState.Firing;
            }

            if (_curCharge == 0 && _curState == fireState.Firing)
            {
                _curState = fireState.Charging;
            }

            if (_curState == fireState.Firing && _currentCoolDown >= _coolDownTotal)
            {
                _currentCoolDown = 0;
                _willFire = true;
            }
            base.Update(gameTime);
        }

        public double GetAngleTwoPoints(Vector2 point1, Vector2 point2)
        {
            double xDif = point1.X - point2.X;
            double yDif = point1.Y - point2.Y;
            return Math.Atan2(yDif, xDif);
        }

        public int getBulVel
        {
            get { return _bulVel;}
        }

        public bool WillFire
        {
            get { return _willFire; }
            set { _willFire = value; }
        }

        public void UpdateCurCharge()
        {
            _curCharge -= 1;
        }
    }
}
