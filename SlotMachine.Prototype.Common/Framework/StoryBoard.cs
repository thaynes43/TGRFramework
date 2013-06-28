// -----------------------------------------------------------------------
// <copyright file="StoryManager.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace SlotMachine.Prototype.Common
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    #region Delegates
    /// <summary>
    /// Callback from screen when it is time to transition to the next screen
    /// </summary>
    public delegate void ScreenCompleteDelegate<T, U>();
    #endregion

    /// <summary>
    /// Manager of all subsystems in game
    /// </summary>
    public abstract class StoryBoard : Subsystem, IGame
    {
        #region Fields
        #endregion

        #region Constructors
        public StoryBoard(GraphicsDeviceManager graphics, ContentManager content)
            : base("Main")
        {
            this.GraphicsDeviceManager = graphics;
            this.ContentManager = content;
            this.Subsystems = new Dictionary<Type, Subsystem>();

            ThreadPool.QueueUserWorkItem( t => { this.Run(); } );
        }
        #endregion

        #region Properties
        /// <summary>
        /// Physical screen
        /// </summary>
        public GraphicsDeviceManager GraphicsDeviceManager { get; set; }

        /// <summary>
        /// Manage physical content
        /// </summary>
        public ContentManager ContentManager { get; set; }

        /// <summary>
        /// Active game screen, avoid managing assets on other screens
        /// </summary>
        public Screen ActiveScreen { get; set; }

        /// <summary>
        /// Collection of subsystems
        /// </summary>
        private Dictionary<Type, Subsystem> Subsystems { get; set; }
        #endregion

        #region Public Methods
        /// <summary>
        /// Manage subsystem references 
        /// </summary>
        /// <param name="type">Subsystem type</param>
        /// <returns>Subsystem</returns>
        public Subsystem FindOrCreateSubsystem(Type type)
        {
            lock (this.SubsystemLock)
            {
                if (!this.Subsystems.ContainsKey(type))
                {
                    Subsystem newSubsystem = this.CreateSubsystem(type);
                    this.Subsystems.Add(type, newSubsystem);
                    Thread thread = new Thread(() =>
                    {
                        newSubsystem.Run();
                    });
                    thread.Name = type.ToString();
                    thread.Start();
                }

                return this.Subsystems[type];
            }
        }

        /// <summary>
        /// Initialize each subsystem
        /// </summary>
        public override void Initialize()
        {
            lock (this.SubsystemLock)
            {
                foreach (Subsystem subsystem in this.Subsystems.Values)
                {
                    subsystem.Initialize();
                }
            }
        }

        /// <summary>
        /// Stop each subsystem
        /// </summary>
        /// <param name="shutdownCompleteEvent">On subsystem shutdown complete</param>
        public override void Stop()
        {
            lock (this.SubsystemLock)
            {
                foreach (Subsystem subsystem in this.Subsystems.Values)
                {
                    subsystem.Stop();
                }

                base.Stop();
            }
        }

        /// <summary>
        /// Load content of each subsystem
        /// </summary>
        /// <param name="content">Access to content</param>
        public virtual void LoadContent(ContentManager content)
        {
            lock (this.SubsystemLock)
            {
                foreach (Subsystem subsystem in this.Subsystems.Values)
                {
                    Screen screen = subsystem as Screen;
                    if (screen != null)
                    {
                        screen.LoadContent(content);
                    }
                }
            }
        }

        /// <summary>
        /// Free resources held by content manager
        /// </summary>
        public virtual void UnloadContent(ContentManager content)
        {
            lock (this.SubsystemLock)
            {
                content.Unload();
            }
        }

        /// <summary>
        /// Update just the active screen
        /// </summary>
        /// <param name="content"></param>
        /// <param name="gameTime"></param>
        public virtual void Update(ContentManager content, GameTime gameTime)
        {
            lock (this.SubsystemLock)
            {
                this.ActiveScreen.Update(content, gameTime);
            }
        }

        /// <summary>
        /// Draw the active screen
        /// </summary>
        /// <param name="theSpriteBatch"></param>
        public virtual void Draw(SpriteBatch theSpriteBatch)
        {
            lock (this.SubsystemLock)
            {
                this.ActiveScreen.Draw(theSpriteBatch);
            }
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Create various subsystems used by game
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected virtual Subsystem CreateSubsystem(Type type)
        {
            return null;
        }

        /// <summary>
        /// Default screen transition handler
        /// </summary>
        /// <typeparam name="T">Sender</typeparam>
        /// <typeparam name="U">Next screen</typeparam>
        protected void OnScreenComplete<T, U>()
            where T : Screen
            where U : Screen
        {
            lock (this.SubsystemLock)
            {
                // Push this work to queue to execute outside the Update
                this.AddMessage(new ScreenCompleteMessage<T,U>(this.SubsystemLock, this.Subsystems));
            }
        }
        #endregion
    }
}