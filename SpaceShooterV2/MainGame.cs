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
        private Texture2D _collisionTex;

        private const int _columnNum = 10;
        private const int _rowNum = 10;
        private float _tileWidth;
        private float _tileHeight;

        private List<GameObject>[,] ObjectCollisionList;
        private List<GameObject> ObjectList;
        private List<Texture2D> TextureList;

        private const int _shipScale = 13;
        private const int _bulletScale = 50;

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

            ObjectList.Add(new PlayerShip(50,50,1,0,0,1,""));
            ObjectList.Add(new Bullet(50,50,1,0,-1));

            #region ObjectCollisionList Set Up
            ObjectCollisionList = new List<GameObject>[_columnNum, _rowNum];

            for (int x = 0; x < _columnNum; x++)
            {
                for (int y = 0; y < _rowNum; y++)
                {
                    ObjectCollisionList[x, y] = new List<GameObject>();
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
            _collisionTex = Content.Load<Texture2D>("CollisionArea");
        }

        protected override void Update(GameTime gameTime)
        {
            Console.Clear();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            #region UpdateObjects

            foreach (Ship CurObj in ObjectList)
            {
                if (CurObj.GetType() == typeof(PlayerShip))
                {

                }
                else if (CurObj.GetType() == typeof(EnemyShip))
                {

                }
                else if (CurObj.GetType() == typeof(Bullet))
                {
                    CurObj.Update(gameTime);
                }
                else
                {

                }
                if (CurObj.BoundingBox.Y > 0 && CurObj.BoundingBox.X > 0)
                {
                    Rectangle CurObjRec = CurObj.BoundingBox;
                    ObjectCollisionList[
                        (int) Math.Truncate(CurObjRec.X/_tileWidth),
                        (int)Math.Truncate(CurObjRec.Y / _tileHeight)].Add(CurObj);
                    ObjectCollisionList[
                        (int)Math.Truncate((CurObjRec.X + CurObjRec.Width) / _tileWidth),
                        (int)Math.Truncate(CurObjRec.Y / _tileHeight)].Add(CurObj);
                    ObjectCollisionList[
                        (int)Math.Truncate(CurObjRec.X / _tileWidth),
                        (int)Math.Truncate((CurObjRec.Y + CurObjRec.Height) / _tileHeight)].Add(CurObj);
                    ObjectCollisionList[
                        (int)Math.Truncate((CurObjRec.X + CurObjRec.Width) / _tileWidth),
                        (int)Math.Truncate((CurObjRec.Y + CurObjRec.Height) / _tileHeight)].Add(CurObj);
                }
            }

            #endregion

            #region Collision Check
            for (var x = 0; x < _columnNum; x++)
            {
                for (var y = 0; y < _rowNum; y++)
                    {
                    //If there are things which can collide
                        if ((ObjectCollisionList[x, y].Count >= 2) &&
                            (ContainsCompareTypes(typeof(PlayerShip), ObjectCollisionList[x, y]) ||
                             ContainsCompareTypes(typeof(EnemyShip), ObjectCollisionList[x, y])) &&
                            ContainsCompareTypes(typeof(Bullet), ObjectCollisionList[x, y]))
                        {
                           //Do collision check
                            //Check if any bullets collide with ships
                            foreach (Bullet curBullet in ObjectCollisionList[x, y])
                            {
                                foreach (Ship curShip in ObjectCollisionList[x,y])
                                {
                                    if (curShip.BoundingBox.Intersects(curBullet.BoundingBox))
                                    {
                                        Console.WriteLine("Collision at: {0},{1},{2}", x, y, (curShip.BoundingBox.Intersects(curBullet.BoundingBox)));
                                        curShip.Collision = true;
                                    }
                                    else
                                    {
                                        curShip.Collision = false;
                                    }
                                }
                            }
                        }
                    ObjectCollisionList[x,y].Clear();
                    }
            }
            #endregion

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.SkyBlue);
            _spriteBatch.Begin();

            #region Drawing Collision Boxes

            if (_testing)
            {
                for (int x = 0; x < _columnNum; x++)
                {
                    for (int y = 0; y < _rowNum; y++)
                    {
                        _spriteBatch.Draw(_collisionTex,
                            new Rectangle((int)(x*_tileWidth), (int)(y*_tileHeight), (int)_tileWidth,
                                (int)_tileHeight), Color.White);
                    }
                }
            }

            #endregion

            #region Drawing Objects

            foreach (Ship curShip in ObjectList)
            {
                curShip.Draw(_spriteBatch, _collisionTex);
            }

            #endregion

            base.Draw(gameTime);
            Console.WriteLine("Draw fps: {0}", Convert.ToInt32(1/gameTime.ElapsedGameTime.TotalSeconds));
            _spriteBatch.End();
        }

        private bool ContainsCompareTypes(Type t, List<GameObject> objList)
        {
            foreach (object Obj in objList)
            {
                if (Obj.GetType() == t)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
