// -----------------------------------------------------------------------
// <copyright file="StoryManager.cs" company="">
// Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TGRFramework.Prototype.Common
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// Manager of all subsystems in game
    /// </summary>
    public abstract class StoryBoard : GameBase
    {
        #region Fields
        #endregion

        #region Constructors
        /// <summary>
        /// Must set active game in constructor TODO Make this obvious
        /// </summary>
        public StoryBoard(IGameCompleteDelegate<IGame> gameComplete, GraphicsDeviceManager graphics, ContentManager content, string logName)
            : base(gameComplete, logName)
        {
            this.GraphicsDeviceManager = graphics;
            this.ContentManager = content;
            this.Subsystems = new Dictionary<Type, Subsystem>();
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
        public IGame ActiveGame { get; set; }

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
            Log.Info("FindOrCreateSubsystem {0}.", type);
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
                    if (!subsystem.Initialized)
                    {
                        Log.Info("Initializing child subsystem {0}.", subsystem.GetType());
                        subsystem.Initialize();
                        subsystem.Initialized = true;
                    }
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
        public override void LoadContent(ContentManager content)
        {
            lock (this.SubsystemLock)
            {
                foreach (Subsystem subsystem in this.Subsystems.Values)
                {
                    IGame game = subsystem as IGame;
                    if (game != null)
                    {
                        game.LoadContent(content);
                    }
                }
            }
        }

        /// <summary>
        /// Free resources held by content manager
        /// </summary>
        public override void UnloadContent(ContentManager content)
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
        public override void Update(ContentManager content, GameTime gameTime)
        {
            lock (this.SubsystemLock)
            {
                this.ActiveGame.Update(content, gameTime);
            }
        }

        /// <summary>
        /// Draw the active screen
        /// </summary>
        /// <param name="theSpriteBatch"></param>
        public override void Draw(SpriteBatch theSpriteBatch)
        {
            lock (this.SubsystemLock)
            {
                this.ActiveGame.Draw(theSpriteBatch);
            }
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Create various subsystems used by game
        /// </summary>
        /// <param name="type">type to create</param>
        /// <returns>new Subsystem</returns>
        protected virtual Subsystem CreateSubsystem(Type type)
        {
            return null;
        }

        /// <summary>
        /// Default screen transition handler
        /// </summary>
        /// <typeparam name="T">Sender</typeparam>
        /// <typeparam name="U">Next screen</typeparam>
        protected virtual void OnIGameComplete<T>(Type nextGame)
            where T : GameBase
        {
            lock (this.SubsystemLock)
            {
                // Push this work to queue to execute outside the Update
                this.AddMessage(new IGameCompleteMessage<T>(nextGame, this.SubsystemLock, this.Subsystems, this.Log));
            }
        }
        #endregion
    }
}