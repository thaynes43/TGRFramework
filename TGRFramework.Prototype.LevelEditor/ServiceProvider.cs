// -----------------------------------------------------------------------
// <copyright file="ServiceProvider.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TGRFramework.Prototype.LevelEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Needed for XNA Framework
    /// </summary>
    public class ServiceProvider : IServiceProvider
    {
        Dictionary<Type, object> services = new Dictionary<Type, object>();

        public void AddService<T>(T service)
        {
            services.Add(typeof(T), service);
        }

        public object GetService(Type serviceType)
        {
            object service;

            services.TryGetValue(serviceType, out service);

            return service;
        }
    }
}
