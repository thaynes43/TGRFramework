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
using SlotMachine.Prototype.SlotSim.Appliance;
using System.Threading;
using SlotMachine.Prototype.Common;

namespace SlotMachine.Prototype.SlotSim
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class SlotSim : Microsoft.Xna.Framework.Game
    {
        public SlotSim()
        {
            this.GraphicsDeviceManager = new GraphicsDeviceManager(this);
            this.GraphicsDeviceManager.PreferredBackBufferHeight = 600;
            this.GraphicsDeviceManager.PreferredBackBufferWidth = 800;
            this.IsMouseVisible = true;
            Content.RootDirectory = "Content";

            // TODO_HIGH configure what is ran?

            this.FrameworkGame = new MachineSimulator(this.GraphicsDeviceManager, this.Content);

            //this.FrameworkGame = new MachineScreen(this.OnScreenComplete<Screen, Screen>, this.GraphicsDeviceManager);
        }

        public IGame FrameworkGame { get; set; }

        public SpriteBatch SpriteBatch { get; set; }

        public GraphicsDeviceManager GraphicsDeviceManager { get; set; }

        /// <summary>
        /// TODO_MEDIUM see if this works - enable resize
        /// </summary>
        public Matrix SpriteScale { get; set; }
         
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            this.FrameworkGame.Initialize();
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game (by XNA) and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            this.SpriteBatch = new SpriteBatch(GraphicsDevice);
            this.FrameworkGame.LoadContent(this.Content);

            float scale = this.GraphicsDevice.Viewport.Width / 800f;
            this.SpriteScale = Matrix.CreateScale(scale, scale, 1);
        }

        /// <summary>
        /// UnloadContent will be called once per game (by XNA) and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            this.FrameworkGame.UnloadContent(this.Content);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (Keyboard.GetState().IsKeyDown(Keys.Q))
            {
                this.FrameworkGame.Stop();
                this.Exit();
            }

            this.FrameworkGame.Update(this.Content, gameTime);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            this.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, this.SpriteScale);
            this.FrameworkGame.Draw(this.SpriteBatch);
            this.SpriteBatch.End();
            base.Draw(gameTime);
        }

        /// <summary>
        /// Handle request for finish if screen is ran as standalone game
        /// </summary>
        private void OnScreenComplete<T, U>()
            where T : Screen
            where U : Screen
        {
            // TODO_HIGH do we want to manage dependencies differently - Top down model (stop)? 
            this.FrameworkGame.Stop();
            (this.FrameworkGame as MachineScreen).DataStore.Stop();
            this.Exit();
        }
    }
}
