// -----------------------------------------------------------------------
// <copyright file="ScreenCompleteMessage.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace SlotMachine.Prototype.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ScreenCompleteMessage<T, U> : Message
        where T : Screen
        where U : Screen
    {
        public ScreenCompleteMessage(object calledLock, Dictionary<Type, Subsystem> subsystems)
        {
            this.CallerLock = calledLock;
            this.Subsystems = subsystems;
        }

        private object CallerLock { get; set; }

        private Dictionary<Type, Subsystem> Subsystems { get; set; }

        public override void Execute()
        {
            StoryBoard storyboard = this.Parent as StoryBoard;

            lock (this.CallerLock)
            {
                // Clean up old screen
                Screen screen = this.Subsystems[typeof(T)] as T;
                if (screen != null)
                {
                    screen.Stop();
                    this.Subsystems.Remove(typeof(T));
                    screen = null;
                    storyboard.UnloadContent(storyboard.ContentManager);
                }

                // Load new screen
                storyboard.ActiveScreen = storyboard.FindOrCreateSubsystem(typeof(U)) as U;
                storyboard.ActiveScreen.Initialize();
                storyboard.ActiveScreen.LoadContent(storyboard.ContentManager);
            }
        }
    }
}
