// -----------------------------------------------------------------------
// <copyright file="SlotSimDatabaseXML.cs" company="">
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
    using System.Xml.Linq;

    /// <summary>
    /// Update summary.
    /// </summary>
    public class SlotSimDatabaseXML : SlotSimDatabase
    {
        public SlotSimDatabaseXML(string logName)
            : base(logName)
        {
        }

        /// <inheritdoc />
        public override void Load(DataStore dataStore)
        {
            XDocument databaseXML = XDocument.Load("SlotGameDatabase.xml"); // TODO this configurable

            SlotSimDataStore slotSimDataStore = dataStore as SlotSimDataStore;

            if (slotSimDataStore != null)
            {
                // Load Symbols
                foreach (XElement element in databaseXML.Root.Element("Symbols").Elements())
                {
                    string content = element.Attribute("content").Value;
                    int probability = int.Parse(element.Attribute("probability").Value);
                    slotSimDataStore.Symbols.Add(content, new Symbol(content, probability));
                }

                // Load Payouts
                foreach (XElement element in databaseXML.Root.Element("Payouts").Elements())
                {
                    int value = int.Parse(element.Attribute("value").Value);
                    List<Symbol> reels = new List<Symbol>();

                    foreach (XElement innerElement in element.Elements())
                    {
                        string symbol = element.Element("Symbol").Value;
                        if (slotSimDataStore.Symbols.ContainsKey(symbol))
                        {
                            reels.Add(slotSimDataStore.Symbols[symbol]);
                        }
                        else
                        {
                            throw new ArgumentException(string.Format("Invalid symbol {0} for payout.", symbol));
                        }
                    }
                    slotSimDataStore.Payouts.Add(new Payout(value, reels));
                }
            }
        }
    }
}
