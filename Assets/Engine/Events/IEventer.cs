using System;
using UnityEngine.Events;

namespace StateEngine.Events
{
    /// <summary>
    /// String-keyed event bus. Each key is bound to a single payload type on first subscription.
    /// </summary>
    public interface IEventer : IDisposable
    {
        int Id { get; }

        bool Contains(string type);

        void Add(string type, UnityAction action);

        void Add<T>(string type, UnityAction<T> action);

        void Remove(string type, UnityAction action);

        void Remove<T>(string type, UnityAction<T> action);

        void RemoveAll(string type);

        void Invoke(string type);

        void Invoke<T>(string type, T data);
    }
}
