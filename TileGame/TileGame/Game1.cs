using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace TileGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        KeyboardState currentState;
        SpriteFont gameFont;

        int cameraPositionX;
        int cameraPositionY;
        int cameraSpeed = 3;

        Texture2D character;

        public Matrix Transform { get; private set; }
        public Viewport Viewport { get; private set; }

        List<Texture2D> tiles = new List<Texture2D>();
        static int tileWidht = 64;
        static int tileHeight = 64;
        int[,] map = {
                         {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                         {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                         {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                         {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                         {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                         {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                         {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                         {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                         {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                         {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                         {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                         {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}
                     };

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.graphics.PreferredBackBufferWidth = 1920;
            this.graphics.PreferredBackBufferHeight = 1080;
            this.graphics.IsFullScreen = true;  
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();

        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            tiles.Add(Content.Load<Texture2D>("grasstile"));
            tiles.Add(Content.Load<Texture2D>("snowtile"));
            character = Content.Load<Texture2D>("char");
            gameFont = Content.Load<SpriteFont>("gameFont");
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            currentState = Keyboard.GetState();
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();


            if (currentState.IsKeyDown(Keys.Up) || currentState.IsKeyDown(Keys.Z))
                ScrollUp();
            if (currentState.IsKeyDown(Keys.Left) || currentState.IsKeyDown(Keys.Q) || currentState.IsKeyDown(Keys.A))
                Scrollleft();
            if (currentState.IsKeyDown(Keys.Down) || currentState.IsKeyDown(Keys.S))
                ScrollDown();
            if (currentState.IsKeyDown(Keys.Right) || currentState.IsKeyDown(Keys.D))
                ScrollRight();
            if (currentState.IsKeyDown(Keys.LeftShift))
                cameraSpeed = 5;
            else if(currentState.IsKeyUp(Keys.LeftShift))
                cameraSpeed = 3;
            LockCamera();

            Transform = Matrix.CreateTranslation(character.GraphicsDevice.Viewport.X, character.GraphicsDevice.Viewport.Y, 0) * Matrix.CreateTranslation(Viewport.Width / 2, Viewport.Height / 2, 0);

            base.Update(gameTime);
        }

        private void ScrollUp()
        {
            cameraPositionY -= cameraSpeed;
        }

        private void ScrollDown()
        {
            cameraPositionY += cameraSpeed;
        }

        private void Scrollleft()
        {
            cameraPositionX -= cameraSpeed;
        }

        private void ScrollRight()
        {
            cameraPositionX += cameraSpeed;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            MouseState mouseState = Mouse.GetState();
            Vector2 mousePosition = new Vector2(mouseState.X, mouseState.Y);
            DrawMap();
            spriteBatch.Draw(character, new Rectangle((GraphicsDevice.Viewport.Bounds.Width /2) - character.Width /2 , (GraphicsDevice.Viewport.Bounds.Height /2) - character.Height /2, 64, 64), Color.White);
            spriteBatch.DrawString(gameFont, "Coords: x:" + cameraPositionX + " y: " + cameraPositionY, new Vector2(10, 10), Color.White);
            base.Draw(gameTime);
            spriteBatch.End();
        }

        private void DrawMap()
        {
            for (int y = 0; y < map.GetLength(0); y++)
            {
                for(int x = 0; x < map.GetLength(1); x++)
                {
                    spriteBatch.Draw(tiles[map[y,x]], new Rectangle(x * tileWidht - cameraPositionX, y * tileHeight - cameraPositionY, tileWidht, tileHeight), Color.White);
                }
            }
        }

        private void LockCamera()
        {
            cameraPositionX = (int)MathHelper.Clamp(cameraPositionX, -GraphicsDevice.Viewport.Width/2 + character.Width /2, tileWidht * map.GetLength(1) - GraphicsDevice.Viewport.Width /2 - character.Width/2);
            cameraPositionY = (int)MathHelper.Clamp(cameraPositionY, -GraphicsDevice.Viewport.Height/2 + character.Height /2, tileHeight * map.GetLength(0) - GraphicsDevice.Viewport.Height /2 - character.Height/2);
        }

        private Vector2 TileToScreen(Vector2 pTileXY)
        {
            // this does the reverse of ScreenToTile

            Vector2 tileSize = new Vector2(64,64);
            Vector2 mapXY = pTileXY * tileSize;       

            Vector2 mapOffset = new Vector2(0,0);

            // you'll have to do this the opposite way from ScreenToTile()
            Vector2 screenXY = mapXY - mapOffset;       

            return screenXY;
        }  
    }
}
