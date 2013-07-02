// -----------------------------------------------------------------------
// <copyright file="Screen.cs" company="">
// Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TGRFramework.Prototype.Common
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// Window which will be active while playing game
    /// </summary>
    public abstract class Screen : GameBase
    {
        #region Constructors    
        public Screen(IGameCompleteDelegate<IGame> gameComplete, GraphicsDeviceManager graphics, string screenLogger)
            : base(gameComplete, screenLogger)
        {
            lock (this.SubsystemLock)
            {
                this.GraphicsDeviceManager = graphics;
                this.Sprites = new List<ISprite>();
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Sprites needed for this screen
        /// </summary>
        public List<ISprite> Sprites { get; protected set; }

        /// <summary>
        /// Physical screen manager
        /// </summary>
        protected GraphicsDeviceManager GraphicsDeviceManager { get; set; }
        #endregion

        #region Public Methods
        /// <inheritdoc />
        public override void Initialize()
        {
            this.Sprites = new List<ISprite>();
        }

        /// <summary>
        /// Load content to screen
        /// </summary>
        /// <param name="content">Access to content</param>
        public override void LoadContent(ContentManager content)
        {
            lock (this.SubsystemLock)
            {
                foreach (ISprite sprite in this.Sprites)
                {
                    sprite.LoadContent(content);
                }
            }
        }

        /// <summary>
        /// Unloads all assets previously loaded 
        /// </summary>
        /// <param name="content">ContentManager</param>
        public override void UnloadContent(ContentManager content)
        {
            lock (this.SubsystemLock)
            {
                content.Unload();
            }
        }

        /// <summary>
        /// Update sprites
        /// </summary>
        /// <param name="content">Content manager to allow loading new content on update</param>
        /// <param name="gameTime">Time elapsed during runtime</param>
        public override void Update(ContentManager content, GameTime gameTime)
        {
            lock (this.SubsystemLock)
            {
                foreach (ISprite sprite in this.Sprites)
                {
                    sprite.Update(content, gameTime);
                }
            }
        }

        /// <summary>
        /// Write sprites to screen
        /// </summary>
        /// <param name="theSpriteBatch">spritebatch from XNA framework</param>
        public override void Draw(SpriteBatch theSpriteBatch)
        {
            lock (this.SubsystemLock)
            {
                foreach (ISprite sprite in this.Sprites)
                {
                    sprite.Draw(theSpriteBatch);
                }
            }
        }
        #endregion
    }
}
