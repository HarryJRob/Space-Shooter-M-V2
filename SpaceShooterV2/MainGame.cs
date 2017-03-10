using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceShooterV2
{
    public class MainGame : Game
    {
        // '_' denotes private/protected, 'Capital' denotes public (I know this doesnt follow standard naming conventions)
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private const bool _multiplayer = false;
        private bool _dead;

        private const bool Testing = false;
        private int _previousFPS = 60;

        private const int ColumnNum = 10;
        private const int RowNum = 10;
        private float _tileWidth;
        private float _tileHeight;

        private List<int>[,] _objectCollisionList;
        private List<GameObject> _objectList;
        private List<Texture2D> _textureList;

        private const int ShipScale = 13;
        private const int BulletScale = 70;

        public MainGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            #region Force FullScreen

            if (!Testing)
            {
                _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                Window.AllowUserResizing = false;
                _graphics.ToggleFullScreen();
            }
            Window.IsBorderless = true;

            Console.WriteLine("Window Size: ({0},{1})", Window.ClientBounds.Width, Window.ClientBounds.Height);

            #endregion

            #region Console SetUp

            Console.Title = "Developer console";
            Console.CursorVisible = false;

            #endregion

            _graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            _objectList = new List<GameObject>();
            _textureList = new List<Texture2D>();

            #region ObjectCollisionList Set Up

            _objectCollisionList = new List<int>[ColumnNum, RowNum];

            for (var x = 0; x < ColumnNum; x++)
                for (var y = 0; y < RowNum; y++)
                    _objectCollisionList[x, y] = new List<int>();

            #endregion

            _tileWidth = (float) Window.ClientBounds.Width/ColumnNum;
            _tileHeight = (float) Window.ClientBounds.Height/RowNum;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            #region Load Textures

            _textureList.Add(Content.Load<Texture2D>("Game Resources/CollisionArea"));
            _textureList.Add(Content.Load<Texture2D>("Game Resources/Backgrounds/BackGround"));
            _textureList.Add(Content.Load<Texture2D>("Game Resources/Ships/ship"));
            _textureList.Add(Content.Load<Texture2D>("Game Resources/Bullets/LongBullet"));
            _textureList.Add(Content.Load<Texture2D>("Game Resources/Bullets/RoundBullet"));
            _textureList.Add(Content.Load<Texture2D>("Game Resources/Ships/HealthBarPiece"));

            //0 = collisionTex, 1 = Background, 2 = playerShip, 3 = Long Bullet, 4 = Round Bullet, 5 = Health bar piece
            Console.WriteLine("Assets loaded");

            #endregion

            #region player SetUp

            if (_multiplayer)
            {
                _objectList.Add(new PlayerShip(_textureList[2].Width/_textureList[2].Height,
                    Window.ClientBounds.Height/ShipScale, 2, 1, "W,A,S,D,Space", Window.ClientBounds.Width,
                    Window.ClientBounds.Height, (double)_textureList[5].Width / _textureList[5].Height));
                _objectList.Add(new PlayerShip(_textureList[2].Width/_textureList[2].Height,
                    Window.ClientBounds.Height/ShipScale, 2, 2, "Up,Left,Down,Right,Enter", Window.ClientBounds.Width,
                    Window.ClientBounds.Height, (double)_textureList[5].Width / _textureList[5].Height));
            }
            else
            {
                _objectList.Add(new PlayerShip(_textureList[2].Width/_textureList[2].Height,
                    Window.ClientBounds.Height/ShipScale, 2, 0, "W,A,S,D,Space", Window.ClientBounds.Width,
                    Window.ClientBounds.Height, (double)_textureList[5].Width / _textureList[5].Height));
            }

            #endregion

            _objectList.Add(new Charger(_textureList[2].Width / _textureList[2].Height,
                Window.ClientBounds.Height / ShipScale,2,Window.ClientBounds.Height/BulletScale,50));
        }

        protected override void Update(GameTime gameTime)
        {
            if ((GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed) ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Environment.Exit(1);

            KeyboardState keyState = Keyboard.GetState();

            if (!_dead)
            {
                #region Update Objects

                for (var i = 0; i < _objectList.Count; i++)
                    if (_objectList[i] != null)
                    {
                        if (_objectList[i].GetType() == typeof(PlayerShip))
                        {
                            ((PlayerShip) _objectList[i]).Update(gameTime, keyState);
                            if (((PlayerShip) _objectList[i]).Firing)
                            {
                                _objectList.Add(new Bullet(_textureList[3].Width/_textureList[3].Height,
                                    Window.ClientBounds.Height/BulletScale, 3, Window.ClientBounds.Width/BulletScale,
                                    0,
                                    ((PlayerShip) _objectList[i]).getCenterPoint, true));
                                ((PlayerShip) _objectList[i]).Firing = false;
                            }
                        }

                        else if (_objectList[i].GetType() == typeof(Charger))
                        {
                            #region Charger Update

                            ((Charger) _objectList[i]).Update(gameTime);

                            if (((Charger) _objectList[i]).WillFire)
                            {
                                ((Charger) _objectList[i]).WillFire = false;

                                if (_multiplayer)
                                {
                                    Random whichShip = new Random();
                                    double movementAngle;

                                    if (((PlayerShip) _objectList[0]).Health != 0 &&
                                        ((PlayerShip) _objectList[1]).Health != 0)
                                    {
                                        movementAngle =
                                            ((Charger) _objectList[i]).GetAngleTwoPoints(_objectList[i].getCenterPoint,
                                                _objectList[whichShip.Next(0, 2)].getCenterPoint);

                                        _objectList.Add(new Bullet(_textureList[4].Width/_textureList[4].Height,
                                            Window.ClientBounds.Height/BulletScale, 4,
                                            (int) (((Charger) _objectList[i]).getBulVel*Math.Cos(movementAngle)*-1),
                                            (int) (((Charger) _objectList[i]).getBulVel*Math.Sin(movementAngle)*-1),
                                            ((Charger) _objectList[i]).getCenterPoint, false));
                                    }
                                    else if (((PlayerShip) _objectList[0]).Health != 0)
                                    {
                                        movementAngle =
                                            ((Charger) _objectList[i]).GetAngleTwoPoints(_objectList[i].getCenterPoint,
                                                _objectList[0].getCenterPoint);

                                        _objectList.Add(new Bullet(_textureList[4].Width/_textureList[4].Height,
                                            Window.ClientBounds.Height/BulletScale, 4,
                                            (int) (((Charger) _objectList[i]).getBulVel*Math.Cos(movementAngle)*-1),
                                            (int) (((Charger) _objectList[i]).getBulVel*Math.Sin(movementAngle)*-1),
                                            ((Charger) _objectList[i]).getCenterPoint, false));
                                    }
                                    else if (((PlayerShip) _objectList[1]).Health != 0)
                                    {
                                        movementAngle =
                                            ((Charger) _objectList[i]).GetAngleTwoPoints(_objectList[i].getCenterPoint,
                                                _objectList[1].getCenterPoint);

                                        _objectList.Add(new Bullet(_textureList[4].Width/_textureList[4].Height,
                                            Window.ClientBounds.Height/BulletScale, 4,
                                            (int) (((Charger) _objectList[i]).getBulVel*Math.Cos(movementAngle)*-1),
                                            (int) (((Charger) _objectList[i]).getBulVel*Math.Sin(movementAngle)*-1),
                                            ((Charger) _objectList[i]).getCenterPoint, false));
                                    }

                                }
                                else
                                {
                                    var movementAngle =
                                        ((Charger) _objectList[i]).GetAngleTwoPoints(_objectList[i].getCenterPoint,
                                            _objectList[0].getCenterPoint);
                                    _objectList.Add(new Bullet(_textureList[4].Width/_textureList[4].Height,
                                        Window.ClientBounds.Height/BulletScale, 4,
                                        (int) (((Charger) _objectList[i]).getBulVel*Math.Cos(movementAngle)*-1),
                                        (int) (((Charger) _objectList[i]).getBulVel*Math.Sin(movementAngle)*-1),
                                        ((Charger) _objectList[i]).getCenterPoint, false));
                                }
                                ((Charger) _objectList[i]).UpdateCurCharge();
                            }

                            if (_objectList[i].Collision)
                            {
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

                        if ((_objectList[i] != null) && !_objectList[i].Collision)
                        {
                            if (
                                !(_objectList[i].GetType() == typeof(PlayerShip) &&
                                  ((PlayerShip) _objectList[i]).Health == 0))
                            {
                                var curObjRec = _objectList[i].BoundingBox;

                                if ((curObjRec.X > 0) && (curObjRec.Y > 0) && (curObjRec.X < Window.ClientBounds.Width) &&
                                    (curObjRec.Y < Window.ClientBounds.Height))
                                    _objectCollisionList[
                                        (int) Math.Truncate(curObjRec.X/_tileWidth),
                                        (int) Math.Truncate(curObjRec.Y/_tileHeight)].Add(i);
                                if ((curObjRec.X + curObjRec.Width > 0) && (curObjRec.Y > 0) &&
                                    (curObjRec.X + curObjRec.Width < Window.ClientBounds.Width) &&
                                    (curObjRec.Y < Window.ClientBounds.Height))
                                    _objectCollisionList[
                                        (int) Math.Truncate((curObjRec.X + curObjRec.Width)/_tileWidth),
                                        (int) Math.Truncate(curObjRec.Y/_tileHeight)].Add(i);
                                if ((curObjRec.X > 0) && (curObjRec.Y + curObjRec.Height > 0) &&
                                    (curObjRec.X < Window.ClientBounds.Width) &&
                                    (curObjRec.Y + curObjRec.Height < Window.ClientBounds.Height))
                                    _objectCollisionList[
                                        (int) Math.Truncate(curObjRec.X/_tileWidth),
                                        (int) Math.Truncate((curObjRec.Y + curObjRec.Height)/_tileHeight)].Add(i);
                                if ((curObjRec.X + curObjRec.Width > 0) && (curObjRec.Y + curObjRec.Height > 0) &&
                                    (curObjRec.X + curObjRec.Width < Window.ClientBounds.Width) &&
                                    (curObjRec.Y + curObjRec.Height < Window.ClientBounds.Height))
                                    _objectCollisionList[
                                        (int) Math.Truncate((curObjRec.X + curObjRec.Width)/_tileWidth),
                                        (int) Math.Truncate((curObjRec.Y + curObjRec.Height)/_tileHeight)].Add(i);

                                if ((_objectList[i].GetType() == typeof(Bullet)) &&
                                    ((curObjRec.Y + curObjRec.Height < 0) || (curObjRec.X + curObjRec.Width < 0) ||
                                     (curObjRec.X > Window.ClientBounds.Width) ||
                                     (curObjRec.Y > Window.ClientBounds.Height)))
                                {
                                    Console.WriteLine("Object {0} left screen", i);
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
                                                Console.WriteLine("Collision at ({0},{1})", x, y);
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
                    Console.WriteLine("Collapsed ObjectList");
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
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.SkyBlue);
            _spriteBatch.Begin();

            if (!Testing)
                _spriteBatch.Draw(_textureList[1],
                    new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight),
                    Color.White);

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
               Console.WriteLine("Game Over"); 
            }

            #endregion

            base.Draw(gameTime);

            _spriteBatch.End();

            #region Calculate FPS

            if (_previousFPS != Convert.ToInt32(1/gameTime.ElapsedGameTime.TotalSeconds))
            {
                Console.WriteLine("Draw fps: {0}, No. Objects {1}",
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
    }
}
