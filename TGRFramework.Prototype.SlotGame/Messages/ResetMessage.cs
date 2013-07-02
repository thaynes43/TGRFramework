// -----------------------------------------------------------------------
// <copyright file="ResetMessage.cs" company="">
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
    public class ResetMessage : Message
    {
        public ResetMessage(ILogTool log = null)
        {
            this.Log = log;
        }

        public ILogTool Log { get; set; }

        public override void Execute()
        {
            if (this.Log != null)
            {
                this.Log.Info("Executing ResetMessage.");
            }

            MachineScreen screen = this.Parent as MachineScreen;
            screen.DataStore.Credits = MachineScreen.STARTING_CREDITS;
            screen.DataStore.CreditsPerSpin = 1;
            screen.DataStore.TempOutput = "Please Spin!";

            // TODO update this with others?
            screen.PayoutText.OutputText = string.Format("{0}", 0);

            screen.ReelManager.Reset();
        }
    }
}
