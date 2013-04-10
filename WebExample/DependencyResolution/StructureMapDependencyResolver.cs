using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StructureMap;

namespace WebExample.DependencyResolution
{
    public class StructureMapDependencyResolver : IDependencyResolver
    {
        private readonly IContainer _container;

        public StructureMapDependencyResolver(IContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            _container = container;
        }

        public object GetService(Type serviceType)
        {
            if (serviceType.IsAbstract || serviceType.IsInterface)
            {
                return _container.TryGetInstance(serviceType);
            }
            else
            {
                return _container.GetInstance(serviceType);
            }
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            foreach (object obj in _container.GetAllInstances(serviceType))
            {
                yield return obj;
            }
        }
    }
}