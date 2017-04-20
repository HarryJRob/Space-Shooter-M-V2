using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace SpaceShooterV2
{
    internal class MainGame
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private GameWindow _window;

        private const int ShipScale = 13;
        private const int BulletScale = 70;
        private const int ColumnNum = 10;
        private const int RowNum = 10;

        private const bool Testing = false;
        private bool _multiplayer;

        private List<int>[,] _objectCollisionList;
        private List<GameObject> _objectList;
        private List<Texture2D> _textureList;
        private SpriteFont _font;
        private KeyboardState _curKeyState;

        private bool _dead;

        private int _previousFPS = 60;
        private int _score;

        private float _tileWidth;
        private float _tileHeight;

        private string _settings;
        private int _diffculty;

        public MainGame(bool multiplayer,ref GraphicsDeviceManager graphics, GameWindow window, SpriteBatch spriteBatch, string settings, int difficulty)
        {
            _multiplayer = multiplayer;
            _graphics = graphics;
            _window = window;
            _spriteBatch = spriteBatch;
            _settings = settings;
            _diffculty = difficulty;

            _graphics.ApplyChanges();
        }

        public void Initialize()
        {
            _objectList = new List<GameObject>();
            _textureList = new List<Texture2D>();

            #region ObjectCollisionList Set Up

            _objectCollisionList = new List<int>[ColumnNum, RowNum];

            for (var x = 0; x < ColumnNum; x++)
                for (var y = 0; y < RowNum; y++)
                    _objectCollisionList[x, y] = new List<int>();

            #endregion

            _tileWidth = (float) _window.ClientBounds.Width/ColumnNum;
            _tileHeight = (float) _window.ClientBounds.Height/RowNum;
        }

        public void LoadContent(List<Texture2D> texList, SpriteFont font)
        {
            //0 = collisionTex, 1 = Background, 2 = playerShip, 3 = Long Bullet, 4 = Round Bullet, 5 = Health bar piece
            _textureList = texList;
            _font = font;
            Debug.WriteLine(" Main Game - Game Assets loaded");

            #region player SetUp
            Debug.WriteLine(" Main Game - ShipScale:" +_window.ClientBounds.Height/ShipScale);

            string[] playerSetting = new string[1];
            playerSetting = _settings.Split(':');

            if (_multiplayer)
            {
                _objectList.Add(new PlayerShip(_textureList[2].Width/_textureList[2].Height,
                    _window.ClientBounds.Height/ShipScale, 2, 1, playerSetting[0], _window.ClientBounds.Width,
                    _window.ClientBounds.Height, (double)_textureList[5].Width / _textureList[5].Height));
                _objectList.Add(new PlayerShip(_textureList[2].Width/_textureList[2].Height,
                    _window.ClientBounds.Height/ShipScale, 2, 2, playerSetting[1], _window.ClientBounds.Width,
                    _window.ClientBounds.Height, (double)_textureList[5].Width / _textureList[5].Height));
            }
            else
            {
                _objectList.Add(new PlayerShip(_textureList[2].Width/_textureList[2].Height,
                    _window.ClientBounds.Height / ShipScale, 2, 0, playerSetting[0], _window.ClientBounds.Width,
                    _window.ClientBounds.Height, (double)_textureList[5].Width / _textureList[5].Height));
            }

            #endregion

            _objectList.Add(new Charger(_textureList[2].Width/_textureList[2].Height,
                _window.ClientBounds.Height/ShipScale, 2, _window.ClientBounds.Height/(int) (0.6f*BulletScale), 50,
                _diffculty));
            _objectList.Add(new Shotgun(_textureList[2].Width/_textureList[2].Height,
                _window.ClientBounds.Height/ShipScale, 2, _window.ClientBounds.Height/(int) (1.3f*BulletScale), 100,
                _diffculty));
        }

        public void Update(GameTime gameTime)
        {
            _curKeyState = Keyboard.GetState();

            if (!_dead)
            {
                #region Update Objects

                for (var i = 0; i < _objectList.Count; i++)
                    if (_objectList[i] != null)
                    {
                        if (_objectList[i].GetType() == typeof(PlayerShip))
                        {
                            #region Player Update
                            ((PlayerShip) _objectList[i]).Update(gameTime, _curKeyState);
                            if (((PlayerShip) _objectList[i]).Firing)
                            {
                                _objectList.Add(new Bullet(_textureList[3].Width/_textureList[3].Height,
                                    _window.ClientBounds.Height/BulletScale, 3, _window.ClientBounds.Height/(int)(0.5f * BulletScale),
                                    0,
                                    ((PlayerShip) _objectList[i]).getCenterPoint, true));
                                ((PlayerShip) _objectList[i]).Firing = false;
                            }
                        #endregion
                        }
                        else if ((_objectList[i].GetType() == typeof(EnemyShip)) ||
                                 _objectList[i].GetType().IsSubclassOf(typeof(EnemyShip)))
                        {
                            #region Charger Update

                            if (_objectList[i].GetType() == typeof(Charger))
                            {
                                ((Charger) _objectList[i]).Update(gameTime);

                                if (((Charger) _objectList[i]).WillFire)
                                {
                                    ((Charger) _objectList[i]).WillFire = false;

                                    if (_multiplayer)
                                    {
                                        var whichShip = new Random();
                                        double movementAngle;

                                        if ((((PlayerShip) _objectList[0]).Health != 0) &&
                                            (((PlayerShip) _objectList[1]).Health != 0))
                                        {
                                            movementAngle =
                                                ((Charger) _objectList[i]).GetAngleTwoPoints(
                                                    _objectList[i].getCenterPoint,
                                                    _objectList[whichShip.Next(0, 2)].getCenterPoint);

                                            _objectList.Add(new Bullet(_textureList[4].Width/_textureList[4].Height,
                                                _window.ClientBounds.Height/BulletScale, 4,
                                                (int) (((Charger) _objectList[i]).GetBulVel*Math.Cos(movementAngle)*-1),
                                                (int) (((Charger) _objectList[i]).GetBulVel*Math.Sin(movementAngle)*-1),
                                                ((Charger) _objectList[i]).getCenterPoint, false));
                                        }
                                        else if (((PlayerShip) _objectList[0]).Health != 0)
                                        {
                                            movementAngle =
                                                ((Charger) _objectList[i]).GetAngleTwoPoints(
                                                    _objectList[i].getCenterPoint,
                                                    _objectList[0].getCenterPoint);

                                            _objectList.Add(new Bullet(_textureList[4].Width/_textureList[4].Height,
                                                _window.ClientBounds.Height/BulletScale, 4,
                                                (int) (((Charger) _objectList[i]).GetBulVel*Math.Cos(movementAngle)*-1),
                                                (int) (((Charger) _objectList[i]).GetBulVel*Math.Sin(movementAngle)*-1),
                                                ((Charger) _objectList[i]).getCenterPoint, false));
                                        }
                                        else if (((PlayerShip) _objectList[1]).Health != 0)
                                        {
                                            movementAngle =
                                                ((Charger) _objectList[i]).GetAngleTwoPoints(
                                                    _objectList[i].getCenterPoint,
                                                    _objectList[1].getCenterPoint);

                                            _objectList.Add(new Bullet(_textureList[4].Width/_textureList[4].Height,
                                                _window.ClientBounds.Height/BulletScale, 4,
                                                (int) (((Charger) _objectList[i]).GetBulVel*Math.Cos(movementAngle)*-1),
                                                (int) (((Charger) _objectList[i]).GetBulVel*Math.Sin(movementAngle)*-1),
                                                ((Charger) _objectList[i]).getCenterPoint, false));
                                        }
                                    }
                                    else
                                    {
                                        var movementAngle =
                                            ((Charger) _objectList[i]).GetAngleTwoPoints(_objectList[i].getCenterPoint,
                                                _objectList[0].getCenterPoint);
                                        _objectList.Add(new Bullet(_textureList[4].Width/_textureList[4].Height,
                                            _window.ClientBounds.Height/BulletScale, 4,
                                            (int) (((Charger) _objectList[i]).GetBulVel*Math.Cos(movementAngle)*-1),
                                            (int) (((Charger) _objectList[i]).GetBulVel*Math.Sin(movementAngle)*-1),
                                            ((Charger) _objectList[i]).getCenterPoint, false));
                                    }
                                    ((Charger) _objectList[i]).UpdateCurCharge();
                                }
                            }
                            #endregion

                            #region ShotGun Update

                            if (_objectList[i].GetType() == typeof(Shotgun))
                            {
                                ((Shotgun) _objectList[i]).Update(gameTime);

                                if (((Shotgun) _objectList[i]).WillFire)
                                {
                                    ((Shotgun) _objectList[i]).WillFire = false;

                                    _objectList.Add(new Bullet(_textureList[4].Width/_textureList[4].Height,
                                        _window.ClientBounds.Height/BulletScale, 4,
                                        Convert.ToInt32(((Shotgun) _objectList[i]).GetBulVel*Math.Sin(Math.PI/2)*-1),
                                        Convert.ToInt32(((Shotgun) _objectList[i]).GetBulVel*Math.Cos(Math.PI/2)*-1),
                                        ((Shotgun) _objectList[i]).getCenterPoint, false));

                                    _objectList.Add(new Bullet(_textureList[4].Width/_textureList[4].Height,
                                        _window.ClientBounds.Height/BulletScale, 4,
                                        Convert.ToInt32(((Shotgun)_objectList[i]).GetBulVel * Math.Sin(1.92) * -1),
                                        Convert.ToInt32(((Shotgun)_objectList[i]).GetBulVel * Math.Cos(1.92) * -1),
                                        ((Shotgun) _objectList[i]).getCenterPoint, false));

                                    _objectList.Add(new Bullet(_textureList[4].Width/_textureList[4].Height,
                                        _window.ClientBounds.Height/BulletScale, 4,
                                        Convert.ToInt32(((Shotgun)_objectList[i]).GetBulVel * Math.Sin(1.22) * -1),
                                        Convert.ToInt32(((Shotgun)_objectList[i]).GetBulVel * Math.Cos(1.22) * -1),
                                        ((Shotgun) _objectList[i]).getCenterPoint, false));
                                }
                            }

                            #endregion

                            #region Deletion of EnemyShips if they have collided
                            if (((Ship)_objectList[i]).Health == 0)
                            {
                                if ((_objectList[i].GetType() == typeof(EnemyShip)) ||
                                    _objectList[i].GetType().IsSubclassOf(typeof(EnemyShip)))
                                {
                                    _score += ((EnemyShip) _objectList[i]).Score;
                                    Debug.WriteLine(" Main Game - Cur Score: " + _score);
                                }
                                _objectList[i] = null;
                            }
                            #endregion
                        }
                        else if (_objectList[i].GetType() == typeof(Bullet))
                        {
                            _objectList[i].Update(gameTime);
                            if (_objectList[i].Collision)
                                _objectList[i] = null;
                        }

                        #region Updating ObjectCollisionList
                        //Every object which has not collided with something and exits i.e. not null, adds a reference to itself in each box its corners are in
                        if ((_objectList[i] != null) && !_objectList[i].Collision)
                        {
                            if (!(_objectList[i].GetType() == typeof(PlayerShip) &&
                                  ((PlayerShip) _objectList[i]).Health == 0))
                            {
                                var curObjRec = _objectList[i].BoundingBox;

                                if ((curObjRec.X > 0) && (curObjRec.Y > 0) && (curObjRec.X < _window.ClientBounds.Width) &&
                                    (curObjRec.Y < _window.ClientBounds.Height))
                                    _objectCollisionList[
                                        (int) Math.Truncate(curObjRec.X/_tileWidth),
                                        (int) Math.Truncate(curObjRec.Y/_tileHeight)].Add(i);

                                if ((curObjRec.X + curObjRec.Width > 0) && (curObjRec.Y > 0) &&
                                    (curObjRec.X + curObjRec.Width < _window.ClientBounds.Width) &&
                                    (curObjRec.Y < _window.ClientBounds.Height))
                                    _objectCollisionList[
                                        (int) Math.Truncate((curObjRec.X + curObjRec.Width)/_tileWidth),
                                        (int) Math.Truncate(curObjRec.Y/_tileHeight)].Add(i);

                                if ((curObjRec.X > 0) && (curObjRec.Y + curObjRec.Height > 0) &&
                                    (curObjRec.X < _window.ClientBounds.Width) &&
                                    (curObjRec.Y + curObjRec.Height < _window.ClientBounds.Height))
                                    _objectCollisionList[
                                        (int) Math.Truncate(curObjRec.X/_tileWidth),
                                        (int) Math.Truncate((curObjRec.Y + curObjRec.Height)/_tileHeight)].Add(i);

                                if ((curObjRec.X + curObjRec.Width > 0) && (curObjRec.Y + curObjRec.Height > 0) &&
                                    (curObjRec.X + curObjRec.Width < _window.ClientBounds.Width) &&
                                    (curObjRec.Y + curObjRec.Height < _window.ClientBounds.Height))
                                    _objectCollisionList[
                                        (int) Math.Truncate((curObjRec.X + curObjRec.Width)/_tileWidth),
                                        (int) Math.Truncate((curObjRec.Y + curObjRec.Height)/_tileHeight)].Add(i);

                                if ((_objectList[i].GetType() == typeof(Bullet)) &&
                                    ((curObjRec.Y + curObjRec.Height < 0) || (curObjRec.X + curObjRec.Width < 0) ||
                                     (curObjRec.X > _window.ClientBounds.Width) ||
                                     (curObjRec.Y > _window.ClientBounds.Height)))
                                {
                                    Debug.WriteLine(" Main Game - Object {0} left screen", i);
                                    _objectList[i] = null;
                                }
                            }
                        }

                        #endregion
                    }

                #endregion

                #region Collision Check

                for (int x = 0; x < ColumnNum; x++)
                    for (int y = 0; y < RowNum; y++)
                    {
                        //If there are things which can collide
                        if ((_objectCollisionList[x, y].Count >= 2) &&
                            (ContainsCompareTypes(typeof(PlayerShip), _objectCollisionList[x, y]) |
                             ContainsCompareTypes(typeof(EnemyShip), _objectCollisionList[x, y])) &&
                            ContainsCompareTypes(typeof(Bullet), _objectCollisionList[x, y]))
                        {
                            //Do collision check
                            //Get the objects which are contained within that square
                            //Updating the filtered list will update the original list due to the way that C# handles lists as a collection of references
                            List<GameObject> filteredList = new List<GameObject>();
                            foreach (int i in _objectCollisionList[x, y])
                            {
                                if (!filteredList.Contains(_objectList[i]))
                                    filteredList.Add(_objectList[i]);
                            }

                            //Check if any bullets collide with ships
                            foreach (Bullet curBullet in filteredList.OfType<Bullet>())
                                //Dont check if the bullet has already collided with something
                                if (!curBullet.Collision)
                                    foreach (Ship curShip in filteredList.OfType<Ship>())
                                        if (!curShip.Collision)
                                            //Now do the collision check
                                            if ((((curShip.GetType() == typeof(PlayerShip)) && !curBullet.Owner) ||
                                                 (curShip.GetType().IsSubclassOf(typeof(EnemyShip)) && curBullet.Owner)) &&
                                                curShip.BoundingBox.Intersects(curBullet.BoundingBox))
                                            {
                                                Debug.WriteLine(" Main Game - Collision at ({0},{1})", x, y);
                                                curShip.Collision = true;
                                                curBullet.Collision = true;
                                            }
                            filteredList.Clear();
                        }
                        _objectCollisionList[x, y].Clear();
                    }

                #endregion

                #region Deleted null from ObjectList
                //!_objectList.OfType<EnemyShip>().Any()
                if (_objectList.Contains(null))
                {
                    _objectList.RemoveAll(item => item == null);
                    Debug.WriteLine(" Main Game - Collapsed ObjectList");
                }

                #endregion

                #region CheckAlive

                if (_multiplayer)
                {
                    if (((PlayerShip) _objectList[0]).Health + ((PlayerShip) _objectList[1]).Health == 0)
                    {
                        _dead = true;
                    }
                }
                else
                {
                    if (((PlayerShip)_objectList[0]).Health == 0)
                    {
                        _dead = true;
                    }
                }

                #endregion
            }
        }

        public void Draw(GameTime gameTime)
        {

                //_spriteBatch.Draw(_textureList[1],
                //    new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight),
                //    Color.White);

            #region Drawing Collision Boxes

            if (Testing)
                for (var x = 0; x < ColumnNum; x++)
                    for (var y = 0; y < RowNum; y++)
                        _spriteBatch.Draw(_textureList[0],
                            new Rectangle((int) (x*_tileWidth), (int) (y*_tileHeight), (int) _tileWidth,
                                (int) _tileHeight), Color.White);

            #endregion

            #region Drawing Objects

            foreach (var curObj in _objectList)
                if (curObj != null)
                    if (!(curObj.GetType() == typeof(PlayerShip)))
                    {
                        curObj.Draw(_spriteBatch, _textureList[curObj.TexNum]);
                    }
                    else if (((PlayerShip) curObj).Health == 0)
                    {
                        ((PlayerShip) curObj).DrawDeath(_spriteBatch, _textureList[curObj.TexNum]);
                    }
                    else
                    {
                        curObj.Draw(_spriteBatch, _textureList[curObj.TexNum]);
                       ((PlayerShip) curObj).DrawUI(_spriteBatch,_textureList[5]); 
                    }

            #endregion

            #region Game Over

            if (_dead)
            {
               Debug.WriteLine(" Main Game - Game Over");
            }

            #endregion

            #region Draw Score 

            _spriteBatch.DrawString(_font, Convert.ToString(_score), new Vector2(_window.ClientBounds.Width / 2 - _font.MeasureString(Convert.ToString(_score)).X / 2, 0), Color.RoyalBlue);

            #endregion

            #region Calculate FPS

            if (_previousFPS != Convert.ToInt32(1/gameTime.ElapsedGameTime.TotalSeconds))
            {
                Debug.WriteLine(" Main Game - Draw fps: {0}, No. Objects {1}",
                    Convert.ToInt32(1/gameTime.ElapsedGameTime.TotalSeconds), _objectList.Count);
                _previousFPS = Convert.ToInt32(1/gameTime.ElapsedGameTime.TotalSeconds);
                //Aprox 500 objects without fps drop
            }

            #endregion
        }

        private bool ContainsCompareTypes(Type t, List<int> intList)
        {
            foreach (var i in intList)
                if (_objectList[i].GetType() == t || _objectList[i].GetType().IsSubclassOf(t))
                    return true;
            return false;
        }

        public bool IsDead
        {
            get { return _dead;}
        }

        public int TotalScore
        {
            get { return _score; }
        }
    }
}
