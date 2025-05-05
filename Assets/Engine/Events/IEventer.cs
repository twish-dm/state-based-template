namespace StateEngine.Events
{
    using System;
    using UnityEngine.Events;
    public interface IEventer : IDisposable
    {
        int id{ get; }
        void Add(string type, UnityAction action);
        void Add<T>(string type, UnityAction<T> action);
        void Add(string type, UnityAction<object> action);
        void Invoke(string type);
        void Invoke<T>(string type, T eventData);
        void RemoveAll(string type);
        bool Contains(string key);
        void Remove(string type, UnityAction action);
        void Remove<T>(string type, UnityAction<T> action);
    }
}