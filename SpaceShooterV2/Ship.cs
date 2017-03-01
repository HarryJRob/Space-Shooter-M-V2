﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceShooterV2
{
    class Ship : GameObject
    {
        protected int _health;

        public Ship() { } //No use but to allow inheritance

        public Ship(int width, int height, byte texNum, int xVelocity, int yVelocity) : base(width,height, texNum, xVelocity, yVelocity)
        {
            
        }
    }
}
