// -----------------------------------------------------------------------
// <copyright file="InitializeMessage.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace SlotMachine.Prototype.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class InitializeMessage : BlockingMessage
    {
        public InitializeMessage(ManualResetEvent initalizationComplete)
            : base(initalizationComplete)
        {
        }

        public override void Execute()
        {
            this.Parent.Log.Info("Execute {0}", this.GetType());
            this.Parent.Initialize();
            this.MessageCompleteEvent.Set();
        }
    }
}
