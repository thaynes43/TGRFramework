// -----------------------------------------------------------------------
// <copyright file="SlotSimDatabaseXML.cs" company="">
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
    using System.Xml.Linq;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class SlotSimDatabaseXML : SlotSimDatabase
    {
        public SlotSimDatabaseXML(DataStore dataStore, string logName)
            : base(dataStore, logName)
        {
        }

        protected override void Load()
        {
            XDocument databaseXML = XDocument.Load("SlotSimDatabase.xml"); // TODO this configurable

            // Load Symbols
            foreach (XElement element in databaseXML.Root.Element("Symbols").Elements())
            {
                string content = element.Attribute("content").Value;
                int probability = int.Parse(element.Attribute("probability").Value);
                this.SlotSimDataStore.Symbols.Add(content, new Symbol(content, probability));
            }

            // Load Payouts
            foreach (XElement element in databaseXML.Root.Element("Payouts").Elements())
            {
                int value = int.Parse(element.Attribute("value").Value);
                List<Symbol> reels = new List<Symbol>();

                foreach (XElement innerElement in element.Elements())
                {
                    string symbol = element.Element("Symbol").Value;
                    if (this.SlotSimDataStore.Symbols.ContainsKey(symbol))
                    {
                        reels.Add(this.SlotSimDataStore.Symbols[symbol]);
                    }
                    else
                    {
                        throw new ArgumentException(string.Format("Invalid symbol {0} for payout.", symbol));
                    }
                }
                this.SlotSimDataStore.Payouts.Add(new Payout(value, reels));
            }
        }
    }
}
