// -----------------------------------------------------------------------
// <copyright file="SpinMessage.cs" company="">
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
    public class SpinMessage : Message
    {
        public SpinMessage(bool spinToggle, ILogTool log = null)
        {
            this.Log = log;
            this.SpinToggle = spinToggle;
        }

        public ILogTool Log { get; set; }

        public bool SpinToggle { get; set; }

        public override void Execute()
        {
            if (this.Log != null)
            {
                this.Log.Info("Executing SpinMessage.");
            }

            MachineScreen screen = this.Parent as MachineScreen;
            screen.PayoutText.OutputText = "0";
            if (this.SpinToggle)
            {
                screen.ReelManager.Spin();
            }
            else
            {
                screen.ReelManager.ForceStopSpin();
            } 
        }
    }
}
