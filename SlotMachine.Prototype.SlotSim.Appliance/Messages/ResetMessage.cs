// -----------------------------------------------------------------------
// <copyright file="ResetMessage.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace SlotMachine.Prototype.SlotSim.Appliance
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using SlotMachine.Prototype.Common;
    using SlotMachine.Prototype.Tools;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
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
