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

        private const bool _testing = false;
        private int _previousFPS = 60;

        private const int _columnNum = 10;
        private const int _rowNum = 10;
        private float _tileWidth;
        private float _tileHeight;

        private List<int>[,] ObjectCollisionList;
        private List<GameObject> ObjectList;
        private List<Texture2D> TextureList;

        private const int _shipScale = 11;
        private const int _bulletScale = 60;

        public MainGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            #region Force FullScreen

            if (!_testing)
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
            ObjectList = new List<GameObject>();
            TextureList = new List<Texture2D>();

            #region ObjectCollisionList Set Up

            ObjectCollisionList = new List<int>[_columnNum, _rowNum];

            for (var x = 0; x < _columnNum; x++)
                for (var y = 0; y < _rowNum; y++)
                    ObjectCollisionList[x, y] = new List<int>();

            #endregion

            _tileWidth = (float) Window.ClientBounds.Width/_columnNum;
            _tileHeight = (float) Window.ClientBounds.Height/_rowNum;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            #region Load Textures

            TextureList.Add(Content.Load<Texture2D>("Game Resources/CollisionArea"));
            TextureList.Add(Content.Load<Texture2D>("Game Resources/Backgrounds/BackGround"));
            TextureList.Add(Content.Load<Texture2D>("Game Resources/Ships/ship"));
            TextureList.Add(Content.Load<Texture2D>("Game Resources/Bullets/Bullet"));

            //0 = collisionTex, 1 = Background, 2 = playerShip
            Console.WriteLine("Assets loaded");

            #endregion

            #region player SetUp

            if (_multiplayer)
            {
                ObjectList.Add(new PlayerShip(TextureList[2].Width/TextureList[2].Height,
                    Window.ClientBounds.Height/_shipScale, 2, 1, "W,A,S,D,Space", Window.ClientBounds.Width,
                    Window.ClientBounds.Height));
                ObjectList.Add(new PlayerShip(TextureList[2].Width/TextureList[2].Height,
                    Window.ClientBounds.Height/_shipScale, 2, 2, "Up,Left,Down,Right,Enter", Window.ClientBounds.Width,
                    Window.ClientBounds.Height));
            }
            ObjectList.Add(new PlayerShip(TextureList[2].Width/TextureList[2].Height,
                Window.ClientBounds.Height/_shipScale, 2, 0, "W,A,S,D,Space", Window.ClientBounds.Width,
                Window.ClientBounds.Height));

            #endregion

            ObjectList.Add(new Charger(TextureList[2].Width / TextureList[2].Height,
                Window.ClientBounds.Height / _shipScale,2,Window.ClientBounds.Height/_bulletScale,50));
        }

        protected override void Update(GameTime gameTime)
        {
            if ((GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed) ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Environment.Exit(1);


            var keyState = Keyboard.GetState();

            #region UpdateObjects

            for (var i = 0; i < ObjectList.Count; i++)
                if (ObjectList[i] != null)
                {
                    if (ObjectList[i].GetType() == typeof(PlayerShip))
                    {
                        ((PlayerShip) ObjectList[i]).Update(gameTime, keyState);
                        if (((PlayerShip) ObjectList[i]).Firing)
                        {
                            ObjectList.Add(new Bullet(TextureList[3].Width/TextureList[3].Height,
                                Window.ClientBounds.Height/_bulletScale, 3, Window.ClientBounds.Width/_bulletScale, 0,
                                ((PlayerShip) ObjectList[i]).getCenterPoint, true));
                            ((PlayerShip) ObjectList[i]).Firing = false;
                        }
                    }

                    else if (ObjectList[i].GetType() == typeof(Charger))
                    {
                        #region Charger Update
                        ((Charger)ObjectList[i]).Update(gameTime);
                        int curState = ((Charger) ObjectList[i]).getState();
                        if (curState == 0)
                        {
                            List<int> intList = ((Charger) ObjectList[i]).GetBulletReference;
                            int _bulXVel = ((Charger) ObjectList[i]).getBulVel;
                            Vector2 _playerPosition;
                            if (_multiplayer)
                            {
                                Random x = new Random();
                                _playerPosition = ObjectList[x.Next(0,1)].getCenterPoint;
                            }
                            else
                            {
                                _playerPosition = ObjectList[0].getCenterPoint;
                            }
                            foreach (int curRef in intList)
                            {
                                if (ObjectList[curRef] != null && ObjectList[curRef].GetType() == typeof(Bullet))
                                {
                                    double angle = ((Charger) ObjectList[i]).GetAngleTwoPoints(_playerPosition, ObjectList[curRef].getCenterPoint);
                                    ((Bullet)ObjectList[curRef]).xVel = (int)(_bulXVel * Math.Cos(angle));
                                    ((Bullet)ObjectList[curRef]).yVel = (int)(_bulXVel * Math.Sin(angle));
                                }
                            }
                        }
                        else if (curState == 1)
                        {
                            ObjectList.Add(new Bullet(TextureList[3].Width / TextureList[3].Height,
                                Window.ClientBounds.Height / _bulletScale, 3, 0, -1, ObjectList[i].getCenterPoint, false));
                            ((Charger) ObjectList[i]).AddBulletReference(ObjectList.Count - 1);
                        }
                        #endregion
                    }
                    else if (ObjectList[i].GetType() == typeof(Bullet))
                    {
                        ObjectList[i].Update(gameTime);
                        if (ObjectList[i].Collision)
                            ObjectList[i] = null;
                    }

                    #region Updating ObjectCollisionList

                    if ((ObjectList[i] != null) && !ObjectList[i].Collision)
                    {
                        var curObjRec = ObjectList[i].BoundingBox;

                        if ((curObjRec.X > 0) && (curObjRec.Y > 0) && (curObjRec.X < Window.ClientBounds.Width) &&
                            (curObjRec.Y < Window.ClientBounds.Height))
                            ObjectCollisionList[
                                (int) Math.Truncate(curObjRec.X/_tileWidth),
                                (int) Math.Truncate(curObjRec.Y/_tileHeight)].Add(i);
                        if ((curObjRec.X + curObjRec.Width > 0) && (curObjRec.Y > 0) &&
                            (curObjRec.X + curObjRec.Width < Window.ClientBounds.Width) &&
                            (curObjRec.Y < Window.ClientBounds.Height))
                            ObjectCollisionList[
                                (int) Math.Truncate((curObjRec.X + curObjRec.Width)/_tileWidth),
                                (int) Math.Truncate(curObjRec.Y/_tileHeight)].Add(i);
                        if ((curObjRec.X > 0) && (curObjRec.Y + curObjRec.Height > 0) &&
                            (curObjRec.X < Window.ClientBounds.Width) &&
                            (curObjRec.Y + curObjRec.Height < Window.ClientBounds.Height))
                            ObjectCollisionList[
                                (int) Math.Truncate(curObjRec.X/_tileWidth),
                                (int) Math.Truncate((curObjRec.Y + curObjRec.Height)/_tileHeight)].Add(i);
                        if ((curObjRec.X + curObjRec.Width > 0) && (curObjRec.Y + curObjRec.Height > 0) &&
                            (curObjRec.X + curObjRec.Width < Window.ClientBounds.Width) &&
                            (curObjRec.Y + curObjRec.Height < Window.ClientBounds.Height))
                            ObjectCollisionList[
                                (int) Math.Truncate((curObjRec.X + curObjRec.Width)/_tileWidth),
                                (int) Math.Truncate((curObjRec.Y + curObjRec.Height)/_tileHeight)].Add(i);

                        if ((ObjectList[i].GetType() == typeof(Bullet)) &&
                            ((curObjRec.Y + curObjRec.Height < 0) || (curObjRec.X + curObjRec.Width < 0) ||
                             (curObjRec.X > Window.ClientBounds.Width) || (curObjRec.Y > Window.ClientBounds.Height)))
                        {
                            Console.WriteLine("Object {0} left screen", i);
                            ObjectList[i] = null;
                        }
                    }

                    #endregion
                }

            #endregion

            #region Collision Check

            for (var x = 0; x < _columnNum; x++)
                for (var y = 0; y < _rowNum; y++)
                {
                    //If there are things which can collide
                    if ((ObjectCollisionList[x, y].Count >= 2) &&
                        (ContainsCompareTypes(typeof(PlayerShip), ObjectCollisionList[x, y]) ||
                         ContainsCompareTypes(typeof(EnemyShip), ObjectCollisionList[x, y])) &&
                        ContainsCompareTypes(typeof(Bullet), ObjectCollisionList[x, y]))
                    {
                        //Do collision check
                        //Get the objects which are contained within that square
                        var filteredList = new List<GameObject>();
                        foreach (var i in ObjectCollisionList[x, y])
                            filteredList.Add(ObjectList[i]);

                        //Check if any bullets collide with ships
                        foreach (var curBullet in filteredList.OfType<Bullet>())
                            //Dont check if the bullet has already collided with something
                            if (!curBullet.Collision)
                                foreach (var curShip in filteredList.OfType<Ship>())
                                    if (!curShip.Collision)
                                        //Now do the collision check
                                        if ((((curShip.GetType() == typeof(PlayerShip)) && !curBullet.Owner) ||
                                             ((curShip.GetType() == typeof(EnemyShip)) && curBullet.Owner)) &&
                                            curShip.BoundingBox.Intersects(curBullet.BoundingBox))
                                        {
                                            Console.WriteLine("Collision at ({0},{1})", x, y);
                                            curShip.Collision = true;
                                            curBullet.Collision = true;
                                        }
                                        else
                                        {
                                            curShip.Collision = false;
                                            curBullet.Collision = false;
                                        }
                        filteredList.Clear();
                    }
                    ObjectCollisionList[x, y].Clear();
                }

            #endregion

            #region Deleted null from ObjectList

            if (!ObjectList.OfType<EnemyShip>().Any() && ObjectList.Contains(null))
            {
                ObjectList.RemoveAll(item => item == null);
                Console.WriteLine("Collapsed ObjectList");
            }

            #endregion

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.SkyBlue);
            _spriteBatch.Begin();

            if (!_testing)
                _spriteBatch.Draw(TextureList[1],
                    new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight),
                    Color.White);

            #region Drawing Collision Boxes

            if (_testing)
                for (var x = 0; x < _columnNum; x++)
                    for (var y = 0; y < _rowNum; y++)
                        _spriteBatch.Draw(TextureList[0],
                            new Rectangle((int) (x*_tileWidth), (int) (y*_tileHeight), (int) _tileWidth,
                                (int) _tileHeight), Color.White);

            #endregion

            #region Drawing Objects

            foreach (var curObj in ObjectList)
                if (curObj != null)
                    curObj.Draw(_spriteBatch, TextureList[curObj.TexNum]);

            #endregion

            base.Draw(gameTime);

            _spriteBatch.End();

            #region Calculate FPS

            if (_previousFPS != Convert.ToInt32(1/gameTime.ElapsedGameTime.TotalSeconds))
            {
                Console.WriteLine("Draw fps: {0}, No. Objects {1}",
                    Convert.ToInt32(1/gameTime.ElapsedGameTime.TotalSeconds), ObjectList.Count);
                _previousFPS = Convert.ToInt32(1/gameTime.ElapsedGameTime.TotalSeconds);
                //Aprox 500 objects without fps drop
            }

            #endregion
        }

        private bool ContainsCompareTypes(Type t, List<int> intList)
        {
            foreach (var i in intList)
                if (ObjectList[i].GetType() == t)
                    return true;
            return false;
        }
    }
}
