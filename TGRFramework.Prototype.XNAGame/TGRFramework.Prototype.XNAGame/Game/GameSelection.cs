// -----------------------------------------------------------------------
// <copyright file="GameSelection.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TGRFramework.Prototype.XNAGame
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using TGRFramework.Prototype.Common;

    /// <summary>
    /// Game selection screen
    /// </summary>
    public class GameSelection : StoryBoard
    {
        public GameSelection(IGameCompleteDelegate<IGame> gameComplete, GraphicsDeviceManager graphics, ContentManager content, string logName)
            : base(gameComplete, graphics, content, logName)
        {        
            this.ActiveGame = this.FindOrCreateSubsystem(typeof(SelectionScreen)) as SelectionScreen;
        }

        /// <inheritdoc />
        protected override Subsystem CreateSubsystem(Type type)
        {
            Log.Info("  Creating {0}.", type);

            Subsystem newSubsystem = null;

            if (type == typeof(SelectionScreen))
            {
                newSubsystem = new SelectionScreen(this.OnIGameComplete<GameBase>, this.GraphicsDeviceManager, "SelectionScreen");
            }

            if (newSubsystem == null)
            {
                throw new ArgumentException(string.Format("Could not create subsystem of type {0}", type));
            }

            return newSubsystem;
        }

        /// <summary>
        /// Forward request to complete to parent
        /// </summary>
        /// <typeparam name="T">Screen to stop</typeparam>
        /// <param name="nextGame">Screen to transition to</param>
        protected override void OnIGameComplete<T>(Type nextGame)
        {
            this.IGameComplete(nextGame);
        }
    }
}
