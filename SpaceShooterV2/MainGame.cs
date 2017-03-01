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

        private const bool _testing = true;
        private int _previousFPS;

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

            Console.WriteLine("Assets loaded");

            #endregion

            ObjectList.Add(new PlayerShip(TextureList[2].Width / TextureList[2].Height, Window.ClientBounds.Height / _shipScale,1, 5, 5, 1, "W,A,S,D,Space", Window.ClientBounds.X, Window.ClientBounds.Y));
            ObjectList.Add(new Bullet(TextureList[2].Width / TextureList[2].Height, Window.ClientBounds.Height / _bulletScale, 1, 0, -1));
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            KeyboardState keyState = Keyboard.GetState();

            #region UpdateObjects

            for (int i = 0; i < ObjectList.Count; i++)
            {
                if (ObjectList[i].GetType() == typeof(PlayerShip))
                {
                    ObjectList[i].Update(gameTime, keyState);
                }
                else if (ObjectList[i].GetType() == typeof(EnemyShip))
                {

                }
                else if (ObjectList[i].GetType() == typeof(Bullet))
                {
                    ObjectList[i].Update(gameTime);
                }

                if (ObjectList[i].BoundingBox.Y > 0 && ObjectList[i].BoundingBox.X > 0 && !ObjectList[i].Collision)
                {
                    Rectangle curObjRec = ObjectList[i].BoundingBox;
                    ObjectCollisionList[
                        (int)Math.Truncate(curObjRec.X/_tileWidth),
                        (int)Math.Truncate(curObjRec.Y / _tileHeight)].Add(i);
                    ObjectCollisionList[
                        (int)Math.Truncate((curObjRec.X + curObjRec.Width) / _tileWidth),
                        (int)Math.Truncate(curObjRec.Y / _tileHeight)].Add(i);
                    ObjectCollisionList[
                        (int)Math.Truncate(curObjRec.X / _tileWidth),
                        (int)Math.Truncate((curObjRec.Y + curObjRec.Height) / _tileHeight)].Add(i);
                    ObjectCollisionList[
                        (int)Math.Truncate((curObjRec.X + curObjRec.Width) / _tileWidth),
                        (int)Math.Truncate((curObjRec.Y + curObjRec.Height) / _tileHeight)].Add(i);
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
                                        if (curShip.BoundingBox.Intersects(curBullet.BoundingBox))
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
                curObj.Draw(_spriteBatch, TextureList[2]);
            }

            #endregion

            base.Draw(gameTime);

            if (_previousFPS != Convert.ToInt32(1/gameTime.ElapsedGameTime.TotalSeconds))
            {
                Console.WriteLine("Draw fps: {0}", Convert.ToInt32(1/gameTime.ElapsedGameTime.TotalSeconds));
                _previousFPS = Convert.ToInt32(1/gameTime.ElapsedGameTime.TotalSeconds);
            }

            _spriteBatch.End();
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
