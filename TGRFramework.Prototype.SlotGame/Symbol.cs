// -----------------------------------------------------------------------
// <copyright file="Symbol.cs" company="">
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
    public class Symbol
    {
        public Symbol(string content, int probability)
        {
            this.Content = content;
            this.Probability = probability;
        }

        public string Content { get; set; }

        public int Probability { get; set; }
    }
}
