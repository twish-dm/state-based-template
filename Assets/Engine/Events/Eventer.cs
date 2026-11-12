using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace StateEngine.Events
{
    public sealed class Eventer : IEventer
    {
        private static int s_NextId;

        private readonly Dictionary<string, UnityEventBase> m_Events = new Dictionary<string, UnityEventBase>();

        public Eventer()
        {
            Id = ++s_NextId;
        }

        public int Id { get; }

        public bool Contains(string type) => m_Events.ContainsKey(type);

        public void Add(string type, UnityAction action) => GetOrCreate<UnityEvent>(type).AddListener(action);

        public void Add<T>(string type, UnityAction<T> action) => GetOrCreate<UnityEvent<T>>(type).AddListener(action);

        public void Remove(string type, UnityAction action)
        {
            if (TryGet(type, out UnityEvent unityEvent))
                unityEvent.RemoveListener(action);
        }

        public void Remove<T>(string type, UnityAction<T> action)
        {
            if (TryGet(type, out UnityEvent<T> unityEvent))
                unityEvent.RemoveListener(action);
        }

        public void RemoveAll(string type)
        {
            if (m_Events.TryGetValue(type, out UnityEventBase unityEvent))
                unityEvent.RemoveAllListeners();
        }

        public void Invoke(string type)
        {
            if (TryGet(type, out UnityEvent unityEvent))
                unityEvent.Invoke();
        }

        public void Invoke<T>(string type, T data)
        {
            if (TryGet(type, out UnityEvent<T> unityEvent))
                unityEvent.Invoke(data);
        }

        public void Dispose()
        {
            foreach (UnityEventBase unityEvent in m_Events.Values)
                unityEvent.RemoveAllListeners();

            m_Events.Clear();
        }

        private TEvent GetOrCreate<TEvent>(string type) where TEvent : UnityEventBase, new()
        {
            if (!m_Events.TryGetValue(type, out UnityEventBase unityEvent))
            {
                unityEvent = new TEvent();
                m_Events.Add(type, unityEvent);
            }

            if (unityEvent is TEvent typed)
                return typed;

            throw new InvalidOperationException(
                $"Event '{type}' is already registered as {unityEvent.GetType().Name}, cannot subscribe as {typeof(TEvent).Name}.");
        }

        private bool TryGet<TEvent>(string type, out TEvent result) where TEvent : UnityEventBase
        {
            m_Events.TryGetValue(type, out UnityEventBase unityEvent);
            result = unityEvent as TEvent;
            return result != null;
        }
    }
}
