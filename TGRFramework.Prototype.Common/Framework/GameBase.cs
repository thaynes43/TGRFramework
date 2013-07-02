// -----------------------------------------------------------------------
// <copyright file="Game.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TGRFramework.Prototype.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using TGRFramework.Prototype.Common;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework;

    #region Delegates
    /// <summary>
    /// Callback from screen when it is time to transition to the next screen
    /// </summary>
    public delegate void IGameCompleteDelegate<T>(Type nextGame);
    #endregion

    /// <summary>
    /// Minimum viable game requirements
    /// </summary>
    public abstract class GameBase : Subsystem, IGame
    {
        public GameBase(IGameCompleteDelegate<IGame> screenComplete, string screenLogger)
            : base(screenLogger)
        {
            this.IGameComplete = screenComplete;
        }

        /// <summary>
        /// Handle screen complete
        /// </summary>
        public IGameCompleteDelegate<IGame> IGameComplete { get; set; }

        /// <inheritdoc />
        public override void Initialize()
        {
            base.Initialize();
        }

        /// <inheritdoc />
        public override void Stop()
        {
            base.Stop();
        }

        public abstract void LoadContent(ContentManager content);

        public abstract void UnloadContent(ContentManager content);

        public abstract void Update(ContentManager content, GameTime gameTime);

        public abstract void Draw(SpriteBatch theSpriteBatch);
    }
}
