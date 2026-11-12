using System;
using System.Collections.Generic;
using UnityEngine;

namespace StateEngine.Models
{
    /// <summary>
    /// Minimal type-keyed registry for scene-level services such as the engine and the viewer.
    /// </summary>
    public static class ServiceRegistry
    {
        private static readonly Dictionary<Type, object> s_Services = new Dictionary<Type, object>();

        public static void Register<T>(T service) where T : class
        {
            if (service == null)
                throw new ArgumentNullException(nameof(service));

            if (s_Services.ContainsKey(typeof(T)))
                throw new InvalidOperationException($"Service {typeof(T).Name} is already registered.");

            s_Services.Add(typeof(T), service);
        }

        public static void Unregister<T>(T service) where T : class
        {
            if (s_Services.TryGetValue(typeof(T), out object current) && ReferenceEquals(current, service))
                s_Services.Remove(typeof(T));
        }

        public static bool TryGet<T>(out T service) where T : class
        {
            s_Services.TryGetValue(typeof(T), out object raw);
            service = raw as T;
            return service != null;
        }

        public static T Get<T>() where T : class
        {
            if (TryGet(out T service))
                return service;

            throw new InvalidOperationException($"Service {typeof(T).Name} is not registered.");
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetOnPlayModeEnter() => s_Services.Clear();
    }
}
