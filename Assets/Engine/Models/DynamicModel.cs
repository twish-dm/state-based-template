using System;
using System.Collections.Generic;
using StateEngine.Events;

namespace StateEngine.Models
{
    public class DynamicModel : IModel
    {
        private static int s_NextId;

        private readonly Dictionary<string, object> m_Values = new Dictionary<string, object>();

        public DynamicModel(IEventer eventer)
        {
            Eventer = eventer ?? throw new ArgumentNullException(nameof(eventer));
            Id = ++s_NextId;
        }

        public int Id { get; }

        public IEventer Eventer { get; }

        public int Count => m_Values.Count;

        public IEnumerable<string> Keys => m_Values.Keys;

        public bool Contains(string key) => m_Values.ContainsKey(key);

        public T Get<T>(string key, T fallback = default) => TryGet(key, out T value) ? value : fallback;

        public bool TryGet<T>(string key, out T value)
        {
            if (m_Values.TryGetValue(key, out object raw) && raw is T typed)
            {
                value = typed;
                return true;
            }

            value = default;
            return false;
        }

        public virtual void Set<T>(string key, T value)
        {
            m_Values[key] = value;
            Eventer.Invoke(key, value);
        }

        public virtual bool Remove(string key) => m_Values.Remove(key);

        public void Refresh<T>(string key)
        {
            if (TryGet(key, out T value))
                Eventer.Invoke(key, value);
        }

        public virtual void Clear() => m_Values.Clear();

        public DynamicList<T> CreateList<T>(string key, IEnumerable<T> items = null)
        {
            var list = new DynamicList<T>(key, Eventer, items);
            m_Values[key] = list;
            Eventer.Invoke(key, new ListEvent<T>(ListChangeType.Cleared, list));
            return list;
        }

        public DynamicList<T> GetList<T>(string key) => Get<DynamicList<T>>(key);
    }
}
