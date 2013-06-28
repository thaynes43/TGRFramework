// -----------------------------------------------------------------------
// <copyright file="Screen.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace SlotMachine.Prototype.Common
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// Window which will be active while playing game
    /// </summary>
    public abstract class Screen : Subsystem, IGame
    {
        #region Constructors    
        public Screen(ScreenCompleteDelegate<Screen, Screen> screenComplete, GraphicsDeviceManager graphics, string screenLogger)
            : base(screenLogger)
        {
            lock (this.SubsystemLock)
            {
                this.ScreenComplete = screenComplete;
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
        /// Handle screen complete
        /// </summary>
        public ScreenCompleteDelegate<Screen, Screen> ScreenComplete { get; set; }

        /// <summary>
        /// Physical screen manager
        /// </summary>
        protected GraphicsDeviceManager GraphicsDeviceManager { get; set; }
        #endregion

        #region Public Methods
        public override void Initialize()
        {
            this.Sprites = new List<ISprite>();
        }

        /// <summary>
        /// Load content to screen
        /// </summary>
        /// <param name="content">Access to content</param>
        public virtual void LoadContent(ContentManager content)
        {
            lock (this.SubsystemLock)
            {
                foreach (ISprite sprite in this.Sprites)
                {
                    sprite.LoadContent(content);
                }
            }
        }

        public virtual void UnloadContent(ContentManager content)
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
        public virtual void Update(ContentManager content, GameTime gameTime)
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
        public virtual void Draw(SpriteBatch theSpriteBatch)
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
