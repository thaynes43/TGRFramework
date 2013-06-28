// -----------------------------------------------------------------------
// <copyright file="Payout.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace SlotMachine.Prototype.SlotSim.Appliance
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Payout
    {
        public Payout(int value, List<Symbol> symbols)
        {
            this.Value = value;
            this.SymbolOrder = symbols;
        }

        public int Value { get; set; }

        public List<Symbol> SymbolOrder { get; set; }
    }
}
