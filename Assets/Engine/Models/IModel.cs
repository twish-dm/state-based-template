using System.Collections.Generic;
using StateEngine.Events;

namespace StateEngine.Models
{
    /// <summary>
    /// Key-value storage that notifies subscribers of <see cref="Eventer"/> whenever a value is set.
    /// </summary>
    public interface IModel
    {
        int Id { get; }

        IEventer Eventer { get; }

        bool Contains(string key);

        T Get<T>(string key, T fallback = default);

        bool TryGet<T>(string key, out T value);

        void Set<T>(string key, T value);

        bool Remove(string key);

        void Refresh<T>(string key);

        void Clear();

        DynamicList<T> CreateList<T>(string key, IEnumerable<T> items = null);

        DynamicList<T> GetList<T>(string key);
    }
}
