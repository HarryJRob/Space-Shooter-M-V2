using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceShooterV2
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;

        private const int _columnNum = 10;
        private const int _rowNum = 10;
        public float TileWidth;
        public float TileHeight;

        public List<object>[,] ObjectCollisionList;
        public List<object> ObjectList;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            #region FullScreen
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            Window.AllowUserResizing = false;
            _graphics.ToggleFullScreen();
            _graphics.ApplyChanges();

            #endregion
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

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
            TileWidth = (float) Window.ClientBounds.Width/_columnNum;
            TileHeight = (float) Window.ClientBounds.Height/_rowNum;
            #endregion
            base.Initialize();

        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

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
            for (int x = 0; x < _columnNum; x++)
            {
                for (int y = 0; y < _rowNum; y++)
                    {
                        if ((ObjectCollisionList[x, y].Count >= 2) &&
                            (ContainsCompareTypes(typeof(PlayerShip), ObjectCollisionList[x, y]) ||
                             ContainsCompareTypes(typeof(EnemyShip), ObjectCollisionList[x, y])) &&
                            ContainsCompareTypes(typeof(Bullet), ObjectCollisionList[x, y]))
                        {
                           System.Diagnostics.Debug.WriteLine("{0},{1}", x,y);
                           //Do collision check
                        }
                    }
            }
            #endregion
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        private bool ContainsCompareTypes(Type t, List<object> ObjList)
        {
            foreach (object Obj in ObjList)
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
