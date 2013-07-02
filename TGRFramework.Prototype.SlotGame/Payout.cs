// -----------------------------------------------------------------------
// <copyright file="Payout.cs" company="">
// Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TGRFramework.Prototype.SlotGame
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Update summary.
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
