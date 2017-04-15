using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace SpaceShooterV2
{
    internal class Charger : EnemyShip
    {
        private enum fireState
        {
            Firing,
            Charging,
        }

        private fireState _curState = fireState.Charging;
        private bool _willFire;
        private const int CoolDownTotal = 40;
        private int _currentCoolDown;
        private int _curCharge;
        private int _chargeTo = 4;
        private readonly int _bulVel;

        public Charger(int width, int height, byte texNum,int bulVel, int score, int diffculty)
            : base(width, height, texNum, 0 , 0, score)
        {
            _bulVel = bulVel;
            _health = 2 * diffculty;
            _score = score * diffculty;
            _position = new Vector2(400,400);
        }

        public override void Update(GameTime gameTime)
        {
            if (_currentCoolDown < CoolDownTotal)
            {
                _currentCoolDown += 1;
            }
            else if (_curState == fireState.Charging && _currentCoolDown >= CoolDownTotal)
            {
                _currentCoolDown = 0;
                _curCharge += 1;
            }

            if (_curCharge >= _chargeTo)
            {
                _curState = fireState.Firing;
            }

            if (_curCharge == 0 && _curState == fireState.Firing)
            {
                Random rnd = new Random();
                _chargeTo = rnd.Next(3, 9);
                _curState = fireState.Charging;
            }

            if (_curState == fireState.Firing && _currentCoolDown >= CoolDownTotal)
            {
                _currentCoolDown = 0;
                _willFire = true;
            }

            if (_collision && _health > 0)
            {
                _health -= 1;
                _collision = false;
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
