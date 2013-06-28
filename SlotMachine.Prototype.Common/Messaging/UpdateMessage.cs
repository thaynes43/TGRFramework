// -----------------------------------------------------------------------
// <copyright file="UpdateMessage.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace SlotMachine.Prototype.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework.Content;
    using System.Threading;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class UpdateMessage : BlockingMessage
    {
        public UpdateMessage(ContentManager content, GameTime gameTime, ManualResetEvent complete)
            : base(complete)
        {
            this.ContentManager = content;
            this.GameTime = gameTime;
        }

        public ContentManager ContentManager { get; set; }

        public GameTime GameTime { get; set; }

        public override void Execute()
        {
            Screen screen = this.Parent as Screen;
            if (screen != null)
            {
                screen.Update(this.ContentManager, this.GameTime);
                this.MessageCompleteEvent.Set();
            }
            else
            {
                throw new ArgumentException(string.Format("{0}, invalid parent for {1}.", this.Parent.GetType(), this.GetType()));
            }
        }
    }
}
