// -----------------------------------------------------------------------
// <copyright file="BlockingMessage.cs" company="">
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
    public class BlockingMessage : Message
    {
        public BlockingMessage(ManualResetEvent messageCompleteEvent)
        {
            this.MessageCompleteEvent = messageCompleteEvent;
        }

        public ManualResetEvent MessageCompleteEvent { get; set; }
    }
}
