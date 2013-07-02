// -----------------------------------------------------------------------
// <copyright file="Database.cs" company="">
// Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TGRFramework.Prototype.Common
{
    /// <summary>
    /// Handle persistent storage of data
    /// </summary>
    public abstract class Database : Subsystem, IDatabase
    {
        public Database(string logName)
            : base(logName)
        {
        }

        /// <summary>
        /// Load data from persistent store into in memory store
        /// </summary>
        public virtual void Load(DataStore dataStore)
        {
        }
    }
}
