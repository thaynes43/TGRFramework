// -----------------------------------------------------------------------
// <copyright file="SlotSimDatabase.cs" company="">
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
    public class SlotSimDatabase : Database
    {
        protected object dbLock = new object();

        public SlotSimDatabase(DataStore dataStore, string logName) // TODO_HIGH Database is dependent on data store ? I think we want this the other way around there buddy.
            : base(dataStore, logName)
        {
        }

        public SlotSimDataStore SlotSimDataStore
        {
            get
            {
                return this.DataStore as SlotSimDataStore;
            }
        }

        public static SlotSimDatabase CreateDatabase(DataStore dataStore, string logName)
        {
            SlotSimDatabase database = null;

            database = new SlotSimDatabaseXML(dataStore, logName);

            return database;
        }
    }
}
