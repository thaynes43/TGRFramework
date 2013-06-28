// -----------------------------------------------------------------------
// <copyright file="Database.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace SlotMachine.Prototype.Common
{
    /// <summary>
    /// Handle persistant storage of data
    /// </summary>
    public abstract class Database : Subsystem
    {
        public Database(DataStore dataStore, string logName)
            : base(logName)
        {
            this.DataStore = dataStore;
        }

        /// <summary>
        /// In memory data store
        /// </summary>
        public DataStore DataStore { get; set; }

        /// <summary>
        /// Subsystem specifc initialize behavior
        /// </summary>
        public override void Initialize()
        {
            this.Load();
        }

        /// <summary>
        /// Load data from persistant store
        /// </summary>
        protected virtual void Load()
        {
        }
    }
}
