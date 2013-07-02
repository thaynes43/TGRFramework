// -----------------------------------------------------------------------
// <copyright file="SlotSimDatastore.cs" company="">
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

    /// <summary>
    /// Update summary.
    /// </summary>
    public class SlotSimDataStore : DataStore
    {
        private int creditsPerSpin = 0;

        public SlotSimDataStore(SlotSimDatabase database, string logName)
            : base(database, logName)
        {
            this.Symbols = new Dictionary<string, Symbol>();
            this.Payouts = new List<Payout>();
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

        public override void Initialize()
        {
            this.TempOutput = "Please spin!";
            this.CreditsPerSpin = 1;
            this.Database.Load(this);
        }
    }
}
