// -----------------------------------------------------------------------
// <copyright file="DataStore.cs" company="">
// Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TGRFramework.Prototype.Common
{
    using System;

    /// <summary>
    /// In memory data store
    /// </summary>
    public class DataStore : Subsystem
    { 
        public DataStore(Database database, string logName)
            : base(logName)
        {
            this.Database = database;
        }

        public event Action<string, object> PropertyChangedEvent;

        protected Database Database { get; set; }

        protected void RaisePropertyChangedEvent(string property, object propertyObj)
        {
            if (this.PropertyChangedEvent != null)
            {
                this.PropertyChangedEvent(property, propertyObj);
            }
        }
    }
}
