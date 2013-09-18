// -----------------------------------------------------------------------
// <copyright file="ActionMessage.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TGRFramework.Prototype.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ActionMessage : Message
    {
        public ActionMessage(Action action)
        {
            this.Action = action;
        }

        public Action Action { get; private set; }

        public override void Execute()
        {
            this.Action();
        }
    }
}
