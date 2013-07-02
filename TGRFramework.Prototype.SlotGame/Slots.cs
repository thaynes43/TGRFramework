// -----------------------------------------------------------------------
// <copyright file="Slots.cs" company="">
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
    /// Update summary. TODO_HIGH Diagram how Slots and ReelManagers interact
    /// </summary>
    public class Slots
    {
        // TODO How do we determine this
        public const int NUMBER_REELS = 3;

        // TODO thread safe?

        public Slots(SlotSimDataStore dataStore)
        {
            this.SlotSimDataStore = dataStore;
            this.Rand = new Random(int.Parse(Guid.NewGuid().ToString().Substring(0, 8), System.Globalization.NumberStyles.HexNumber));
        }

        private SlotSimDataStore SlotSimDataStore;

        private Random Rand;

        public int SpinReels(out string tempDisplay, out List<Symbol> spunSymbols)
        {
            int pay = 0;
            tempDisplay = string.Empty;
            spunSymbols = new List<Symbol>();

            for (int i = 0; i < NUMBER_REELS; i++)
            {
                Symbol nextSpin = this.SpinSingleReel();
                tempDisplay += nextSpin.Content + " ";
                spunSymbols.Add(nextSpin);
            }

            foreach (Payout payout in this.SlotSimDataStore.Payouts)
            {
                bool winner = true;
                for (int i = 0; i < payout.SymbolOrder.Count; i++)
                {
                    if (payout.SymbolOrder[i] != spunSymbols[i])
                    {
                        winner = false;              
                    }
                }
                if (winner)
                {
                    // May hit muliple payouts in one spin - pay highest
                    pay = pay < payout.Value ?  payout.Value : pay;
                }
            }

            return pay;
        }

        private Symbol SpinSingleReel()
        {
            int roll = this.Rand.Next(1, 100);
            List<Symbol> sortedSymbols = this.LowToHigh(this.SlotSimDataStore.Symbols.Values.ToList());

            int lowBound = 1;
            for (int i = 0; i < sortedSymbols.Count; i++)
            {           
                for (int j = lowBound; j < sortedSymbols[i].Probability + lowBound; j++)
                {
                    if (roll == j)
                    {
                        return sortedSymbols[i];
                    }
                }
                lowBound += sortedSymbols[i].Probability;
            }

            throw new Exception("Did not produce a valid spin with roll " + roll);
        }

        // TODO Extension method?
        private List<Symbol> LowToHigh(List<Symbol> symbols)
        {
            List<Symbol> sortedList = symbols;
            bool swapped;

            do
            {
                swapped = false;
                for (int i = 1; i < sortedList.Count; i++)
                {
                    if (symbols[i - 1].Probability > symbols[i].Probability)
                    {
                        Symbol temp = symbols[i];
                        symbols[i] = symbols[i - 1];
                        symbols[i - 1] = temp;
                        swapped = true;
                    }
                }
            }
            while (swapped);

            return sortedList;
        }
    }
}
