// -----------------------------------------------------------------------
// <copyright file="DrawMessage.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace SlotMachine.Prototype.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework.Graphics;
    using System.Threading;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class DrawMessage : BlockingMessage
    {
        public DrawMessage(SpriteBatch theSpriteBatch, ManualResetEvent drawComplete)
            : base(drawComplete)
        {
            this.SpriteBatch = theSpriteBatch;
        }

        public SpriteBatch SpriteBatch { get; set; }

        public override void Execute()
        {
            Screen screen = this.Parent as Screen;
            if (screen != null)
            {
                screen.Draw(this.SpriteBatch);
                this.MessageCompleteEvent.Set();
            }
            else
            {
                throw new ArgumentException(string.Format("{0}, invalid parent for {1}.", this.Parent.GetType(), this.GetType()));
            }
        }
    }
}
