﻿// -----------------------------------------------------------------------
// <copyright file="Message.cs" company="">
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
