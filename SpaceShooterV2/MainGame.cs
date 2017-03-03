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
        private GraphicsDeviceManager _graphics;
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
        private const int _bulletScale = 15;

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

            Console.WriteLine("Window Size: ({0},{1})",Window.ClientBounds.Width,Window.ClientBounds.Height);

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

            for (int x = 0; x < _columnNum; x++)
            {
                for (int y = 0; y < _rowNum; y++)
                {
                    ObjectCollisionList[x, y] = new List<int>();
                }
            }
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


            //0 = collisionTex, 1 = Background, 2 = playerShip
            Console.WriteLine("Assets loaded");

            #endregion

            if (_multiplayer)
            {
                ObjectList.Add(new PlayerShip(TextureList[2].Width/TextureList[2].Height,
                    Window.ClientBounds.Height/_shipScale, 2, 1, "W,A,S,D,Space", Window.ClientBounds.Width,
                    Window.ClientBounds.Height));
                ObjectList.Add(new PlayerShip(TextureList[2].Width/TextureList[2].Height,
                    Window.ClientBounds.Height/_shipScale, 2, 2, "Up,Left,Down,Right,Enter", Window.ClientBounds.Width,
                    Window.ClientBounds.Height));
            }
            else
            {
                ObjectList.Add(new PlayerShip(TextureList[2].Width/TextureList[2].Height,
                    Window.ClientBounds.Height/_shipScale, 2, 0, "W,A,S,D,Space", Window.ClientBounds.Width,
                    Window.ClientBounds.Height));
            }
            ObjectList.Add(new Bullet(TextureList[2].Width / TextureList[2].Height, Window.ClientBounds.Height / _bulletScale, 2, 0, -2, false));
            ObjectList.Add(new Bullet(TextureList[2].Width / TextureList[2].Height, Window.ClientBounds.Height / _bulletScale, 2, 0, 0, false));
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Environment.Exit(1);


            KeyboardState keyState = Keyboard.GetState();

            #region UpdateObjects

            for (int i = 0; i < ObjectList.Count; i++)
            {
                if (ObjectList[i] != null)
                {
                    if (ObjectList[i].GetType() == typeof(PlayerShip))
                    {
                        ((PlayerShip)ObjectList[i]).Update(gameTime,keyState);
                        
                    }
                    else if (ObjectList[i].GetType() == typeof(EnemyShip))
                    {

                    }
                    else if (ObjectList[i].GetType() == typeof(Bullet))
                    {
                        ObjectList[i].Update(gameTime);
                        if (ObjectList[i].Collision)
                        {
                                ObjectList[i] = null;
                        }
                    }


                    #region Updating ObjectCollisionList

                    if (ObjectList[i] != null && !ObjectList[i].Collision)
                    {
                        Rectangle curObjRec = ObjectList[i].BoundingBox;

                        if (curObjRec.X > 0 && curObjRec.Y > 0 && curObjRec.X < Window.ClientBounds.Width &&
                            curObjRec.Y < Window.ClientBounds.Height)
                        {
                            //top left
                            ObjectCollisionList[
                                (int) Math.Truncate(curObjRec.X/_tileWidth),
                                (int) Math.Truncate(curObjRec.Y/_tileHeight)].Add(i);
                        }
                        if (curObjRec.X + curObjRec.Width > 0 && curObjRec.Y > 0 &&
                            curObjRec.X + curObjRec.Width < Window.ClientBounds.Width &&
                            curObjRec.Y < Window.ClientBounds.Height)
                        {
                            //top right
                            ObjectCollisionList[
                                (int) Math.Truncate((curObjRec.X + curObjRec.Width)/_tileWidth),
                                (int) Math.Truncate(curObjRec.Y/_tileHeight)].Add(i);
                        }
                        if (curObjRec.X > 0 && curObjRec.Y + curObjRec.Height > 0 &&
                            curObjRec.X < Window.ClientBounds.Width &&
                            curObjRec.Y + curObjRec.Height < Window.ClientBounds.Height)
                        {
                            //bottom left
                            ObjectCollisionList[
                                (int) Math.Truncate(curObjRec.X/_tileWidth),
                                (int) Math.Truncate((curObjRec.Y + curObjRec.Height)/_tileHeight)].Add(i);
                        }
                        if (curObjRec.X + curObjRec.Width > 0 && curObjRec.Y + curObjRec.Height > 0 &&
                            curObjRec.X + curObjRec.Width < Window.ClientBounds.Width &&
                            curObjRec.Y + curObjRec.Height < Window.ClientBounds.Height)
                        {
                            //bottom right
                            ObjectCollisionList[
                                (int) Math.Truncate((curObjRec.X + curObjRec.Width)/_tileWidth),
                                (int) Math.Truncate((curObjRec.Y + curObjRec.Height)/_tileHeight)].Add(i);
                        }

                        if (ObjectList[i].GetType() == typeof(Bullet) &&
                            (curObjRec.Y + curObjRec.Height < 0 || curObjRec.X + curObjRec.Width < 0 ||
                             curObjRec.X > Window.ClientBounds.Width || curObjRec.Y > Window.ClientBounds.Height))
                        {
                            Console.WriteLine("Object {0} left screen", i);
                            ObjectList[i] = null;
                            Console.WriteLine("");
                        }
                    }

                    #endregion
                }
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
                                //Check collision with ships
                                foreach (var curShip in filteredList.OfType<Ship>())
                                    //Don't check if the ship has already collided with something
                                    if (!curShip.Collision)
                                        //Now do the collision check
                                        if (((curShip.GetType() == typeof(PlayerShip) && !curBullet.Owner) ||(curShip.GetType() == typeof(EnemyShip) && curBullet.Owner)) && curShip.BoundingBox.Intersects(curBullet.BoundingBox))
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
            {
                _spriteBatch.Draw(TextureList[1],
                    new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight),
                    Color.White);
            }

            #region Drawing Collision Boxes

            if (_testing)
            {
                for (int x = 0; x < _columnNum; x++)
                {
                    for (int y = 0; y < _rowNum; y++)
                    {
                        _spriteBatch.Draw(TextureList[0],
                            new Rectangle((int)(x*_tileWidth), (int)(y*_tileHeight), (int)_tileWidth,
                                (int)_tileHeight), Color.White);
                    }
                }
            }

            #endregion

            #region Drawing Objects

            foreach (GameObject curObj in ObjectList)
            {
                if (curObj != null)
                {
                    curObj.Draw(_spriteBatch, TextureList[curObj.TexNum]);
                }
            }

            #endregion

            base.Draw(gameTime);

            _spriteBatch.End();


            #region Calculate FPS

            if (_previousFPS != Convert.ToInt32(1 / gameTime.ElapsedGameTime.TotalSeconds))
            {
                Console.WriteLine("Draw fps: {0}", Convert.ToInt32(1 / gameTime.ElapsedGameTime.TotalSeconds));
                _previousFPS = Convert.ToInt32(1 / gameTime.ElapsedGameTime.TotalSeconds);
            }

            #endregion
        }

        private bool ContainsCompareTypes(Type t, List<int> intList)
        {
            foreach (int i in intList)
            {
                if (ObjectList[i].GetType() == t)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
