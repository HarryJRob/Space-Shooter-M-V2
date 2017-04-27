using System;
using Microsoft.Xna.Framework;

namespace SpaceShooterV2
{
    internal class Charger : EnemyShip
    {
        //Variables
        private enum fireState
        {
            Firing,
            Charging,
        }

        private fireState _curState = fireState.Charging;
        private const int CoolDownTotal = 35;
        private int _curCharge;
        private int _chargeTo = 4;

        //Public Procedures
        public Charger(double widthByHeight, int height, byte texNum, int bulVel, int score, int diffculty, int maxX, int maxY, float startingPosition)
            : base(widthByHeight, height, texNum, 0 , 0, score, maxX, maxY, startingPosition)
        {
            _xBulVel = bulVel;
            _health = 1 * diffculty;
            _score = score * diffculty;
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
            base.Update(gameTime);
        }

        public void UpdateCurCharge()
        {
            _curCharge -= 1;
        }

        //Public Functions
        public double GetAngleTwoPoints(Vector2 point1, Vector2 point2)
        {
            double xDif = point1.X - point2.X;
            double yDif = point1.Y - point2.Y;
            return Math.Atan2(yDif, xDif);
        }

    }
}
