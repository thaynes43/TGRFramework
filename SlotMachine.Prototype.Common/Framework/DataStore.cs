// -----------------------------------------------------------------------
// <copyright file="DataStore.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace SlotMachine.Prototype.Common
{
    using System;

    /// <summary>
    /// In memory data store
    /// </summary>
    public class DataStore : Subsystem
    { 
        public DataStore(string logName)
            : base(logName)
        {
        }

        public event Action<string, object> PropertyChangedEvent;

        protected void RaisePropertyChangedEvent(string property, object propertyObj)
        {
            if (this.PropertyChangedEvent != null)
            {
                this.PropertyChangedEvent(property, propertyObj);
            }
        }
    }
}
