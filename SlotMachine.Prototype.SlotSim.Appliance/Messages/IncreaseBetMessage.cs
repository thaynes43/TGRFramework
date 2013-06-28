// -----------------------------------------------------------------------
// <copyright file="IncreaseBetMessage.cs" company="">
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
    public class IncreaseBetMessage : Message
    {
        public IncreaseBetMessage(ILogTool log = null)
        {
            this.Log = log;
        }

        public ILogTool Log { get; set; }

        public override void  Execute()
        {
            if (this.Log != null)
            {
                this.Log.Info("Executing IncreaseBetMessage.");
            }

            MachineScreen screen = this.Parent as MachineScreen;
            screen.DataStore.CreditsPerSpin++;
        }
    }
}
