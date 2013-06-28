// -----------------------------------------------------------------------
// <copyright file="ReduceBetMessage.cs" company="">
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
