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
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;

        private const bool _testing = true;
        private Texture2D _collisionTex;

        private const int _columnNum = 10;
        private const int _rowNum = 10;
        public float TileWidth;
        public float TileHeight;

        public List<object>[,] ObjectCollisionList;
        public List<object> ObjectList;

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

            #endregion

            _graphics.ApplyChanges();
        }

        protected override void Initialize()
        {

            ObjectList = new List<object>();

            #region ObjectCollisionList Set Up
            ObjectCollisionList = new List<object>[_columnNum, _rowNum];

            for (int x = 0; x < _columnNum; x++)
            {
                for (int y = 0; y < _rowNum; y++)
                {
                    ObjectCollisionList[x,y] = new List<object>();
                }
            }
            #endregion

            TileWidth = (float) Window.ClientBounds.Width/_columnNum;
            TileHeight = (float) Window.ClientBounds.Height/_rowNum;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _collisionTex = Content.Load<Texture2D>("CollisionArea");
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            Console.Clear();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            #region UpdateObjects

            foreach (object CurObj in ObjectList)
            {
                if (CurObj.GetType() == typeof(PlayerShip))
                {
                    
                }
                else if (CurObj.GetType() == typeof(EnemyShip))
                {
                    
                }
                else if (CurObj.GetType() == typeof(Bullet))
                {

                }
                else
                {

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
                           Console.WriteLine("Collision at: {0},{1}",x ,y );
                           //Do collision check
                            var filteredBullets = ObjectCollisionList[x, y].OfType<Bullet>();
                            //Check if any bullets collide with ships
                            foreach (Bullet b in filteredBullets)
                            {
                                
                            }
                        }
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
                            new Rectangle((int) (x*TileWidth), (int) (y*TileHeight), (int) (TileWidth),
                                (int) (TileHeight)), Color.White);
                    }
                }
            }

            #endregion

            base.Draw(gameTime);
            Console.WriteLine("Draw fps: {0}", Convert.ToInt32(1/gameTime.ElapsedGameTime.TotalSeconds));
            _spriteBatch.End();
        }

        private bool ContainsCompareTypes(Type t, List<object>objList)
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
