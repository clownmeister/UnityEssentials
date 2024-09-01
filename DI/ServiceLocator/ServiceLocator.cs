using System;
using System.Collections.Generic;

namespace ClownMeister.UnityEssentials.DI.ServiceLocator
{
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> Services = new();

        public static T Resolve<T>()
        {
            try
            {
                return (T)Services[typeof(T)];
            }
            catch (KeyNotFoundException)
            {
                throw new ApplicationException($"Service of type {typeof(T)} not found.");
            }
        }

        public static void Register<T>(T service)
        {
            if (Services.ContainsKey(typeof(T)))
            {
                throw new ApplicationException($"Service of type {typeof(T)} is already registered.");
            }

            Services[typeof(T)] = service;
        }

        public static void Unregister<T>()
        {
            if (!Services.ContainsKey(typeof(T)))
            {
                throw new ApplicationException($"Service of type {typeof(T)} is not registered. Can't unregister.");
            }

            Services.Remove(typeof(T));
        }
    }
}