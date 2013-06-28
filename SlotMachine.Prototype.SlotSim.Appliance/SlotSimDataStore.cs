// -----------------------------------------------------------------------
// <copyright file="SlotSimDatastore.cs" company="">
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

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class SlotSimDataStore : DataStore
    {
        private int creditsPerSpin = 0;

        public SlotSimDataStore(string logName)
            : base(logName)
        {
            this.Symbols = new Dictionary<string, Symbol>();
            this.Payouts = new List<Payout>();
            this.TempOutput = "Please spin!";
            this.CreditsPerSpin = 1;
        }

        public int CreditsPerSpin
        {
            get
            {
                return creditsPerSpin;
            }
            set
            {
                this.creditsPerSpin = value;
                this.RaisePropertyChangedEvent("CreditsPerSpin", this.CreditsPerSpin);
            }
        }

        public int Credits { get; set; }

        // TODO Do we want this stuff here?
        public Dictionary<string, Symbol> Symbols { get; set; }

        public List<Payout> Payouts { get; set; }

        public string TempOutput { get; set; }
    }
}
