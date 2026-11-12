using System.Collections;
using System.Collections.Generic;
using StateEngine.Events;

namespace StateEngine.Models
{
    /// <summary>
    /// List stored in a model that raises a <see cref="ListEvent{T}"/> under its model key on every change.
    /// </summary>
    public sealed class DynamicList<T> : IList<T>, IReadOnlyList<T>
    {
        private readonly List<T> m_Items;
        private readonly IEventer m_Eventer;
        private readonly string m_Key;

        public DynamicList(string key, IEventer eventer, IEnumerable<T> items = null)
        {
            m_Key = key;
            m_Eventer = eventer;
            m_Items = items != null ? new List<T>(items) : new List<T>();
        }

        public int Count => m_Items.Count;

        bool ICollection<T>.IsReadOnly => false;

        public T this[int index]
        {
            get => m_Items[index];
            set
            {
                m_Items[index] = value;
                Notify(ListChangeType.Replaced, value);
            }
        }

        public void Add(T item)
        {
            m_Items.Add(item);
            Notify(ListChangeType.Added, item);
        }

        public void Insert(int index, T item)
        {
            m_Items.Insert(index, item);
            Notify(ListChangeType.Added, item);
        }

        public bool Remove(T item)
        {
            if (!m_Items.Remove(item))
                return false;

            Notify(ListChangeType.Removed, item);
            return true;
        }

        public void RemoveAt(int index)
        {
            T item = m_Items[index];
            m_Items.RemoveAt(index);
            Notify(ListChangeType.Removed, item);
        }

        public void Clear()
        {
            m_Items.Clear();
            Notify(ListChangeType.Cleared);
        }

        public bool Contains(T item) => m_Items.Contains(item);

        public int IndexOf(T item) => m_Items.IndexOf(item);

        public void CopyTo(T[] array, int arrayIndex) => m_Items.CopyTo(array, arrayIndex);

        public List<T>.Enumerator GetEnumerator() => m_Items.GetEnumerator();

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private void Notify(ListChangeType type, T item = default)
        {
            m_Eventer.Invoke(m_Key, new ListEvent<T>(type, this, item));
        }
    }
}
