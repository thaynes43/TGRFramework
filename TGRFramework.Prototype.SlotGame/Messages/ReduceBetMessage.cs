// -----------------------------------------------------------------------
// <copyright file="ReduceBetMessage.cs" company="">
// Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TGRFramework.Prototype.SlotGame
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using TGRFramework.Prototype.Common;
    using TGRFramework.Prototype.Tools;

    /// <inheritdoc />
    public class ReduceBetMessage : Message
    {
        public ReduceBetMessage(ILogTool log = null)
        {

            this.Log = log;
        }

        public ILogTool Log { get; set; }

        public override void Execute()
        {
            if (this.Log != null)
            {
                this.Log.Info("Executing ReduceBetMessage.");
            }

            MachineScreen screen = this.Parent as MachineScreen;
            if (screen.DataStore.CreditsPerSpin > 1)
            {
                screen.DataStore.CreditsPerSpin--;
            }
        }
    }
}
