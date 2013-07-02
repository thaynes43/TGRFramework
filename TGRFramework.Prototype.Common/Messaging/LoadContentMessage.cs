// -----------------------------------------------------------------------
// <copyright file="LoadContentMessage.cs" company="">
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

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class LoadContentMessage : BlockingMessage
    {
        public LoadContentMessage(ContentManager content, ManualResetEvent loadContentComplete)
            : base(loadContentComplete)
        {
            this.ContentManager = content;
        }

        public ContentManager ContentManager { get; set; }

        public override void Execute()
        {
            this.Parent.Log.Info("Execute {0}", this.GetType());
            Screen screen = this.Parent as Screen;
            if (screen != null)
            {
                screen.LoadContent(this.ContentManager);
                this.MessageCompleteEvent.Set();
            }
            else
            {
                throw new ArgumentException(string.Format("{0}, invalid parent for {1}.", this.Parent.GetType(), this.GetType()));
            }
        }
    }
}
