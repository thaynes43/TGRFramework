// -----------------------------------------------------------------------
// <copyright file="SlotSimDatabase.cs" company="">
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
    /// Database API for SlotSim based games
    /// </summary>
    public class SlotSimDatabase : Database
    {
        protected object dbLock = new object();

        public SlotSimDatabase(string logName)
            : base(logName)
        {
        }

        /// <summary>
        /// Create an instance of a SlotSimDatabase
        /// </summary>
        /// <param name="logName">Log file name</param>
        /// <returns>SlotSimDatabase type</returns>
        public static SlotSimDatabase CreateDatabase(string logName)
        {
            SlotSimDatabase database = null;

            database = new SlotSimDatabaseXML(logName);

            return database;
        }
    }
}
