// -----------------------------------------------------------------------
// <copyright file="ScreenCompleteMessage.cs" company="">
// Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TGRFramework.Prototype.Common
{
    using System;
    using System.Collections.Generic;
    using TGRFramework.Prototype.Tools;

    /// <summary>
    /// Update summary.
    /// </summary>
    public class IGameCompleteMessage<T> : Message
        where T : GameBase
    {
        public IGameCompleteMessage(Type nextType, object calledLock, Dictionary<Type, Subsystem> subsystems, ILogTool log)
        {
            this.CallerLock = calledLock;
            this.Subsystems = subsystems;
            this.NextType = nextType;
            this.Log = log;
        }

        private object CallerLock { get; set; }

        private Dictionary<Type, Subsystem> Subsystems { get; set; }

        private IGame test { get; set; }

        private Type NextType { get; set; }

        private ILogTool Log { get; set; }

        public override void Execute()
        {
            StoryBoard storyboard = this.Parent as StoryBoard;

            lock (this.CallerLock)
            {
                // Clean up old screen
                IGame game = this.Subsystems[typeof(T)] as T;
                if (game != null)
                {
                    Log.Info("Stopping {0}.", game.GetType());
                    game.Stop();
                    this.Subsystems.Remove(typeof(T));
                    game = null;
                    storyboard.UnloadContent(storyboard.ContentManager);
                }

                // Load new screen
                storyboard.ActiveGame = Convert.ChangeType(storyboard.FindOrCreateSubsystem(NextType), NextType) as IGame;
                storyboard.Initialize();
                storyboard.LoadContent(storyboard.ContentManager);
            }
        }
    }
}
