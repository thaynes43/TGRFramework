// -----------------------------------------------------------------------
// <copyright file="Message.cs" company="">
// Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TGRFramework.Prototype.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Framework message
    /// </summary>
    public class Message
    {
        public Message()
        {
        }

        public Subsystem Parent { get; set; }

        public virtual void Execute()
        {
        }
    }
}
